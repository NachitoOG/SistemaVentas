using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Vista_usuario
{
    public partial class FrmUsuarioDatos : Form
    {
        private int idUsuario;
        private DataRow filaUsuario;

        public FrmUsuarioDatos()
        {
            InitializeComponent();
        }

        public FrmUsuarioDatos(DataRow fila)
            : this()
        {
            filaUsuario = fila;
            idUsuario = Convert.ToInt32(fila["id_usuario"]);
        }

        private void FrmUsuarioDatos_Load(object sender, EventArgs e)
        {
            CargarComboBoxTipoDoc();
            CargarComboBoxRoles();

            if (idUsuario == 0)
            {
                this.Text = "Nuevo usuario";
                lblTitulo.Text = "Nuevo usuario";
                chkEstado.Checked = true;
                lblAyudaUsuario.Text = "Se arma solo con la inicial del nombre y el apellido. La contraseña se genera sola y le llega por correo.";
            }
            else
            {
                this.Text = "Modificar usuario";
                lblTitulo.Text = "Modificar usuario " + filaUsuario["usuario"];
                txtNombre.Text = filaUsuario["nombre"].ToString();
                txtApellido.Text = filaUsuario["apellido"].ToString();
                txtCorreo.Text = filaUsuario["correo"].ToString();
                txtNroDoc.Text = filaUsuario["nrodoc"].ToString();
                txtUsuario.Text = filaUsuario["usuario"].ToString();
                cmbTipoDoc.SelectedValue = filaUsuario["id_tipodoc"];

                if (filaUsuario["id_rol"] != DBNull.Value)
                {
                    cmbRol.SelectedValue = filaUsuario["id_rol"];
                }

                chkEstado.Checked = Convert.ToBoolean(filaUsuario["estado"]);
                lblAyudaUsuario.Text = "El nombre de usuario no se cambia: con él se arma la contraseña guardada.";
                btnGuardar.Text = "Guardar cambios";
            }

            MostrarGrupo();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string error = Validar();
            if (error != "")
            {
                MessageBox.Show(error, "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (PersonaYaExiste(txtNroDoc.Text.Trim(), txtCorreo.Text.Trim(), idUsuario))
                {
                    MessageBox.Show("Ya hay otra persona registrada con ese documento o ese correo.", "Datos duplicados",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (idUsuario == 0)
                {
                    DarDeAlta();
                }
                else
                {
                    Modificar();
                }

                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo guardar el usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Nombre_Cambio(object sender, EventArgs e)
        {
            if (idUsuario == 0)
            {
                txtUsuario.Text = GenerarUsuarioBase();
            }
        }

        private void cmbRol_SelectedIndexChanged(object sender, EventArgs e)
        {
            MostrarGrupo();
        }

        private void txtNroDoc_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utilidades.SoloNumeros(e);
        }

        private string Validar()
        {
            if (txtNombre.Text.Trim() == "")
                return "Completá el nombre.";

            if (txtApellido.Text.Trim() == "")
                return "Completá el apellido.";

            string correo = txtCorreo.Text.Trim();
            if (correo.IndexOf('@') < 1 || correo.IndexOf('.', correo.IndexOf('@')) < 0)
                return "Escribí un correo válido, por ejemplo nombre@empresa.com.";

            if (cmbTipoDoc.SelectedValue == null)
                return "Elegí el tipo de documento.";

            if (txtNroDoc.Text.Trim() == "")
                return "Completá el número de documento.";

            if (cmbRol.SelectedValue == null)
                return "Elegí el rol del usuario.";

            return "";
        }

        private void DarDeAlta()
        {
            string usuario = GenerarUsuarioUnico();
            string password = Utilidades.GenerarContraseña();

            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                conexion.Open();

                using (SqlCommand cmd = new SqlCommand("sp_RegistrarUsuario", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@nombre", txtNombre.Text.Trim());
                    cmd.Parameters.AddWithValue("@apellido", txtApellido.Text.Trim());
                    cmd.Parameters.AddWithValue("@correo", txtCorreo.Text.Trim());
                    cmd.Parameters.AddWithValue("@id_tipodoc", cmbTipoDoc.SelectedValue);
                    cmd.Parameters.AddWithValue("@nrodoc", txtNroDoc.Text.Trim());
                    cmd.Parameters.AddWithValue("@usuario", usuario);
                    cmd.Parameters.AddWithValue("@contraseña", Utilidades.HashContraseña(password, usuario));
                    cmd.Parameters.AddWithValue("@estado", chkEstado.Checked);
                    cmd.Parameters.AddWithValue("@id_rol", cmbRol.SelectedValue);
                    cmd.ExecuteNonQuery();
                }
            }

            try
            {
                ArmarMail.DireccionCorreo = txtCorreo.Text.Trim();
                ArmarMail.Asunto = "Datos de acceso";
                ArmarMail.Usuario = usuario;
                ArmarMail.NuevaContraseña = password;
                ArmarMail.Preparar();

                MessageBox.Show("Usuario creado: " + usuario + ".\nLe enviamos la contraseña temporal a " + txtCorreo.Text.Trim() + ".",
                                "Usuario creado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Usuario creado: " + usuario + ".\n\nNo se pudo mandar el correo: " + ex.Message +
                                "\n\nPasale esta contraseña temporal: " + password +
                                "\nAl entrar, el sistema le va a pedir que la cambie.",
                                "Usuario creado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Modificar()
        {
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                conexion.Open();

                using (SqlCommand cmd = new SqlCommand("sp_ModificarUsuarioCompleto", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                    cmd.Parameters.AddWithValue("@nombre", txtNombre.Text.Trim());
                    cmd.Parameters.AddWithValue("@apellido", txtApellido.Text.Trim());
                    cmd.Parameters.AddWithValue("@correo", txtCorreo.Text.Trim());
                    cmd.Parameters.AddWithValue("@id_tipodoc", cmbTipoDoc.SelectedValue);
                    cmd.Parameters.AddWithValue("@nrodoc", txtNroDoc.Text.Trim());
                    cmd.Parameters.AddWithValue("@estado", chkEstado.Checked);
                    cmd.Parameters.AddWithValue("@id_rol", cmbRol.SelectedValue);
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Usuario modificado.", "Listo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MostrarGrupo()
        {
            DataRowView rol = cmbRol.SelectedItem as DataRowView;

            if (rol == null)
            {
                lblGrupo.Text = "";
                return;
            }

            lblGrupo.Text = "Grupo: " + rol["familia"];
        }

        private string GenerarUsuarioBase()
        {
            string nombre = txtNombre.Text.Trim().ToLower();
            string apellido = txtApellido.Text.Trim().ToLower().Replace(" ", "");
            string usuarioBase = apellido;

            if (nombre.Length > 0)
            {
                usuarioBase = nombre.Substring(0, 1) + apellido;
            }

            if (usuarioBase.Length > 18)
            {
                usuarioBase = usuarioBase.Substring(0, 18);
            }

            return usuarioBase;
        }

        private string GenerarUsuarioUnico()
        {
            string usuarioBase = GenerarUsuarioBase();
            string usuarioFinal = usuarioBase;
            int contador = 1;

            while (UsuarioExisteEnDB(usuarioFinal))
            {
                usuarioFinal = usuarioBase + contador;
                contador++;
            }

            return usuarioFinal;
        }

        private void CargarComboBoxRoles()
        {
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand("sp_ListarRoles", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            cmbRol.DisplayMember = "Roles";
                            cmbRol.ValueMember = "id_rol";
                            cmbRol.DataSource = dt;
                            cmbRol.SelectedIndex = -1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar roles: " + ex.Message);
                }
            }
        }

        private void CargarComboBoxTipoDoc()
        {
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand("sp_ListarTipoDoc", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            cmbTipoDoc.DisplayMember = "descripcion";
                            cmbTipoDoc.ValueMember = "id_tipodoc";
                            cmbTipoDoc.DataSource = dt;
                            cmbTipoDoc.SelectedIndex = -1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar los tipos de documento: " + ex.Message);
                }
            }
        }

        private bool PersonaYaExiste(string nrodoc, string correo, int idUsuarioIgnorar)
        {
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                conexion.Open();

                using (SqlCommand cmd = new SqlCommand("sp_VerificarPersonaDuplicada", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@nrodoc", nrodoc);
                    cmd.Parameters.AddWithValue("@correo", correo);
                    cmd.Parameters.AddWithValue("@id_usuario", idUsuarioIgnorar);

                    SqlParameter outParam = new SqlParameter("@Existe", SqlDbType.Int);
                    outParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outParam);

                    cmd.ExecuteNonQuery();
                    return Convert.ToInt32(outParam.Value) == 1;
                }
            }
        }

        private bool UsuarioExisteEnDB(string nombreUsuario)
        {
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                conexion.Open();

                using (SqlCommand cmd = new SqlCommand("sp_VerificarUsuarioExiste", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@usuario", nombreUsuario);
                    cmd.Parameters.AddWithValue("@id_usuario", 0);

                    SqlParameter outParam = new SqlParameter("@Existe", SqlDbType.Int);
                    outParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outParam);

                    cmd.ExecuteNonQuery();
                    return Convert.ToInt32(outParam.Value) == 1;
                }
            }
        }
    }
}
