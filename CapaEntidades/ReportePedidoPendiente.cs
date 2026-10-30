using System;

namespace CapaEntidades
{
    public class ReportePedidoPendiente
    {
        public int NumeroPedido { get; set; }
        public DateTime FechaPedido { get; set; }
        public string Cliente { get; set; }
        public string TomadoPor { get; set; }
        public int Lineas { get; set; }
        public int Unidades { get; set; }

        public decimal HorasEsperando
        {
            get
            {
                TimeSpan diferencia = DateTime.Now - FechaPedido;
                return Math.Round((decimal)diferencia.TotalHours, 1);
            }
        }

        public ReportePedidoPendiente()
        {
        }

        public ReportePedidoPendiente(int numeroPedido, DateTime fechaPedido, string cliente, string tomadoPor, int lineas, int unidades)
        {
            NumeroPedido = numeroPedido;
            FechaPedido = fechaPedido;
            Cliente = cliente;
            TomadoPor = tomadoPor;
            Lineas = lineas;
            Unidades = unidades;
        }
    }
}
