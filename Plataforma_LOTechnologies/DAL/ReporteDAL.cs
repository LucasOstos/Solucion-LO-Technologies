using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;

namespace DAL
{
    public class ReporteDAL
    {
        private AccesoDatos acceso = new AccesoDatos();
        public List<ReporteBE> ObtenerReportesPorLocal(int pCodigoLocal)
        {
            var reportes = new List<ReporteBE>();
            string query = @"SELECT CodigoReporte, CodigoLocal, CodigoEscenario, CodigoComparacion, Titulo, TipoReporte, Descripcion, RutaArchivo, PeriodoDesde, PeriodoHasta, FechaGeneracion
                             FROM Reporte WHERE CodigoLocal = @CodigoLocal ORDER BY FechaGeneracion DESC";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoLocal", pCodigoLocal);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    while (DR.Read())
                    {
                        reportes.Add(MapearReporte(DR));
                    }
                }
            }
            return reportes;
        }
        public ReporteBE ObtenerReportePorCodigo(int pCodigoReporte)
        {
            ReporteBE reporte = null;
            string query = @"SELECT CodigoReporte, CodigoLocal, CodigoEscenario, CodigoComparacion, Titulo, TipoReporte, Descripcion, RutaArchivo, PeriodoDesde, PeriodoHasta, FechaGeneracion
                             FROM Reporte WHERE CodigoReporte = @CodigoReporte";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoReporte", pCodigoReporte);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    if (DR.Read()) reporte = MapearReporte(DR);
                }
            }
            return reporte;
        }
        public int AgregarReporte(ReporteBE pReporte)
        {
            string query = @"INSERT INTO Reporte (CodigoLocal, CodigoEscenario, CodigoComparacion, Titulo, TipoReporte, Descripcion, RutaArchivo, PeriodoDesde, PeriodoHasta, FechaGeneracion)
                             VALUES (@CodigoLocal, @CodigoEscenario, @CodigoComparacion, @Titulo, @TipoReporte, @Descripcion, @RutaArchivo, @PeriodoDesde, @PeriodoHasta, @FechaGeneracion);
                             SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoLocal", pReporte.CodigoLocal);
                CM.Parameters.AddWithValue("@CodigoEscenario", (object)pReporte.CodigoEscenario ?? DBNull.Value);
                CM.Parameters.AddWithValue("@CodigoComparacion", (object)pReporte.CodigoComparacion ?? DBNull.Value);
                CM.Parameters.AddWithValue("@Titulo", pReporte.Titulo);
                CM.Parameters.AddWithValue("@TipoReporte", pReporte.TipoReporte);
                CM.Parameters.AddWithValue("@Descripcion", (object)pReporte.Descripcion ?? DBNull.Value);
                CM.Parameters.AddWithValue("@RutaArchivo", pReporte.RutaArchivo);
                CM.Parameters.AddWithValue("@PeriodoDesde", (object)pReporte.PeriodoDesde ?? DBNull.Value);
                CM.Parameters.AddWithValue("@PeriodoHasta", (object)pReporte.PeriodoHasta ?? DBNull.Value);
                CM.Parameters.AddWithValue("@FechaGeneracion", pReporte.FechaGeneracion);
                CO.Open();
                return (int)CM.ExecuteScalar();
            }
        }
        private ReporteBE MapearReporte(SqlDataReader DR)
        {
            return new ReporteBE(int.Parse(DR["CodigoReporte"].ToString()), int.Parse(DR["CodigoLocal"].ToString()),
                                 DR["CodigoEscenario"] == DBNull.Value ? (int?)null : int.Parse(DR["CodigoEscenario"].ToString()),
                                 DR["CodigoComparacion"] == DBNull.Value ? (int?)null : int.Parse(DR["CodigoComparacion"].ToString()),
                                 DR["Titulo"].ToString(), DR["TipoReporte"].ToString(), DR["Descripcion"] == DBNull.Value ? "" : DR["Descripcion"].ToString(), DR["RutaArchivo"].ToString(),
                                 DR["PeriodoDesde"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(DR["PeriodoDesde"]),
                                 DR["PeriodoHasta"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(DR["PeriodoHasta"]), Convert.ToDateTime(DR["FechaGeneracion"]));            
        }
    }
}
