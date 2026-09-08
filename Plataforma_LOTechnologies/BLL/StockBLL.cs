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
    public class StockBLL
    {
        private StockDAL stockDAL = new StockDAL();
        private DigitoVerificador digitos = new DigitoVerificador();
        public List<StockListado> ObtenerStockPorLocal(int pCodigoLocal)
        {
            return stockDAL.ObtenerStockPorLocal(pCodigoLocal);
        }
        public StockBE ObtenerStockPorCodigo(int pCodigoStock)
        {
            return stockDAL.ObtenerStockPorCodigo(pCodigoStock);
        }
        public StockBE ObtenerStockPorProducto(int pCodigoLocal, int pCodigoProducto)
        {
            return stockDAL.ObtenerStockPorProducto(pCodigoLocal, pCodigoProducto);
        }
        public void GuardarStock(StockBE pStock)
        {
            if (pStock.CodigoStock > 0)
            {
                stockDAL.ActualizarStock(pStock);
                digitos.ActualizarDigitoFila("Stock", pStock.CodigoStock);
            }
            else
            {
                int codigoNuevo = stockDAL.AgregarStock(pStock);
                digitos.ActualizarDigitoFila("Stock", codigoNuevo);
            }
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "Cargar datos de stock y ventas", "Gestión operaciones", 2);
        }
    }
}
