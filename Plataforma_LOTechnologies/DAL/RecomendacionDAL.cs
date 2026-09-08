using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;

namespace DAL
{
    public class RecomendacionDAL
    {
        private AccesoDatos acceso = new AccesoDatos();
        public int AgregarRecomendacion(RecomendacionBE pRecomendacion)
        {
            string query = @"INSERT INTO Recomendacion (CodigoEscenario, CodigoLocal, CodigoProducto, CodigoSector, Descripcion, Motivo, Prioridad, TipoRecomendacion, Estado, FechaGeneracion)
                              VALUES (@CodigoEscenario, @CodigoLocal, @CodigoProducto, @CodigoSector, @Descripcion, @Motivo, @Prioridad, @TipoRecomendacion, @Estado, @FechaGeneracion);
                              SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoEscenario", (object)pRecomendacion.CodigoEscenario ?? DBNull.Value);
                CM.Parameters.AddWithValue("@CodigoLocal", pRecomendacion.CodigoLocal);
                CM.Parameters.AddWithValue("@CodigoProducto", (object)pRecomendacion.CodigoProducto ?? DBNull.Value);
                CM.Parameters.AddWithValue("@CodigoSector", (object)pRecomendacion.CodigoSector ?? DBNull.Value);
                CM.Parameters.AddWithValue("@Descripcion", pRecomendacion.Descripcion);
                CM.Parameters.AddWithValue("@Motivo", (object)pRecomendacion.Motivo ?? DBNull.Value);
                CM.Parameters.AddWithValue("@Prioridad", pRecomendacion.Prioridad);
                CM.Parameters.AddWithValue("@TipoRecomendacion", (object)pRecomendacion.TipoRecomendacion ?? DBNull.Value);
                CM.Parameters.AddWithValue("@Estado", pRecomendacion.Estado);
                CM.Parameters.AddWithValue("@FechaGeneracion", pRecomendacion.FechaGeneracion);
                CO.Open();
                return (int)CM.ExecuteScalar();
            }
        }
    }
}
