using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Vista_usuario
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            lblMensaje.Text = "";
            string usuario = txtUsuario.Text.Trim();

            if (usuario == "" || txtPassword.Text == "")
            {
                lblMensaje.Text = "Ingresá tu usuario y tu contraseña.";
                return;
            }

            try
            {
                int idUsuario;
                bool primerLogin;
                bool activo;

                bool correcta = ValidarCredenciales(usuario, Utilidades.HashContraseña(txtPassword.Text, usuario),
                                                    out idUsuario, out primerLogin, out activo);

                if (!correcta && ValidarCredenciales(usuario, Utilidades.HashSHA256(txtPassword.Text),
                                                     out idUsuario, out primerLogin, out activo))
                {
                    MigrarContraseña(idUsuario, Utilidades.HashContraseña(txtPassword.Text, usuario));
                    correcta = true;
                }

                if (!correcta)
                {
                    lblMensaje.Text = "Usuario o contraseña incorrectos.";
                    return;
                }

                if (!activo)
                {
                    lblMensaje.Text = "Tu usuario está dado de baja. Hablá con el administrador.";
                    return;
                }

                Politicas.Cargar();

                if (Politicas.Requiere2FA && !VerificarSegundoPaso(idUsuario, usuario))
                {
                    return;
                }

                CargarSesion(idUsuario, usuario);
                AbrirSistema(primerLogin, idUsuario);
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al conectar: " + ex.Message;
            }
        }

        private bool ValidarCredenciales(string usuario, string hash, out int idUsuario, out bool primerLogin, out bool activo)
        {
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                conexion.Open();

                using (SqlCommand cmd = new SqlCommand("SP_validacion_contraseña", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@usuario", usuario);
                    cmd.Parameters.AddWithValue("@contraseña_escrita", hash);

                    SqlParameter paramCoincideUser = new SqlParameter("@coincide_user", SqlDbType.Bit) { Direction = ParameterDirection.Output };
                    SqlParameter paramCoincidePass = new SqlParameter("@coincide_pass", SqlDbType.Bit) { Direction = ParameterDirection.Output };
                    SqlParameter paramPrimerLogin = new SqlParameter("@primer_login", SqlDbType.Bit) { Direction = ParameterDirection.Output };
                    SqlParameter paramIdUsuario = new SqlParameter("@id_usuario", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    SqlParameter paramActivo = new SqlParameter("@activo", SqlDbType.Bit) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(paramCoincideUser);
                    cmd.Parameters.Add(paramCoincidePass);
                    cmd.Parameters.Add(paramPrimerLogin);
                    cmd.Parameters.Add(paramIdUsuario);
                    cmd.Parameters.Add(paramActivo);

                    cmd.ExecuteNonQuery();

                    idUsuario = Convert.ToInt32(paramIdUsuario.Value);
                    primerLogin = Convert.ToBoolean(paramPrimerLogin.Value);
                    activo = Convert.ToBoolean(paramActivo.Value);

                    return Convert.ToBoolean(paramCoincideUser.Value) && Convert.ToBoolean(paramCoincidePass.Value);
                }
            }
        }

        private void MigrarContraseña(int idUsuario, string hashNuevo)
        {
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                conexion.Open();

                using (SqlCommand cmd = new SqlCommand("sp_MigrarContraseña", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                    cmd.Parameters.AddWithValue("@nueva_contraseña", hashNuevo);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private bool VerificarSegundoPaso(int idUsuario, string usuario)
        {
            string correo = ObtenerCorreo(idUsuario);
            string codigo = Utilidades.GenerarCodigo();

            try
            {
                ArmarMail.DireccionCorreo = correo;
                ArmarMail.Asunto = "Código de ingreso - Sistema de Gestión";
                ArmarMail.Usuario = usuario;
                ArmarMail.CodigoVerificacion = codigo;
                ArmarMail.PrepararCodigoIngreso();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = ex.Message;
                return false;
            }

            FrmCodigo2FA ventana = new FrmCodigo2FA(codigo, correo);
            return ventana.ShowDialog() == DialogResult.OK;
        }

        private string ObtenerCorreo(int idUsuario)
        {
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                conexion.Open();

                using (SqlCommand cmd = new SqlCommand("sp_ObtenerDatosPersonalesUsuario", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_usuario", idUsuario);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            return dr["correo"].ToString();
                        }
                    }
                }
            }

            return "";
        }

        private void CargarSesion(int idUsuario, string usuario)
        {
            SesionUsuario.CerrarSesion();
            SesionUsuario.IdUsuario = idUsuario;
            SesionUsuario.NombreUsuario = usuario;

            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                conexion.Open();

                using (SqlCommand cmdRoles = new SqlCommand("sp_ObtenerRolesUsuario", conexion))
                {
                    cmdRoles.CommandType = CommandType.StoredProcedure;
                    cmdRoles.Parameters.AddWithValue("@id_usuario", idUsuario);

                    using (SqlDataReader dr = cmdRoles.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            SesionUsuario.Roles.Add(dr["NombreRol"].ToString());
                            SesionUsuario.Familias.Add(dr["NombreFamilia"].ToString());
                        }
                    }
                }

                using (SqlCommand cmdDatos = new SqlCommand("sp_ObtenerDatosPersonalesUsuario", conexion))
                {
                    cmdDatos.CommandType = CommandType.StoredProcedure;
                    cmdDatos.Parameters.AddWithValue("@id_usuario", idUsuario);

                    using (SqlDataReader dr = cmdDatos.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            SesionUsuario.NombreCompleto = dr["nombre"] + " " + dr["apellido"];
                        }
                    }
                }
            }
        }

        private void AbrirSistema(bool primerLogin, int idUsuario)
        {
            this.Hide();

            if (primerLogin)
            {
                FormPrimerLogin ventana = new FormPrimerLogin(idUsuario);
                ventana.ShowDialog();
            }
            else
            {
                frmHome ventana = new frmHome();
                ventana.ShowDialog();
            }

            SesionUsuario.CerrarSesion();
            txtPassword.Clear();
            lblMensaje.Text = "";
            this.Show();
            txtPassword.Focus();
        }

        private void LlblRecuperar_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FrmRecuperarContra ventana = new FrmRecuperarContra();
            ventana.ShowDialog();
        }

        private void chkMostrar_CheckedChanged(object sender, EventArgs e)
        {
            Utilidades.AlternarVisibilidadTexto(chkMostrar, txtPassword);
        }
    }
}
