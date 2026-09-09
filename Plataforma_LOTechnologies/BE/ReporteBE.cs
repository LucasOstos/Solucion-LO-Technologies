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
        public ReporteBE(int codigoLocal, int? codigoEscenario, int? codigoComparacion, string titulo, string tipoReporte, string descripcion, string rutaArchivo, DateTime? periodoDesde, DateTime? periodoHasta, DateTime fechaGeneracion)
        {
            CodigoLocal = codigoLocal;
            CodigoEscenario = codigoEscenario;
            CodigoComparacion = codigoComparacion;
            Titulo = titulo;
            TipoReporte = tipoReporte;
            Descripcion = descripcion;
            RutaArchivo = rutaArchivo;
            PeriodoDesde = periodoDesde;
            PeriodoHasta = periodoHasta;
            FechaGeneracion = fechaGeneracion;
        }
        public ReporteBE(int codigoReporte, int codigoLocal, int? codigoEscenario, int? codigoComparacion, string titulo, string tipoReporte, string descripcion, string rutaArchivo, DateTime? periodoDesde, DateTime? periodoHasta, DateTime fechaGeneracion)
        {
            CodigoReporte = codigoReporte;
            CodigoLocal = codigoLocal;
            CodigoEscenario = codigoEscenario;
            CodigoComparacion = codigoComparacion;
            Titulo = titulo;
            TipoReporte = tipoReporte;
            Descripcion = descripcion;
            RutaArchivo = rutaArchivo;
            PeriodoDesde = periodoDesde;
            PeriodoHasta = periodoHasta;
            FechaGeneracion = fechaGeneracion;
        }
    }
}
