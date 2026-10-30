using System;

namespace CapaEntidades
{
    public class AvisoMercaderia
    {
        public int IdAviso { get; set; }
        public int IdNotaPedido { get; set; }
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string NombreCliente { get; set; }
        public DateTime FechaPedido { get; set; }
        public DateTime FechaEmision { get; set; }
        public string EstadoPedido { get; set; }
        public bool Activo { get; set; }

        public decimal HorasPreparacion
        {
            get
            {
                TimeSpan diferencia = FechaEmision - FechaPedido;
                return Math.Round((decimal)diferencia.TotalHours, 2);
            }
        }

        public string EstadoTexto
        {
            get
            {
                if (Activo)
                    return "Activo";

                return "Dado de baja";
            }
        }

        public AvisoMercaderia()
        {
            FechaEmision = DateTime.Now;
            Activo = true;
        }

        public AvisoMercaderia(int idNotaPedido, int idUsuario, DateTime fechaEmision)
            : this()
        {
            IdNotaPedido = idNotaPedido;
            IdUsuario = idUsuario;
            FechaEmision = fechaEmision;
        }

        public AvisoMercaderia(int idAviso, int idNotaPedido, int idUsuario, DateTime fechaEmision, bool activo)
            : this(idNotaPedido, idUsuario, fechaEmision)
        {
            IdAviso = idAviso;
            Activo = activo;
        }

        public override string ToString()
        {
            return "Aviso N° " + IdAviso + " - Pedido N° " + IdNotaPedido;
        }
    }
}
