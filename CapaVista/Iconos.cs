using System;
using System.Windows.Forms;

namespace CapaVista
{
    public static class Iconos
    {
        public const string DETALLES = "";
        public const string EDITAR = "";
        public const string APROBAR = "";
        public const string RECHAZAR = "";
        public const string BAJA = "";
        public const string ENTREGADO = "";
        public const string FACTURADO = "";

        public static string Mostrar(string icono, bool corresponde)
        {
            if (corresponde)
            {
                return icono;
            }

            return "";
        }

        public static bool HayIcono(DataGridView grilla, int columna, int fila)
        {
            if (columna < 0 || fila < 0)
            {
                return false;
            }

            if (!grilla.Columns[columna].Name.StartsWith("ico"))
            {
                return false;
            }

            return Convert.ToString(grilla[columna, fila].FormattedValue) != "";
        }

        public static void CambiarCursor(DataGridView grilla, int columna, int fila)
        {
            if (HayIcono(grilla, columna, fila))
            {
                grilla.Cursor = Cursors.Hand;
            }
            else
            {
                grilla.Cursor = Cursors.Default;
            }
        }

        public static string Ayuda(DataGridView grilla, int columna, int fila)
        {
            if (HayIcono(grilla, columna, fila))
            {
                return grilla.Columns[columna].ToolTipText;
            }

            return "";
        }
    }
}
