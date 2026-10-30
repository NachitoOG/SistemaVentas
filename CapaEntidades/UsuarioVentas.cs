namespace CapaEntidades
{
    public class UsuarioVentas
    {
        public const string ROL_DEPOSITO = "Deposito";

        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string NombreCompleto { get; set; }
        public string Rol { get; set; }

        public bool EsDeposito
        {
            get { return Rol == ROL_DEPOSITO; }
        }

        public UsuarioVentas()
        {
        }

        public UsuarioVentas(int idUsuario, string nombreUsuario, string nombreCompleto, string rol)
        {
            IdUsuario = idUsuario;
            NombreUsuario = nombreUsuario;
            NombreCompleto = nombreCompleto;
            Rol = rol;
        }

        public override string ToString()
        {
            return NombreCompleto + " (" + Rol + ")";
        }
    }
}
