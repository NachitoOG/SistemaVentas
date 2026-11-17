using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CapaEntidades;
using CapaLogica;

namespace CapaVista
{
    public partial class FrmNotasPedido : Form
    {
        private readonly CN_NotasPedido cnNotas = new CN_NotasPedido();
        private readonly CN_ListasVentas cnListas = new CN_ListasVentas();

        private bool cargando;

        public FrmNotasPedido()
        {
            InitializeComponent();
            dgvNotas.AutoGenerateColumns = false;
        }

        private void FrmNotasPedido_Load(object sender, EventArgs e)
        {
            cargando = true;
            CargarFiltros();
            cargando = false;

            ListarNotas();

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

            ListarNotas();
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            FrmNotaPedidoDatos ventana = new FrmNotaPedidoDatos();

            if (ventana.ShowDialog() == DialogResult.OK)
            {
                ListarNotas();
            }
        }

        private void dgvNotas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!Iconos.HayIcono(dgvNotas, e.ColumnIndex, e.RowIndex))
            {
                return;
            }

            NotaPedido nota = (NotaPedido)dgvNotas.Rows[e.RowIndex].DataBoundItem;
            string columna = dgvNotas.Columns[e.ColumnIndex].Name;

            if (columna == "icoDetalles")
            {
                VerDetalles(nota);
            }
            else if (columna == "icoEditar")
            {
                Editar(nota);
            }
            else if (columna == "icoEntregado")
            {
                MarcarEntregado(nota);
            }
            else if (columna == "icoFacturado")
            {
                MarcarFacturado(nota);
            }
            else if (columna == "icoBaja")
            {
                DarDeBaja(nota);
            }
        }

        private void dgvNotas_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            NotaPedido nota = dgvNotas.Rows[e.RowIndex].DataBoundItem as NotaPedido;
            if (nota == null)
            {
                return;
            }

            string columna = dgvNotas.Columns[e.ColumnIndex].Name;

            if (columna == "icoDetalles")
            {
                e.Value = Iconos.DETALLES;
            }
            else if (columna == "icoEditar")
            {
                e.Value = Iconos.Mostrar(Iconos.EDITAR, cnNotas.PuedeModificarse(nota));
            }
            else if (columna == "icoEntregado")
            {
                e.Value = Iconos.Mostrar(Iconos.ENTREGADO, cnNotas.PuedePasarA(nota, NotaPedido.ENTREGADO));
            }
            else if (columna == "icoFacturado")
            {
                e.Value = Iconos.Mostrar(Iconos.FACTURADO, cnNotas.PuedePasarA(nota, NotaPedido.FACTURADO));
            }
            else if (columna == "icoBaja")
            {
                e.Value = Iconos.Mostrar(Iconos.BAJA, cnNotas.PuedeDarseDeBaja(nota));
            }
            else if (!nota.Activo)
            {
                e.CellStyle.ForeColor = Color.Gray;
            }
        }

        private void dgvNotas_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            Iconos.CambiarCursor(dgvNotas, e.ColumnIndex, e.RowIndex);
        }

        private void dgvNotas_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            dgvNotas.Cursor = Cursors.Default;
        }

        private void dgvNotas_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            e.ToolTipText = Iconos.Ayuda(dgvNotas, e.ColumnIndex, e.RowIndex);
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void VerDetalles(NotaPedido elegida)
        {
            try
            {
                NotaPedido nota = cnNotas.ObtenerPorId(elegida.IdNotaPedido);

                FrmNotaPedidoDetalle ventana = new FrmNotaPedidoDetalle(nota);
                ventana.ShowDialog();
            }
            catch (Exception ex)
            {
                MostrarAviso(ex.Message);
            }
        }

        private void Editar(NotaPedido elegida)
        {
            try
            {
                NotaPedido nota = cnNotas.ObtenerPorId(elegida.IdNotaPedido);

                FrmNotaPedidoDatos ventana = new FrmNotaPedidoDatos(nota);
                if (ventana.ShowDialog() == DialogResult.OK)
                {
                    ListarNotas();
                }
            }
            catch (Exception ex)
            {
                MostrarAviso(ex.Message);
            }
        }

        private void MarcarEntregado(NotaPedido nota)
        {
            try
            {
                if (!Confirmar("¿Confirma que el pedido N° " + nota.IdNotaPedido + " fue entregado?"))
                {
                    return;
                }

                cnNotas.MarcarEntregado(nota);

                MessageBox.Show("El pedido N° " + nota.IdNotaPedido + " quedó Entregado.", "Entregado",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                ListarNotas();
            }
            catch (Exception ex)
            {
                MostrarAviso(ex.Message);
            }
        }

        private void MarcarFacturado(NotaPedido nota)
        {
            try
            {
                if (!Confirmar("¿Confirma que el pedido N° " + nota.IdNotaPedido + " fue facturado?"))
                {
                    return;
                }

                cnNotas.MarcarFacturado(nota);

                MessageBox.Show("El pedido N° " + nota.IdNotaPedido + " quedó Facturado.", "Facturado",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                ListarNotas();
            }
            catch (Exception ex)
            {
                MostrarAviso(ex.Message);
            }
        }

        private void DarDeBaja(NotaPedido nota)
        {
            try
            {
                if (!Confirmar("¿Confirma dar de baja la nota de pedido N° " + nota.IdNotaPedido + "?"))
                {
                    return;
                }

                cnNotas.Baja(nota);

                MessageBox.Show("La nota de pedido fue dada de baja.", "Baja",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                ListarNotas();
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
                cboFiltroEstado.Items.Add(NotaPedido.PENDIENTE);
                cboFiltroEstado.Items.Add(NotaPedido.PREPARADO);
                cboFiltroEstado.Items.Add(NotaPedido.ENTREGADO);
                cboFiltroEstado.Items.Add(NotaPedido.FACTURADO);
                cboFiltroEstado.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MostrarAviso("No se pudieron cargar los filtros:\n" + ex.Message);
            }
        }

        private void ListarNotas()
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

                List<NotaPedido> notas = cnNotas.Consultar(idCliente, estado, chkIncluirBajas.Checked);

                dgvNotas.DataSource = null;
                dgvNotas.DataSource = notas;
                dgvNotas.ClearSelection();

                lblCantidadRegistros.ForeColor = SystemColors.ControlText;
                lblCantidadRegistros.Text = "Registros: " + notas.Count;
            }
            catch (Exception ex)
            {
                dgvNotas.DataSource = null;
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
