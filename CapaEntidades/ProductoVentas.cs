namespace CapaEntidades
{
    public class ProductoVentas
    {
        public int IdProducto { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public decimal PrecioActual { get; set; }
        public int StockActual { get; set; }

        public ProductoVentas()
        {
        }

        public ProductoVentas(int idProducto, string codigo, string descripcion, decimal precioActual, int stockActual)
        {
            IdProducto = idProducto;
            Codigo = codigo;
            Descripcion = descripcion;
            PrecioActual = precioActual;
            StockActual = stockActual;
        }

        public override string ToString()
        {
            return Codigo + " - " + Descripcion;
        }
    }
}
