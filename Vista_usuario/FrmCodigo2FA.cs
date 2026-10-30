using System;
using System.Windows.Forms;

namespace Vista_usuario
{
    public partial class FrmCodigo2FA : Form
    {
        private const int INTENTOS_PERMITIDOS = 3;

        private readonly string codigoEnviado;
        private int intentos;

        public FrmCodigo2FA(string codigo, string correo)
        {
            InitializeComponent();
            codigoEnviado = codigo;
            lblExplicacion.Text = "Te mandamos un código de 6 números a " + Utilidades.OcultarCorreo(correo) +
                                  ". Escribilo para terminar de entrar.";
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            if (txtCodigo.Text.Trim() == codigoEnviado)
            {
                this.DialogResult = DialogResult.OK;
                return;
            }

            intentos++;

            if (intentos >= INTENTOS_PERMITIDOS)
            {
                MessageBox.Show("Te equivocaste " + INTENTOS_PERMITIDOS + " veces. Volvé a iniciar sesión.", "Código incorrecto",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.Cancel;
                return;
            }

            lblMensaje.Text = "El código no es correcto. Te quedan " + (INTENTOS_PERMITIDOS - intentos) + " intentos.";
            txtCodigo.SelectAll();
            txtCodigo.Focus();
        }

        private void txtCodigo_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utilidades.SoloNumeros(e);
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
