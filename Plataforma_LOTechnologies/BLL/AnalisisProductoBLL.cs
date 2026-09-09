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
        private AnalisisProductoDAL analisisDAL = new AnalisisProductoDAL();
        private SectorBLL sectorBLL = new SectorBLL();
        public List<ProductoAnalisisListado> DetectarOportunidades(int pCodigoLocal, string pPeriodoDesde)
        {
            List<ProductoMetrica> metricas = analisisDAL.ObtenerMetricasPorLocal(pCodigoLocal, pPeriodoDesde);
            List<SectorListado> sectores = sectorBLL.ObtenerSectoresPorLocal(pCodigoLocal);
            var resultado = new List<ProductoAnalisisListado>();
            foreach (var m in metricas)
            {
                decimal rotacion = m.Stock > 0 ? System.Math.Round((m.Ventas / (decimal)m.Stock) * 100, 1) : 0;
                SectorListado mejorSector = sectores.Where(s => s.CodigoSector != m.CodigoSectorActual).OrderByDescending(s => s.Circulacion).FirstOrDefault();
                decimal? gap = mejorSector != null ? mejorSector.Circulacion - m.CirculacionActual : 0;
                bool esCandidato = m.Ventas >= 1 && mejorSector != null && gap >= 10m;
                if (esCandidato)
                {
                    resultado.Add(new ProductoAnalisisListado
                    {
                        CodigoProducto = m.CodigoProducto,
                        NombreProducto = m.NombreProducto,
                        NombreCategoria = m.NombreCategoria,
                        SectorActual = m.NombreSectorActual,
                        SectorSugerido = mejorSector.Nombre,
                        Ventas = m.Ventas,
                        Rotacion = rotacion,
                        Circulacion = m.CirculacionActual,
                        Prioridad = ClasificarPrioridad(gap)
                    });
                }
                else
                {
                    resultado.Add(new ProductoAnalisisListado
                    {
                        CodigoProducto = m.CodigoProducto,
                        NombreProducto = m.NombreProducto,
                        NombreCategoria = m.NombreCategoria,
                        SectorActual = m.NombreSectorActual,
                        SectorSugerido = "",
                        Ventas = m.Ventas,
                        Rotacion = rotacion,
                        Circulacion = m.CirculacionActual,
                        Prioridad = ""
                    });
                }
            }
            return resultado;
        }

        private string ClasificarPrioridad(decimal? gap)
        {
            if (gap >= 25) return "Alta";
            if (gap >= 15) return "Media";
            return "Baja";
        }
    }
}
