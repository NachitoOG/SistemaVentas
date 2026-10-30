using System;
using System.Windows.Forms;
using CapaLogica;

namespace CapaVista
{
    public partial class FrmMenuVentas : Form
    {
        private readonly CN_ListasVentas cnListas = new CN_ListasVentas();

        private int idUsuarioSesion;

        public FrmMenuVentas()
        {
            InitializeComponent();
        }

        public FrmMenuVentas(int idUsuario)
            : this()
        {
            idUsuarioSesion = idUsuario;
        }

        private void FrmMenuVentas_Load(object sender, EventArgs e)
        {
            if (idUsuarioSesion == 0)
            {
                lblUsuario.Text = "Sin iniciar sesión (modo de prueba)";
                return;
            }

            try
            {
                SesionVentas.UsuarioActual = cnListas.ObtenerUsuario(idUsuarioSesion);
                lblUsuario.Text = "Usuario: " + SesionVentas.UsuarioActual.NombreCompleto;
                AcomodarMenu();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
            }
        }

        private void AcomodarMenu()
        {
            bool esDeposito = SesionVentas.UsuarioActual.EsDeposito;

            Button[] botones = { btnPresupuestos, btnNotasPedido, btnAvisos, btnReportes };
            bool[] mostrar = { !esDeposito, !esDeposito, esDeposito, true };

            int arriba = 92;

            for (int i = 0; i < botones.Length; i++)
            {
                botones[i].Visible = mostrar[i];

                if (mostrar[i])
                {
                    botones[i].Top = arriba;
                    arriba = arriba + 48;
                }
            }

            btnSalir.Top = arriba + 12;
            lblUsuario.Top = arriba + 19;
            this.ClientSize = new System.Drawing.Size(this.ClientSize.Width, arriba + 60);
        }

        private void FrmMenuVentas_FormClosed(object sender, FormClosedEventArgs e)
        {
            SesionVentas.Cerrar();
        }

        private void btnPresupuestos_Click(object sender, EventArgs e)
        {
            FrmPresupuestosVenta ventana = new FrmPresupuestosVenta();
            ventana.ShowDialog();
        }

        private void btnNotasPedido_Click(object sender, EventArgs e)
        {
            FrmNotasPedido ventana = new FrmNotasPedido();
            ventana.ShowDialog();
        }

        private void btnAvisos_Click(object sender, EventArgs e)
        {
            FrmAvisos ventana = new FrmAvisos();
            ventana.ShowDialog();
        }

        private void btnReportes_Click(object sender, EventArgs e)
        {
            FrmReportesVentas ventana = new FrmReportesVentas();
            ventana.ShowDialog();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
