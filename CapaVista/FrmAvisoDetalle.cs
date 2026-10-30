using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CapaEntidades;

namespace CapaVista
{
    public partial class FrmAvisoDetalle : Form
    {
        private readonly AvisoMercaderia aviso;
        private readonly List<DetalleNotaPedido> lineas;

        public FrmAvisoDetalle(AvisoMercaderia aviso, List<DetalleNotaPedido> lineas)
        {
            InitializeComponent();
            dgvLineas.AutoGenerateColumns = false;
            this.aviso = aviso;
            this.lineas = lineas;
        }

        private void FrmAvisoDetalle_Load(object sender, EventArgs e)
        {
            lblTitulo.Text = "Aviso N° " + aviso.IdAviso;
            lblPedido.Text = "N° " + aviso.IdNotaPedido;
            lblEstadoPedido.Text = aviso.EstadoPedido;
            lblCliente.Text = aviso.NombreCliente;
            lblEstadoAviso.Text = aviso.EstadoTexto;
            lblUsuario.Text = aviso.NombreUsuario;
            lblHoras.Text = aviso.HorasPreparacion.ToString("N2") + " horas";
            lblFechaPedido.Text = aviso.FechaPedido.ToString("dd/MM/yyyy HH:mm");
            lblFechaAviso.Text = aviso.FechaEmision.ToString("dd/MM/yyyy HH:mm");

            dgvLineas.DataSource = lineas;
            dgvLineas.ClearSelection();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
