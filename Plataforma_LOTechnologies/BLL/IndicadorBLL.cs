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
    public class IndicadorBLL
    {
        private IndicadorDAL indicadorDAL = new IndicadorDAL();
        private DigitoVerificador digitos = new DigitoVerificador();
        public List<IndicadorSectorListado> CalcularIndicadoresPorSector(int pCodigoLocal, string pPeriodoDesde)
        {
            return indicadorDAL.CalcularIndicadoresPorSector(pCodigoLocal, pPeriodoDesde);
        }
        public void GuardarIndicadores(int pCodigoLocal, string pPeriodo, List<IndicadorSectorListado> pIndicadores)
        {
            foreach (var ind in pIndicadores)
            {
                GuardarUnIndicador(pCodigoLocal, ind.CodigoSector, pPeriodo, "Ventas", ind.Ventas, "$");
                GuardarUnIndicador(pCodigoLocal, ind.CodigoSector, pPeriodo, "Circulación", ind.Circulacion, "%");
                GuardarUnIndicador(pCodigoLocal, ind.CodigoSector, pPeriodo, "Rotación", ind.Rotacion, "%");
                GuardarUnIndicador(pCodigoLocal, ind.CodigoSector, pPeriodo, "Índice Global", ind.IndiceGlobal, "pts");
            }
        }
        private void GuardarUnIndicador(int codigoLocal, int codigoSector, string periodo, string nombre, decimal valor, string unidad)
        {
            var indicador = new IndicadorBE
            {
                CodigoLocal = codigoLocal,
                CodigoSector = codigoSector,
                Nombre = nombre,
                TipoIndicador = nombre,
                Fecha = DateTime.Now,
                Periodo = periodo,
                Valor = valor,
                UnidadMedida = unidad
            };
            int codigoNuevo = indicadorDAL.AgregarIndicador(indicador);
            digitos.ActualizarDigitoFila("Indicador", codigoNuevo);
        }
        private class CoeficienteCambio
        {
            public decimal DeltaCirculacion { get; set; }
            public decimal DeltaVentas { get; set; }
        }
        private static readonly Dictionary<string, CoeficienteCambio> COEFICIENTES = new Dictionary<string, CoeficienteCambio>
        {
            { "Mover al frente", new CoeficienteCambio { DeltaCirculacion = 15m, DeltaVentas = 10m } },
            { "Mover al fondo", new CoeficienteCambio { DeltaCirculacion = -15m, DeltaVentas = -10m } },
            { "Ampliar superficie", new CoeficienteCambio { DeltaCirculacion = 5m, DeltaVentas = 12m } },
            { "Reducir superficie", new CoeficienteCambio { DeltaCirculacion = -5m, DeltaVentas = -8m } },
            { "Reubicar productos destacados", new CoeficienteCambio { DeltaCirculacion = 8m, DeltaVentas = 6m } }
        };
        public List<IndicadorSectorListado> ProyectarIndicadores(List<IndicadorSectorListado> pIndicadoresReales, List<EscenarioCambio> pCambios)
        {
            var accionesPorSector = new Dictionary<string, string>();
            foreach (var cambio in pCambios)
            {
                int pos = cambio.Descripcion.IndexOf(": ");
                if (pos > 0)
                {
                    string sector = cambio.Descripcion.Substring(0, pos);
                    string accion = cambio.Descripcion.Substring(pos + 2);
                    accionesPorSector[sector] = accion;
                }
            }
            var proyectados = new List<IndicadorSectorListado>();
            foreach (var real in pIndicadoresReales)
            {
                decimal ventasProyectadas = real.Ventas;
                decimal circulacionProyectada = real.Circulacion;
                string accion;
                CoeficienteCambio coef;
                if (accionesPorSector.TryGetValue(real.Sector, out accion) && COEFICIENTES.TryGetValue(accion, out coef))
                {
                    ventasProyectadas = real.Ventas * (1 + coef.DeltaVentas / 100);
                    circulacionProyectada = Math.Max(0, Math.Min(100, real.Circulacion * (1 + coef.DeltaCirculacion / 100)));
                }
                int indiceGlobalProyectado = (int)Math.Round((circulacionProyectada + real.Rotacion) / 2, 0);
                proyectados.Add(new IndicadorSectorListado
                {
                    CodigoSector = real.CodigoSector,
                    Sector = real.Sector,
                    Ventas = Math.Round(ventasProyectadas, 2),
                    Margen = 0,
                    Circulacion = Math.Round(circulacionProyectada, 1),
                    Rotacion = real.Rotacion,
                    IndiceGlobal = indiceGlobalProyectado,
                    ClaseIndiceGlobal = indiceGlobalProyectado >= 80 ? "etiqueta-activo" : indiceGlobalProyectado >= 60 ? "etiqueta-bloqueado" : "etiqueta-inactivo"
                });
            }
            return proyectados;
        }
    }
}
