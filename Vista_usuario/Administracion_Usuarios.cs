using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CapaVista;
using Microsoft.Data.SqlClient;

namespace Vista_usuario
{
    public partial class Administracion_Usuarios : Form
    {
        private DataTable usuarios;
        private bool cargando;

        public Administracion_Usuarios()
        {
            InitializeComponent();
            dgvUsuarios.AutoGenerateColumns = false;
        }

        private void Administracion_Usuarios_Load(object sender, EventArgs e)
        {
            cargando = true;
            CargarFiltros();
            cargando = false;

            CargarGrillaUsuarios();
        }

        private void Filtros_Cambiaron(object sender, EventArgs e)
        {
            if (cargando)
            {
                return;
            }

            AplicarFiltros();
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            FrmUsuarioDatos ventana = new FrmUsuarioDatos();

            if (ventana.ShowDialog() == DialogResult.OK)
            {
                CargarGrillaUsuarios();
            }
        }

        private void dgvUsuarios_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!Iconos.HayIcono(dgvUsuarios, e.ColumnIndex, e.RowIndex))
            {
                return;
            }

            DataRow fila = ((DataRowView)dgvUsuarios.Rows[e.RowIndex].DataBoundItem).Row;
            string columna = dgvUsuarios.Columns[e.ColumnIndex].Name;

            if (columna == "icoEditar")
            {
                Editar(fila);
            }
            else if (columna == "icoEstado")
            {
                CambiarEstado(fila);
            }
        }

        private void dgvUsuarios_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            DataRowView fila = dgvUsuarios.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (fila == null)
            {
                return;
            }

            bool activo = Convert.ToBoolean(fila["estado"]);
            bool esUnoMismo = Convert.ToInt32(fila["id_usuario"]) == SesionUsuario.IdUsuario;
            string columna = dgvUsuarios.Columns[e.ColumnIndex].Name;

            if (columna == "colEstado")
            {
                if (activo)
                {
                    e.Value = "Activo";
                }
                else
                {
                    e.Value = "Inactivo";
                }
                e.FormattingApplied = true;
            }
            else if (columna == "icoEditar")
            {
                e.Value = Iconos.EDITAR;
            }
            else if (columna == "icoEstado")
            {
                if (esUnoMismo)
                {
                    e.Value = "";
                }
                else if (activo)
                {
                    e.Value = Iconos.BAJA;
                }
                else
                {
                    e.Value = Iconos.APROBAR;
                    e.CellStyle.ForeColor = Color.FromArgb(16, 124, 16);
                    e.CellStyle.SelectionForeColor = Color.FromArgb(16, 124, 16);
                }
                return;
            }

            if (!activo && columna != "icoEditar")
            {
                e.CellStyle.ForeColor = Color.Gray;
            }
        }

        private void dgvUsuarios_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            Iconos.CambiarCursor(dgvUsuarios, e.ColumnIndex, e.RowIndex);
        }

        private void dgvUsuarios_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            dgvUsuarios.Cursor = Cursors.Default;
        }

        private void dgvUsuarios_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            e.ToolTipText = Iconos.Ayuda(dgvUsuarios, e.ColumnIndex, e.RowIndex);

            if (e.ToolTipText != "" && dgvUsuarios.Columns[e.ColumnIndex].Name == "icoEstado")
            {
                DataRowView fila = (DataRowView)dgvUsuarios.Rows[e.RowIndex].DataBoundItem;

                if (!Convert.ToBoolean(fila["estado"]))
                {
                    e.ToolTipText = "Reactivar";
                }
            }
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Editar(DataRow fila)
        {
            FrmUsuarioDatos ventana = new FrmUsuarioDatos(fila);

            if (ventana.ShowDialog() == DialogResult.OK)
            {
                CargarGrillaUsuarios();
            }
        }

        private void CambiarEstado(DataRow fila)
        {
            bool activo = Convert.ToBoolean(fila["estado"]);
            string pregunta;

            if (activo)
            {
                pregunta = "¿Dar de baja al usuario " + fila["usuario"] + "? No va a poder entrar al sistema hasta que lo reactives.";
            }
            else
            {
                pregunta = "¿Reactivar al usuario " + fila["usuario"] + "?";
            }

            if (MessageBox.Show(pregunta, "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                try
                {
                    conexion.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_CambiarEstadoUsuario", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_usuario", Convert.ToInt32(fila["id_usuario"]));
                        cmd.Parameters.AddWithValue("@estado", !activo);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No se pudo cambiar el estado: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            CargarGrillaUsuarios();
        }

        private void CargarFiltros()
        {
            cmbFiltroEstado.Items.Clear();
            cmbFiltroEstado.Items.Add("(Todos)");
            cmbFiltroEstado.Items.Add("Activos");
            cmbFiltroEstado.Items.Add("Inactivos");
            cmbFiltroEstado.SelectedIndex = 0;

            cmbFiltroRol.Items.Clear();
            cmbFiltroRol.Items.Add("(Todos)");

            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                try
                {
                    conexion.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_ListarRoles", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                cmbFiltroRol.Items.Add(dr["Roles"].ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar los roles: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            cmbFiltroRol.SelectedIndex = 0;
        }

        private void CargarGrillaUsuarios()
        {
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand("sp_ListarUsuariosCompletos", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            usuarios = new DataTable();
                            da.Fill(usuarios);
                        }
                    }

                    dgvUsuarios.DataSource = null;
                    dgvUsuarios.DataSource = usuarios;
                    AplicarFiltros();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar los usuarios: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void AplicarFiltros()
        {
            if (usuarios == null)
            {
                return;
            }

            List<string> condiciones = new List<string>();

            string busqueda = txtBuscar.Text.Trim().Replace("'", "''");
            if (busqueda != "")
            {
                condiciones.Add("(usuario LIKE '%" + busqueda + "%' OR nombre LIKE '%" + busqueda + "%' OR apellido LIKE '%" + busqueda +
                                "%' OR correo LIKE '%" + busqueda + "%' OR nrodoc LIKE '%" + busqueda + "%')");
            }

            if (cmbFiltroEstado.SelectedIndex == 1)
            {
                condiciones.Add("estado = true");
            }
            else if (cmbFiltroEstado.SelectedIndex == 2)
            {
                condiciones.Add("estado = false");
            }

            if (cmbFiltroRol.SelectedIndex > 0)
            {
                condiciones.Add("nombre_rol = '" + cmbFiltroRol.Text.Replace("'", "''") + "'");
            }

            usuarios.DefaultView.RowFilter = string.Join(" AND ", condiciones);
            dgvUsuarios.ClearSelection();
            lblCantidadRegistros.Text = "Usuarios: " + usuarios.DefaultView.Count;
        }
    }
}
