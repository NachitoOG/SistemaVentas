using System;
using System.Collections.Generic;
using CapaDatos;
using CapaEntidades;

namespace CapaLogica
{
    public class CN_NotasPedido
    {
        private readonly CD_NotasPedido cdNotas = new CD_NotasPedido();
        private readonly CD_DetallesNotaPedido cdDetalles = new CD_DetallesNotaPedido();
        private readonly CD_PresupuestosVenta cdPresupuestos = new CD_PresupuestosVenta();
        private readonly CD_ListasVentas cdListas = new CD_ListasVentas();

        public int Alta(NotaPedido nota)
        {
            Validar(nota);

            int idNuevo = cdNotas.Alta(nota);
            nota.IdNotaPedido = idNuevo;

            GuardarLineas(nota);

            return idNuevo;
        }

        public int GenerarDesdePresupuesto(PresupuestoVenta presupuesto, int idUsuario)
        {
            if (presupuesto == null || presupuesto.IdPresupuesto <= 0)
                throw new ArgumentException("Elija el presupuesto aprobado del que sale el pedido.");

            if (idUsuario <= 0)
                throw new ArgumentException("Elija en 'Emitido por' el usuario que toma el pedido.");

            if (EsDeDeposito(idUsuario))
                throw new ArgumentException("Las notas de pedido las hace Ventas: un usuario de Depósito no puede emitirlas.");

            if (!presupuesto.Activo || presupuesto.Estado != PresupuestoVenta.APROBADO)
                throw new ArgumentException("Solo se puede generar una nota de pedido desde un presupuesto Aprobado.");

            if (TieneNotaDePedido(presupuesto.IdPresupuesto))
                throw new ArgumentException("El presupuesto N° " + presupuesto.IdPresupuesto + " ya tiene una nota de pedido.");

            return cdNotas.GenerarDesdePresupuesto(presupuesto.IdPresupuesto, idUsuario);
        }

        public void Modificar(NotaPedido nota)
        {
            if (nota == null || nota.IdNotaPedido <= 0)
                throw new ArgumentException("Seleccione una nota de pedido para modificar.");

            ValidarQueSePuedaModificar(nota);
            Validar(nota);

            NotaPedido original = cdNotas.ObtenerPorId(nota.IdNotaPedido);
            if (original != null && original.IdCliente != nota.IdCliente)
                throw new ArgumentException("No se puede cambiar el cliente de una nota de pedido. Si es para otro cliente, hacé una nota nueva.");

            cdNotas.Modificar(nota);
            cdDetalles.BorrarPorNotaPedido(nota.IdNotaPedido);
            GuardarLineas(nota);
        }

        public void MarcarEntregado(NotaPedido nota)
        {
            CambiarEstado(nota, NotaPedido.ENTREGADO);
        }

        public void MarcarFacturado(NotaPedido nota)
        {
            CambiarEstado(nota, NotaPedido.FACTURADO);
        }

        public void Baja(NotaPedido nota)
        {
            if (nota == null || nota.IdNotaPedido <= 0)
                throw new ArgumentException("Seleccione una nota de pedido para dar de baja.");

            if (!PuedeDarseDeBaja(nota))
                throw new ArgumentException("Solo se puede dar de baja una nota de pedido activa que esté Pendiente.");

            cdNotas.Baja(nota.IdNotaPedido);
        }

        public List<NotaPedido> Listar(bool incluirBajas)
        {
            return cdNotas.Listar(incluirBajas);
        }

        public List<NotaPedido> Consultar(int? idCliente, string estado, bool incluirBajas)
        {
            return cdNotas.Consultar(null, idCliente, estado, incluirBajas);
        }

        public NotaPedido ObtenerPorId(int idNotaPedido)
        {
            if (idNotaPedido <= 0)
                throw new ArgumentException("Seleccione una nota de pedido.");

            NotaPedido nota = cdNotas.ObtenerPorId(idNotaPedido);

            if (nota == null)
                throw new ArgumentException("La nota de pedido N° " + idNotaPedido + " no existe.");

            nota.Lineas = cdDetalles.Listar(idNotaPedido);
            return nota;
        }

        public List<PresupuestoVenta> ListarPresupuestosDisponibles()
        {
            List<PresupuestoVenta> aprobados = cdPresupuestos.Consultar(null, null, PresupuestoVenta.APROBADO, false);
            List<NotaPedido> notas = cdNotas.Listar(false);
            List<PresupuestoVenta> disponibles = new List<PresupuestoVenta>();

            foreach (PresupuestoVenta presupuesto in aprobados)
            {
                bool yaTieneNota = false;

                foreach (NotaPedido nota in notas)
                {
                    if (nota.IdPresupuesto == presupuesto.IdPresupuesto)
                        yaTieneNota = true;
                }

                if (!yaTieneNota)
                    disponibles.Add(presupuesto);
            }

            return disponibles;
        }

        public void AgregarLinea(NotaPedido nota, DetalleNotaPedido linea)
        {
            ValidarQueSePuedaModificar(nota);
            ValidarLinea(linea);

            nota.Lineas.Add(linea);
        }

        public void QuitarLinea(NotaPedido nota, DetalleNotaPedido linea)
        {
            ValidarQueSePuedaModificar(nota);

            if (linea == null)
                throw new ArgumentException("Seleccione la línea que quiere quitar.");

            nota.Lineas.Remove(linea);
        }

        public bool PuedeModificarse(NotaPedido nota)
        {
            if (nota == null)
                return false;

            return nota.Activo && nota.Estado == NotaPedido.PENDIENTE;
        }

        public bool PuedeDarseDeBaja(NotaPedido nota)
        {
            if (nota == null)
                return false;

            return nota.Activo && nota.Estado == NotaPedido.PENDIENTE;
        }

        public bool PuedeCambiarCliente(NotaPedido nota)
        {
            if (nota == null)
                return false;

            return nota.IdNotaPedido == 0;
        }

        public bool PuedePasarA(NotaPedido nota, string estadoNuevo)
        {
            if (nota == null || !nota.Activo)
                return false;

            if (estadoNuevo == NotaPedido.PREPARADO)
                return false;

            return SiguienteEstado(nota.Estado) == estadoNuevo;
        }

        private string SiguienteEstado(string estadoActual)
        {
            if (estadoActual == NotaPedido.PENDIENTE)
                return NotaPedido.PREPARADO;

            if (estadoActual == NotaPedido.PREPARADO)
                return NotaPedido.ENTREGADO;

            if (estadoActual == NotaPedido.ENTREGADO)
                return NotaPedido.FACTURADO;

            return "";
        }

        private void CambiarEstado(NotaPedido nota, string estadoNuevo)
        {
            if (nota == null || nota.IdNotaPedido <= 0)
                throw new ArgumentException("Seleccione una nota de pedido.");

            if (!nota.Activo)
                throw new ArgumentException("La nota de pedido está dada de baja.");

            if (estadoNuevo == NotaPedido.PREPARADO)
                throw new ArgumentException("Un pedido pasa a Preparado cuando Depósito carga el aviso de mercadería preparada.");

            if (SiguienteEstado(nota.Estado) != estadoNuevo)
                throw new ArgumentException("No se puede pasar de " + nota.Estado + " a " + estadoNuevo +
                                            ". El orden es Pendiente, Preparado, Entregado, Facturado.");

            cdNotas.CambiarEstado(nota.IdNotaPedido, estadoNuevo);
        }

        private bool TieneNotaDePedido(int idPresupuesto)
        {
            foreach (NotaPedido nota in cdNotas.Listar(false))
            {
                if (nota.IdPresupuesto == idPresupuesto)
                    return true;
            }

            return false;
        }

        private void ValidarQueSePuedaModificar(NotaPedido nota)
        {
            if (nota == null)
                throw new ArgumentException("No se recibieron los datos de la nota de pedido.");

            if (!nota.Activo)
                throw new ArgumentException("La nota de pedido está dada de baja.");

            if (!PuedeModificarse(nota))
                throw new ArgumentException("La nota de pedido está en estado " + nota.Estado + ". Solo se puede cambiar una Pendiente.");
        }

        private void Validar(NotaPedido nota)
        {
            if (nota == null)
                throw new ArgumentException("No se recibieron los datos de la nota de pedido.");

            if (nota.IdCliente <= 0)
                throw new ArgumentException("Elija un cliente.");

            if (nota.IdUsuario <= 0)
                throw new ArgumentException("Elija el usuario que toma el pedido.");

            if (EsDeDeposito(nota.IdUsuario))
                throw new ArgumentException("Las notas de pedido las hace Ventas: un usuario de Depósito no puede emitirlas.");

            if (nota.FechaEmision.Date > DateTime.Today)
                throw new ArgumentException("La fecha de emisión no puede ser futura.");

            if (nota.Lineas.Count == 0)
                throw new ArgumentException("La nota de pedido tiene que tener al menos una línea.");

            foreach (DetalleNotaPedido linea in nota.Lineas)
            {
                ValidarLinea(linea);
            }
        }

        private void ValidarLinea(DetalleNotaPedido linea)
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

        private void GuardarLineas(NotaPedido nota)
        {
            foreach (DetalleNotaPedido linea in nota.Lineas)
            {
                linea.IdNotaPedido = nota.IdNotaPedido;
                linea.IdDetalle = cdDetalles.Alta(linea);
            }
        }
    }
}
