using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Vista_usuario
{
    public partial class FrmRecuperarContra : Form
    {
        private int idUsuarioRecup = 0;
        private string usuarioRecup = "";
        private string correoRecup = "";
        private int cantPreguntas;

        private Label[] arregloLabels;
        private TextBox[] arregloTextos;
        private int[] arregloIdPreguntas;

        public FrmRecuperarContra()
        {
            InitializeComponent();
        }

        private void FrmRecuperarContra_Load(object sender, EventArgs e)
        {
            arregloLabels = new Label[] { lblPregunta1, lblPregunta2, lblPregunta3, lblPregunta4, lblPregunta5 };
            arregloTextos = new TextBox[] { txtRespuesta1, txtRespuesta2, txtRespuesta3, txtRespuesta4, txtRespuesta5 };
            arregloIdPreguntas = new int[arregloLabels.Length];

            for (int i = 0; i < arregloLabels.Length; i++)
            {
                arregloLabels[i].Visible = false;
                arregloTextos[i].Visible = false;
            }

            grpPaso2.Enabled = false;

            try
            {
                Politicas.Cargar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las políticas de seguridad: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBuscarUsuario_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsuarioABuscar.Text))
            {
                MostrarAviso("Escribí tu usuario.");
                return;
            }

            int maximo = Math.Min(Math.Max(Politicas.CantPreguntas, 1), arregloLabels.Length);
            int indice = 0;

            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexion())
                {
                    conexion.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_ObtenerPreguntasRecuperacion", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@usuario", txtUsuarioABuscar.Text.Trim());

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read() && indice < maximo)
                            {
                                idUsuarioRecup = Convert.ToInt32(dr["id_usuario"]);
                                correoRecup = dr["correo"].ToString();

                                arregloIdPreguntas[indice] = Convert.ToInt32(dr["idpregunta"]);
                                arregloLabels[indice].Text = dr["pregunta"].ToString();
                                arregloLabels[indice].Visible = true;
                                arregloTextos[indice].Visible = true;
                                indice++;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar el usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (indice == 0)
            {
                MostrarAviso("Ese usuario no existe o todavía no cargó sus preguntas de seguridad.");
                return;
            }

            cantPreguntas = indice;
            usuarioRecup = txtUsuarioABuscar.Text.Trim();
            grpPaso1.Enabled = false;
            grpPaso2.Enabled = true;
            this.AcceptButton = btnValidarPreguntas;
            txtRespuesta1.Focus();
        }

        private void btnValidarPreguntas_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < cantPreguntas; i++)
            {
                if (string.IsNullOrWhiteSpace(arregloTextos[i].Text))
                {
                    MostrarAviso("Respondé todas las preguntas.");
                    return;
                }
            }

            try
            {
                if (!RespuestasCorrectas())
                {
                    MostrarAviso("Alguna de las respuestas no es correcta.");
                    return;
                }

                RestablecerContraseña();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo recuperar la contraseña", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool RespuestasCorrectas()
        {
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                conexion.Open();

                for (int i = 0; i < cantPreguntas; i++)
                {
                    using (SqlCommand cmd = new SqlCommand("sp_ValidarRespuesta", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_usuario", idUsuarioRecup);
                        cmd.Parameters.AddWithValue("@id_pregunta", arregloIdPreguntas[i]);
                        cmd.Parameters.AddWithValue("@respuesta", arregloTextos[i].Text.Trim().ToUpper());

                        SqlParameter pEsValido = new SqlParameter("@es_valido", SqlDbType.Bit);
                        pEsValido.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(pEsValido);

                        cmd.ExecuteNonQuery();

                        if (!Convert.ToBoolean(pEsValido.Value))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private void RestablecerContraseña()
        {
            string nueva = Utilidades.GenerarContraseña();

            ArmarMail.DireccionCorreo = correoRecup;
            ArmarMail.Asunto = "Tu contraseña nueva - Sistema de Gestión";
            ArmarMail.Usuario = usuarioRecup;
            ArmarMail.NuevaContraseña = nueva;
            ArmarMail.PrepararNuevaContraseña();

            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                conexion.Open();

                using (SqlCommand cmd = new SqlCommand("sp_RestablecerContraseña", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_usuario", idUsuarioRecup);
                    cmd.Parameters.AddWithValue("@nueva_contraseña", Utilidades.HashContraseña(nueva, usuarioRecup));
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Listo. Te mandamos una contraseña nueva a " + Utilidades.OcultarCorreo(correoRecup) +
                            ".\nCuando entres con ella, el sistema te va a pedir que elijas una propia.",
                            "Contraseña restablecida", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MostrarAviso(string mensaje)
        {
            MessageBox.Show(mensaje, "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
