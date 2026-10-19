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
        public List<ProductoListado> ObtenerProductosListado(int pCodigoLocal, string pBusqueda, int? pCodigoSector)
        {
            int empresa = ControlAcceso.ResolverEmpresaPorLocal(pCodigoLocal);
            if (empresa <= 0) return new List<ProductoListado>();
            if (pCodigoSector != null) ControlAcceso.Validar(EntidadEmpresa.Sector, pCodigoSector.Value);
            return productoDAL.ObtenerProductosListado(empresa, pBusqueda, pCodigoSector);
        }
        public ProductoBE ObtenerProductoPorCodigo(int pCodigoProducto)
        {
            ControlAcceso.Validar(EntidadEmpresa.Producto, pCodigoProducto);
            return productoDAL.ObtenerProductoPorCodigo(pCodigoProducto);
        }
        public int ObtenerProximoNumeroSerie(int pCodigoLocal)
        {
            int empresa = ControlAcceso.ResolverEmpresaPorLocal(pCodigoLocal);
            return productoDAL.ObtenerProximoNumeroSerie(empresa);
        }
        public void CrearProducto(ProductoBE pProducto, int pCodigoLocal)
        {
            pProducto.CodigoEmpresa = ControlAcceso.ResolverEmpresaPorLocal(pCodigoLocal);
            if (pProducto.CodigoEmpresa <= 0) throw new InvalidOperationException("Seleccioná un local para saber a qué empresa pertenece el producto.");
            ValidarCategoriaYSector(pProducto);
            int codigoNuevo = productoDAL.AgregarProducto(pProducto);
            digitos.ActualizarDigitoFila("Producto", codigoNuevo);
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "ABM de productos y/o categorías", "Gestión productos", 2);
        }
        public void ModificarProducto(ProductoBE pProducto)
        {
            pProducto.CodigoEmpresa = ControlAcceso.Validar(EntidadEmpresa.Producto, pProducto.CodigoProducto);
            ValidarCategoriaYSector(pProducto);
            productoDAL.ModificarProducto(pProducto);
            digitos.ActualizarDigitoFila("Producto", pProducto.CodigoProducto);
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "ABM de productos y/o categorías", "Gestión productos", 2);
        }
        public void CambiarEstado(int pCodigoProducto, bool pNuevoEstado)
        {
            ControlAcceso.Validar(EntidadEmpresa.Producto, pCodigoProducto);
            productoDAL.CambiarEstado(pCodigoProducto, pNuevoEstado);
            digitos.ActualizarDigitoFila("Producto", pCodigoProducto);
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "ABM de productos y/o categorías", "Gestión productos", 2);
        }
        private void ValidarCategoriaYSector(ProductoBE pProducto)
        {
            int empresaCategoria = ControlAcceso.Validar(EntidadEmpresa.Categoria, pProducto.CodigoCategoria);
            bool sectorOk = pProducto.CodigoSector == null || ControlAcceso.Validar(EntidadEmpresa.Sector, pProducto.CodigoSector.Value) == pProducto.CodigoEmpresa;
            if (empresaCategoria != pProducto.CodigoEmpresa || !sectorOk)
            {
                throw new UnauthorizedAccessException("La categoría o el sector no pertenecen a la empresa del producto.");
            }
        }
    }
}
