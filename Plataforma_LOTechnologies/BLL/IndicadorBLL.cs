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
        private VentaDAL ventaDAL = new VentaDAL();
        private DigitoVerificador digitos = new DigitoVerificador();
        public const decimal PESO_CIRCULACION = 0.4m;
        public const decimal PESO_ROTACION = 0.3m;
        public const decimal PESO_VENTAS_M2 = 0.3m;
        public const int INDICE_BUENO = 70;    // verde desde 70
        public const int INDICE_REGULAR = 50;  // amarillo desde 50, menos de 50 rojo

        // pPeriodoHasta null = hasta hoy. Si el período hasta es el mes en curso, se cuenta hasta hoy
        public List<IndicadorSectorListado> CalcularIndicadoresPorSector(int pCodigoLocal, string pPeriodoDesde, string pPeriodoHasta = null)
        {
            ControlAcceso.Validar(EntidadEmpresa.Local, pCodigoLocal);
            List<IndicadorSectorListado> indicadores = indicadorDAL.CalcularIndicadoresPorSector(pCodigoLocal, pPeriodoDesde, pPeriodoHasta);
            DateTime inicio = InicioDelPeriodo(pPeriodoDesde);
            DateTime fin = pPeriodoHasta == null ? DateTime.Today : FinDelMes(InicioDelPeriodo(pPeriodoHasta));
            int meses = Math.Max(1, (fin.Year - inicio.Year) * 12 + fin.Month - inicio.Month + 1);
            int dias = Math.Max(1, (fin - inicio).Days + 1);
            foreach (var ind in indicadores)
            {
                // Rotación mensual: qué % del stock actual se vende por mes
                ind.Rotacion = ind.StockDisponible > 0 ? Math.Round(ind.UnidadesVendidas / (decimal)meses / ind.StockDisponible * 100, 1) : 0;
                // Cobertura: cuántos días alcanza el stock al ritmo de venta diario del período
                ind.CoberturaDias = ind.StockDisponible > 0 && ind.UnidadesVendidas > 0
                    ? Math.Round(ind.StockDisponible / (ind.UnidadesVendidas / (decimal)dias), 0) : (decimal?)null;
            }
            AsignarIndices(indicadores, MaximaRotacion(indicadores), MaximasVentasPorM2(indicadores));
            return indicadores;
        }
        public decimal CompletarVariacion(int pCodigoLocal, int pMeses, List<IndicadorSectorListado> pIndicadores)
        {
            DateTime hoy = DateTime.Today;
            DateTime inicioActual = new DateTime(hoy.Year, hoy.Month, 1).AddMonths(-(Math.Max(pMeses, 1) - 1));
            return CompletarVariacion(pCodigoLocal, inicioActual, hoy, pIndicadores);
        }
        public decimal CompletarVariacion(int pCodigoLocal, DateTime pDesde, DateTime pHasta, List<IndicadorSectorListado> pIndicadores)
        {
            ControlAcceso.Validar(EntidadEmpresa.Local, pCodigoLocal);
            DateTime inicioActual = new DateTime(pDesde.Year, pDesde.Month, 1);
            int meses = Math.Max(1, (pHasta.Year - inicioActual.Year) * 12 + pHasta.Month - inicioActual.Month + 1);
            DateTime inicioAnterior = inicioActual.AddMonths(-meses);
            DateTime finAnterior = pHasta.AddMonths(-meses);
            Dictionary<int, decimal> anteriores = indicadorDAL.ObtenerVentasPorSectorEntreFechas(pCodigoLocal, inicioAnterior, finAnterior);
            foreach (var ind in pIndicadores)
            {
                decimal anterior;
                ind.VentasAnterior = anteriores.TryGetValue(ind.CodigoSector, out anterior) ? anterior : 0;
                ind.VariacionVentas = Variacion(ind.Ventas, ind.VentasAnterior);
            }
            return ventaDAL.ObtenerTotalVentasEntreFechas(pCodigoLocal, inicioAnterior, finAnterior);
        }
        public static decimal? Variacion(decimal pActual, decimal pAnterior)
        {
            if (pAnterior <= 0) return null;
            return Math.Round((pActual - pAnterior) / pAnterior * 100, 1);
        }

        // Ventas del local de los últimos meses (incluido el actual, marcado como "en curso")
        public List<VentaMensual> ObtenerEvolucionVentas(int pCodigoLocal, int pMeses)
        {
            DateTime primerMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-(pMeses - 1));
            return ObtenerEvolucionVentas(pCodigoLocal, primerMes, DateTime.Today);
        }
        // Ventas del local mes por mes entre dos meses inclusive (los meses sin ventas van en 0)
        public List<VentaMensual> ObtenerEvolucionVentas(int pCodigoLocal, DateTime pDesde, DateTime pHasta)
        {
            ControlAcceso.Validar(EntidadEmpresa.Local, pCodigoLocal);
            DateTime primerMes = new DateTime(pDesde.Year, pDesde.Month, 1);
            DateTime ultimoMes = new DateTime(pHasta.Year, pHasta.Month, 1);
            string mesEnCurso = DateTime.Today.ToString("yyyy-MM");
            Dictionary<string, decimal> ventas = ventaDAL.ObtenerVentasPorMes(pCodigoLocal, primerMes.ToString("yyyy-MM"), ultimoMes.ToString("yyyy-MM"));
            var evolucion = new List<VentaMensual>();
            for (DateTime mes = primerMes; mes <= ultimoMes; mes = mes.AddMonths(1))
            {
                string periodo = mes.ToString("yyyy-MM");
                decimal monto;
                evolucion.Add(new VentaMensual { Periodo = periodo, Monto = ventas.TryGetValue(periodo, out monto) ? monto : 0, EnCurso = periodo == mesEnCurso });
            }
            return evolucion;
        }

        //Un registro por sector e indicador. Si ya existe se actualiza (solo si cambió el valor)
        public void GuardarIndicadores(int pCodigoLocal, string pPeriodo, List<IndicadorSectorListado> pIndicadores)
        {
            ControlAcceso.Validar(EntidadEmpresa.Local, pCodigoLocal);
            var existentes = new Dictionary<string, IndicadorBE>();
            foreach (IndicadorBE ind in indicadorDAL.ObtenerIndicadoresDelPeriodo(pCodigoLocal, pPeriodo))
            {
                existentes[ind.CodigoSector + "|" + ind.TipoIndicador] = ind;
            }
            var modificados = new List<object>();
            foreach (var ind in pIndicadores)
            {
                GuardarUnIndicador(existentes, modificados, pCodigoLocal, ind.CodigoSector, pPeriodo, "Ventas", ind.Ventas, "$");
                GuardarUnIndicador(existentes, modificados, pCodigoLocal, ind.CodigoSector, pPeriodo, "Circulación", ind.Circulacion, "%");
                GuardarUnIndicador(existentes, modificados, pCodigoLocal, ind.CodigoSector, pPeriodo, "Rotación", ind.Rotacion, "%/mes");
                GuardarUnIndicador(existentes, modificados, pCodigoLocal, ind.CodigoSector, pPeriodo, "Índice Global", ind.IndiceGlobal, "pts");
                if (ind.Margen != null) GuardarUnIndicador(existentes, modificados, pCodigoLocal, ind.CodigoSector, pPeriodo, "Margen", ind.Margen.Value, "%");
                if (ind.VentasPorM2 != null) GuardarUnIndicador(existentes, modificados, pCodigoLocal, ind.CodigoSector, pPeriodo, "Ventas por m²", ind.VentasPorM2.Value, "$/m²");
                if (ind.Superficie != null) GuardarUnIndicador(existentes, modificados, pCodigoLocal, ind.CodigoSector, pPeriodo, "Superficie", ind.Superficie.Value, "m²");
                if (ind.CoberturaDias != null) GuardarUnIndicador(existentes, modificados, pCodigoLocal, ind.CodigoSector, pPeriodo, "Cobertura", ind.CoberturaDias.Value, "días");
            }
            if (modificados.Count > 0) digitos.ActualizarDigitoFilas("Indicador", modificados);
        }
        private void GuardarUnIndicador(Dictionary<string, IndicadorBE> pExistentes, List<object> pModificados, int codigoLocal, int codigoSector, string periodo, string nombre, decimal valor, string unidad)
        {
            IndicadorBE existente;
            if (pExistentes.TryGetValue(codigoSector + "|" + nombre, out existente))
            {
                if (existente.Valor == valor) return;
                indicadorDAL.ActualizarIndicador(existente.CodigoIndicador, valor, DateTime.Now);
                pModificados.Add(existente.CodigoIndicador);
                return;
            }
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
            pModificados.Add(indicadorDAL.AgregarIndicador(indicador));
        }

        #region Índice global
        private static DateTime InicioDelPeriodo(string pPeriodoDesde)
        {
            DateTime inicio;
            if (DateTime.TryParseExact(pPeriodoDesde + "-01", "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out inicio)) return inicio;
            return new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        }
        // Último día del mes, sin pasar de hoy
        private static DateTime FinDelMes(DateTime pInicioMes)
        {
            DateTime fin = pInicioMes.AddMonths(1).AddDays(-1);
            return fin > DateTime.Today ? DateTime.Today : fin;
        }
        private static decimal MaximaRotacion(List<IndicadorSectorListado> pIndicadores)
        {
            return pIndicadores.Count == 0 ? 0 : pIndicadores.Max(i => i.Rotacion);
        }
        private static decimal MaximasVentasPorM2(List<IndicadorSectorListado> pIndicadores)
        {
            return pIndicadores.Where(i => i.VentasPorM2 != null).Select(i => i.VentasPorM2.Value).DefaultIfEmpty(0).Max();
        }
        private static void AsignarIndices(List<IndicadorSectorListado> pIndicadores, decimal pMaximaRotacion, decimal pMaximasVentasM2)
        {
            foreach (var ind in pIndicadores)
            {
                ind.IndiceGlobal = CalcularIndice(ind.Circulacion, ind.Rotacion, ind.VentasPorM2, pMaximaRotacion, pMaximasVentasM2);
                ind.ClaseIndiceGlobal = ClasificarIndice(ind.IndiceGlobal);
            }
        }
        public static int CalcularIndice(decimal pCirculacion, decimal pRotacion, decimal? pVentasM2, decimal pMaximaRotacion, decimal pMaximasVentasM2)
        {
            decimal circulacion = Math.Max(0, Math.Min(100, pCirculacion));
            decimal rotacion = pMaximaRotacion > 0 ? Math.Min(100, pRotacion / pMaximaRotacion * 100) : 0;
            decimal indice;
            if (pVentasM2 != null && pMaximasVentasM2 > 0)
            {
                decimal ventasM2 = Math.Min(100, pVentasM2.Value / pMaximasVentasM2 * 100);
                indice = PESO_CIRCULACION * circulacion + PESO_ROTACION * rotacion + PESO_VENTAS_M2 * ventasM2;
            }
            else
            {
                indice = (PESO_CIRCULACION * circulacion + PESO_ROTACION * rotacion) / (PESO_CIRCULACION + PESO_ROTACION);
            }
            return (int)Math.Round(indice, 0, MidpointRounding.AwayFromZero);
        }
        public static string ClasificarIndice(int pIndice)
        {
            if (pIndice >= INDICE_BUENO) return "etiqueta-activo";
            if (pIndice >= INDICE_REGULAR) return "etiqueta-bloqueado";
            return "etiqueta-inactivo";
        }
        #endregion
        private class CoeficienteCambio
        {
            public decimal DeltaCirculacion { get; set; }
            public decimal DeltaVentas { get; set; }
            public decimal DeltaSuperficie { get; set; }
        }
        private static readonly Dictionary<string, CoeficienteCambio> COEFICIENTES = new Dictionary<string, CoeficienteCambio>
        {
            { "Mover al frente", new CoeficienteCambio { DeltaCirculacion = 15m, DeltaVentas = 10m } },
            { "Mover al fondo", new CoeficienteCambio { DeltaCirculacion = -15m, DeltaVentas = -10m } },
            { "Ampliar superficie", new CoeficienteCambio { DeltaCirculacion = 5m, DeltaVentas = 12m, DeltaSuperficie = 20m } },
            { "Reducir superficie", new CoeficienteCambio { DeltaCirculacion = -5m, DeltaVentas = -8m, DeltaSuperficie = -20m } },
            { "Reubicar productos destacados", new CoeficienteCambio { DeltaCirculacion = 8m, DeltaVentas = 6m } }
        };
        // Supuestos de la simulación, para mostrarlos en Escenarios y Comparación ("¿de dónde salen los números?")
        public static List<SupuestoSimulacion> ObtenerSupuestos()
        {
            return COEFICIENTES.Select(c => new SupuestoSimulacion
            {
                Accion = c.Key,
                DeltaCirculacion = c.Value.DeltaCirculacion,
                DeltaVentas = c.Value.DeltaVentas,
                DeltaSuperficie = c.Value.DeltaSuperficie
            }).ToList();
        }
        public List<IndicadorSectorListado> ProyectarIndicadores(List<IndicadorSectorListado> pIndicadoresReales, List<EscenarioCambio> pCambios)
        {
            var accionesPorCodigo = new Dictionary<int, string>();
            var accionesPorNombre = new Dictionary<string, string>();
            foreach (var cambio in pCambios)
            {
                if (cambio.CodigoSector != null && !string.IsNullOrEmpty(cambio.Accion))
                {
                    accionesPorCodigo[cambio.CodigoSector.Value] = cambio.Accion;
                    continue;
                }
                int pos = cambio.Descripcion == null ? -1 : cambio.Descripcion.IndexOf(": ");
                if (pos > 0)
                {
                    accionesPorNombre[cambio.Descripcion.Substring(0, pos)] = cambio.Descripcion.Substring(pos + 2);
                }
            }
            // El índice proyectado se mide contra los MEJORES valores reales, así dos escenarios comparten la misma escala
            decimal maximaRotacion = MaximaRotacion(pIndicadoresReales);
            decimal maximasVentasM2 = MaximasVentasPorM2(pIndicadoresReales);
            var proyectados = new List<IndicadorSectorListado>();
            foreach (var real in pIndicadoresReales)
            {
                decimal ventasProyectadas = real.Ventas;
                decimal circulacionProyectada = real.Circulacion;
                decimal? superficieProyectada = real.Superficie;
                string accion;
                CoeficienteCambio coef;
                bool tieneAccion = accionesPorCodigo.TryGetValue(real.CodigoSector, out accion) || accionesPorNombre.TryGetValue(real.Sector, out accion);
                if (tieneAccion && COEFICIENTES.TryGetValue(accion, out coef))
                {
                    ventasProyectadas = real.Ventas * (1 + coef.DeltaVentas / 100);
                    circulacionProyectada = Math.Max(0, Math.Min(100, real.Circulacion * (1 + coef.DeltaCirculacion / 100)));
                    if (real.Superficie != null) superficieProyectada = Math.Round(real.Superficie.Value * (1 + coef.DeltaSuperficie / 100), 2);
                }
                decimal? ventasM2Proyectadas = DAL.IndicadorDAL.CalcularVentasPorM2(ventasProyectadas, superficieProyectada);
                int indiceGlobalProyectado = CalcularIndice(circulacionProyectada, real.Rotacion, ventasM2Proyectadas, maximaRotacion, maximasVentasM2);
                proyectados.Add(new IndicadorSectorListado
                {
                    CodigoSector = real.CodigoSector,
                    Sector = real.Sector,
                    Ventas = Math.Round(ventasProyectadas, 2),
                    Margen = real.Margen, // el escenario cambia ubicación y superficie, no precios ni costos
                    Superficie = superficieProyectada,
                    VentasPorM2 = ventasM2Proyectadas,
                    Circulacion = Math.Round(circulacionProyectada, 1),
                    Rotacion = real.Rotacion,
                    CoberturaDias = real.CoberturaDias,
                    UnidadesVendidas = real.UnidadesVendidas,
                    StockDisponible = real.StockDisponible,
                    IndiceGlobal = indiceGlobalProyectado,
                    ClaseIndiceGlobal = ClasificarIndice(indiceGlobalProyectado)
                });
            }
            return proyectados;
        }
    }
}
