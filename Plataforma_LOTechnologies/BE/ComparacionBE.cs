using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class ComparacionBE
    {
        public int CodigoComparacion { get; set; }
        public int CodigoEscenarioA { get; set; }
        public int CodigoEscenarioB { get; set; }
        public DateTime FechaComparacion { get; set; }
        public string Resultado { get; set; }
        public string Observaciones { get; set; }
    }
}
