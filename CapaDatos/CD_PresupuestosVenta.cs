using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using CapaEntidades;

namespace CapaDatos
{
    public class CD_PresupuestosVenta : CD_EjecutarConsultasSP
    {
        public int Alta(PresupuestoVenta presupuesto)
        {
            SqlParameter pId = new SqlParameter("@ID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            SqlParameter[] parametros =
            {
                new SqlParameter("@FK_Cliente", SqlDbType.Int)          { Value = presupuesto.IdCliente },
                new SqlParameter("@FK_Usuario", SqlDbType.Int)          { Value = presupuesto.IdUsuario },
                new SqlParameter("@Fecha_Emision", SqlDbType.DateTime2) { Value = presupuesto.FechaEmision },
                pId
            };

            EjecutarConsulta("dbo.sp_PresupuestoVenta_Alta", parametros, devuelveDatos: false);

            return (int)pId.Value;
        }

        public void Modificar(PresupuestoVenta presupuesto)
        {
            SqlParameter[] parametros =
            {
                new SqlParameter("@ID", SqlDbType.Int)                  { Value = presupuesto.IdPresupuesto },
                new SqlParameter("@FK_Cliente", SqlDbType.Int)          { Value = presupuesto.IdCliente },
                new SqlParameter("@FK_Usuario", SqlDbType.Int)          { Value = presupuesto.IdUsuario },
                new SqlParameter("@Fecha_Emision", SqlDbType.DateTime2) { Value = presupuesto.FechaEmision }
            };

            EjecutarConsulta("dbo.sp_PresupuestoVenta_Modificar", parametros, devuelveDatos: false);
        }

        public void CambiarEstado(int idPresupuesto, string estado)
        {
            SqlParameter[] parametros =
            {
                new SqlParameter("@ID", SqlDbType.Int)              { Value = idPresupuesto },
                new SqlParameter("@Estado", SqlDbType.VarChar, 30)  { Value = estado }
            };

            EjecutarConsulta("dbo.sp_PresupuestoVenta_CambiarEstado", parametros, devuelveDatos: false);
        }

        public void Baja(int idPresupuesto)
        {
            SqlParameter[] parametros =
            {
                new SqlParameter("@ID", SqlDbType.Int) { Value = idPresupuesto }
            };

            EjecutarConsulta("dbo.sp_PresupuestoVenta_Baja", parametros, devuelveDatos: false);
        }

        public List<PresupuestoVenta> Listar(bool incluirBajas)
        {
            SqlParameter[] parametros =
            {
                new SqlParameter("@Incluir_Bajas", SqlDbType.Bit) { Value = incluirBajas }
            };

            DataTable tabla = EjecutarConsulta("dbo.sp_PresupuestoVenta_Listar", parametros);
            return ConvertirALista(tabla);
        }

        public List<PresupuestoVenta> Consultar(int? idPresupuesto, int? idCliente, string estado, bool incluirBajas)
        {
            SqlParameter[] parametros =
            {
                new SqlParameter("@ID", SqlDbType.Int)              { Value = ValorONulo(idPresupuesto) },
                new SqlParameter("@FK_Cliente", SqlDbType.Int)      { Value = ValorONulo(idCliente) },
                new SqlParameter("@Estado", SqlDbType.VarChar, 30)  { Value = ValorONulo(estado) },
                new SqlParameter("@Incluir_Bajas", SqlDbType.Bit)   { Value = incluirBajas }
            };

            DataTable tabla = EjecutarConsulta("dbo.sp_PresupuestoVenta_Consultar", parametros);
            return ConvertirALista(tabla);
        }

        public PresupuestoVenta ObtenerPorId(int idPresupuesto)
        {
            List<PresupuestoVenta> resultado = Consultar(idPresupuesto, null, null, true);

            if (resultado.Count == 0)
            {
                return null;
            }
            return resultado[0];
        }

        private static List<PresupuestoVenta> ConvertirALista(DataTable tabla)
        {
            List<PresupuestoVenta> lista = new List<PresupuestoVenta>();

            foreach (DataRow fila in tabla.Rows)
            {
                PresupuestoVenta presupuesto = new PresupuestoVenta(
                    Convert.ToInt32(fila["ID"]),
                    Convert.ToInt32(fila["FK_Cliente"]),
                    Convert.ToInt32(fila["FK_Usuario"]),
                    Convert.ToDateTime(fila["Fecha_Emision"]),
                    fila["Estado"].ToString(),
                    Convert.ToBoolean(fila["Activo"]));

                presupuesto.NombreCliente = fila["Cliente"].ToString();
                presupuesto.NombreUsuario = fila["Usuario"].ToString();
                presupuesto.Total = Convert.ToDecimal(fila["Total"]);

                lista.Add(presupuesto);
            }

            return lista;
        }
    }
}
