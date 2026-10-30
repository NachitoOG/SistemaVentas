namespace CapaEntidades
{
    public class ReportePresupuestosVenta
    {
        public string Cliente { get; set; }
        public string Estado { get; set; }
        public int Cantidad { get; set; }
        public decimal MontoTotal { get; set; }

        public ReportePresupuestosVenta()
        {
        }

        public ReportePresupuestosVenta(string cliente, string estado, int cantidad, decimal montoTotal)
        {
            Cliente = cliente;
            Estado = estado;
            Cantidad = cantidad;
            MontoTotal = montoTotal;
        }
    }
}
