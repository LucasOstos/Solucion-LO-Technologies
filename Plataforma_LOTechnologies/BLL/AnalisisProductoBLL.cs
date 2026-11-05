using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;
using DAL;

namespace BLL
{
    public class AnalisisProductoBLL
    {
        public const decimal PROPORCION_RANKING = 0.25m;   // 25% superior / inferior
        public const decimal PUNTOS_CIRCULACION = 10m;
        public const decimal PUNTOS_PRIORIDAD_ALTA = 25m;  // estrella escondida: diferencia de circulación para prioridad alta
        public const int DIAS_QUIEBRE = 7;
        public const int COBERTURA_MAXIMA_ESTRELLA = 60;   // una "estrella" tiene que rotar: stock para 60 días o menos
        private const int MINIMO_PRODUCTOS_RANKING = 4;    // con menos productos con ventas no tiene sentido hablar de "25%"
        private AnalisisProductoDAL analisisDAL = new AnalisisProductoDAL();
        private SectorBLL sectorBLL = new SectorBLL();

        public List<ProductoAnalisisListado> DetectarOportunidades(int pCodigoLocal, DateTime pDesde, DateTime pHasta)
        {
            ControlAcceso.Validar(EntidadEmpresa.Local, pCodigoLocal);
            if (pHasta.Date > DateTime.Today) pHasta = DateTime.Today;
            int dias = Math.Max(1, (pHasta.Date - pDesde.Date).Days + 1);
            List<ProductoMetrica> metricas = analisisDAL.ObtenerMetricasPorLocal(pCodigoLocal, pDesde, pHasta);
            List<SectorListado> sectores = sectorBLL.ObtenerSectoresPorLocal(pCodigoLocal);
            decimal promedioCirculacion = sectores.Count > 0 ? sectores.Average(s => s.Circulacion ?? 0) : 0;

            // Umbrales del ranking por importe (entre los productos ubicados en el local)
            List<decimal> importesUbicados = metricas.Where(m => m.CodigoSectorActual != null).Select(m => m.Importe).OrderByDescending(i => i).ToList();
            bool hayRanking = importesUbicados.Count(i => i > 0) >= MINIMO_PRODUCTOS_RANKING;
            int cantidadRanking = Math.Max(1, (int)Math.Ceiling(importesUbicados.Count * PROPORCION_RANKING));
            decimal umbralSuperior = hayRanking ? importesUbicados[cantidadRanking - 1] : decimal.MaxValue;
            decimal umbralInferior = hayRanking ? importesUbicados[importesUbicados.Count - cantidadRanking] : decimal.MinValue;

            var resultado = new List<ProductoAnalisisListado>();
            foreach (ProductoMetrica m in metricas)
            {
                int cantidadAntes = resultado.Count;
                bool ubicado = m.CodigoSectorActual != null;
                bool circulacionBaja = ubicado && sectores.Count >= 2 && m.CirculacionActual <= promedioCirculacion - PUNTOS_CIRCULACION;
                bool circulacionAlta = ubicado && sectores.Count >= 2 && m.CirculacionActual >= promedioCirculacion + PUNTOS_CIRCULACION;
                decimal? cobertura = m.Ventas > 0 ? Math.Round(m.Stock / (m.Ventas / (decimal)dias), 0) : (decimal?)null;

                // Riesgo de quiebre: se vende y el stock no llega a 7 días (o ya no hay)
                if (m.Ventas > 0 && cobertura < DIAS_QUIEBRE)
                {
                    resultado.Add(Nueva(m, cobertura, ProductoAnalisisListado.TIPO_RIESGO_QUIEBRE, "Alta", null, null,
                        "analisis.motivoQuiebre", m.Stock, cobertura.Value, DIAS_QUIEBRE));
                }
                // Sin ubicar con ventas
                if (!ubicado && m.Ventas > 0)
                {
                    resultado.Add(Nueva(m, cobertura, ProductoAnalisisListado.TIPO_SIN_UBICAR, "Media", null, null,
                        "analisis.motivoSinUbicar", m.Ventas, m.Importe));
                }
                // Estrella escondida: vende mucho (y rota) en una zona de poco tránsito
                if (hayRanking && circulacionBaja && m.Importe > 0 && m.Importe >= umbralSuperior && cobertura <= COBERTURA_MAXIMA_ESTRELLA)
                {
                    SectorListado destino = BuscarSectorCompatible(m, sectores, metricas);
                    if (destino != null)
                    {
                        decimal diferencia = (destino.Circulacion ?? 0) - m.CirculacionActual;
                        resultado.Add(Nueva(m, cobertura, ProductoAnalisisListado.TIPO_ESTRELLA, diferencia >= PUNTOS_PRIORIDAD_ALTA ? "Alta" : "Media",
                            destino, "Reubicar productos destacados",
                            "analisis.motivoEstrella", m.CirculacionActual, destino.Nombre, destino.Circulacion ?? 0));
                    }
                    else
                    {
                        // Ningún sector compatible tiene más tránsito: se sugiere acercar su propio sector al frente
                        SectorListado propio = sectores.FirstOrDefault(s => s.CodigoSector == m.CodigoSectorActual);
                        resultado.Add(Nueva(m, cobertura, ProductoAnalisisListado.TIPO_ESTRELLA, "Media", propio, "Mover al frente",
                            "analisis.motivoEstrellaFrente", m.CirculacionActual, m.NombreSectorActual));
                    }
                }
                // Sin ventas en el período y con stock
                if (m.Ventas == 0 && m.Stock > 0)
                {
                    if (circulacionAlta)
                    {
                        resultado.Add(Nueva(m, cobertura, ProductoAnalisisListado.TIPO_LUGAR_PRIVILEGIADO, "Media", null, null,
                            "analisis.motivoLugarSinVentas", m.CirculacionActual, m.Stock));
                    }
                    else
                    {
                        resultado.Add(Nueva(m, cobertura, ProductoAnalisisListado.TIPO_SIN_MOVIMIENTO, "Baja", null, null,
                            "analisis.motivoSinMovimiento", m.Stock, dias));
                    }
                }
                // Ocupa lugar privilegiado: vende poco en una zona de mucho tránsito
                else if (hayRanking && circulacionAlta && m.Ventas > 0 && m.Importe <= umbralInferior)
                {
                    resultado.Add(Nueva(m, cobertura, ProductoAnalisisListado.TIPO_LUGAR_PRIVILEGIADO, "Media", null, null,
                        "analisis.motivoLugarPrivilegiado", m.CirculacionActual, promedioCirculacion));
                }
                if (resultado.Count == cantidadAntes)
                {
                    resultado.Add(Nueva(m, cobertura, null, "", null, null, null));
                }
            }
            string[] orden = { "Alta", "Media", "Baja", "" };
            return resultado.OrderBy(r => Array.IndexOf(orden, r.Prioridad)).ThenByDescending(r => r.Importe).ToList();
        }
        private static SectorListado BuscarSectorCompatible(ProductoMetrica pProducto, List<SectorListado> pSectores, List<ProductoMetrica> pMetricas)
        {
            SectorListado actual = pSectores.FirstOrDefault(s => s.CodigoSector == pProducto.CodigoSectorActual);
            HashSet<int> sectoresDeLaCategoria = new HashSet<int>(pMetricas
                .Where(x => x.CodigoSectorActual != null && x.NombreCategoria == pProducto.NombreCategoria)
                .Select(x => x.CodigoSectorActual.Value));
            return pSectores
                .Where(s => s.CodigoSector != pProducto.CodigoSectorActual)
                .Where(s => (actual != null && !string.IsNullOrEmpty(s.Tipo) && s.Tipo != "Otro" && s.Tipo == actual.Tipo)
                            || sectoresDeLaCategoria.Contains(s.CodigoSector))
                .Where(s => (s.Circulacion ?? 0) >= pProducto.CirculacionActual + PUNTOS_CIRCULACION)
                .OrderByDescending(s => s.Circulacion ?? 0)
                .FirstOrDefault();
        }
        private static ProductoAnalisisListado Nueva(ProductoMetrica pMetrica, decimal? pCobertura, string pTipo, string pPrioridad,
                                                     SectorListado pSectorSugerido, string pAccion, string pClaveMotivo, params object[] pParametros)
        {
            return new ProductoAnalisisListado
            {
                CodigoProducto = pMetrica.CodigoProducto,
                CodigoInterno = $"PROD-{pMetrica.NumeroSerie:D3}",
                NombreProducto = pMetrica.NombreProducto,
                NombreCategoria = pMetrica.NombreCategoria,
                CodigoSectorActual = pMetrica.CodigoSectorActual,
                SectorActual = pMetrica.NombreSectorActual,
                CodigoSectorSugerido = pSectorSugerido?.CodigoSector,
                SectorSugerido = pSectorSugerido?.Nombre,
                Ventas = pMetrica.Ventas,
                Importe = pMetrica.Importe,
                Margen = pMetrica.Costo != null && pMetrica.Importe > 0
                    ? Math.Round((pMetrica.Importe - pMetrica.Ventas * pMetrica.Costo.Value) / pMetrica.Importe * 100, 1) : (decimal?)null,
                Stock = pMetrica.Stock,
                CoberturaDias = pCobertura,
                Circulacion = pMetrica.CirculacionActual,
                Tipo = pTipo,
                Prioridad = pPrioridad,
                ClaveMotivo = pClaveMotivo,
                Parametros = pParametros ?? new object[0],
                AccionEscenario = pAccion
            };
        }
    }
}
