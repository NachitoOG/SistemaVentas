using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace Vista_usuario
{
    public static class Politicas
    {
        public static int MinCaracteres { get; private set; }
        public static int CantPreguntas { get; private set; }
        public static bool ReqMayusMinus { get; private set; }
        public static bool ReqAlfanumerico { get; private set; }
        public static bool ReqEspecial { get; private set; }
        public static bool Requiere2FA { get; private set; }
        public static bool BloquearAnteriores { get; private set; }
        public static bool BloquearDatosPersonales { get; private set; }

        public static void Cargar()
        {
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                conexion.Open();

                using (SqlCommand cmd = new SqlCommand("sp_ObtenerPoliticasSeguridad", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            MinCaracteres = Convert.ToInt32(dr["min_caracteres"]);
                            CantPreguntas = Convert.ToInt32(dr["cant_preguntas_validar"]);
                            ReqMayusMinus = Convert.ToBoolean(dr["requiere_mayus_minus"]);
                            ReqAlfanumerico = Convert.ToBoolean(dr["requiere_alfanumerico"]);
                            ReqEspecial = Convert.ToBoolean(dr["requiere_caracter_especial"]);
                            Requiere2FA = Convert.ToBoolean(dr["requiere_2fa_correo"]);
                            BloquearAnteriores = Convert.ToBoolean(dr["bloquear_contrasenas_anteriores"]);
                            BloquearDatosPersonales = Convert.ToBoolean(dr["bloquear_datos_personales"]);
                        }
                    }
                }
            }
        }

        public static string DescribirReglas()
        {
            List<string> reglas = new List<string>();
            reglas.Add("al menos " + MinCaracteres + " caracteres");

            if (ReqMayusMinus)
                reglas.Add("mayúsculas y minúsculas");

            if (ReqAlfanumerico)
                reglas.Add("números y letras");

            if (ReqEspecial)
                reglas.Add("un carácter especial");

            if (BloquearAnteriores)
                reglas.Add("que no la hayas usado antes");

            if (BloquearDatosPersonales)
                reglas.Add("sin tu nombre, apellido ni documento");

            return "La contraseña tiene que tener: " + string.Join(", ", reglas) + ".";
        }

        public static bool Cumple(string pass, int idUsuario, string usuario, out string error)
        {
            Cargar();
            error = "";

            if (pass.Length < MinCaracteres)
            {
                error = "La contraseña debe tener al menos " + MinCaracteres + " caracteres.";
                return false;
            }

            if (ReqMayusMinus && (!Regex.IsMatch(pass, "[A-Z]") || !Regex.IsMatch(pass, "[a-z]")))
            {
                error = "Debe contener al menos una mayúscula y una minúscula.";
                return false;
            }

            if (ReqAlfanumerico && (!Regex.IsMatch(pass, "[0-9]") || !Regex.IsMatch(pass, "[a-zA-Z]")))
            {
                error = "Debe contener números y letras.";
                return false;
            }

            if (ReqEspecial && !Regex.IsMatch(pass, "[^a-zA-Z0-9]"))
            {
                error = "Debe contener al menos un carácter especial.";
                return false;
            }

            if (BloquearAnteriores && ExisteEnHistorial(pass, idUsuario, usuario))
            {
                error = "No podés usar una contraseña que ya usaste antes.";
                return false;
            }

            if (BloquearDatosPersonales && ContieneDatosPersonales(pass, idUsuario))
            {
                error = "La contraseña no puede contener tu nombre, tu apellido ni tu documento.";
                return false;
            }

            return true;
        }

        private static bool ExisteEnHistorial(string pass, int idUsuario, string usuario)
        {
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                conexion.Open();

                using (SqlCommand cmd = new SqlCommand("sp_VerificarHistorialPass", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                    cmd.Parameters.AddWithValue("@pass_hash", Utilidades.HashContraseña(pass, usuario));

                    int conteo = Convert.ToInt32(cmd.ExecuteScalar());
                    return conteo > 0;
                }
            }
        }

        private static bool ContieneDatosPersonales(string pass, int idUsuario)
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
                        if (!dr.Read())
                        {
                            return false;
                        }

                        string passMin = pass.ToLower();
                        string nombre = dr["nombre"].ToString().ToLower();
                        string apellido = dr["apellido"].ToString().ToLower();
                        string dni = dr["nrodoc"].ToString().ToLower();

                        if (nombre.Length > 2 && passMin.Contains(nombre))
                            return true;

                        if (apellido.Length > 2 && passMin.Contains(apellido))
                            return true;

                        if (dni.Length > 4 && passMin.Contains(dni))
                            return true;

                        return false;
                    }
                }
            }
        }
    }
}
