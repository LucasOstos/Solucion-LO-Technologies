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
    public class RecomendacionBLL
    {
        private RecomendacionDAL recomendacionDAL = new RecomendacionDAL();
        private DigitoVerificador digitos = new DigitoVerificador();
        private const decimal UMBRAL_CIRCULACION_BAJA = 55m;
        private const decimal UMBRAL_VENTAS_META = 3000m;
        private const int STOCK_CRITICO_MINIMO = 5;
        public List<RecomendacionBE> GenerarRecomendaciones(int pCodigoLocal, List<IndicadorSectorListado> pIndicadores, List<StockListado> pStockCritico)
        {
            var recomendaciones = new List<RecomendacionBE>();
            foreach (var ind in pIndicadores)
            {
                if (ind.Circulacion < UMBRAL_CIRCULACION_BAJA && ind.Circulacion > 0)
                {
                    recomendaciones.Add(new RecomendacionBE
                    {
                        CodigoLocal = pCodigoLocal,
                        CodigoSector = ind.CodigoSector,
                        Descripcion = $"Baja circulación en sector {ind.Sector}",
                        Motivo = $"El sector {ind.Sector} registra un {ind.Circulacion}% de circulación estimada, más bajo que el resto. Considerar redistribuir productos de alta rotación.",
                        Prioridad = "Medio",
                        TipoRecomendacion = "Circulación",
                        Estado = "Pendiente",
                        FechaGeneracion = DateTime.Now
                    });
                }
                if (ind.Ventas > UMBRAL_VENTAS_META)
                {
                    recomendaciones.Add(new RecomendacionBE
                    {
                        CodigoLocal = pCodigoLocal,
                        CodigoSector = ind.CodigoSector,
                        Descripcion = $"{ind.Sector} supera meta de ventas",
                        Motivo = $"El sector {ind.Sector} superó el umbral de ventas del período. Se recomienda ampliar su exposición.",
                        Prioridad = "Positivo",
                        TipoRecomendacion = "Ventas",
                        Estado = "Pendiente",
                        FechaGeneracion = DateTime.Now
                    });
                }
            }
            foreach (var stock in pStockCritico)
            {
                if (stock.CantidadDisponible <= STOCK_CRITICO_MINIMO)
                {
                    recomendaciones.Add(new RecomendacionBE
                    {
                        CodigoLocal = pCodigoLocal,
                        CodigoProducto = stock.CodigoProducto,
                        Descripcion = $"Stock crítico en {stock.Producto}",
                        Motivo = $"'{stock.Producto}' tiene solo {stock.CantidadDisponible} unidades. Se recomienda reposición urgente.",
                        Prioridad = "Alto",
                        TipoRecomendacion = "Stock",
                        Estado = "Pendiente",
                        FechaGeneracion = DateTime.Now
                    });
                }
            }
            return recomendaciones;
        }
        public void GuardarRecomendaciones(List<RecomendacionBE> pRecomendaciones)
        {
            foreach (var rec in pRecomendaciones)
            {
                int codigoNuevo = recomendacionDAL.AgregarRecomendacion(rec);
                digitos.ActualizarDigitoFila("Recomendacion", codigoNuevo);
            }
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "Visualizar indicadores y recomendaciones", "Reportes", 3);
        }
    }
}
