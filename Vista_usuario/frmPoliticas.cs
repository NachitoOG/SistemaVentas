using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Vista_usuario
{
    public partial class frmPoliticas : Form
    {
        public frmPoliticas()
        {
            InitializeComponent();
        }

        private void frmPoliticas_Load(object sender, EventArgs e)
        {
            CargarPoliticasActuales();
        }

        private void CargarPoliticasActuales()
        {
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                using (SqlCommand comando = new SqlCommand("sp_ObtenerPoliticasSeguridad", conexion))
                {
                    comando.CommandType = CommandType.StoredProcedure;

                    try
                    {
                        conexion.Open();
                        using (SqlDataReader lector = comando.ExecuteReader())
                        {
                            if (lector.Read())
                            {
                                chkMayusMinus.Checked = Convert.ToBoolean(lector["requiere_mayus_minus"]);
                                chkAlfanumerico.Checked = Convert.ToBoolean(lector["requiere_alfanumerico"]);
                                chkEspeciales.Checked = Convert.ToBoolean(lector["requiere_caracter_especial"]);
                                chk2FA.Checked = Convert.ToBoolean(lector["requiere_2fa_correo"]);
                                chkBloquearAnteriores.Checked = Convert.ToBoolean(lector["bloquear_contrasenas_anteriores"]);
                                chkBloqueardatospersonales.Checked = Convert.ToBoolean(lector["bloquear_datos_personales"]);

                                ActualizarMinimo();
                                nudMinCaracteres.Value = Math.Max(Convert.ToDecimal(lector["min_caracteres"]), nudMinCaracteres.Minimum);
                                nudCantPreguntas.Value = Math.Min(Math.Max(Convert.ToDecimal(lector["cant_preguntas_validar"]), 1), 5);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al cargar las políticas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                using (SqlCommand comando = new SqlCommand("sp_ActualizarPoliticasSeguridad", conexion))
                {
                    comando.CommandType = CommandType.StoredProcedure;

                    comando.Parameters.AddWithValue("@min_caracteres", (int)nudMinCaracteres.Value);
                    comando.Parameters.AddWithValue("@cant_preguntas_validar", (int)nudCantPreguntas.Value);
                    comando.Parameters.AddWithValue("@requiere_mayus_minus", chkMayusMinus.Checked);
                    comando.Parameters.AddWithValue("@requiere_alfanumerico", chkAlfanumerico.Checked);
                    comando.Parameters.AddWithValue("@requiere_caracter_especial", chkEspeciales.Checked);
                    comando.Parameters.AddWithValue("@requiere_2fa_correo", chk2FA.Checked);
                    comando.Parameters.AddWithValue("@bloquear_contrasenas_anteriores", chkBloquearAnteriores.Checked);
                    comando.Parameters.AddWithValue("@bloquear_datos_personales", chkBloqueardatospersonales.Checked);

                    try
                    {
                        conexion.Open();
                        comando.ExecuteNonQuery();

                        MessageBox.Show("Políticas de seguridad actualizadas.", "Configuración guardada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al guardar los cambios: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Opciones_Cambiaron(object sender, EventArgs e)
        {
            ActualizarMinimo();
        }

        private void ActualizarMinimo()
        {
            int minimo = 1;

            if (chkMayusMinus.Checked)
            {
                minimo = minimo + 1;
            }

            if (chkAlfanumerico.Checked)
            {
                minimo = minimo + 1;
            }

            if (chkEspeciales.Checked)
            {
                minimo = minimo + 1;
            }

            nudMinCaracteres.Minimum = minimo;
            lblMinimoAyuda.Text = "(con estas opciones, no menos de " + minimo + ")";
        }
    }
}
