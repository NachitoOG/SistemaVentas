using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CapaEntidades;
using CapaLogica;

namespace CapaVista
{
    public partial class FrmReportesVentas : Form
    {
        private const int PRESUPUESTOS = 0;
        private const int NOTAS_POR_ESTADO = 1;
        private const int PEDIDOS_PENDIENTES = 2;
        private const int AVISOS = 3;
        private const int TIEMPO_PROMEDIO = 4;

        private readonly CN_ReportesVentas cnReportes = new CN_ReportesVentas();
        private readonly CN_ListasVentas cnListas = new CN_ListasVentas();

        private readonly List<int> reportes = new List<int>();

        private bool cargando;

        public FrmReportesVentas()
        {
            InitializeComponent();
        }

        private void FrmReportesVentas_Load(object sender, EventArgs e)
        {
            cargando = true;

            dtpDesde.Value = DateTime.Today.AddMonths(-1);
            dtpDesde.MaxDate = DateTime.Today;
            dtpHasta.Value = DateTime.Today;

            CargarCombos();

            bool hayUsuario = SesionVentas.HaySesion();
            bool esDeposito = hayUsuario && SesionVentas.UsuarioActual.EsDeposito;

            if (!esDeposito)
            {
                AgregarReporte(PRESUPUESTOS, "Presupuestos de venta por cliente y por estado");
                AgregarReporte(NOTAS_POR_ESTADO, "Notas de pedido agrupadas por estado");
            }

            if (!hayUsuario || esDeposito)
            {
                AgregarReporte(PEDIDOS_PENDIENTES, "Pedidos pendientes de preparar");
                AgregarReporte(AVISOS, "Avisos de mercadería preparada por período");
                AgregarReporte(TIEMPO_PROMEDIO, "Tiempo promedio entre el pedido y el aviso");
            }

            cboReporte.SelectedIndex = 0;

            cargando = false;
            AplicarFiltros();
            Generar();
        }

        private void AgregarReporte(int reporte, string titulo)
        {
            reportes.Add(reporte);
            cboReporte.Items.Add((cboReporte.Items.Count + 1) + ". " + titulo);
        }

        private int ReporteElegido()
        {
            return reportes[cboReporte.SelectedIndex];
        }

        private void cboReporte_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cargando)
            {
                return;
            }

            AplicarFiltros();
            Generar();
        }

        private void Filtros_Cambiaron(object sender, EventArgs e)
        {
            if (cargando)
            {
                return;
            }

            Generar();
        }

        private void chkDesde_CheckedChanged(object sender, EventArgs e)
        {
            dtpDesde.Enabled = chkDesde.Enabled && chkDesde.Checked;
            Filtros_Cambiaron(sender, e);
        }

        private void chkHasta_CheckedChanged(object sender, EventArgs e)
        {
            dtpHasta.Enabled = chkHasta.Enabled && chkHasta.Checked;
            Filtros_Cambiaron(sender, e);
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CargarCombos()
        {
            try
            {
                List<ClienteVentas> clientes = cnListas.ListarClientes();
                clientes.Insert(0, new ClienteVentas(0, "(Todos)", ""));
                cboCliente.DisplayMember = "Nombre";
                cboCliente.ValueMember = "IdCliente";
                cboCliente.DataSource = clientes;

                cboEstado.Items.Clear();
                cboEstado.Items.Add("(Todos)");
                cboEstado.Items.Add(PresupuestoVenta.PENDIENTE);
                cboEstado.Items.Add(PresupuestoVenta.APROBADO);
                cboEstado.Items.Add(PresupuestoVenta.RECHAZADO);
                cboEstado.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MostrarAviso("No se pudieron cargar las listas:\n" + ex.Message);
            }
        }

        private void AplicarFiltros()
        {
            int reporte = ReporteElegido();

            bool usaCliente = reporte == PRESUPUESTOS || reporte == PEDIDOS_PENDIENTES;
            bool usaEstado = reporte == PRESUPUESTOS;
            bool usaFechas = reporte != PEDIDOS_PENDIENTES;

            lblCliente.Enabled = usaCliente;
            cboCliente.Enabled = usaCliente;
            lblEstado.Enabled = usaEstado;
            cboEstado.Enabled = usaEstado;
            chkDesde.Enabled = usaFechas;
            chkHasta.Enabled = usaFechas;
            dtpDesde.Enabled = usaFechas && chkDesde.Checked;
            dtpHasta.Enabled = usaFechas && chkHasta.Checked;

            lblDescripcion.Text = DescripcionDelReporte(reporte);
        }

        private string DescripcionDelReporte(int reporte)
        {
            if (reporte == PRESUPUESTOS)
                return "Cuántos presupuestos tiene cada cliente en cada estado y por cuánto dinero. No cuenta los dados de baja.";

            if (reporte == NOTAS_POR_ESTADO)
                return "Cuántas notas de pedido hay en cada etapa del circuito. Muestra los cuatro estados aunque alguno tenga cero.";

            if (reporte == PEDIDOS_PENDIENTES)
                return "Pedidos que Depósito todavía tiene que preparar, del que espera hace más tiempo al más nuevo.";

            if (reporte == AVISOS)
                return "Avisos de mercadería preparada cargados en el período, con quién preparó cada pedido y cuánto tardó.";

            return "Cuánto tarda Depósito, en promedio, desde que se emite un pedido hasta que avisa que está preparado.";
        }

        private void Generar()
        {
            try
            {
                int reporte = ReporteElegido();
                int filas = 0;

                dgvReporte.DataSource = null;

                if (reporte == PRESUPUESTOS)
                {
                    List<ReportePresupuestosVenta> datos = cnReportes.PresupuestosPorClienteEstado(
                        LeerCliente(), LeerEstado(), LeerDesde(), LeerHasta());
                    dgvReporte.DataSource = datos;
                    filas = datos.Count;

                    Titulo("MontoTotal", "Monto total");
                    Formato("MontoTotal", "N2");
                }
                else if (reporte == NOTAS_POR_ESTADO)
                {
                    List<ReporteNotasPorEstado> datos = cnReportes.NotasPedidoPorEstado(LeerDesde(), LeerHasta());
                    dgvReporte.DataSource = datos;
                    filas = datos.Count;

                    Titulo("MontoTotal", "Monto total");
                    Formato("MontoTotal", "N2");
                }
                else if (reporte == PEDIDOS_PENDIENTES)
                {
                    List<ReportePedidoPendiente> datos = cnReportes.PedidosPendientesPreparar(LeerCliente());
                    dgvReporte.DataSource = datos;
                    filas = datos.Count;

                    Titulo("NumeroPedido", "Pedido");
                    Titulo("FechaPedido", "Fecha del pedido");
                    Titulo("TomadoPor", "Tomado por");
                    Titulo("Lineas", "Líneas");
                    Titulo("HorasEsperando", "Horas esperando");
                    Formato("FechaPedido", "dd/MM/yyyy HH:mm");
                    Formato("Unidades", "N0");
                    Formato("HorasEsperando", "N1");
                }
                else if (reporte == AVISOS)
                {
                    List<ReporteAviso> datos = cnReportes.AvisosPorPeriodo(LeerDesde(), LeerHasta());
                    dgvReporte.DataSource = datos;
                    filas = datos.Count;

                    Titulo("NumeroAviso", "Aviso");
                    Titulo("FechaAviso", "Fecha del aviso");
                    Titulo("NumeroPedido", "Pedido");
                    Titulo("FechaPedido", "Fecha del pedido");
                    Titulo("PreparadoPor", "Preparado por");
                    Titulo("EstadoPedido", "Estado del pedido");
                    Titulo("HorasPreparacion", "Horas de preparación");
                    Formato("FechaAviso", "dd/MM/yyyy HH:mm");
                    Formato("FechaPedido", "dd/MM/yyyy HH:mm");
                    Formato("HorasPreparacion", "N2");
                }
                else if (reporte == TIEMPO_PROMEDIO)
                {
                    List<ReporteTiempoPreparacion> datos = cnReportes.TiempoPromedioPreparacion(LeerDesde(), LeerHasta());
                    dgvReporte.DataSource = datos;
                    filas = datos.Count;

                    Titulo("PedidosPreparados", "Pedidos preparados");
                    Titulo("PromedioHoras", "Promedio (horas)");
                    Titulo("MinimoHoras", "El más rápido (horas)");
                    Titulo("MaximoHoras", "El más lento (horas)");
                    Titulo("PromedioDias", "Promedio (días)");
                    Formato("PromedioHoras", "N2");
                    Formato("MinimoHoras", "N2");
                    Formato("MaximoHoras", "N2");
                    Formato("PromedioDias", "N2");
                }

                dgvReporte.ClearSelection();
                lblCantidadRegistros.ForeColor = SystemColors.ControlText;

                if (filas == 0)
                    lblCantidadRegistros.Text = "Filas: 0 (no hay datos con esos filtros)";
                else
                    lblCantidadRegistros.Text = "Filas: " + filas;
            }
            catch (Exception ex)
            {
                dgvReporte.DataSource = null;
                lblCantidadRegistros.ForeColor = Color.Firebrick;
                lblCantidadRegistros.Text = ex.Message;
            }
        }

        private void Titulo(string columna, string texto)
        {
            dgvReporte.Columns[columna].HeaderText = texto;
        }

        private void Formato(string columna, string formato)
        {
            dgvReporte.Columns[columna].DefaultCellStyle.Format = formato;
        }

        private int? LeerCliente()
        {
            if (cboCliente.Enabled && cboCliente.SelectedIndex > 0)
            {
                ClienteVentas cliente = (ClienteVentas)cboCliente.SelectedItem;
                return cliente.IdCliente;
            }
            return null;
        }

        private string LeerEstado()
        {
            if (cboEstado.Enabled && cboEstado.SelectedIndex > 0)
            {
                return cboEstado.Text;
            }
            return null;
        }

        private DateTime? LeerDesde()
        {
            if (chkDesde.Enabled && chkDesde.Checked)
            {
                return dtpDesde.Value.Date;
            }
            return null;
        }

        private DateTime? LeerHasta()
        {
            if (chkHasta.Enabled && chkHasta.Checked)
            {
                return dtpHasta.Value.Date;
            }
            return null;
        }

        private void MostrarAviso(string mensaje)
        {
            MessageBox.Show(mensaje, "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
