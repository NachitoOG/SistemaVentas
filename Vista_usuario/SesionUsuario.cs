using System.Collections.Generic;

namespace Vista_usuario
{
    public static class SesionUsuario
    {
        public const string FAMILIA_ADMINISTRACION = "Administración";
        public const string FAMILIA_VENTAS = "Ventas";
        public const string FAMILIA_OTROS = "Otros módulos";

        public static int IdUsuario { get; set; }
        public static string NombreUsuario { get; set; }
        public static string NombreCompleto { get; set; }

        public static List<string> Roles { get; set; } = new List<string>();
        public static List<string> Familias { get; set; } = new List<string>();

        public static bool TieneRol(string nombreRol)
        {
            return Roles.Contains(nombreRol);
        }

        public static bool TieneFamilia(string nombreFamilia)
        {
            return Familias.Contains(nombreFamilia);
        }

        public static void CerrarSesion()
        {
            IdUsuario = 0;
            NombreUsuario = "";
            NombreCompleto = "";
            Roles.Clear();
            Familias.Clear();
        }
    }
}
