using System;
using System.Collections.Generic;
using System.Data;
using CapaEntidades;

namespace CapaDatos
{
    public class CD_ListasVentas : CD_EjecutarConsultasSP
    {
        public List<ClienteVentas> ListarClientes()
        {
            DataTable tabla = EjecutarConsulta("dbo.sp_Ventas_ListarClientes", null);

            List<ClienteVentas> lista = new List<ClienteVentas>();

            foreach (DataRow fila in tabla.Rows)
            {
                lista.Add(new ClienteVentas(
                    Convert.ToInt32(fila["ID"]),
                    fila["Nombre"].ToString(),
                    fila["CUIT_DNI"].ToString(),
                    Convert.ToDecimal(fila["Deuda"])));
            }

            return lista;
        }

        public List<ProductoVentas> ListarProductos()
        {
            DataTable tabla = EjecutarConsulta("dbo.sp_Ventas_ListarProductos", null);

            List<ProductoVentas> lista = new List<ProductoVentas>();

            foreach (DataRow fila in tabla.Rows)
            {
                lista.Add(new ProductoVentas(
                    Convert.ToInt32(fila["ID"]),
                    fila["Codigo"].ToString(),
                    fila["Descripcion"].ToString(),
                    Convert.ToDecimal(fila["PrecioActual"]),
                    Convert.ToInt32(fila["StockActual"])));
            }

            return lista;
        }

        public List<UsuarioVentas> ListarUsuarios()
        {
            DataTable tabla = EjecutarConsulta("dbo.sp_Ventas_ListarUsuarios", null);

            List<UsuarioVentas> lista = new List<UsuarioVentas>();

            foreach (DataRow fila in tabla.Rows)
            {
                lista.Add(new UsuarioVentas(
                    Convert.ToInt32(fila["ID"]),
                    fila["NombreUsuario"].ToString(),
                    fila["NombreCompleto"].ToString(),
                    fila["Rol"].ToString()));
            }

            return lista;
        }
    }
}
