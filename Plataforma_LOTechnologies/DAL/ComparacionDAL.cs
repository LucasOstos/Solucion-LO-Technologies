using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;

namespace DAL
{
    public class ComparacionDAL
    {
        private AccesoDatos acceso = new AccesoDatos();
        public int AgregarComparacion(ComparacionBE pComparacion)
        {
            string query = @"INSERT INTO Comparacion (CodigoEscenarioA, CodigoEscenarioB, FechaComparacion, Resultado, Observaciones)
                             VALUES (@CodigoEscenarioA, @CodigoEscenarioB, @FechaComparacion, @Resultado, @Observaciones); SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoEscenarioA", pComparacion.CodigoEscenarioA);
                CM.Parameters.AddWithValue("@CodigoEscenarioB", pComparacion.CodigoEscenarioB);
                CM.Parameters.AddWithValue("@FechaComparacion", pComparacion.FechaComparacion);
                CM.Parameters.AddWithValue("@Resultado", (object)pComparacion.Resultado ?? DBNull.Value);
                CM.Parameters.AddWithValue("@Observaciones", (object)pComparacion.Observaciones ?? DBNull.Value);
                CO.Open();
                return (int)CM.ExecuteScalar();
            }
        }
    }
}
