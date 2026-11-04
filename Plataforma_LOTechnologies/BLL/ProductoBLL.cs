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
        private SectorDAL sectorDAL = new SectorDAL();
        private DigitoVerificador digitos = new DigitoVerificador();
        public List<ProductoListado> ObtenerProductosListado(int pCodigoLocal, string pBusqueda, int? pCodigoSector)
        {
            int empresa = ControlAcceso.ResolverEmpresaPorLocal(pCodigoLocal);
            if (empresa <= 0) return new List<ProductoListado>();
            if (pCodigoSector != null) ControlAcceso.Validar(EntidadEmpresa.Sector, pCodigoSector.Value);
            return productoDAL.ObtenerProductosListado(empresa, pCodigoLocal, pBusqueda, pCodigoSector);
        }
        public ProductoBE ObtenerProductoPorCodigo(int pCodigoProducto)
        {
            ControlAcceso.Validar(EntidadEmpresa.Producto, pCodigoProducto);
            return productoDAL.ObtenerProductoPorCodigo(pCodigoProducto);
        }
        // Sector en el que está el producto en ese local (null = no está ubicado en ese local)
        public int? ObtenerSectorEnLocal(int pCodigoProducto, int pCodigoLocal)
        {
            ControlAcceso.Validar(EntidadEmpresa.Producto, pCodigoProducto);
            ControlAcceso.Validar(EntidadEmpresa.Local, pCodigoLocal);
            ProductoUbicacionBE ubicacion = productoDAL.ObtenerUbicacion(pCodigoProducto, pCodigoLocal);
            return ubicacion == null ? (int?)null : ubicacion.CodigoSector;
        }
        public int ObtenerProximoNumeroSerie(int pCodigoLocal)
        {
            int empresa = ControlAcceso.ResolverEmpresaPorLocal(pCodigoLocal);
            return productoDAL.ObtenerProximoNumeroSerie(empresa);
        }
        // pCodigoSector = sector del producto en el local, pCodigoLocal (null = sin ubicar en ese local)
        public void CrearProducto(ProductoBE pProducto, int pCodigoLocal, int? pCodigoSector)
        {
            pProducto.CodigoEmpresa = ControlAcceso.ResolverEmpresaPorLocal(pCodigoLocal);
            if (pProducto.CodigoEmpresa <= 0) throw new ExcepcionTraducible("productos.errLocalProducto");
            ValidarCategoria(pProducto);
            ValidarSectorDelLocal(pCodigoSector, pCodigoLocal);
            int codigoNuevo = productoDAL.AgregarProducto(pProducto);
            digitos.ActualizarDigitoFila("Producto", codigoNuevo);
            GuardarUbicacion(codigoNuevo, pCodigoLocal, pCodigoSector);
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "ABM de productos y/o categorías", "Gestión productos", 2);
        }
        public void ModificarProducto(ProductoBE pProducto, int pCodigoLocal, int? pCodigoSector)
        {
            pProducto.CodigoEmpresa = ControlAcceso.Validar(EntidadEmpresa.Producto, pProducto.CodigoProducto);
            if (ControlAcceso.Validar(EntidadEmpresa.Local, pCodigoLocal) != pProducto.CodigoEmpresa)
            {
                throw new UnauthorizedAccessException("El local no pertenece a la empresa del producto.");
            }
            ValidarCategoria(pProducto);
            ValidarSectorDelLocal(pCodigoSector, pCodigoLocal);
            productoDAL.ModificarProducto(pProducto);
            digitos.ActualizarDigitoFila("Producto", pProducto.CodigoProducto);
            GuardarUbicacion(pProducto.CodigoProducto, pCodigoLocal, pCodigoSector);
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "ABM de productos y/o categorías", "Gestión productos", 2);
        }
        public void CambiarEstado(int pCodigoProducto, bool pNuevoEstado)
        {
            ControlAcceso.Validar(EntidadEmpresa.Producto, pCodigoProducto);
            productoDAL.CambiarEstado(pCodigoProducto, pNuevoEstado);
            digitos.ActualizarDigitoFila("Producto", pCodigoProducto);
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "ABM de productos y/o categorías", "Gestión productos", 2);
        }
        private void ValidarCategoria(ProductoBE pProducto)
        {
            if (ControlAcceso.Validar(EntidadEmpresa.Categoria, pProducto.CodigoCategoria) != pProducto.CodigoEmpresa)
            {
                throw new UnauthorizedAccessException("La categoría no pertenece a la empresa del producto.");
            }
        }
        private void ValidarSectorDelLocal(int? pCodigoSector, int pCodigoLocal)
        {
            if (pCodigoSector == null) return;
            ControlAcceso.Validar(EntidadEmpresa.Sector, pCodigoSector.Value);
            SectorBE sector = sectorDAL.ObtenerSectorPorCodigo(pCodigoSector.Value);
            if (sector == null || sector.CodigoLocal != pCodigoLocal)
            {
                throw new UnauthorizedAccessException("El sector no pertenece al local.");
            }
        }
        // Alta, cambio o baja de la ubicación del producto en un local, con sus dígitos verificadores
        private void GuardarUbicacion(int pCodigoProducto, int pCodigoLocal, int? pCodigoSector)
        {
            ProductoUbicacionBE actual = productoDAL.ObtenerUbicacion(pCodigoProducto, pCodigoLocal);
            if (actual == null && pCodigoSector == null) return;
            if (actual == null)
            {
                int codigoUbicacion = productoDAL.AgregarUbicacion(new ProductoUbicacionBE(0, pCodigoProducto, pCodigoLocal, pCodigoSector.Value));
                digitos.ActualizarDigitoFila("ProductoUbicacion", codigoUbicacion);
            }
            else if (pCodigoSector == null)
            {
                productoDAL.EliminarUbicacion(actual.CodigoUbicacion);
                digitos.ActualizarDigitoTabla("ProductoUbicacion");
            }
            else if (actual.CodigoSector != pCodigoSector.Value)
            {
                productoDAL.ModificarUbicacion(actual.CodigoUbicacion, pCodigoSector.Value);
                digitos.ActualizarDigitoFila("ProductoUbicacion", actual.CodigoUbicacion);
            }
        }
    }
}
