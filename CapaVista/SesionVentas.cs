using CapaEntidades;

namespace CapaVista
{
    public static class SesionVentas
    {
        public static UsuarioVentas UsuarioActual { get; set; }

        public static bool HaySesion()
        {
            return UsuarioActual != null;
        }

        public static void Cerrar()
        {
            UsuarioActual = null;
        }
    }
}
