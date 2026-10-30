using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using CapaEntidades;

namespace CapaDatos
{
    public class CD_DetallesNotaPedido : CD_EjecutarConsultasSP
    {
        public int Alta(DetalleNotaPedido linea)
        {
            SqlParameter pId = new SqlParameter("@ID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            SqlParameter[] parametros =
            {
                new SqlParameter("@FK_Nota_Pedido", SqlDbType.Int)      { Value = linea.IdNotaPedido },
                new SqlParameter("@FK_Producto", SqlDbType.Int)         { Value = linea.IdProducto },
                new SqlParameter("@Cantidad", SqlDbType.Int)        { Value = linea.Cantidad },
                new SqlParameter("@Precio_Unitario", SqlDbType.Decimal) { Value = linea.PrecioUnitario, Precision = 18, Scale = 2 },
                pId
            };

            EjecutarConsulta("dbo.sp_DetalleNotaPedido_Alta", parametros, devuelveDatos: false);

            return (int)pId.Value;
        }

        public void BorrarPorNotaPedido(int idNotaPedido)
        {
            SqlParameter[] parametros =
            {
                new SqlParameter("@FK_Nota_Pedido", SqlDbType.Int) { Value = idNotaPedido }
            };

            EjecutarConsulta("dbo.sp_DetalleNotaPedido_BorrarPorNotaPedido", parametros, devuelveDatos: false);
        }

        public List<DetalleNotaPedido> Listar(int idNotaPedido)
        {
            SqlParameter[] parametros =
            {
                new SqlParameter("@FK_Nota_Pedido", SqlDbType.Int) { Value = idNotaPedido }
            };

            DataTable tabla = EjecutarConsulta("dbo.sp_DetalleNotaPedido_Listar", parametros);
            return ConvertirALista(tabla);
        }

        private static List<DetalleNotaPedido> ConvertirALista(DataTable tabla)
        {
            List<DetalleNotaPedido> lista = new List<DetalleNotaPedido>();

            foreach (DataRow fila in tabla.Rows)
            {
                DetalleNotaPedido linea = new DetalleNotaPedido(
                    Convert.ToInt32(fila["ID"]),
                    Convert.ToInt32(fila["FK_Nota_Pedido"]),
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
