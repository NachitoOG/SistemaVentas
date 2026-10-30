using System;

namespace CapaEntidades
{
    public class ReporteAviso
    {
        public int NumeroAviso { get; set; }
        public DateTime FechaAviso { get; set; }
        public int NumeroPedido { get; set; }
        public DateTime FechaPedido { get; set; }
        public string Cliente { get; set; }
        public string PreparadoPor { get; set; }
        public string EstadoPedido { get; set; }

        public decimal HorasPreparacion
        {
            get
            {
                TimeSpan diferencia = FechaAviso - FechaPedido;
                return Math.Round((decimal)diferencia.TotalHours, 2);
            }
        }

        public ReporteAviso()
        {
        }

        public ReporteAviso(int numeroAviso, DateTime fechaAviso, int numeroPedido, DateTime fechaPedido,
                            string cliente, string preparadoPor, string estadoPedido)
        {
            NumeroAviso = numeroAviso;
            FechaAviso = fechaAviso;
            NumeroPedido = numeroPedido;
            FechaPedido = fechaPedido;
            Cliente = cliente;
            PreparadoPor = preparadoPor;
            EstadoPedido = estadoPedido;
        }
    }
}
