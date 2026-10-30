using System;
using System.Windows.Forms;
using CapaEntidades;

namespace CapaVista
{
    public partial class FrmPresupuestoVentaDetalle : Form
    {
        private readonly PresupuestoVenta presupuesto;

        public FrmPresupuestoVentaDetalle(PresupuestoVenta presupuesto)
        {
            InitializeComponent();
            dgvLineas.AutoGenerateColumns = false;
            this.presupuesto = presupuesto;
        }

        private void FrmPresupuestoVentaDetalle_Load(object sender, EventArgs e)
        {
            lblTitulo.Text = "Presupuesto N° " + presupuesto.IdPresupuesto;
            lblCliente.Text = presupuesto.NombreCliente;
            lblEstado.Text = presupuesto.EstadoTexto;
            lblUsuario.Text = presupuesto.NombreUsuario;
            lblFecha.Text = presupuesto.FechaEmision.ToString("dd/MM/yyyy");

            dgvLineas.DataSource = presupuesto.Lineas;
            dgvLineas.ClearSelection();

            lblTotal.Text = "Total: $ " + presupuesto.CalcularTotal().ToString("N2");
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
