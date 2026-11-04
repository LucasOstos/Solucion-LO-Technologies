using System;
using System.Collections.Generic;
using System.Globalization;
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

        public const decimal FACTOR_VENTAS_M2_ALTAS = 1.5m;   // ventas/m2 >= 1,5 veces el promedio -> ampliar
        public const decimal FACTOR_VENTAS_M2_BAJAS = 0.5m;   // ventas/m2 <= la mitad del promedio -> reducir
        public const decimal PUNTOS_CIRCULACION = 15m;        // circulación 15 puntos debajo / arriba del promedio
        public const decimal FACTOR_ROTACION_BAJA = 0.5m;     // rotación menor a la mitad del promedio
        public const decimal PUNTOS_MARGEN_BAJO = 10m;        // margen 10 puntos debajo del promedio
        public const int MAXIMO_POR_ESTADO_STOCK = 5;         // productos que se listan por estado de stock (el resto se resume)
        private const int MINIMO_SECTORES = 2;                // para comparar contra el promedio hacen falta al menos 2 sectores

        private const string ALTO = "Alto", MEDIO = "Medio", POSITIVO = "Positivo", BAJO = "Bajo";
        private static readonly CultureInfo ES = new CultureInfo("es-AR");

        public List<RecomendacionBE> GenerarRecomendaciones(int pCodigoLocal, List<IndicadorSectorListado> pIndicadores, List<StockListado> pStock)
        {
            ControlAcceso.Validar(EntidadEmpresa.Local, pCodigoLocal);
            var recomendaciones = new List<RecomendacionBE>();
            AgregarRecomendacionesDeSector(pCodigoLocal, pIndicadores, recomendaciones);
            AgregarRecomendacionesDeStock(pCodigoLocal, pStock, recomendaciones);
            AgregarRecomendacionesDeDatos(pCodigoLocal, pIndicadores, recomendaciones);
            string[] orden = { ALTO, MEDIO, POSITIVO, BAJO };
            return recomendaciones.OrderBy(r => Array.IndexOf(orden, r.Prioridad)).ToList();
        }

        #region Reglas
        private void AgregarRecomendacionesDeSector(int pCodigoLocal, List<IndicadorSectorListado> pIndicadores, List<RecomendacionBE> pRecomendaciones)
        {
            List<decimal> ventasM2 = pIndicadores.Where(i => i.Ventas > 0 && i.VentasPorM2 != null).Select(i => i.VentasPorM2.Value).ToList();
            decimal? promedioVentasM2 = ventasM2.Count >= MINIMO_SECTORES ? ventasM2.Average() : (decimal?)null;
            decimal? promedioCirculacion = pIndicadores.Count >= MINIMO_SECTORES ? pIndicadores.Average(i => i.Circulacion) : (decimal?)null;
            List<decimal> rotaciones = pIndicadores.Where(i => i.StockDisponible > 0).Select(i => i.Rotacion).ToList();
            decimal? promedioRotacion = rotaciones.Count >= MINIMO_SECTORES && rotaciones.Average() > 0 ? rotaciones.Average() : (decimal?)null;
            List<decimal> margenes = pIndicadores.Where(i => i.Margen != null).Select(i => i.Margen.Value).ToList();
            decimal? promedioMargen = margenes.Count >= MINIMO_SECTORES ? margenes.Average() : (decimal?)null;

            foreach (var ind in pIndicadores)
            {
                // 1) Superficie: el sector vende mucho (o muy poco) por cada m2 comparado con el resto del local
                if (promedioVentasM2 != null && ind.VentasPorM2 != null && ind.Ventas > 0)
                {
                    decimal veces = Math.Round(ind.VentasPorM2.Value / promedioVentasM2.Value, 1);
                    if (ind.VentasPorM2 >= promedioVentasM2 * FACTOR_VENTAS_M2_ALTAS)
                    {
                        pRecomendaciones.Add(Nueva(pCodigoLocal, ind.CodigoSector, null, "Superficie", POSITIVO, "Ampliar superficie",
                            "recomendacion.ampliarSuperficie", $"Ampliar la superficie de {ind.Sector}",
                            "recomendacion.ampliarSuperficieMotivo", "{0} vende {1:$#,##0} por m², {2:0.#} veces el promedio del local ({3:$#,##0}/m²). Con más espacio podría vender más.",
                            ind.Sector, ind.VentasPorM2.Value, veces, promedioVentasM2.Value));
                    }
                    else if (ind.VentasPorM2 <= promedioVentasM2 * FACTOR_VENTAS_M2_BAJAS)
                    {
                        pRecomendaciones.Add(Nueva(pCodigoLocal, ind.CodigoSector, null, "Superficie", MEDIO, "Reducir superficie",
                            "recomendacion.reducirSuperficie", $"Reducir la superficie de {ind.Sector}",
                            "recomendacion.reducirSuperficieMotivo", "{0} vende {1:$#,##0} por m², menos de la mitad del promedio del local ({2:$#,##0}/m²). Ese espacio podría rendir más en otro sector.",
                            ind.Sector, ind.VentasPorM2.Value, promedioVentasM2.Value));
                    }
                }
                // 2) Ubicación: buena rotación con poco tránsito, o mucho tránsito con poca rotación
                if (promedioCirculacion != null && promedioRotacion != null && ind.StockDisponible > 0)
                {
                    if (ind.Circulacion <= promedioCirculacion - PUNTOS_CIRCULACION && ind.Rotacion >= promedioRotacion)
                    {
                        pRecomendaciones.Add(Nueva(pCodigoLocal, ind.CodigoSector, null, "Circulación", MEDIO, "Mover al frente",
                            "recomendacion.moverFrente", $"Acercar {ind.Sector} al frente del local",
                            "recomendacion.moverFrenteMotivo", "{0} rota bien ({1:0.#}% por mes, el promedio es {2:0.#}%) pero tiene poca circulación ({3:0.#}% contra {4:0.#}% del local). En una zona de más tránsito podría vender más.",
                            ind.Sector, ind.Rotacion, promedioRotacion.Value, ind.Circulacion, promedioCirculacion.Value));
                    }
                    else if (ind.Circulacion >= promedioCirculacion + PUNTOS_CIRCULACION && ind.Rotacion < promedioRotacion * FACTOR_ROTACION_BAJA)
                    {
                        pRecomendaciones.Add(Nueva(pCodigoLocal, ind.CodigoSector, null, "Circulación", MEDIO, "Reubicar productos destacados",
                            "recomendacion.aprovecharTransito", $"Aprovechar el tránsito de {ind.Sector}",
                            "recomendacion.aprovecharTransitoMotivo", "{0} es una de las zonas más transitadas ({1:0.#}% de circulación) pero rota poco ({2:0.#}% por mes, el promedio es {3:0.#}%). Conviene exhibir ahí los productos que más se venden.",
                            ind.Sector, ind.Circulacion, ind.Rotacion, promedioRotacion.Value));
                    }
                }
                // 3) Margen bajo: no se resuelve moviendo el sector, sino revisando precios y costos
                if (promedioMargen != null && ind.Margen != null && ind.Margen <= promedioMargen - PUNTOS_MARGEN_BAJO)
                {
                    pRecomendaciones.Add(Nueva(pCodigoLocal, ind.CodigoSector, null, "Margen", MEDIO, null,
                        "recomendacion.margenBajo", $"Revisar precios y costos de {ind.Sector}",
                        "recomendacion.margenBajoMotivo", "{0} tiene un margen de {1:0.#}%, por debajo del promedio del local ({2:0.#}%).",
                        ind.Sector, ind.Margen.Value, promedioMargen.Value));
                }
                // 4) Sector con mercadería y sin ventas en el período
                if (ind.Ventas <= 0 && ind.StockDisponible > 0)
                {
                    pRecomendaciones.Add(Nueva(pCodigoLocal, ind.CodigoSector, null, "Ventas", MEDIO, "Mover al frente",
                        "recomendacion.sinVentas", $"{ind.Sector} no registra ventas",
                        "recomendacion.sinVentasMotivo", "{0} tiene {1} unidades en stock y ninguna venta en el período. Revisá la exhibición o la ubicación del sector.",
                        ind.Sector, ind.StockDisponible));
                }
            }
        }

        private void AgregarRecomendacionesDeStock(int pCodigoLocal, List<StockListado> pStock, List<RecomendacionBE> pRecomendaciones)
        {
            // Mismos estados que la pestaña "Datos de stock" (según el mínimo y el máximo de cada producto)
            AgregarPorEstado(pCodigoLocal, pStock.Where(s => s.EstadoStock == "Crítico").OrderBy(s => s.Producto).ToList(), ALTO,
                "recomendacion.sinStock", "Sin stock de {0}", "recomendacion.sinStockMotivo", "{0} no tiene unidades disponibles. Reponer cuanto antes.",
                "recomendacion.masSinStock", "Hay {0} productos más sin stock", pRecomendaciones);
            AgregarPorEstado(pCodigoLocal, pStock.Where(s => s.EstadoStock == "Bajo").OrderBy(s => s.CantidadDisponible).ToList(), ALTO,
                "recomendacion.stockBajo", "Stock bajo de {0}", "recomendacion.stockBajoMotivo", "{0} tiene {1} unidades y su mínimo es {2}. Reponer.",
                "recomendacion.masStockBajo", "Hay {0} productos más con stock bajo el mínimo", pRecomendaciones);
            AgregarPorEstado(pCodigoLocal, pStock.Where(s => s.EstadoStock == "Sobrestock").OrderByDescending(s => s.CantidadDisponible).ToList(), MEDIO,
                "recomendacion.sobrestock", "Sobrestock de {0}", "recomendacion.sobrestockMotivo", "{0} tiene {1} unidades y su máximo es {3}. Mercadería inmovilizada: frenar compras o promocionar.",
                "recomendacion.masSobrestock", "Hay {0} productos más con sobrestock", pRecomendaciones);
        }
        private void AgregarPorEstado(int pCodigoLocal, List<StockListado> pProductos, string pPrioridad, string pClave, string pTexto, string pClaveMotivo, string pTextoMotivo,
                                      string pClaveResumen, string pTextoResumen, List<RecomendacionBE> pRecomendaciones)
        {
            foreach (StockListado stock in pProductos.Take(MAXIMO_POR_ESTADO_STOCK))
            {
                pRecomendaciones.Add(Nueva(pCodigoLocal, null, stock.CodigoProducto, "Stock", pPrioridad, null, pClave, string.Format(ES, pTexto, stock.Producto),
                    pClaveMotivo, pTextoMotivo, stock.Producto, stock.CantidadDisponible, stock.Minimo ?? 0, stock.Maximo ?? 0));
            }
            int resto = pProductos.Count - MAXIMO_POR_ESTADO_STOCK;
            if (resto > 0)
            {
                pRecomendaciones.Add(Nueva(pCodigoLocal, null, null, "Stock", pPrioridad, null, pClaveResumen, string.Format(ES, pTextoResumen, resto),
                    "recomendacion.verStock", "Detalle en Stock y ventas > Datos de stock.", resto));
            }
        }

        // Datos que faltan para que los indicadores estén completos
        private void AgregarRecomendacionesDeDatos(int pCodigoLocal, List<IndicadorSectorListado> pIndicadores, List<RecomendacionBE> pRecomendaciones)
        {
            int sinSuperficie = pIndicadores.Count(i => i.Superficie == null || i.Superficie <= 0);
            if (sinSuperficie > 0)
            {
                pRecomendaciones.Add(Nueva(pCodigoLocal, null, null, "Datos", BAJO, null,
                    "recomendacion.faltaSuperficie", $"Cargar la superficie de {sinSuperficie} sectores",
                    "recomendacion.faltaSuperficieMotivo", "Sin la superficie (m²) no se calculan las ventas por m² ni las recomendaciones de espacio de esos sectores. Se carga en Layout y sectores.",
                    sinSuperficie));
            }
            if (pIndicadores.Any(i => i.Ventas > 0) && pIndicadores.All(i => i.Margen == null))
            {
                pRecomendaciones.Add(Nueva(pCodigoLocal, null, null, "Datos", BAJO, null,
                    "recomendacion.faltaCosto", "Cargar el costo de los productos",
                    "recomendacion.faltaCostoMotivo", "Sin el costo unitario no se calcula el margen. Se carga en Productos y categorías."));
            }
        }
        private static RecomendacionBE Nueva(int pCodigoLocal, int? pCodigoSector, int? pCodigoProducto, string pTipo, string pPrioridad, string pAccion,
                                             string pClaveDescripcion, string pDescripcion, string pClaveMotivo, string pTextoMotivo, params object[] pParametros)
        {
            return new RecomendacionBE
            {
                CodigoLocal = pCodigoLocal,
                CodigoSector = pCodigoSector,
                CodigoProducto = pCodigoProducto,
                Descripcion = pDescripcion,
                Motivo = string.Format(ES, pTextoMotivo, pParametros),
                ClaveDescripcion = pClaveDescripcion,
                ClaveMotivo = pClaveMotivo,
                Parametros = pParametros,
                Prioridad = pPrioridad,
                TipoRecomendacion = pTipo,
                AccionEscenario = pAccion,
                Estado = "Pendiente",
                FechaGeneracion = DateTime.Now
            };
        }
        #endregion

        // Las recomendaciones pendientes del local se REEMPLAZAN por las nuevas, y solo si cambiaron
        public void GuardarRecomendaciones(int pCodigoLocal, List<RecomendacionBE> pRecomendaciones)
        {
            ControlAcceso.Validar(EntidadEmpresa.Local, pCodigoLocal);
            Func<RecomendacionBE, string> clave = r => $"{r.TipoRecomendacion}|{r.CodigoSector}|{r.CodigoProducto}|{r.Prioridad}|{r.Descripcion}";
            List<string> actuales = recomendacionDAL.ObtenerPendientes(pCodigoLocal).Select(clave).OrderBy(c => c).ToList();
            List<string> nuevas = pRecomendaciones.Select(clave).OrderBy(c => c).ToList();
            if (!actuales.SequenceEqual(nuevas))
            {
                if (recomendacionDAL.EliminarPendientes(pCodigoLocal) > 0) digitos.ActualizarDigitoVertical("Recomendacion");
                var codigos = new List<object>();
                foreach (var rec in pRecomendaciones)
                {
                    codigos.Add(recomendacionDAL.AgregarRecomendacion(rec));
                }
                if (codigos.Count > 0) digitos.ActualizarDigitoFilas("Recomendacion", codigos);
            }
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "Visualizar indicadores y recomendaciones", "Reportes", 3);
        }
    }
}
