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
    public class ReporteBLL
    {
        private ReporteDAL reporteDAL = new ReporteDAL();
        private DigitoVerificador digitos = new DigitoVerificador();
        public List<ReporteBE> ObtenerReportesPorLocal(int pCodigoLocal)
        {
            return reporteDAL.ObtenerReportesPorLocal(pCodigoLocal);
        }
        public ReporteBE ObtenerReportePorCodigo(int pCodigoReporte)
        {
            return reporteDAL.ObtenerReportePorCodigo(pCodigoReporte);
        }
        public int CrearReporte(ReporteBE pReporte)
        {
            int codigoNuevo = reporteDAL.AgregarReporte(pReporte);
            digitos.ActualizarDigitoFila("Reporte", codigoNuevo);
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "Generar reporte", "Reportes", 3);
            return codigoNuevo;
        }
    }
}
