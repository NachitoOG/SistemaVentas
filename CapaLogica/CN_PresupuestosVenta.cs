using System;
using System.Collections.Generic;
using CapaDatos;
using CapaEntidades;

namespace CapaLogica
{
    public class CN_PresupuestosVenta
    {
        private readonly CD_PresupuestosVenta cdPresupuestos = new CD_PresupuestosVenta();
        private readonly CD_DetallesPresupuestoVenta cdDetalles = new CD_DetallesPresupuestoVenta();
        private readonly CD_ListasVentas cdListas = new CD_ListasVentas();

        public int Alta(PresupuestoVenta presupuesto)
        {
            Validar(presupuesto);

            int idNuevo = cdPresupuestos.Alta(presupuesto);
            presupuesto.IdPresupuesto = idNuevo;

            GuardarLineas(presupuesto);

            return idNuevo;
        }

        public void Modificar(PresupuestoVenta presupuesto)
        {
            if (presupuesto == null || presupuesto.IdPresupuesto <= 0)
                throw new ArgumentException("Seleccione un presupuesto para modificar.");

            ValidarQueSePuedaModificar(presupuesto);
            Validar(presupuesto);

            PresupuestoVenta original = cdPresupuestos.ObtenerPorId(presupuesto.IdPresupuesto);
            if (original != null && original.IdCliente != presupuesto.IdCliente)
                throw new ArgumentException("No se puede cambiar el cliente de un presupuesto. Si es para otro cliente, hacé un presupuesto nuevo.");

            cdPresupuestos.Modificar(presupuesto);
            cdDetalles.BorrarPorPresupuesto(presupuesto.IdPresupuesto);
            GuardarLineas(presupuesto);
        }

        public void Aprobar(PresupuestoVenta presupuesto)
        {
            CambiarEstado(presupuesto, PresupuestoVenta.APROBADO);
        }

        public void Rechazar(PresupuestoVenta presupuesto)
        {
            CambiarEstado(presupuesto, PresupuestoVenta.RECHAZADO);
        }

        public void Baja(PresupuestoVenta presupuesto)
        {
            if (presupuesto == null || presupuesto.IdPresupuesto <= 0)
                throw new ArgumentException("Seleccione un presupuesto para dar de baja.");

            if (!PuedeDarseDeBaja(presupuesto))
                throw new ArgumentException("Solo se puede dar de baja un presupuesto activo que no esté aprobado.");

            cdPresupuestos.Baja(presupuesto.IdPresupuesto);
        }

        public List<PresupuestoVenta> Listar(bool incluirBajas)
        {
            return cdPresupuestos.Listar(incluirBajas);
        }

        public List<PresupuestoVenta> Consultar(int? idCliente, string estado, bool incluirBajas)
        {
            return cdPresupuestos.Consultar(null, idCliente, estado, incluirBajas);
        }

        public PresupuestoVenta ObtenerPorId(int idPresupuesto)
        {
            if (idPresupuesto <= 0)
                throw new ArgumentException("Seleccione un presupuesto.");

            PresupuestoVenta presupuesto = cdPresupuestos.ObtenerPorId(idPresupuesto);

            if (presupuesto == null)
                throw new ArgumentException("El presupuesto N° " + idPresupuesto + " no existe.");

            presupuesto.Lineas = cdDetalles.Listar(idPresupuesto);
            return presupuesto;
        }

        public void AgregarLinea(PresupuestoVenta presupuesto, DetallePresupuestoVenta linea)
        {
            ValidarQueSePuedaModificar(presupuesto);
            ValidarLinea(linea);

            presupuesto.Lineas.Add(linea);
        }

        public void QuitarLinea(PresupuestoVenta presupuesto, DetallePresupuestoVenta linea)
        {
            ValidarQueSePuedaModificar(presupuesto);

            if (linea == null)
                throw new ArgumentException("Seleccione la línea que quiere quitar.");

            presupuesto.Lineas.Remove(linea);
        }

        public bool PuedeModificarse(PresupuestoVenta presupuesto)
        {
            if (presupuesto == null)
                return false;

            return presupuesto.Activo && presupuesto.Estado == PresupuestoVenta.PENDIENTE;
        }

        public bool PuedeCambiarCliente(PresupuestoVenta presupuesto)
        {
            if (presupuesto == null)
                return false;

            return presupuesto.IdPresupuesto == 0;
        }

        public bool PuedeDarseDeBaja(PresupuestoVenta presupuesto)
        {
            if (presupuesto == null)
                return false;

            return presupuesto.Activo && presupuesto.Estado != PresupuestoVenta.APROBADO;
        }

        private void CambiarEstado(PresupuestoVenta presupuesto, string estadoNuevo)
        {
            if (presupuesto == null || presupuesto.IdPresupuesto <= 0)
                throw new ArgumentException("Seleccione un presupuesto.");

            ValidarQueSePuedaModificar(presupuesto);

            cdPresupuestos.CambiarEstado(presupuesto.IdPresupuesto, estadoNuevo);
        }

        private void ValidarQueSePuedaModificar(PresupuestoVenta presupuesto)
        {
            if (presupuesto == null)
                throw new ArgumentException("No se recibieron los datos del presupuesto.");

            if (!presupuesto.Activo)
                throw new ArgumentException("El presupuesto está dado de baja.");

            if (!PuedeModificarse(presupuesto))
                throw new ArgumentException("El presupuesto ya está " + presupuesto.Estado + ". Solo se puede cambiar uno Pendiente.");
        }

        private void Validar(PresupuestoVenta presupuesto)
        {
            if (presupuesto == null)
                throw new ArgumentException("No se recibieron los datos del presupuesto.");

            if (presupuesto.IdCliente <= 0)
                throw new ArgumentException("Elija un cliente.");

            if (presupuesto.IdUsuario <= 0)
                throw new ArgumentException("Elija el usuario que emite el presupuesto.");

            if (EsDeDeposito(presupuesto.IdUsuario))
                throw new ArgumentException("Los presupuestos los hace Ventas: un usuario de Depósito no puede emitirlos.");

            if (presupuesto.FechaEmision.Date > DateTime.Today)
                throw new ArgumentException("La fecha de emisión no puede ser futura.");

            if (presupuesto.Lineas.Count == 0)
                throw new ArgumentException("El presupuesto tiene que tener al menos una línea.");

            foreach (DetallePresupuestoVenta linea in presupuesto.Lineas)
            {
                ValidarLinea(linea);
            }
        }

        private void ValidarLinea(DetallePresupuestoVenta linea)
        {
            if (linea == null)
                throw new ArgumentException("No se recibieron los datos de la línea.");

            if (linea.IdProducto <= 0)
                throw new ArgumentException("Elija un producto.");

            if (linea.Cantidad <= 0)
                throw new ArgumentException("La cantidad tiene que ser mayor a cero.");

            if (linea.PrecioUnitario < 0)
                throw new ArgumentException("El precio no puede ser negativo.");
        }

        private bool EsDeDeposito(int idUsuario)
        {
            foreach (UsuarioVentas usuario in cdListas.ListarUsuarios())
            {
                if (usuario.IdUsuario == idUsuario)
                    return usuario.EsDeposito;
            }

            return false;
        }

        private void GuardarLineas(PresupuestoVenta presupuesto)
        {
            foreach (DetallePresupuestoVenta linea in presupuesto.Lineas)
            {
                linea.IdPresupuesto = presupuesto.IdPresupuesto;
                linea.IdDetalle = cdDetalles.Alta(linea);
            }
        }
    }
}
