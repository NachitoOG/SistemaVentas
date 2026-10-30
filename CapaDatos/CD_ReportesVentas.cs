using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using CapaEntidades;

namespace CapaDatos
{
    public class CD_ReportesVentas : CD_EjecutarConsultasSP
    {
        public List<ReportePresupuestosVenta> PresupuestosPorClienteEstado(int? idCliente, string estado, DateTime? desde, DateTime? hasta)
        {
            SqlParameter[] parametros =
            {
                new SqlParameter("@FK_Cliente", SqlDbType.Int)     { Value = ValorONulo(idCliente) },
                new SqlParameter("@Estado", SqlDbType.VarChar, 30) { Value = ValorONulo(estado) },
                new SqlParameter("@Fecha_Desde", SqlDbType.Date)   { Value = ValorONulo(desde) },
                new SqlParameter("@Fecha_Hasta", SqlDbType.Date)   { Value = ValorONulo(hasta) }
            };

            DataTable tabla = EjecutarConsulta("dbo.sp_Rpt_PresupuestosVentaPorClienteEstado", parametros);

            List<ReportePresupuestosVenta> lista = new List<ReportePresupuestosVenta>();

            foreach (DataRow fila in tabla.Rows)
            {
                lista.Add(new ReportePresupuestosVenta(
                    fila["Cliente"].ToString(),
                    fila["Estado"].ToString(),
                    Convert.ToInt32(fila["Cantidad"]),
                    ADecimal(fila["Monto_Total"])));
            }

            return lista;
        }

        public List<ReporteNotasPorEstado> NotasPedidoPorEstado(DateTime? desde, DateTime? hasta)
        {
            SqlParameter[] parametros =
            {
                new SqlParameter("@Fecha_Desde", SqlDbType.Date) { Value = ValorONulo(desde) },
                new SqlParameter("@Fecha_Hasta", SqlDbType.Date) { Value = ValorONulo(hasta) }
            };

            DataTable tabla = EjecutarConsulta("dbo.sp_Rpt_NotasPedidoPorEstado", parametros);

            List<ReporteNotasPorEstado> lista = new List<ReporteNotasPorEstado>();

            foreach (DataRow fila in tabla.Rows)
            {
                lista.Add(new ReporteNotasPorEstado(
                    fila["Estado"].ToString(),
                    Convert.ToInt32(fila["Cantidad"]),
                    ADecimal(fila["Monto_Total"])));
            }

            return lista;
        }

        public List<ReportePedidoPendiente> PedidosPendientesPreparar(int? idCliente)
        {
            SqlParameter[] parametros =
            {
                new SqlParameter("@FK_Cliente", SqlDbType.Int) { Value = ValorONulo(idCliente) }
            };

            DataTable tabla = EjecutarConsulta("dbo.sp_Rpt_PedidosPendientesPreparar", parametros);

            List<ReportePedidoPendiente> lista = new List<ReportePedidoPendiente>();

            foreach (DataRow fila in tabla.Rows)
            {
                lista.Add(new ReportePedidoPendiente(
                    Convert.ToInt32(fila["Numero_Pedido"]),
                    Convert.ToDateTime(fila["Fecha_Pedido"]),
                    fila["Cliente"].ToString(),
                    fila["Usuario"].ToString(),
                    Convert.ToInt32(fila["Lineas"]),
                    Convert.ToInt32(fila["Unidades"])));
            }

            return lista;
        }

        public List<ReporteAviso> AvisosPorPeriodo(DateTime? desde, DateTime? hasta)
        {
            SqlParameter[] parametros =
            {
                new SqlParameter("@Fecha_Desde", SqlDbType.Date) { Value = ValorONulo(desde) },
                new SqlParameter("@Fecha_Hasta", SqlDbType.Date) { Value = ValorONulo(hasta) }
            };

            DataTable tabla = EjecutarConsulta("dbo.sp_Rpt_AvisosPorPeriodo", parametros);

            List<ReporteAviso> lista = new List<ReporteAviso>();

            foreach (DataRow fila in tabla.Rows)
            {
                lista.Add(new ReporteAviso(
                    Convert.ToInt32(fila["Numero_Aviso"]),
                    Convert.ToDateTime(fila["Fecha_Aviso"]),
                    Convert.ToInt32(fila["Numero_Pedido"]),
                    Convert.ToDateTime(fila["Fecha_Pedido"]),
                    fila["Cliente"].ToString(),
                    fila["Usuario"].ToString(),
                    fila["Estado_Pedido"].ToString()));
            }

            return lista;
        }

        public List<ReporteTiempoPreparacion> TiempoPromedioPreparacion(DateTime? desde, DateTime? hasta)
        {
            SqlParameter[] parametros =
            {
                new SqlParameter("@Fecha_Desde", SqlDbType.Date) { Value = ValorONulo(desde) },
                new SqlParameter("@Fecha_Hasta", SqlDbType.Date) { Value = ValorONulo(hasta) }
            };

            DataTable tabla = EjecutarConsulta("dbo.sp_Rpt_TiempoPromedioPreparacion", parametros);

            List<ReporteTiempoPreparacion> lista = new List<ReporteTiempoPreparacion>();

            foreach (DataRow fila in tabla.Rows)
            {
                lista.Add(new ReporteTiempoPreparacion(
                    Convert.ToInt32(fila["Pedidos_Preparados"]),
                    ADecimal(fila["Promedio_Horas"]),
                    ADecimal(fila["Minimo_Horas"]),
                    ADecimal(fila["Maximo_Horas"])));
            }

            return lista;
        }

        private static decimal ADecimal(object valor)
        {
            if (valor == DBNull.Value)
            {
                return 0;
            }
            return Convert.ToDecimal(valor);
        }
    }
}
