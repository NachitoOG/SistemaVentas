namespace CapaEntidades
{
    public class DetalleNotaPedido
    {
        public int IdDetalle { get; set; }
        public int IdNotaPedido { get; set; }
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        public decimal Subtotal
        {
            get { return Cantidad * PrecioUnitario; }
        }

        public DetalleNotaPedido()
        {
        }

        public DetalleNotaPedido(int idProducto, string nombreProducto, int cantidad, decimal precioUnitario)
        {
            IdProducto = idProducto;
            NombreProducto = nombreProducto;
            Cantidad = cantidad;
            PrecioUnitario = precioUnitario;
        }

        public DetalleNotaPedido(int idDetalle, int idNotaPedido, int idProducto, string nombreProducto, int cantidad, decimal precioUnitario)
            : this(idProducto, nombreProducto, cantidad, precioUnitario)
        {
            IdDetalle = idDetalle;
            IdNotaPedido = idNotaPedido;
        }

        public override string ToString()
        {
            return NombreProducto + " x " + Cantidad;
        }
    }
}
