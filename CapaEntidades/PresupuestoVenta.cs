using System;
using System.Collections.Generic;

namespace CapaEntidades
{
    public class PresupuestoVenta
    {
        public const string PENDIENTE = "Pendiente";
        public const string APROBADO = "Aprobado";
        public const string RECHAZADO = "Rechazado";

        public int IdPresupuesto { get; set; }
        public int IdCliente { get; set; }
        public string NombreCliente { get; set; }
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public DateTime FechaEmision { get; set; }
        public string Estado { get; set; }
        public bool Activo { get; set; }
        public decimal Total { get; set; }
        public List<DetallePresupuestoVenta> Lineas { get; set; }

        public string EstadoTexto
        {
            get
            {
                if (Activo)
                    return Estado;

                return "Dado de baja";
            }
        }

        public PresupuestoVenta()
        {
            FechaEmision = DateTime.Today;
            Estado = PENDIENTE;
            Activo = true;
            Lineas = new List<DetallePresupuestoVenta>();
        }

        public PresupuestoVenta(int idCliente, int idUsuario, DateTime fechaEmision)
            : this()
        {
            IdCliente = idCliente;
            IdUsuario = idUsuario;
            FechaEmision = fechaEmision;
        }

        public PresupuestoVenta(int idPresupuesto, int idCliente, int idUsuario, DateTime fechaEmision, string estado, bool activo)
            : this(idCliente, idUsuario, fechaEmision)
        {
            IdPresupuesto = idPresupuesto;
            Estado = estado;
            Activo = activo;
        }

        public decimal CalcularTotal()
        {
            decimal total = 0;

            foreach (DetallePresupuestoVenta linea in Lineas)
            {
                total = total + linea.Subtotal;
            }

            return total;
        }

        public override string ToString()
        {
            return "Presupuesto N° " + IdPresupuesto + " - " + NombreCliente;
        }
    }
}
