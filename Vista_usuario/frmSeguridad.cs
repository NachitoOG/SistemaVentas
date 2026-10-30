using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Vista_usuario
{
    public partial class frmSeguridad : Form
    {
        public frmSeguridad()
        {
            InitializeComponent();
        }

        private void frmSeguridad_Load(object sender, EventArgs e)
        {
            try
            {
                Politicas.Cargar();
                lblReglas.Text = Politicas.DescribirReglas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las políticas: " + ex.Message);
            }

            CargarPreguntas();
        }

        private void btnCambiarContraseña_Click(object sender, EventArgs e)
        {
            if (!ConfirmarContraseñaActual())
            {
                return;
            }

            if (txtNuevaPass.Text == "")
            {
                MostrarAviso("Escribí la contraseña nueva.");
                return;
            }

            if (txtNuevaPass.Text != txtRepetirPass.Text)
            {
                MostrarAviso("Las contraseñas nuevas no coinciden.");
                return;
            }

            try
            {
                string mensajeError;
                if (!Politicas.Cumple(txtNuevaPass.Text, SesionUsuario.IdUsuario, SesionUsuario.NombreUsuario, out mensajeError))
                {
                    MostrarAviso(mensajeError);
                    return;
                }

                using (SqlConnection conexion = Conexion.ObtenerConexion())
                {
                    conexion.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_CambiarContraseñaPrimerLogin", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_usuario", SesionUsuario.IdUsuario);
                        cmd.Parameters.AddWithValue("@nueva_contraseña", Utilidades.HashContraseña(txtNuevaPass.Text, SesionUsuario.NombreUsuario));
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Contraseña cambiada.", "Listo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtActualPass.Clear();
                txtNuevaPass.Clear();
                txtRepetirPass.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cambiar la contraseña: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCambiarRespuesta_Click(object sender, EventArgs e)
        {
            if (!ConfirmarContraseñaActual())
            {
                return;
            }

            if (cmbPreguntas.SelectedValue == null)
            {
                MostrarAviso("Elegí la pregunta que querés cambiar.");
                return;
            }

            if (txtRespuesta1.Text.Trim() == "")
            {
                MostrarAviso("Escribí la respuesta nueva.");
                return;
            }

            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexion())
                {
                    conexion.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_ActualizarRespuestaSeguridad", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_usuario", SesionUsuario.IdUsuario);
                        cmd.Parameters.AddWithValue("@id_pregunta", cmbPreguntas.SelectedValue);
                        cmd.Parameters.AddWithValue("@nueva_respuesta", txtRespuesta1.Text);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Respuesta cambiada.", "Listo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtActualPass.Clear();
                txtRespuesta1.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar la respuesta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ConfirmarContraseñaActual()
        {
            if (txtActualPass.Text == "")
            {
                MostrarAviso("Para hacer cambios, escribí arriba tu contraseña actual.");
                txtActualPass.Focus();
                return false;
            }

            try
            {
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("SP_validacion_contraseña", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@usuario", SesionUsuario.NombreUsuario);
                        cmd.Parameters.AddWithValue("@contraseña_escrita", Utilidades.HashContraseña(txtActualPass.Text, SesionUsuario.NombreUsuario));

                        SqlParameter paramCoincidePass = new SqlParameter("@coincide_pass", SqlDbType.Bit) { Direction = ParameterDirection.Output };
                        cmd.Parameters.Add(paramCoincidePass);
                        cmd.Parameters.Add(new SqlParameter("@coincide_user", SqlDbType.Bit) { Direction = ParameterDirection.Output });
                        cmd.Parameters.Add(new SqlParameter("@primer_login", SqlDbType.Bit) { Direction = ParameterDirection.Output });
                        cmd.Parameters.Add(new SqlParameter("@id_usuario", SqlDbType.Int) { Direction = ParameterDirection.Output });

                        cmd.ExecuteNonQuery();

                        if (Convert.ToBoolean(paramCoincidePass.Value))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al validar la contraseña: " + ex.Message);
                return false;
            }

            MostrarAviso("La contraseña actual no es correcta.");
            return false;
        }

        private void CargarPreguntas()
        {
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_ObtenerPreguntasRecuperacion", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@usuario", SesionUsuario.NombreUsuario);

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            cmbPreguntas.DisplayMember = "pregunta";
                            cmbPreguntas.ValueMember = "idpregunta";
                            cmbPreguntas.DataSource = dt;
                            cmbPreguntas.SelectedIndex = -1;

                            if (dt.Rows.Count == 0)
                            {
                                grpPregunta.Enabled = false;
                                grpPregunta.Text = "Cambiar una respuesta de seguridad (todavía no tenés preguntas cargadas)";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar las preguntas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void MostrarAviso(string mensaje)
        {
            MessageBox.Show(mensaje, "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void chkMostrarContraseñaNueva_CheckedChanged(object sender, EventArgs e)
        {
            Utilidades.AlternarVisibilidadTexto(chkMostrarContraseñaNueva, txtNuevaPass);
        }

        private void chkMostrarRepetirPass_CheckedChanged(object sender, EventArgs e)
        {
            Utilidades.AlternarVisibilidadTexto(chkMostrarRepetirPass, txtRepetirPass);
        }

        private void chkMostrarPassActual_CheckedChanged(object sender, EventArgs e)
        {
            Utilidades.AlternarVisibilidadTexto(chkMostrarPassActual, txtActualPass);
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
