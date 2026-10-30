using System;
using System.Collections.Generic;

namespace CapaEntidades
{
    public class NotaPedido
    {
        public const string PENDIENTE = "Pendiente";
        public const string PREPARADO = "Preparado";
        public const string ENTREGADO = "Entregado";
        public const string FACTURADO = "Facturado";

        public int IdNotaPedido { get; set; }
        public int IdCliente { get; set; }
        public string NombreCliente { get; set; }
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public int? IdPresupuesto { get; set; }
        public DateTime FechaEmision { get; set; }
        public string Estado { get; set; }
        public bool Activo { get; set; }
        public decimal Total { get; set; }
        public List<DetalleNotaPedido> Lineas { get; set; }

        public string EstadoTexto
        {
            get
            {
                if (Activo)
                    return Estado;

                return "Dado de baja";
            }
        }

        public string Origen
        {
            get
            {
                if (IdPresupuesto == null)
                    return "Pedido directo";

                return "Presupuesto N° " + IdPresupuesto;
            }
        }

        public NotaPedido()
        {
            FechaEmision = DateTime.Now;
            Estado = PENDIENTE;
            Activo = true;
            Lineas = new List<DetalleNotaPedido>();
        }

        public NotaPedido(int idCliente, int idUsuario, DateTime fechaEmision)
            : this()
        {
            IdCliente = idCliente;
            IdUsuario = idUsuario;
            FechaEmision = fechaEmision;
        }

        public NotaPedido(int idNotaPedido, int idCliente, int idUsuario, int? idPresupuesto, DateTime fechaEmision, string estado, bool activo)
            : this(idCliente, idUsuario, fechaEmision)
        {
            IdNotaPedido = idNotaPedido;
            IdPresupuesto = idPresupuesto;
            Estado = estado;
            Activo = activo;
        }

        public decimal CalcularTotal()
        {
            decimal total = 0;

            foreach (DetalleNotaPedido linea in Lineas)
            {
                total = total + linea.Subtotal;
            }

            return total;
        }

        public override string ToString()
        {
            return "Nota de pedido N° " + IdNotaPedido + " - " + NombreCliente;
        }
    }
}
