using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CapaEntidades;
using CapaLogica;

namespace CapaVista
{
    public partial class FrmAvisoDatos : Form
    {
        private readonly CN_Avisos cnAvisos = new CN_Avisos();
        private readonly CN_NotasPedido cnNotas = new CN_NotasPedido();

        private AvisoMercaderia aviso;
        private NotaPedido pedidoElegido;

        public FrmAvisoDatos()
        {
            InitializeComponent();
            dgvPendientes.AutoGenerateColumns = false;
            dgvLineasPedido.AutoGenerateColumns = false;
            aviso = new AvisoMercaderia();
        }

        public FrmAvisoDatos(AvisoMercaderia avisoAModificar)
            : this()
        {
            aviso = avisoAModificar;
        }

        private void FrmAvisoDatos_Load(object sender, EventArgs e)
        {
            CargarEmpleados();

            if (EsNuevo())
            {
                this.Text = "Nuevo aviso";
                lblTitulo.Text = "Nuevo aviso de mercadería preparada";
                ListarPendientes();
                cboUsuario.SelectedIndex = -1;
                dtpFechaEmision.Value = DateTime.Now;
            }
            else
            {
                this.Text = "Modificar aviso";
                lblTitulo.Text = "Modificar aviso N° " + aviso.IdAviso;
                grpPedido.Text = "Pedido del aviso";
                MostrarPedidoDelAviso();
                cboUsuario.SelectedValue = aviso.IdUsuario;
                dtpFechaEmision.Value = aviso.FechaEmision;
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
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (EsNuevo())
                {
                    int idPedido = 0;
                    if (pedidoElegido != null)
                    {
                        idPedido = pedidoElegido.IdNotaPedido;
                    }

                    AvisoMercaderia nuevo = new AvisoMercaderia(
                        idPedido,
                        LeerIdCombo(cboUsuario),
                        dtpFechaEmision.Value);

                    int idNuevo = cnAvisos.Alta(nuevo);

                    MessageBox.Show("Aviso dado de alta correctamente.\nNúmero asignado: " + idNuevo +
                                    "\nEl pedido N° " + idPedido + " pasó a Preparado.",
                                    "Alta", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    AvisoMercaderia modificado = new AvisoMercaderia(
                        aviso.IdAviso,
                        aviso.IdNotaPedido,
                        LeerIdCombo(cboUsuario),
                        dtpFechaEmision.Value,
                        aviso.Activo);

                    cnAvisos.Modificar(modificado);

                    MessageBox.Show("Aviso modificado correctamente.", "Modificación",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MostrarMensaje(ex.Message);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvPendientes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || !EsNuevo())
            {
                return;
            }

            pedidoElegido = (NotaPedido)dgvPendientes.Rows[e.RowIndex].DataBoundItem;
            MostrarLineasDelPedido(pedidoElegido.IdNotaPedido);
        }

        private void CargarEmpleados()
        {
            try
            {
                cboUsuario.DisplayMember = "NombreCompleto";
                cboUsuario.ValueMember = "IdUsuario";
                cboUsuario.DataSource = cnAvisos.ListarEmpleadosDeposito();
            }
            catch (Exception ex)
            {
                MostrarMensaje("No se pudo cargar la lista de empleados:\n" + ex.Message);
            }
        }

        private void ListarPendientes()
        {
            try
            {
                List<NotaPedido> pendientes = cnAvisos.ListarPedidosPendientes();

                dgvPendientes.DataSource = pendientes;
                dgvPendientes.ClearSelection();

                if (pendientes.Count == 0)
                {
                    grpPedido.Text = "1. No hay pedidos pendientes de preparar";
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("No se pudieron obtener los pedidos pendientes:\n" + ex.Message);
            }
        }

        private void MostrarPedidoDelAviso()
        {
            try
            {
                List<NotaPedido> pedido = new List<NotaPedido>();
                pedido.Add(cnNotas.ObtenerPorId(aviso.IdNotaPedido));

                dgvPendientes.DataSource = pedido;
                dgvPendientes.ClearSelection();
                dgvPendientes.Enabled = false;

                MostrarLineasDelPedido(aviso.IdNotaPedido);
            }
            catch (Exception ex)
            {
                MostrarMensaje(ex.Message);
            }
        }

        private void MostrarLineasDelPedido(int idNotaPedido)
        {
            try
            {
                dgvLineasPedido.DataSource = cnAvisos.ListarLineasDelPedido(idNotaPedido);
                dgvLineasPedido.ClearSelection();
            }
            catch (Exception ex)
            {
                MostrarMensaje(ex.Message);
            }
        }

        private bool EsNuevo()
        {
            return aviso.IdAviso == 0;
        }

        private int LeerIdCombo(ComboBox combo)
        {
            if (combo.SelectedValue == null)
            {
                return 0;
            }
            return Convert.ToInt32(combo.SelectedValue);
        }

        private void MostrarMensaje(string mensaje)
        {
            MessageBox.Show(mensaje, "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
