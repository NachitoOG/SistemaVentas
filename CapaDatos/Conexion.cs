using System;
using System.IO;
using System.Text.Json;
using Microsoft.Data.SqlClient;

namespace CapaDatos
{
    public abstract class Conexion
    {
        private const string ArchivoConfiguracion = "appsettings.json";
        private const string NombreCadena = "SistemaVentas";

        private static string cadena;

        protected SqlConnection GetConexion()
        {
            if (cadena == null)
            {
                cadena = LeerCadenaDeConexion();
            }
            return new SqlConnection(cadena);
        }

        private static string LeerCadenaDeConexion()
        {
            string ruta = Path.Combine(AppContext.BaseDirectory, ArchivoConfiguracion);

            if (!File.Exists(ruta))
            {
                throw new Exception("No se encontró el archivo de configuración:\n" + ruta);
            }

            JsonDocumentOptions opciones = new JsonDocumentOptions
            {
                CommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };

            try
            {
                using (JsonDocument documento = JsonDocument.Parse(File.ReadAllText(ruta), opciones))
                {
                    JsonElement cadenas;
                    JsonElement valor;

                    if (documento.RootElement.TryGetProperty("ConnectionStrings", out cadenas) &&
                        cadenas.TryGetProperty(NombreCadena, out valor) &&
                        valor.ValueKind == JsonValueKind.String &&
                        !string.IsNullOrWhiteSpace(valor.GetString()))
                    {
                        return valor.GetString();
                    }
                }
            }
            catch (JsonException ex)
            {
                throw new Exception("El archivo " + ArchivoConfiguracion + " está mal escrito:\n" + ex.Message, ex);
            }

            throw new Exception("En " + ArchivoConfiguracion + " falta la cadena \"" + NombreCadena +
                                "\" dentro de \"ConnectionStrings\".");
        }
    }
}
