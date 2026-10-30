using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Vista_usuario
{
    public partial class FormPrimerLogin : Form
    {
        private int idUsuarioActual;
        private int cantPreguntas;
        private bool soloContraseña;

        private Label[] arregloLabels;
        private TextBox[] arregloTextos;
        private int[] arregloIdPreguntas;

        public FormPrimerLogin(int idUsuario)
        {
            InitializeComponent();
            idUsuarioActual = idUsuario;
        }

        private void FormPrimerLogin_Load(object sender, EventArgs e)
        {
            arregloLabels = new Label[] { lblPregunta1, lblPregunta2, lblPregunta3, lblPregunta4, lblPregunta5 };
            arregloTextos = new TextBox[] { txtRespuesta1, txtRespuesta2, txtRespuesta3, txtRespuesta4, txtRespuesta5 };

            for (int i = 0; i < arregloLabels.Length; i++)
            {
                arregloLabels[i].Visible = false;
                arregloTextos[i].Visible = false;
            }

            try
            {
                Politicas.Cargar();
                lblReglas.Text = Politicas.DescribirReglas();
                soloContraseña = TieneRespuestasGuardadas();

                if (soloContraseña)
                {
                    lblTitulo.Text = "Elegí una contraseña nueva";
                    lblExplicacion.Text = "Entraste con una contraseña temporal. Antes de seguir, elegí una propia.";
                    OcultarPreguntas();
                }
                else
                {
                    lblTitulo.Text = "Bienvenido/a, es tu primer ingreso";
                    lblExplicacion.Text = "Respondé las preguntas de seguridad (sirven para recuperar la contraseña si te la olvidás) y elegí tu contraseña.";
                    cantPreguntas = Math.Min(Politicas.CantPreguntas, arregloLabels.Length);
                    arregloIdPreguntas = new int[cantPreguntas];
                    CargarPreguntaAlAzar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al preparar el primer ingreso: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string nuevaPass = txtNuevaPassword.Text;

            if (!soloContraseña)
            {
                for (int i = 0; i < cantPreguntas; i++)
                {
                    if (string.IsNullOrWhiteSpace(arregloTextos[i].Text))
                    {
                        MostrarAviso("Respondé todas las preguntas de seguridad.");
                        return;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(nuevaPass))
            {
                MostrarAviso("Escribí tu contraseña nueva.");
                return;
            }

            if (nuevaPass != txtRepetirPass.Text)
            {
                MostrarAviso("Las contraseñas no coinciden.");
                return;
            }

            bool guardado = false;

            try
            {
                string msjError;
                if (!Politicas.Cumple(nuevaPass, idUsuarioActual, SesionUsuario.NombreUsuario, out msjError))
                {
                    MostrarAviso(msjError);
                    return;
                }

                using (SqlConnection conexion = Conexion.ObtenerConexion())
                {
                    conexion.Open();

                    using (SqlCommand cmdPass = new SqlCommand("sp_CambiarContraseñaPrimerLogin", conexion))
                    {
                        cmdPass.CommandType = CommandType.StoredProcedure;
                        cmdPass.Parameters.AddWithValue("@id_usuario", idUsuarioActual);
                        cmdPass.Parameters.AddWithValue("@nueva_contraseña", Utilidades.HashContraseña(nuevaPass, SesionUsuario.NombreUsuario));
                        cmdPass.ExecuteNonQuery();
                    }

                    if (!soloContraseña)
                    {
                        for (int i = 0; i < cantPreguntas; i++)
                        {
                            using (SqlCommand cmdResp = new SqlCommand("sp_GuardarRespuestaSeguridad", conexion))
                            {
                                cmdResp.CommandType = CommandType.StoredProcedure;
                                cmdResp.Parameters.AddWithValue("@id_pregunta", arregloIdPreguntas[i]);
                                cmdResp.Parameters.AddWithValue("@id_usuario", idUsuarioActual);
                                cmdResp.Parameters.AddWithValue("@respuesta", arregloTextos[i].Text.Trim().ToUpper());
                                cmdResp.ExecuteNonQuery();
                            }
                        }
                    }
                }

                guardado = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar los cambios: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (guardado)
            {
                this.Hide();
                frmHome form = new frmHome();
                form.ShowDialog();
                this.Close();
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool TieneRespuestasGuardadas()
        {
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                conexion.Open();

                using (SqlCommand cmd = new SqlCommand("sp_ObtenerPreguntasRecuperacion", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@usuario", SesionUsuario.NombreUsuario);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        return dr.Read();
                    }
                }
            }
        }

        private void OcultarPreguntas()
        {
            int diferencia = grpContraseña.Top - grpPreguntas.Top;

            grpPreguntas.Visible = false;
            grpContraseña.Top = grpContraseña.Top - diferencia;
            btnGuardar.Top = btnGuardar.Top - diferencia;
            btnCancelar.Top = btnCancelar.Top - diferencia;
            this.ClientSize = new System.Drawing.Size(this.ClientSize.Width, this.ClientSize.Height - diferencia);
        }

        private void CargarPreguntaAlAzar()
        {
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                conexion.Open();

                using (SqlCommand cmd = new SqlCommand("sp_ObtenerPreguntaAlAzar", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@cantidad", cantPreguntas);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        int indice = 0;
                        while (dr.Read() && indice < cantPreguntas)
                        {
                            arregloIdPreguntas[indice] = Convert.ToInt32(dr["id_pregunta"]);
                            arregloLabels[indice].Text = dr["pregunta"].ToString();
                            arregloLabels[indice].Visible = true;
                            arregloTextos[indice].Visible = true;
                            indice++;
                        }

                        cantPreguntas = indice;
                    }
                }
            }
        }

        private void MostrarAviso(string mensaje)
        {
            MessageBox.Show(mensaje, "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void chkMostrarC1_CheckedChanged(object sender, EventArgs e)
        {
            Utilidades.AlternarVisibilidadTexto(chkMostrarC1, txtNuevaPassword);
        }

        private void chkMostrarC2_CheckedChanged(object sender, EventArgs e)
        {
            Utilidades.AlternarVisibilidadTexto(chkMostrarC2, txtRepetirPass);
        }
    }
}
