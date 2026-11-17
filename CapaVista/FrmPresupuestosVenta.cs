using CapaEntidades;
using CapaLogica;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CapaVista
{
    public partial class FrmPresupuestosVenta : Form
    {
        private readonly CN_PresupuestosVenta cnPresupuestos = new CN_PresupuestosVenta();
        private readonly CN_ListasVentas cnListas = new CN_ListasVentas();

        private bool cargando;

        public FrmPresupuestosVenta()
        {
            InitializeComponent();
            dgvPresupuestos.AutoGenerateColumns = false;
        }

        private void FrmPresupuestosVenta_Load(object sender, EventArgs e)
        {
            cargando = true;
            CargarFiltros();
            cargando = false;

            ListarPresupuestos();
            cboFiltroCliente.DropDownStyle = ComboBoxStyle.DropDown;
            cboFiltroCliente.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboFiltroCliente.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        private void Filtros_Cambiaron(object sender, EventArgs e)
        {
            if (cargando)
            {
                return;
            }

            ListarPresupuestos();
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            FrmPresupuestoVentaDatos ventana = new FrmPresupuestoVentaDatos();

            if (ventana.ShowDialog() == DialogResult.OK)
            {
                ListarPresupuestos();
            }
        }

        private void dgvPresupuestos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!Iconos.HayIcono(dgvPresupuestos, e.ColumnIndex, e.RowIndex))
            {
                return;
            }

            PresupuestoVenta presupuesto = (PresupuestoVenta)dgvPresupuestos.Rows[e.RowIndex].DataBoundItem;
            string columna = dgvPresupuestos.Columns[e.ColumnIndex].Name;

            if (columna == "icoDetalles")
            {
                VerDetalles(presupuesto);
            }
            else if (columna == "icoEditar")
            {
                Editar(presupuesto);
            }
            else if (columna == "icoAprobar")
            {
                Aprobar(presupuesto);
            }
            else if (columna == "icoRechazar")
            {
                Rechazar(presupuesto);
            }
            else if (columna == "icoBaja")
            {
                DarDeBaja(presupuesto);
            }
        }

        private void dgvPresupuestos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            PresupuestoVenta presupuesto = dgvPresupuestos.Rows[e.RowIndex].DataBoundItem as PresupuestoVenta;
            if (presupuesto == null)
            {
                return;
            }

            string columna = dgvPresupuestos.Columns[e.ColumnIndex].Name;

            if (columna == "icoDetalles")
            {
                e.Value = Iconos.DETALLES;
            }
            else if (columna == "icoEditar")
            {
                e.Value = Iconos.Mostrar(Iconos.EDITAR, cnPresupuestos.PuedeModificarse(presupuesto));
            }
            else if (columna == "icoAprobar")
            {
                e.Value = Iconos.Mostrar(Iconos.APROBAR, cnPresupuestos.PuedeModificarse(presupuesto));
            }
            else if (columna == "icoRechazar")
            {
                e.Value = Iconos.Mostrar(Iconos.RECHAZAR, cnPresupuestos.PuedeModificarse(presupuesto));
            }
            else if (columna == "icoBaja")
            {
                e.Value = Iconos.Mostrar(Iconos.BAJA, cnPresupuestos.PuedeDarseDeBaja(presupuesto));
            }
            else if (!presupuesto.Activo)
            {
                e.CellStyle.ForeColor = Color.Gray;
            }
        }

        private void dgvPresupuestos_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            Iconos.CambiarCursor(dgvPresupuestos, e.ColumnIndex, e.RowIndex);
        }

        private void dgvPresupuestos_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            dgvPresupuestos.Cursor = Cursors.Default;
        }

        private void dgvPresupuestos_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            e.ToolTipText = Iconos.Ayuda(dgvPresupuestos, e.ColumnIndex, e.RowIndex);
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void VerDetalles(PresupuestoVenta elegido)
        {
            try
            {
                PresupuestoVenta presupuesto = cnPresupuestos.ObtenerPorId(elegido.IdPresupuesto);

                FrmPresupuestoVentaDetalle ventana = new FrmPresupuestoVentaDetalle(presupuesto);
                ventana.ShowDialog();
            }
            catch (Exception ex)
            {
                MostrarAviso(ex.Message);
            }
        }

        private void Editar(PresupuestoVenta elegido)
        {
            try
            {
                PresupuestoVenta presupuesto = cnPresupuestos.ObtenerPorId(elegido.IdPresupuesto);

                FrmPresupuestoVentaDatos ventana = new FrmPresupuestoVentaDatos(presupuesto);
                if (ventana.ShowDialog() == DialogResult.OK)
                {
                    ListarPresupuestos();
                }
            }
            catch (Exception ex)
            {
                MostrarAviso(ex.Message);
            }
        }

        private void Aprobar(PresupuestoVenta presupuesto)
        {
            try
            {
                if (!Confirmar("¿Confirma que el cliente aprobó el presupuesto N° " + presupuesto.IdPresupuesto + "?"))
                {
                    return;
                }

                cnPresupuestos.Aprobar(presupuesto);

                MessageBox.Show("El presupuesto N° " + presupuesto.IdPresupuesto + " quedó Aprobado.", "Aprobar",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                ListarPresupuestos();
            }
            catch (Exception ex)
            {
                MostrarAviso(ex.Message);
            }
        }

        private void Rechazar(PresupuestoVenta presupuesto)
        {
            try
            {
                if (!Confirmar("¿Confirma que el cliente rechazó el presupuesto N° " + presupuesto.IdPresupuesto + "?"))
                {
                    return;
                }

                cnPresupuestos.Rechazar(presupuesto);

                MessageBox.Show("El presupuesto N° " + presupuesto.IdPresupuesto + " quedó Rechazado.", "Rechazar",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                ListarPresupuestos();
            }
            catch (Exception ex)
            {
                MostrarAviso(ex.Message);
            }
        }

        private void DarDeBaja(PresupuestoVenta presupuesto)
        {
            try
            {
                if (!Confirmar("¿Confirma dar de baja el presupuesto N° " + presupuesto.IdPresupuesto + "?"))
                {
                    return;
                }

                cnPresupuestos.Baja(presupuesto);

                MessageBox.Show("El presupuesto fue dado de baja.", "Baja",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                ListarPresupuestos();
            }
            catch (Exception ex)
            {
                MostrarAviso(ex.Message);
            }
        }

        private void CargarFiltros()
        {
            try
            {
                List<ClienteVentas> clientes = cnListas.ListarClientes();
                clientes.Insert(0, new ClienteVentas(0, "(Todos)", ""));
                cboFiltroCliente.DisplayMember = "Nombre";
                cboFiltroCliente.ValueMember = "IdCliente";
                cboFiltroCliente.DataSource = clientes;

                cboFiltroEstado.Items.Clear();
                cboFiltroEstado.Items.Add("(Todos)");
                cboFiltroEstado.Items.Add(PresupuestoVenta.PENDIENTE);
                cboFiltroEstado.Items.Add(PresupuestoVenta.APROBADO);
                cboFiltroEstado.Items.Add(PresupuestoVenta.RECHAZADO);
                cboFiltroEstado.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MostrarAviso("No se pudieron cargar los filtros:\n" + ex.Message);
            }
        }

        private void ListarPresupuestos()
        {
            try
            {
                int? idCliente = null;
                if (cboFiltroCliente.SelectedIndex > 0)
                {
                    ClienteVentas cliente = (ClienteVentas)cboFiltroCliente.SelectedItem;
                    idCliente = cliente.IdCliente;
                }

                string estado = null;
                if (cboFiltroEstado.SelectedIndex > 0)
                {
                    estado = cboFiltroEstado.Text;
                }

                List<PresupuestoVenta> presupuestos = cnPresupuestos.Consultar(idCliente, estado, chkIncluirBajas.Checked);

                dgvPresupuestos.DataSource = null;
                dgvPresupuestos.DataSource = presupuestos;
                dgvPresupuestos.ClearSelection();

                lblCantidadRegistros.ForeColor = SystemColors.ControlText;
                lblCantidadRegistros.Text = "Registros: " + presupuestos.Count;
            }
            catch (Exception ex)
            {
                dgvPresupuestos.DataSource = null;
                lblCantidadRegistros.ForeColor = Color.Firebrick;
                lblCantidadRegistros.Text = ex.Message;
            }
        }

        private bool Confirmar(string pregunta)
        {
            DialogResult respuesta = MessageBox.Show(pregunta, "Confirmar",
                                                     MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return respuesta == DialogResult.Yes;
        }

        private void MostrarAviso(string mensaje)
        {
            MessageBox.Show(mensaje, "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
