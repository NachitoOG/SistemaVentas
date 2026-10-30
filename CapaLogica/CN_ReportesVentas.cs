using System;
using System.Collections.Generic;
using CapaDatos;
using CapaEntidades;

namespace CapaLogica
{
    public class CN_ReportesVentas
    {
        private readonly CD_ReportesVentas cdReportes = new CD_ReportesVentas();

        public List<ReportePresupuestosVenta> PresupuestosPorClienteEstado(int? idCliente, string estado, DateTime? desde, DateTime? hasta)
        {
            ValidarPeriodo(desde, hasta);
            return cdReportes.PresupuestosPorClienteEstado(idCliente, estado, desde, hasta);
        }

        public List<ReporteNotasPorEstado> NotasPedidoPorEstado(DateTime? desde, DateTime? hasta)
        {
            ValidarPeriodo(desde, hasta);
            return cdReportes.NotasPedidoPorEstado(desde, hasta);
        }

        public List<ReportePedidoPendiente> PedidosPendientesPreparar(int? idCliente)
        {
            return cdReportes.PedidosPendientesPreparar(idCliente);
        }

        public List<ReporteAviso> AvisosPorPeriodo(DateTime? desde, DateTime? hasta)
        {
            ValidarPeriodo(desde, hasta);
            return cdReportes.AvisosPorPeriodo(desde, hasta);
        }

        public List<ReporteTiempoPreparacion> TiempoPromedioPreparacion(DateTime? desde, DateTime? hasta)
        {
            ValidarPeriodo(desde, hasta);
            return cdReportes.TiempoPromedioPreparacion(desde, hasta);
        }

        private void ValidarPeriodo(DateTime? desde, DateTime? hasta)
        {
            if (desde != null && desde.Value.Date > DateTime.Today)
                throw new ArgumentException("La fecha 'Desde' no puede ser futura.");

            if (desde != null && hasta != null && desde.Value.Date > hasta.Value.Date)
                throw new ArgumentException("La fecha 'Desde' no puede ser posterior a la fecha 'Hasta'.");
        }
    }
}
