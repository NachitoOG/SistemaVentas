using System;
using System.Collections.Generic;
using CapaDatos;
using CapaEntidades;

namespace CapaLogica
{
    public class CN_ListasVentas
    {
        private readonly CD_ListasVentas cdListas = new CD_ListasVentas();

        public List<ClienteVentas> ListarClientes()
        {
            return cdListas.ListarClientes();
        }

        public List<ProductoVentas> ListarProductos()
        {
            return cdListas.ListarProductos();
        }

        public List<UsuarioVentas> ListarUsuarios()
        {
            return cdListas.ListarUsuarios();
        }

        public UsuarioVentas ObtenerUsuario(int idUsuario)
        {
            foreach (UsuarioVentas usuario in cdListas.ListarUsuarios())
            {
                if (usuario.IdUsuario == idUsuario)
                    return usuario;
            }

            throw new ArgumentException("El usuario N° " + idUsuario + " no está activo o no existe.");
        }
    }
}
