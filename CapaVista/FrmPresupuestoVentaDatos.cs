using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CapaEntidades;
using CapaLogica;

namespace CapaVista
{
    public partial class FrmPresupuestoVentaDatos : Form
    {
        private readonly CN_PresupuestosVenta cnPresupuestos = new CN_PresupuestosVenta();
        private readonly CN_ListasVentas cnListas = new CN_ListasVentas();

        private PresupuestoVenta presupuesto;

        public FrmPresupuestoVentaDatos()
        {
            InitializeComponent();
            dgvLineas.AutoGenerateColumns = false;
            presupuesto = new PresupuestoVenta();
        }

        public FrmPresupuestoVentaDatos(PresupuestoVenta presupuestoAModificar)
            : this()
        {
            presupuesto = presupuestoAModificar;
        }

        private void FrmPresupuestoVentaDatos_Load(object sender, EventArgs e)
        {
            dtpFechaEmision.MaxDate = DateTime.Today;
            CargarCombos();

            if (EsNuevo())
            {
                this.Text = "Nuevo presupuesto";
                lblTitulo.Text = "Nuevo presupuesto";
                cboCliente.SelectedIndex = -1;
                cboUsuario.SelectedIndex = -1;
                dtpFechaEmision.Value = DateTime.Today;
            }
            else
            {
                this.Text = "Modificar presupuesto";
                lblTitulo.Text = "Modificar presupuesto N° " + presupuesto.IdPresupuesto;
                cboCliente.SelectedValue = presupuesto.IdCliente;
                cboCliente.Enabled = cnPresupuestos.PuedeCambiarCliente(presupuesto);
                cboUsuario.SelectedValue = presupuesto.IdUsuario;
                dtpFechaEmision.Value = presupuesto.FechaEmision.Date;
                btnGuardar.Text = "Guardar cambios";
            }

            if (SesionVentas.HaySesion())
            {
                if (EsNuevo())
                {
                    cboUsuario.SelectedValue = SesionVentas.UsuarioActual.IdUsuario;
                }
                cboUsuario.Enabled = false;
            }

            MostrarLineas();

            cboCliente.DropDownStyle = ComboBoxStyle.DropDown;
            cboCliente.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboCliente.AutoCompleteSource = AutoCompleteSource.ListItems;

            cboProducto.DropDownStyle = ComboBoxStyle.DropDown;
            cboProducto.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboProducto.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (EsNuevo())
                {
                    PresupuestoVenta nuevo = new PresupuestoVenta(
                        LeerIdCombo(cboCliente),
                        LeerIdCombo(cboUsuario),
                        dtpFechaEmision.Value);
                    nuevo.Lineas = presupuesto.Lineas;

                    int idNuevo = cnPresupuestos.Alta(nuevo);

                    MessageBox.Show("Presupuesto dado de alta correctamente.\nNúmero asignado: " + idNuevo,
                                    "Alta", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    PresupuestoVenta modificado = new PresupuestoVenta(
                        presupuesto.IdPresupuesto,
                        LeerIdCombo(cboCliente),
                        LeerIdCombo(cboUsuario),
                        dtpFechaEmision.Value,
                        presupuesto.Estado,
                        presupuesto.Activo);
                    modificado.Lineas = presupuesto.Lineas;

                    cnPresupuestos.Modificar(modificado);

                    MessageBox.Show("Presupuesto modificado correctamente.", "Modificación",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MostrarAviso(ex.Message);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAgregarLinea_Click(object sender, EventArgs e)
        {
            try
            {
                int idProducto = 0;
                string nombreProducto = "";

                if (cboProducto.SelectedItem != null)
                {
                    ProductoVentas producto = (ProductoVentas)cboProducto.SelectedItem;
                    idProducto = producto.IdProducto;
                    nombreProducto = producto.Descripcion;
                }

                DetallePresupuestoVenta linea = new DetallePresupuestoVenta(
                    idProducto,
                    nombreProducto,
                    Convert.ToInt32(nudCantidad.Value),
                    nudPrecio.Value);

                cnPresupuestos.AgregarLinea(presupuesto, linea);

                nudCantidad.Value = 1;
                MostrarLineas();
            }
            catch (Exception ex)
            {
                MostrarAviso(ex.Message);
            }
        }

        private void dgvLineas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!Iconos.HayIcono(dgvLineas, e.ColumnIndex, e.RowIndex))
            {
                return;
            }

            try
            {
                DetallePresupuestoVenta linea = (DetallePresupuestoVenta)dgvLineas.Rows[e.RowIndex].DataBoundItem;

                cnPresupuestos.QuitarLinea(presupuesto, linea);

                MostrarLineas();
            }
            catch (Exception ex)
            {
                MostrarAviso(ex.Message);
            }
        }

        private void dgvLineas_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvLineas.Columns[e.ColumnIndex].Name == "icoQuitar")
            {
                e.Value = Iconos.BAJA;
            }
        }

        private void dgvLineas_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            Iconos.CambiarCursor(dgvLineas, e.ColumnIndex, e.RowIndex);
        }

        private void dgvLineas_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            dgvLineas.Cursor = Cursors.Default;
        }

        private void dgvLineas_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            e.ToolTipText = Iconos.Ayuda(dgvLineas, e.ColumnIndex, e.RowIndex);
        }

        private void nudCantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void cboProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboProducto.SelectedItem == null)
            {
                return;
            }

            ProductoVentas producto = (ProductoVentas)cboProducto.SelectedItem;
            nudPrecio.Value = producto.PrecioActual;
        }

        private void CargarCombos()
        {
            try
            {
                cboCliente.DisplayMember = "Nombre";
                cboCliente.ValueMember = "IdCliente";
                cboCliente.DataSource = cnListas.ListarClientes();

                cboUsuario.DisplayMember = "NombreCompleto";
                cboUsuario.ValueMember = "IdUsuario";
                cboUsuario.DataSource = cnListas.ListarUsuarios();

                cboProducto.DisplayMember = "Descripcion";
                cboProducto.ValueMember = "IdProducto";
                cboProducto.DataSource = cnListas.ListarProductos();
            }
            catch (Exception ex)
            {
                MostrarAviso("No se pudieron cargar las listas:\n" + ex.Message);
            }
        }

        private void MostrarLineas()
        {
            dgvLineas.DataSource = null;
            dgvLineas.DataSource = new List<DetallePresupuestoVenta>(presupuesto.Lineas);
            dgvLineas.ClearSelection();

            lblTotal.Text = "Total: $ " + presupuesto.CalcularTotal().ToString("N2");
        }

        private bool EsNuevo()
        {
            return presupuesto.IdPresupuesto == 0;
        }

        private int LeerIdCombo(ComboBox combo)
        {
            if (combo.SelectedValue == null)
            {
                return 0;
            }
            return Convert.ToInt32(combo.SelectedValue);
        }

        private void MostrarAviso(string mensaje)
        {
            MessageBox.Show(mensaje, "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
