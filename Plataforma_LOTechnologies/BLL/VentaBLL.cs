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
    public class VentaBLL
    {
        private VentaDAL ventaDAL = new VentaDAL();
        private DigitoVerificador digitos = new DigitoVerificador();
        //pAgrupacion: VentaDAL.AGRUPAR_SECTOR / AGRUPAR_CATEGORIA / AGRUPAR_PRODUCTO
        public List<VentaListado> ObtenerVentasAgrupadas(int pCodigoLocal, string pPeriodoDesde, string pAgrupacion = VentaDAL.AGRUPAR_SECTOR)
        {
            ControlAcceso.Validar(EntidadEmpresa.Local, pCodigoLocal);
            return ventaDAL.ObtenerVentasAgrupadas(pCodigoLocal, pPeriodoDesde, pAgrupacion);
        }
        public List<VentaDetalle> ObtenerUltimasVentas(int pCodigoLocal, int pCantidad)
        {
            ControlAcceso.Validar(EntidadEmpresa.Local, pCodigoLocal);
            return ventaDAL.ObtenerUltimasVentas(pCodigoLocal, pCantidad);
        }
        public decimal ObtenerTotalVentas(int pCodigoLocal, string pPeriodoDesde)
        {
            ControlAcceso.Validar(EntidadEmpresa.Local, pCodigoLocal);
            return ventaDAL.ObtenerTotalVentas(pCodigoLocal, pPeriodoDesde);
        }
        public decimal? ObtenerMargenTotal(int pCodigoLocal, string pPeriodoDesde)
        {
            ControlAcceso.Validar(EntidadEmpresa.Local, pCodigoLocal);
            return ventaDAL.ObtenerMargenTotal(pCodigoLocal, pPeriodoDesde);
        }
        public int ObtenerUnidadesVendidas(int pCodigoLocal, string pPeriodoDesde)
        {
            ControlAcceso.Validar(EntidadEmpresa.Local, pCodigoLocal);
            return ventaDAL.ObtenerUnidadesVendidas(pCodigoLocal, pPeriodoDesde);
        }
        //Carga manual de una venta. Si pImporte es null se calculca asi: precio del producto x cantidad.
        public void CrearVenta(int pCodigoLocal, int pCodigoProducto, DateTime pFecha, int pCantidad, decimal? pImporte)
        {
            StockBLL.ValidarLocalYProducto(pCodigoLocal, pCodigoProducto);
            ProductoBE producto = new ProductoDAL().ObtenerProductoPorCodigo(pCodigoProducto);
            decimal importe;
            string error = ValidarVenta(pFecha, pCantidad, pImporte, producto?.Precio, out importe);
            if (error != null) throw new ExcepcionTraducible(error);
            int codigoNuevo = ventaDAL.AgregarVenta(ArmarVenta(pCodigoLocal, pCodigoProducto, pFecha, pCantidad, importe));
            digitos.ActualizarDigitoFila("Venta", codigoNuevo);
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "Cargar datos de stock y ventas", "Gestión operaciones", 2);
        }
        //Para corregir una carga equivocada. Si la venta vino de una importación, el resto del lote no se toca.
        public void EliminarVenta(int pCodigoVenta)
        {
            ControlAcceso.Validar(EntidadEmpresa.Venta, pCodigoVenta);
            ventaDAL.EliminarVenta(pCodigoVenta);
            digitos.ActualizarDigitoVertical("Venta");
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "Eliminar venta", "Gestión operaciones", 2);
        }
        //Reglas de una venta (carga manual e importación). Devuelve la clave del error o null si es válida.
        //Si no viene importe se usa precio x cantidad; sin importe ni precio no se puede registrar.
        internal static string ValidarVenta(DateTime pFecha, int pCantidad, decimal? pImporte, decimal? pPrecioProducto, out decimal pImporteFinal)
        {
            pImporteFinal = 0;
            if (pCantidad <= 0) return "stock.errCantidadPositiva";
            if (pFecha.Date > DateTime.Today) return "stock.errFechaFutura";
            if (pImporte != null && pImporte.Value < 0) return "stock.errImporteNegativo";
            if (pImporte == null)
            {
                if (pPrecioProducto == null) return "stock.errSinImporte";
                pImporteFinal = Math.Round(pPrecioProducto.Value * pCantidad, 2);
            }
            else pImporteFinal = pImporte.Value;
            return null;
        }
        internal static VentaBE ArmarVenta(int pCodigoLocal, int pCodigoProducto, DateTime pFecha, int pCantidad, decimal pImporte)
        {
            decimal precioUnitario = Math.Round(pImporte / pCantidad, 2);
            return new VentaBE(pCodigoLocal, pCodigoProducto, pFecha.Date, pCantidad, precioUnitario, pImporte, pFecha.ToString("yyyy-MM"));
        }
    }
}
