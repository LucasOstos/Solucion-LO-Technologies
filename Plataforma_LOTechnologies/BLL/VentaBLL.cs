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
        public List<VentaListado> ObtenerVentasAgrupadas(int pCodigoLocal, string pPeriodoDesde)
        {
            ControlAcceso.Validar(EntidadEmpresa.Local, pCodigoLocal);
            return ventaDAL.ObtenerVentasAgrupadas(pCodigoLocal, pPeriodoDesde);
        }
        public decimal ObtenerTotalVentas(int pCodigoLocal, string pPeriodoDesde)
        {
            ControlAcceso.Validar(EntidadEmpresa.Local, pCodigoLocal);
            return ventaDAL.ObtenerTotalVentas(pCodigoLocal, pPeriodoDesde);
        }
        public decimal ObtenerTicketPromedio(int pCodigoLocal, string pPeriodoDesde)
        {
            ControlAcceso.Validar(EntidadEmpresa.Local, pCodigoLocal);
            return ventaDAL.ObtenerTicketPromedio(pCodigoLocal, pPeriodoDesde);
        }
        public int ObtenerUnidadesVendidas(int pCodigoLocal, string pPeriodoDesde)
        {
            ControlAcceso.Validar(EntidadEmpresa.Local, pCodigoLocal);
            return ventaDAL.ObtenerUnidadesVendidas(pCodigoLocal, pPeriodoDesde);
        }
        public void CrearVenta(VentaBE pVenta)
        {
            StockBLL.ValidarLocalYProducto(pVenta.CodigoLocal, pVenta.CodigoProducto);
            int codigoNuevo = ventaDAL.AgregarVenta(pVenta);
            digitos.ActualizarDigitoFila("Venta", codigoNuevo);
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "Cargar datos de stock y ventas", "Gestión operaciones", 2);
        }
    }
}
