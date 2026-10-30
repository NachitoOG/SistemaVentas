using System;
using System.Drawing;
using System.Windows.Forms;

namespace Vista_usuario
{
    public partial class frmHome : Form
    {
        public frmHome()
        {
            InitializeComponent();
        }

        private void frmHome_Load(object sender, EventArgs e)
        {
            lblTitulo.Text = "Hola, " + SesionUsuario.NombreCompleto;
            lblPerfil.Text = "Perfil: " + string.Join(", ", SesionUsuario.Roles);

            bool esAdministrador = SesionUsuario.TieneFamilia(SesionUsuario.FAMILIA_ADMINISTRACION);

            Button[] botones = { btnVentas, btnOtrosModulos, btnAdministrar, btnIrPoliticas, btnConfiguracion, btnCerrarSesion };
            bool[] mostrar =
            {
                SesionUsuario.TieneFamilia(SesionUsuario.FAMILIA_VENTAS),
                SesionUsuario.TieneFamilia(SesionUsuario.FAMILIA_OTROS),
                esAdministrador,
                esAdministrador,
                true,
                true
            };

            int arriba = 96;

            for (int i = 0; i < botones.Length; i++)
            {
                botones[i].Visible = mostrar[i];

                if (mostrar[i])
                {
                    botones[i].Top = arriba;
                    arriba = arriba + 46;
                }
            }

            this.ClientSize = new Size(this.ClientSize.Width, arriba + 6);
        }

        private void btnVentas_Click(object sender, EventArgs e)
        {
            CapaVista.FrmMenuVentas ventana = new CapaVista.FrmMenuVentas(SesionUsuario.IdUsuario);
            ventana.ShowDialog();
        }

        private void btnOtrosModulos_Click(object sender, EventArgs e)
        {
            FrmEnConstruccion ventana = new FrmEnConstruccion();
            ventana.ShowDialog();
        }

        private void btnAdministrar_Click(object sender, EventArgs e)
        {
            Administracion_Usuarios ventana = new Administracion_Usuarios();
            ventana.ShowDialog();
        }

        private void btnIrPoliticas_Click(object sender, EventArgs e)
        {
            frmPoliticas ventana = new frmPoliticas();
            ventana.ShowDialog();
        }

        private void btnConfiguracion_Click(object sender, EventArgs e)
        {
            frmSeguridad ventana = new frmSeguridad();
            ventana.ShowDialog();
        }

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
