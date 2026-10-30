using System;
using System.Collections.Generic;
using CapaDatos;
using CapaEntidades;

namespace CapaLogica
{
    public class CN_Avisos
    {
        private readonly CD_Avisos cdAvisos = new CD_Avisos();
        private readonly CD_NotasPedido cdNotas = new CD_NotasPedido();
        private readonly CD_DetallesNotaPedido cdDetalles = new CD_DetallesNotaPedido();
        private readonly CD_ListasVentas cdListas = new CD_ListasVentas();

        public int Alta(AvisoMercaderia aviso)
        {
            Validar(aviso);

            NotaPedido nota = cdNotas.ObtenerPorId(aviso.IdNotaPedido);

            if (nota == null || !nota.Activo)
                throw new ArgumentException("La nota de pedido no existe o está dada de baja.");

            if (nota.Estado != NotaPedido.PENDIENTE)
                throw new ArgumentException("Solo se puede avisar la preparación de un pedido Pendiente. Este está en estado " + nota.Estado + ".");

            if (cdDetalles.Listar(nota.IdNotaPedido).Count == 0)
                throw new ArgumentException("El pedido no tiene líneas para preparar.");

            ValidarFechaContraPedido(aviso, nota);

            return cdAvisos.Alta(aviso);
        }

        public void Modificar(AvisoMercaderia aviso)
        {
            if (aviso == null || aviso.IdAviso <= 0)
                throw new ArgumentException("Seleccione un aviso para modificar.");

            if (!PuedeModificarse(aviso))
                throw new ArgumentException("El aviso está dado de baja: no se puede modificar.");

            Validar(aviso);

            NotaPedido nota = cdNotas.ObtenerPorId(aviso.IdNotaPedido);
            ValidarFechaContraPedido(aviso, nota);

            cdAvisos.Modificar(aviso);
        }

        public void Baja(AvisoMercaderia aviso)
        {
            if (aviso == null || aviso.IdAviso <= 0)
                throw new ArgumentException("Seleccione un aviso para dar de baja.");

            if (!PuedeDarseDeBaja(aviso))
                throw new ArgumentException("Solo se puede dar de baja un aviso activo cuyo pedido siga en estado Preparado.");

            cdAvisos.Baja(aviso.IdAviso);
        }

        public List<AvisoMercaderia> Listar(bool incluirBajas)
        {
            return cdAvisos.Listar(incluirBajas);
        }

        public List<AvisoMercaderia> Consultar(DateTime? desde, DateTime? hasta, bool incluirBajas)
        {
            if (desde != null && hasta != null && desde.Value.Date > hasta.Value.Date)
                throw new ArgumentException("La fecha 'Desde' no puede ser posterior a la fecha 'Hasta'.");

            return cdAvisos.Consultar(null, null, desde, hasta, incluirBajas);
        }

        public AvisoMercaderia ObtenerPorId(int idAviso)
        {
            if (idAviso <= 0)
                throw new ArgumentException("Seleccione un aviso.");

            AvisoMercaderia aviso = cdAvisos.ObtenerPorId(idAviso);

            if (aviso == null)
                throw new ArgumentException("El aviso N° " + idAviso + " no existe.");

            return aviso;
        }

        public List<NotaPedido> ListarPedidosPendientes()
        {
            return cdNotas.Consultar(null, null, NotaPedido.PENDIENTE, false);
        }

        public List<DetalleNotaPedido> ListarLineasDelPedido(int idNotaPedido)
        {
            return cdDetalles.Listar(idNotaPedido);
        }

        public List<UsuarioVentas> ListarEmpleadosDeposito()
        {
            List<UsuarioVentas> empleados = new List<UsuarioVentas>();

            foreach (UsuarioVentas usuario in cdListas.ListarUsuarios())
            {
                if (usuario.Rol == UsuarioVentas.ROL_DEPOSITO)
                    empleados.Add(usuario);
            }

            return empleados;
        }

        public bool PuedeModificarse(AvisoMercaderia aviso)
        {
            if (aviso == null)
                return false;

            return aviso.Activo;
        }

        public bool PuedeDarseDeBaja(AvisoMercaderia aviso)
        {
            if (aviso == null)
                return false;

            return aviso.Activo && aviso.EstadoPedido == NotaPedido.PREPARADO;
        }

        private void Validar(AvisoMercaderia aviso)
        {
            if (aviso == null)
                throw new ArgumentException("No se recibieron los datos del aviso.");

            if (aviso.IdNotaPedido <= 0)
                throw new ArgumentException("Elija el pedido que se preparó.");

            if (aviso.IdUsuario <= 0)
                throw new ArgumentException("Elija el empleado de Depósito que preparó el pedido.");

            if (!EsDeDeposito(aviso.IdUsuario))
                throw new ArgumentException("El aviso lo tiene que cargar un empleado de Depósito.");

            if (aviso.FechaEmision > DateTime.Now)
                throw new ArgumentException("La fecha del aviso no puede ser futura.");
        }

        private void ValidarFechaContraPedido(AvisoMercaderia aviso, NotaPedido nota)
        {
            if (nota != null && aviso.FechaEmision < nota.FechaEmision.AddSeconds(-1))
                throw new ArgumentException("La fecha del aviso no puede ser anterior a la del pedido (" +
                                            nota.FechaEmision.ToString("dd/MM/yyyy HH:mm") + ").");
        }

        public bool EsDeDeposito(int idUsuario)
        {
            foreach (UsuarioVentas usuario in cdListas.ListarUsuarios())
            {
                if (usuario.IdUsuario == idUsuario && usuario.Rol == UsuarioVentas.ROL_DEPOSITO)
                    return true;
            }

            return false;
        }
    }
}
