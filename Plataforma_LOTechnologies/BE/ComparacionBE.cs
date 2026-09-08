using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{

    #region Auxiliar
    public class IndicadorComparado
    {
        public string Nombre { get; set; }
        public string Valor { get; set; }
    }
    public class DiferenciaComparacion
    {
        public string Indicador { get; set; }
        public string ValorActual { get; set; }
        public string ValorPropuesto { get; set; }
        public string Diferencia { get; set; }
        public string ClaseDiferencia { get; set; }
    }
    #endregion

    public class ComparacionBE
    {
        public int CodigoComparacion { get; set; }
        public int CodigoEscenarioA { get; set; }
        public int CodigoEscenarioB { get; set; }
        public DateTime FechaComparacion { get; set; }
        public string Resultado { get; set; }
        public string Observaciones { get; set; }
        public ComparacionBE(int codigoEscenarioA, int codigoEscenarioB, DateTime fechaComparacion, string resultado, string observaciones)
        {
            CodigoEscenarioA = codigoEscenarioA;
            CodigoEscenarioB = codigoEscenarioB;
            FechaComparacion = fechaComparacion;
            Resultado = resultado;
            Observaciones = observaciones;
        }
        public ComparacionBE(int codigoComparacion, int codigoEscenarioA, int codigoEscenarioB, DateTime fechaComparacion, string resultado, string observaciones)
        {
            CodigoComparacion = codigoComparacion;
            CodigoEscenarioA = codigoEscenarioA;
            CodigoEscenarioB = codigoEscenarioB;
            FechaComparacion = fechaComparacion;
            Resultado = resultado;
            Observaciones = observaciones;
        }
    }
}
