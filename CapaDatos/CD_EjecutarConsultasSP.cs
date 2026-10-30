using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace CapaDatos
{
    public abstract class CD_EjecutarConsultasSP : Conexion
    {
        protected DataTable EjecutarConsulta(string nombreSP, SqlParameter[] parametros, bool devuelveDatos = true)
        {
            DataTable tabla = new DataTable();

            try
            {
                using (SqlConnection cn = GetConexion())
                using (SqlCommand cmd = new SqlCommand(nombreSP, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (parametros != null)
                    {
                        cmd.Parameters.AddRange(parametros);
                    }

                    cn.Open();

                    if (devuelveDatos)
                    {
                        using (SqlDataReader lector = cmd.ExecuteReader())
                        {
                            tabla.Load(lector);
                        }
                    }
                    else
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 50000)
            {
                throw new Exception(ex.Message, ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("No se pudo acceder a la base de datos.\n" + ex.Message, ex);
            }

            return tabla;
        }

        protected static object ValorONulo(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return DBNull.Value;
            }
            return texto.Trim();
        }

        protected static object ValorONulo(int? numero)
        {
            if (numero == null)
            {
                return DBNull.Value;
            }
            return numero.Value;
        }

        protected static object ValorONulo(DateTime? fecha)
        {
            if (fecha == null)
            {
                return DBNull.Value;
            }
            return fecha.Value.Date;
        }
    }
}
