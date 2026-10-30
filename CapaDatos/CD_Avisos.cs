using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using CapaEntidades;

namespace CapaDatos
{
    public class CD_Avisos : CD_EjecutarConsultasSP
    {
        public int Alta(AvisoMercaderia aviso)
        {
            SqlParameter pId = new SqlParameter("@ID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            SqlParameter[] parametros =
            {
                new SqlParameter("@FK_Nota_Pedido", SqlDbType.Int)      { Value = aviso.IdNotaPedido },
                new SqlParameter("@FK_Usuario", SqlDbType.Int)          { Value = aviso.IdUsuario },
                new SqlParameter("@Fecha_Emision", SqlDbType.DateTime2) { Value = aviso.FechaEmision },
                pId
            };

            EjecutarConsulta("dbo.sp_Aviso_Alta", parametros, devuelveDatos: false);

            return (int)pId.Value;
        }

        public void Modificar(AvisoMercaderia aviso)
        {
            SqlParameter[] parametros =
            {
                new SqlParameter("@ID", SqlDbType.Int)                  { Value = aviso.IdAviso },
                new SqlParameter("@FK_Usuario", SqlDbType.Int)          { Value = aviso.IdUsuario },
                new SqlParameter("@Fecha_Emision", SqlDbType.DateTime2) { Value = aviso.FechaEmision }
            };

            EjecutarConsulta("dbo.sp_Aviso_Modificar", parametros, devuelveDatos: false);
        }

        public void Baja(int idAviso)
        {
            SqlParameter[] parametros =
            {
                new SqlParameter("@ID", SqlDbType.Int) { Value = idAviso }
            };

            EjecutarConsulta("dbo.sp_Aviso_Baja", parametros, devuelveDatos: false);
        }

        public List<AvisoMercaderia> Listar(bool incluirBajas)
        {
            SqlParameter[] parametros =
            {
                new SqlParameter("@Incluir_Bajas", SqlDbType.Bit) { Value = incluirBajas }
            };

            DataTable tabla = EjecutarConsulta("dbo.sp_Aviso_Listar", parametros);
            return ConvertirALista(tabla);
        }

        public List<AvisoMercaderia> Consultar(int? idAviso, int? idNotaPedido, DateTime? desde, DateTime? hasta, bool incluirBajas)
        {
            SqlParameter[] parametros =
            {
                new SqlParameter("@ID", SqlDbType.Int)             { Value = ValorONulo(idAviso) },
                new SqlParameter("@FK_Nota_Pedido", SqlDbType.Int) { Value = ValorONulo(idNotaPedido) },
                new SqlParameter("@Fecha_Desde", SqlDbType.Date)   { Value = ValorONulo(desde) },
                new SqlParameter("@Fecha_Hasta", SqlDbType.Date)   { Value = ValorONulo(hasta) },
                new SqlParameter("@Incluir_Bajas", SqlDbType.Bit)  { Value = incluirBajas }
            };

            DataTable tabla = EjecutarConsulta("dbo.sp_Aviso_Consultar", parametros);
            return ConvertirALista(tabla);
        }

        public AvisoMercaderia ObtenerPorId(int idAviso)
        {
            List<AvisoMercaderia> resultado = Consultar(idAviso, null, null, null, true);

            if (resultado.Count == 0)
            {
                return null;
            }
            return resultado[0];
        }

        private static List<AvisoMercaderia> ConvertirALista(DataTable tabla)
        {
            List<AvisoMercaderia> lista = new List<AvisoMercaderia>();

            foreach (DataRow fila in tabla.Rows)
            {
                AvisoMercaderia aviso = new AvisoMercaderia(
                    Convert.ToInt32(fila["ID"]),
                    Convert.ToInt32(fila["FK_Nota_Pedido"]),
                    Convert.ToInt32(fila["FK_Usuario"]),
                    Convert.ToDateTime(fila["Fecha_Emision"]),
                    Convert.ToBoolean(fila["Activo"]));

                aviso.NombreUsuario = fila["Usuario"].ToString();
                aviso.NombreCliente = fila["Cliente"].ToString();
                aviso.FechaPedido = Convert.ToDateTime(fila["Fecha_Pedido"]);
                aviso.EstadoPedido = fila["Estado_Pedido"].ToString();

                lista.Add(aviso);
            }

            return lista;
        }
    }
}
