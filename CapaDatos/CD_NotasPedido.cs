using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using CapaEntidades;

namespace CapaDatos
{
    public class CD_NotasPedido : CD_EjecutarConsultasSP
    {
        public int Alta(NotaPedido nota)
        {
            SqlParameter pId = new SqlParameter("@ID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            SqlParameter[] parametros =
            {
                new SqlParameter("@FK_Cliente", SqlDbType.Int)          { Value = nota.IdCliente },
                new SqlParameter("@FK_Usuario", SqlDbType.Int)          { Value = nota.IdUsuario },
                new SqlParameter("@Fecha_Emision", SqlDbType.DateTime2) { Value = nota.FechaEmision },
                pId
            };

            EjecutarConsulta("dbo.sp_NotaPedido_Alta", parametros, devuelveDatos: false);

            return (int)pId.Value;
        }

        public int GenerarDesdePresupuesto(int idPresupuesto, int idUsuario)
        {
            SqlParameter pId = new SqlParameter("@ID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            SqlParameter[] parametros =
            {
                new SqlParameter("@FK_Presupuesto_Venta", SqlDbType.Int) { Value = idPresupuesto },
                new SqlParameter("@FK_Usuario", SqlDbType.Int)           { Value = idUsuario },
                pId
            };

            EjecutarConsulta("dbo.sp_NotaPedido_GenerarDesdePresupuesto", parametros, devuelveDatos: false);

            return (int)pId.Value;
        }

        public void Modificar(NotaPedido nota)
        {
            SqlParameter[] parametros =
            {
                new SqlParameter("@ID", SqlDbType.Int)                  { Value = nota.IdNotaPedido },
                new SqlParameter("@FK_Cliente", SqlDbType.Int)          { Value = nota.IdCliente },
                new SqlParameter("@FK_Usuario", SqlDbType.Int)          { Value = nota.IdUsuario },
                new SqlParameter("@Fecha_Emision", SqlDbType.DateTime2) { Value = nota.FechaEmision }
            };

            EjecutarConsulta("dbo.sp_NotaPedido_Modificar", parametros, devuelveDatos: false);
        }

        public void CambiarEstado(int idNotaPedido, string estado)
        {
            SqlParameter[] parametros =
            {
                new SqlParameter("@ID", SqlDbType.Int)             { Value = idNotaPedido },
                new SqlParameter("@Estado", SqlDbType.VarChar, 30) { Value = estado }
            };

            EjecutarConsulta("dbo.sp_NotaPedido_CambiarEstado", parametros, devuelveDatos: false);
        }

        public void Baja(int idNotaPedido)
        {
            SqlParameter[] parametros =
            {
                new SqlParameter("@ID", SqlDbType.Int) { Value = idNotaPedido }
            };

            EjecutarConsulta("dbo.sp_NotaPedido_Baja", parametros, devuelveDatos: false);
        }

        public List<NotaPedido> Listar(bool incluirBajas)
        {
            SqlParameter[] parametros =
            {
                new SqlParameter("@Incluir_Bajas", SqlDbType.Bit) { Value = incluirBajas }
            };

            DataTable tabla = EjecutarConsulta("dbo.sp_NotaPedido_Listar", parametros);
            return ConvertirALista(tabla);
        }

        public List<NotaPedido> Consultar(int? idNotaPedido, int? idCliente, string estado, bool incluirBajas)
        {
            SqlParameter[] parametros =
            {
                new SqlParameter("@ID", SqlDbType.Int)             { Value = ValorONulo(idNotaPedido) },
                new SqlParameter("@FK_Cliente", SqlDbType.Int)     { Value = ValorONulo(idCliente) },
                new SqlParameter("@Estado", SqlDbType.VarChar, 30) { Value = ValorONulo(estado) },
                new SqlParameter("@Incluir_Bajas", SqlDbType.Bit)  { Value = incluirBajas }
            };

            DataTable tabla = EjecutarConsulta("dbo.sp_NotaPedido_Consultar", parametros);
            return ConvertirALista(tabla);
        }

        public NotaPedido ObtenerPorId(int idNotaPedido)
        {
            List<NotaPedido> resultado = Consultar(idNotaPedido, null, null, true);

            if (resultado.Count == 0)
            {
                return null;
            }
            return resultado[0];
        }

        private static List<NotaPedido> ConvertirALista(DataTable tabla)
        {
            List<NotaPedido> lista = new List<NotaPedido>();

            foreach (DataRow fila in tabla.Rows)
            {
                int? idPresupuesto = null;
                if (fila["FK_Presupuesto_Venta"] != DBNull.Value)
                {
                    idPresupuesto = Convert.ToInt32(fila["FK_Presupuesto_Venta"]);
                }

                NotaPedido nota = new NotaPedido(
                    Convert.ToInt32(fila["ID"]),
                    Convert.ToInt32(fila["FK_Cliente"]),
                    Convert.ToInt32(fila["FK_Usuario"]),
                    idPresupuesto,
                    Convert.ToDateTime(fila["Fecha_Emision"]),
                    fila["Estado"].ToString(),
                    Convert.ToBoolean(fila["Activo"]));

                nota.NombreCliente = fila["Cliente"].ToString();
                nota.NombreUsuario = fila["Usuario"].ToString();
                nota.Total = Convert.ToDecimal(fila["Total"]);

                lista.Add(nota);
            }

            return lista;
        }
    }
}
