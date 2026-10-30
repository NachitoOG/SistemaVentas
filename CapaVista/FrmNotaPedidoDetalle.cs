using System;
using System.Windows.Forms;
using CapaEntidades;

namespace CapaVista
{
    public partial class FrmNotaPedidoDetalle : Form
    {
        private readonly NotaPedido nota;

        public FrmNotaPedidoDetalle(NotaPedido nota)
        {
            InitializeComponent();
            dgvLineas.AutoGenerateColumns = false;
            this.nota = nota;
        }

        private void FrmNotaPedidoDetalle_Load(object sender, EventArgs e)
        {
            lblTitulo.Text = "Nota de pedido N° " + nota.IdNotaPedido;
            lblCliente.Text = nota.NombreCliente;
            lblEstado.Text = nota.EstadoTexto;
            lblUsuario.Text = nota.NombreUsuario;
            lblFecha.Text = nota.FechaEmision.ToString("dd/MM/yyyy HH:mm");
            lblOrigen.Text = nota.Origen;

            dgvLineas.DataSource = nota.Lineas;
            dgvLineas.ClearSelection();

            lblTotal.Text = "Total: $ " + nota.CalcularTotal().ToString("N2");
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
