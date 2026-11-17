using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CapaEntidades;
using CapaLogica;

namespace CapaVista
{
    public partial class FrmNotaPedidoDatos : Form
    {
        private readonly CN_NotasPedido cnNotas = new CN_NotasPedido();
        private readonly CN_PresupuestosVenta cnPresupuestos = new CN_PresupuestosVenta();
        private readonly CN_ListasVentas cnListas = new CN_ListasVentas();

        private NotaPedido nota;
        private PresupuestoVenta presupuestoElegido;
        private bool cargando;

        public FrmNotaPedidoDatos()
        {
            InitializeComponent();
            dgvLineas.AutoGenerateColumns = false;
            nota = new NotaPedido();
        }

        public FrmNotaPedidoDatos(NotaPedido notaAModificar)
            : this()
        {
            nota = notaAModificar;
        }

        private void FrmNotaPedidoDatos_Load(object sender, EventArgs e)
        {
            cargando = true;
            dtpFechaEmision.MaxDate = DateTime.Today;
            CargarCombos();

            if (EsNueva())
            {
                this.Text = "Nueva nota de pedido";
                lblTitulo.Text = "Nueva nota de pedido";
                CargarPresupuestosDisponibles();
                rdbDirecto.Checked = true;
                cboPresupuesto.Enabled = false;
                cboCliente.SelectedIndex = -1;
                cboUsuario.SelectedIndex = -1;
                dtpFechaEmision.Value = DateTime.Today;
            }
            else
            {
                this.Text = "Modificar nota de pedido";
                lblTitulo.Text = "Modificar nota de pedido N° " + nota.IdNotaPedido;
                rdbDirecto.Visible = false;
                rdbDesdePresupuesto.Visible = false;
                cboPresupuesto.Visible = false;
                lblOrigen.Text = "Origen: " + nota.Origen;
                lblOrigen.Visible = true;
                cboCliente.SelectedValue = nota.IdCliente;
                cboCliente.Enabled = cnNotas.PuedeCambiarCliente(nota);
                cboUsuario.SelectedValue = nota.IdUsuario;
                dtpFechaEmision.Value = nota.FechaEmision.Date;
                btnGuardar.Text = "Guardar cambios";
            }

            if (SesionVentas.HaySesion())
            {
                if (EsNueva())
                {
                    cboUsuario.SelectedValue = SesionVentas.UsuarioActual.IdUsuario;
                }
                cboUsuario.Enabled = false;
            }

            cargando = false;
            MostrarDeuda();
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
                if (EsNueva() && rdbDesdePresupuesto.Checked)
                {
                    int idNuevo = cnNotas.GenerarDesdePresupuesto(presupuestoElegido, LeerIdCombo(cboUsuario));

                    MessageBox.Show("Nota de pedido generada correctamente.\nNúmero asignado: " + idNuevo +
                                    "\nSe copiaron los productos y los precios del presupuesto.",
                                    "Alta", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (EsNueva())
                {
                    NotaPedido nueva = new NotaPedido(
                        LeerIdCombo(cboCliente),
                        LeerIdCombo(cboUsuario),
                        LeerFechaEmision());
                    nueva.Lineas = nota.Lineas;

                    int idNuevo = cnNotas.Alta(nueva);

                    MessageBox.Show("Nota de pedido dada de alta correctamente.\nNúmero asignado: " + idNuevo,
                                    "Alta", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    NotaPedido modificada = new NotaPedido(
                        nota.IdNotaPedido,
                        LeerIdCombo(cboCliente),
                        LeerIdCombo(cboUsuario),
                        nota.IdPresupuesto,
                        LeerFechaEmision(),
                        nota.Estado,
                        nota.Activo);
                    modificada.Lineas = nota.Lineas;

                    cnNotas.Modificar(modificada);

                    MessageBox.Show("Nota de pedido modificada correctamente.", "Modificación",
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

        private void rdbDesdePresupuesto_CheckedChanged(object sender, EventArgs e)
        {
            if (cargando)
            {
                return;
            }

            cboPresupuesto.Enabled = rdbDesdePresupuesto.Checked;

            if (rdbDesdePresupuesto.Checked)
            {
                CargarPresupuestoElegido();
            }
            else
            {
                presupuestoElegido = null;
                nota.Lineas = new List<DetalleNotaPedido>();
                cboCliente.SelectedIndex = -1;
                HabilitarCarga(true);
                MostrarLineas();
            }
        }

        private void cboPresupuesto_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cargando || !rdbDesdePresupuesto.Checked)
            {
                return;
            }

            CargarPresupuestoElegido();
        }

        private void cboCliente_SelectedIndexChanged(object sender, EventArgs e)
        {
            MostrarDeuda();
        }

        private void cboCliente_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            if (e.Index < 0)
            {
                return;
            }

            ClienteVentas cliente = (ClienteVentas)cboCliente.Items[e.Index];
            string texto = cliente.Nombre;
            Brush pincel = Brushes.Black;

            if (cliente.EsDeudor)
            {
                texto = texto + "   (debe $ " + cliente.Deuda.ToString("N2") + ")";
                pincel = Brushes.Red;
            }

            if (e.State.HasFlag(DrawItemState.Selected))
            {
                pincel = Brushes.White;
            }

            e.Graphics.DrawString(texto, e.Font, pincel, e.Bounds.X, e.Bounds.Y + 1);
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

                DetalleNotaPedido linea = new DetalleNotaPedido(
                    idProducto,
                    nombreProducto,
                    Convert.ToInt32(nudCantidad.Value),
                    nudPrecio.Value);

                cnNotas.AgregarLinea(nota, linea);

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
                DetalleNotaPedido linea = (DetalleNotaPedido)dgvLineas.Rows[e.RowIndex].DataBoundItem;

                cnNotas.QuitarLinea(nota, linea);

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
                e.Value = Iconos.Mostrar(Iconos.BAJA, presupuestoElegido == null);
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

        private void CargarPresupuestosDisponibles()
        {
            try
            {
                List<PresupuestoVenta> disponibles = cnNotas.ListarPresupuestosDisponibles();
                cboPresupuesto.DataSource = disponibles;

                if (disponibles.Count == 0)
                {
                    rdbDesdePresupuesto.Enabled = false;
                    rdbDesdePresupuesto.Text = "Desde un presupuesto aprobado (no hay ninguno disponible)";
                    cboPresupuesto.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MostrarAviso("No se pudieron cargar los presupuestos aprobados:\n" + ex.Message);
            }
        }

        private void CargarPresupuestoElegido()
        {
            try
            {
                presupuestoElegido = null;
                nota.Lineas = new List<DetalleNotaPedido>();

                if (cboPresupuesto.SelectedItem != null)
                {
                    PresupuestoVenta elegido = (PresupuestoVenta)cboPresupuesto.SelectedItem;
                    presupuestoElegido = cnPresupuestos.ObtenerPorId(elegido.IdPresupuesto);

                    cboCliente.SelectedValue = presupuestoElegido.IdCliente;

                    foreach (DetallePresupuestoVenta linea in presupuestoElegido.Lineas)
                    {
                        nota.Lineas.Add(new DetalleNotaPedido(linea.IdProducto, linea.NombreProducto,
                                                              linea.Cantidad, linea.PrecioUnitario));
                    }
                }

                dtpFechaEmision.Value = DateTime.Today;
                HabilitarCarga(false);
                MostrarLineas();
            }
            catch (Exception ex)
            {
                MostrarAviso(ex.Message);
            }
        }

        private void HabilitarCarga(bool habilitar)
        {
            cboCliente.Enabled = habilitar;
            dtpFechaEmision.Enabled = habilitar;
            cboProducto.Enabled = habilitar;
            nudCantidad.Enabled = habilitar;
            nudPrecio.Enabled = habilitar;
            btnAgregarLinea.Enabled = habilitar;
        }

        private void MostrarDeuda()
        {
            lblDeuda.Visible = false;

            ClienteVentas cliente = cboCliente.SelectedItem as ClienteVentas;
            if (cliente != null && cliente.EsDeudor)
            {
                lblDeuda.Text = "Este cliente debe $ " + cliente.Deuda.ToString("N2") +
                                " de pedidos entregados que todavía no se facturaron.";
                lblDeuda.Visible = true;
            }
        }

        private void MostrarLineas()
        {
            dgvLineas.DataSource = null;
            dgvLineas.DataSource = new List<DetalleNotaPedido>(nota.Lineas);
            dgvLineas.ClearSelection();

            lblTotal.Text = "Total: $ " + nota.CalcularTotal().ToString("N2");
        }

        private bool EsNueva()
        {
            return nota.IdNotaPedido == 0;
        }

        private DateTime LeerFechaEmision()
        {
            DateTime elegida = dtpFechaEmision.Value.Date;

            if (elegida == nota.FechaEmision.Date)
            {
                return nota.FechaEmision;
            }

            if (elegida == DateTime.Today)
            {
                return DateTime.Now;
            }

            return elegida;
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
