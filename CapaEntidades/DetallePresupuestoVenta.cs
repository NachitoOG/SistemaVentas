namespace CapaEntidades
{
    public class DetallePresupuestoVenta
    {
        public int IdDetalle { get; set; }
        public int IdPresupuesto { get; set; }
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        public decimal Subtotal
        {
            get { return Cantidad * PrecioUnitario; }
        }

        public DetallePresupuestoVenta()
        {
        }

        public DetallePresupuestoVenta(int idProducto, string nombreProducto, int cantidad, decimal precioUnitario)
        {
            IdProducto = idProducto;
            NombreProducto = nombreProducto;
            Cantidad = cantidad;
            PrecioUnitario = precioUnitario;
        }

        public DetallePresupuestoVenta(int idDetalle, int idPresupuesto, int idProducto, string nombreProducto, int cantidad, decimal precioUnitario)
            : this(idProducto, nombreProducto, cantidad, precioUnitario)
        {
            IdDetalle = idDetalle;
            IdPresupuesto = idPresupuesto;
        }

        public override string ToString()
        {
            return NombreProducto + " x " + Cantidad;
        }
    }
}
