namespace CapaEntidades
{
    public class ReporteTiempoPreparacion
    {
        public int PedidosPreparados { get; set; }
        public decimal PromedioHoras { get; set; }
        public decimal MinimoHoras { get; set; }
        public decimal MaximoHoras { get; set; }

        public decimal PromedioDias
        {
            get { return PromedioHoras / 24; }
        }

        public ReporteTiempoPreparacion()
        {
        }

        public ReporteTiempoPreparacion(int pedidosPreparados, decimal promedioHoras, decimal minimoHoras, decimal maximoHoras)
        {
            PedidosPreparados = pedidosPreparados;
            PromedioHoras = promedioHoras;
            MinimoHoras = minimoHoras;
            MaximoHoras = maximoHoras;
        }
    }
}
