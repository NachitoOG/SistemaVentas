using System;
using System.IO;
using System.Text.Json;
using Microsoft.Data.SqlClient;

namespace Vista_usuario
{
    public static class Conexion
    {
        private static string cadenaConexion;

        public static SqlConnection ObtenerConexion()
        {
            if (cadenaConexion == null)
            {
                string ruta = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

                using (JsonDocument documento = JsonDocument.Parse(File.ReadAllText(ruta)))
                {
                    cadenaConexion = documento.RootElement.GetProperty("ConnectionStrings").GetProperty("SistemaVentas").GetString();
                }
            }

            return new SqlConnection(cadenaConexion);
        }
    }
}
