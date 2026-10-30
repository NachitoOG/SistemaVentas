using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CapaEntidades;
using CapaLogica;

namespace CapaVista
{
    public partial class FrmAvisos : Form
    {
        private readonly CN_Avisos cnAvisos = new CN_Avisos();

        private bool cargando;
        private bool puedeOperar;

        public FrmAvisos()
        {
            InitializeComponent();
            dgvAvisos.AutoGenerateColumns = false;
        }

        private void FrmAvisos_Load(object sender, EventArgs e)
        {
            cargando = true;
            puedeOperar = PuedeOperar();
            btnNuevo.Enabled = puedeOperar;
            dtpDesde.Value = DateTime.Today.AddMonths(-1);
            dtpHasta.Value = DateTime.Today;
            cargando = false;

            ListarAvisos();
        }

        private void Filtros_Cambiaron(object sender, EventArgs e)
        {
            if (cargando)
            {
                return;
            }

            ListarAvisos();
        }

        private void chkDesde_CheckedChanged(object sender, EventArgs e)
        {
            dtpDesde.Enabled = chkDesde.Checked;
            Filtros_Cambiaron(sender, e);
        }

        private void chkHasta_CheckedChanged(object sender, EventArgs e)
        {
            dtpHasta.Enabled = chkHasta.Checked;
            Filtros_Cambiaron(sender, e);
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            FrmAvisoDatos ventana = new FrmAvisoDatos();

            if (ventana.ShowDialog() == DialogResult.OK)
            {
                ListarAvisos();
            }
        }

        private void dgvAvisos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!Iconos.HayIcono(dgvAvisos, e.ColumnIndex, e.RowIndex))
            {
                return;
            }

            AvisoMercaderia aviso = (AvisoMercaderia)dgvAvisos.Rows[e.RowIndex].DataBoundItem;
            string columna = dgvAvisos.Columns[e.ColumnIndex].Name;

            if (columna == "icoDetalles")
            {
                VerDetalles(aviso);
            }
            else if (columna == "icoEditar")
            {
                Editar(aviso);
            }
            else if (columna == "icoBaja")
            {
                DarDeBaja(aviso);
            }
        }

        private void dgvAvisos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            AvisoMercaderia aviso = dgvAvisos.Rows[e.RowIndex].DataBoundItem as AvisoMercaderia;
            if (aviso == null)
            {
                return;
            }

            string columna = dgvAvisos.Columns[e.ColumnIndex].Name;

            if (columna == "icoDetalles")
            {
                e.Value = Iconos.DETALLES;
            }
            else if (columna == "icoEditar")
            {
                e.Value = Iconos.Mostrar(Iconos.EDITAR, puedeOperar && cnAvisos.PuedeModificarse(aviso));
            }
            else if (columna == "icoBaja")
            {
                e.Value = Iconos.Mostrar(Iconos.BAJA, puedeOperar && cnAvisos.PuedeDarseDeBaja(aviso));
            }
            else if (!aviso.Activo)
            {
                e.CellStyle.ForeColor = Color.Gray;
            }
        }

        private void dgvAvisos_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            Iconos.CambiarCursor(dgvAvisos, e.ColumnIndex, e.RowIndex);
        }

        private void dgvAvisos_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            dgvAvisos.Cursor = Cursors.Default;
        }

        private void dgvAvisos_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            e.ToolTipText = Iconos.Ayuda(dgvAvisos, e.ColumnIndex, e.RowIndex);
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void VerDetalles(AvisoMercaderia elegido)
        {
            try
            {
                AvisoMercaderia aviso = cnAvisos.ObtenerPorId(elegido.IdAviso);
                List<DetalleNotaPedido> lineas = cnAvisos.ListarLineasDelPedido(aviso.IdNotaPedido);

                FrmAvisoDetalle ventana = new FrmAvisoDetalle(aviso, lineas);
                ventana.ShowDialog();
            }
            catch (Exception ex)
            {
                MostrarMensaje(ex.Message);
            }
        }

        private void Editar(AvisoMercaderia elegido)
        {
            try
            {
                AvisoMercaderia aviso = cnAvisos.ObtenerPorId(elegido.IdAviso);

                FrmAvisoDatos ventana = new FrmAvisoDatos(aviso);
                if (ventana.ShowDialog() == DialogResult.OK)
                {
                    ListarAvisos();
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje(ex.Message);
            }
        }

        private void DarDeBaja(AvisoMercaderia aviso)
        {
            try
            {
                if (!Confirmar("¿Confirma dar de baja el aviso N° " + aviso.IdAviso + "?\n" +
                               "El pedido N° " + aviso.IdNotaPedido + " va a volver a Pendiente."))
                {
                    return;
                }

                cnAvisos.Baja(aviso);

                MessageBox.Show("El aviso fue dado de baja y el pedido volvió a Pendiente.", "Baja",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                ListarAvisos();
            }
            catch (Exception ex)
            {
                MostrarMensaje(ex.Message);
            }
        }

        private void ListarAvisos()
        {
            try
            {
                DateTime? desde = null;
                if (chkDesde.Checked)
                {
                    desde = dtpDesde.Value.Date;
                }

                DateTime? hasta = null;
                if (chkHasta.Checked)
                {
                    hasta = dtpHasta.Value.Date;
                }

                List<AvisoMercaderia> avisos = cnAvisos.Consultar(desde, hasta, chkIncluirBajas.Checked);

                dgvAvisos.DataSource = null;
                dgvAvisos.DataSource = avisos;
                dgvAvisos.ClearSelection();

                lblCantidadRegistros.ForeColor = SystemColors.ControlText;
                lblCantidadRegistros.Text = "Registros: " + avisos.Count;
                lblPendientes.Text = "Pedidos esperando preparación: " + cnAvisos.ListarPedidosPendientes().Count;
                if (!puedeOperar)
                {
                    lblPendientes.Text = "Solo Depósito puede cargar avisos";
                }
            }
            catch (Exception ex)
            {
                dgvAvisos.DataSource = null;
                lblCantidadRegistros.ForeColor = Color.Firebrick;
                lblCantidadRegistros.Text = ex.Message;
            }
        }

        private bool PuedeOperar()
        {
            if (!SesionVentas.HaySesion())
            {
                return true;
            }

            try
            {
                return cnAvisos.EsDeDeposito(SesionVentas.UsuarioActual.IdUsuario);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool Confirmar(string pregunta)
        {
            DialogResult respuesta = MessageBox.Show(pregunta, "Confirmar",
                                                     MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return respuesta == DialogResult.Yes;
        }

        private void MostrarMensaje(string mensaje)
        {
            MessageBox.Show(mensaje, "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
