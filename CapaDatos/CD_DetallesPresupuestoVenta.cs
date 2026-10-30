using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using CapaEntidades;

namespace CapaDatos
{
    public class CD_DetallesPresupuestoVenta : CD_EjecutarConsultasSP
    {
        public int Alta(DetallePresupuestoVenta linea)
        {
            SqlParameter pId = new SqlParameter("@ID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            SqlParameter[] parametros =
            {
                new SqlParameter("@FK_Presupuesto_Venta", SqlDbType.Int)  { Value = linea.IdPresupuesto },
                new SqlParameter("@FK_Producto", SqlDbType.Int)           { Value = linea.IdProducto },
                new SqlParameter("@Cantidad", SqlDbType.Int)          { Value = linea.Cantidad },
                new SqlParameter("@Precio_Unitario", SqlDbType.Decimal)   { Value = linea.PrecioUnitario, Precision = 18, Scale = 2 },
                pId
            };

            EjecutarConsulta("dbo.sp_DetallePresupuestoVenta_Alta", parametros, devuelveDatos: false);

            return (int)pId.Value;
        }

        public void BorrarPorPresupuesto(int idPresupuesto)
        {
            SqlParameter[] parametros =
            {
                new SqlParameter("@FK_Presupuesto_Venta", SqlDbType.Int) { Value = idPresupuesto }
            };

            EjecutarConsulta("dbo.sp_DetallePresupuestoVenta_BorrarPorPresupuesto", parametros, devuelveDatos: false);
        }

        public List<DetallePresupuestoVenta> Listar(int idPresupuesto)
        {
            SqlParameter[] parametros =
            {
                new SqlParameter("@FK_Presupuesto_Venta", SqlDbType.Int) { Value = idPresupuesto }
            };

            DataTable tabla = EjecutarConsulta("dbo.sp_DetallePresupuestoVenta_Listar", parametros);
            return ConvertirALista(tabla);
        }

        private static List<DetallePresupuestoVenta> ConvertirALista(DataTable tabla)
        {
            List<DetallePresupuestoVenta> lista = new List<DetallePresupuestoVenta>();

            foreach (DataRow fila in tabla.Rows)
            {
                DetallePresupuestoVenta linea = new DetallePresupuestoVenta(
                    Convert.ToInt32(fila["ID"]),
                    Convert.ToInt32(fila["FK_Presupuesto_Venta"]),
                    Convert.ToInt32(fila["FK_Producto"]),
                    fila["Producto"].ToString(),
                    Convert.ToInt32(fila["Cantidad"]),
                    Convert.ToDecimal(fila["Precio_Unitario"]));

                lista.Add(linea);
            }

            return lista;
        }
    }
}
