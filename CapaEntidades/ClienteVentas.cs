namespace CapaEntidades
{
    public class ClienteVentas
    {
        public int IdCliente { get; set; }
        public string Nombre { get; set; }
        public string CuitDni { get; set; }
        public decimal Deuda { get; set; }

        public bool EsDeudor
        {
            get { return Deuda > 0; }
        }

        public ClienteVentas()
        {
        }

        public ClienteVentas(int idCliente, string nombre, string cuitDni)
        {
            IdCliente = idCliente;
            Nombre = nombre;
            CuitDni = cuitDni;
        }

        public ClienteVentas(int idCliente, string nombre, string cuitDni, decimal deuda)
            : this(idCliente, nombre, cuitDni)
        {
            Deuda = deuda;
        }

        public override string ToString()
        {
            return Nombre;
        }
    }
}
