using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class ReporteBE
    {
        public int CodigoReporte { get; set; }
        public int CodigoLocal { get; set; }
        public int? CodigoEscenario { get; set; }
        public int? CodigoComparacion { get; set; }
        public string Titulo { get; set; }
        public string TipoReporte { get; set; }
        public string Descripcion { get; set; }
        public string RutaArchivo { get; set; }
        public DateTime? PeriodoDesde { get; set; }
        public DateTime? PeriodoHasta { get; set; }
        public DateTime FechaGeneracion { get; set; }
    }
}
