namespace CapaEntidades
{
    public class ReporteNotasPorEstado
    {
        public string Estado { get; set; }
        public int Cantidad { get; set; }
        public decimal MontoTotal { get; set; }

        public ReporteNotasPorEstado()
        {
        }

        public ReporteNotasPorEstado(string estado, int cantidad, decimal montoTotal)
        {
            Estado = estado;
            Cantidad = cantidad;
            MontoTotal = montoTotal;
        }
    }
}
