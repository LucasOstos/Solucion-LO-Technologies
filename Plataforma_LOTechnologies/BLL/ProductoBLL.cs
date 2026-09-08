using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;
using DAL;
using SERVICIO.Logica;

namespace BLL
{
    public class ProductoBLL
    {
        private ProductoDAL productoDAL = new ProductoDAL();
        private DigitoVerificador digitos = new DigitoVerificador();
        public List<ProductoListado> ObtenerProductosListado(string pBusqueda, int? pCodigoSector)
        {
            return productoDAL.ObtenerProductosListado(pBusqueda, pCodigoSector);
        }
        public ProductoBE ObtenerProductoPorCodigo(int pCodigoProducto)
        {
            return productoDAL.ObtenerProductoPorCodigo(pCodigoProducto);
        }
        public int ObtenerProximoNumeroSerie()
        {
            return productoDAL.ObtenerProximoNumeroSerie();
        }
        public void CrearProducto(ProductoBE pProducto)
        {
            int codigoNuevo = productoDAL.AgregarProducto(pProducto);
            digitos.ActualizarDigitoFila("Producto", codigoNuevo);
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "ABM de productos y/o categorías", "Gestión productos", 2);
        }
        public void ModificarProducto(ProductoBE pProducto)
        {
            productoDAL.ModificarProducto(pProducto);
            digitos.ActualizarDigitoFila("Producto", pProducto.CodigoProducto);
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "ABM de productos y/o categorías", "Gestión productos", 2);
        }
        public void CambiarEstado(int pCodigoProducto, bool pNuevoEstado)
        {
            productoDAL.CambiarEstado(pCodigoProducto, pNuevoEstado);
            digitos.ActualizarDigitoFila("Producto", pCodigoProducto);
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "ABM de productos y/o categorías", "Gestión productos", 2);
        }
    }
}
