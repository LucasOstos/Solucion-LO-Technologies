using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;

namespace DAL
{
    public class EscenarioDAL
    {
        private AccesoDatos acceso = new AccesoDatos();
        public List<EscenarioListado> ObtenerEscenariosPorLocal(int pCodigoLocal)
        {
            List<EscenarioListado> escenarios = new List<EscenarioListado>();
            string query = @"SELECT e.CodigoEscenario, e.CodigoLocal, e.Nombre, e.Descripcion, e.Estado,
                             (SELECT COUNT(*) FROM EscenarioCambio ec WHERE ec.CodigoEscenario = e.CodigoEscenario) AS CantidadCambios FROM Escenario e
                             WHERE e.CodigoLocal = @CodigoLocal ORDER BY e.FechaCreacion DESC";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoLocal", pCodigoLocal);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    while (DR.Read())
                    {
                        escenarios.Add(new EscenarioListado
                        {
                            CodigoEscenario = int.Parse(DR["CodigoEscenario"].ToString()),
                            CodigoLocal = int.Parse(DR["CodigoLocal"].ToString()),
                            Nombre = DR["Nombre"].ToString(),
                            Descripcion = DR["Descripcion"] == DBNull.Value ? "" : DR["Descripcion"].ToString(),
                            Estado = DR["Estado"].ToString(),
                            CantidadCambios = int.Parse(DR["CantidadCambios"].ToString()),
                            // Valores en 0 hasta que exista ndicador del CUN 009
                            VentasEstimadas = 0,
                            CirculacionEstimada = 0,
                            SatisfaccionEstimada = 0
                        });
                    }
                }
            }
            return escenarios;
        }
        public EscenarioBE ObtenerEscenarioPorCodigo(int pCodigoEscenario)
        {
            EscenarioBE escenario = null;
            string query = @"SELECT CodigoEscenario, CodigoLocal, Nombre, Descripcion, Objetivo, FechaCreacion, Estado
                             FROM Escenario WHERE CodigoEscenario = @CodigoEscenario";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoEscenario", pCodigoEscenario);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    if (DR.Read())
                    {
                        escenario = new EscenarioBE(int.Parse(DR["CodigoEscenario"].ToString()), int.Parse(DR["CodigoLocal"].ToString()), DR["Nombre"].ToString(),
                                                    DR["Descripcion"] == DBNull.Value ? "" : DR["Descripcion"].ToString(), DR["Objetivo"] == DBNull.Value ? "" : DR["Objetivo"].ToString(),
                                                    Convert.ToDateTime(DR["FechaCreacion"]), DR["Estado"].ToString());                        
                    }
                }
            }
            return escenario;
        }
        public List<EscenarioCambio> ObtenerCambiosPorEscenario(int pCodigoEscenario)
        {
            List<EscenarioCambio> cambios = new List<EscenarioCambio>();
            string query = "SELECT CodigoCambio, CodigoEscenario, TipoCambio, Descripcion FROM EscenarioCambio WHERE CodigoEscenario = @CodigoEscenario";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoEscenario", pCodigoEscenario);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    while (DR.Read())
                    {
                        cambios.Add(new EscenarioCambio
                        {
                            CodigoCambio = int.Parse(DR["CodigoCambio"].ToString()),
                            CodigoEscenario = int.Parse(DR["CodigoEscenario"].ToString()),
                            TipoCambio = DR["TipoCambio"].ToString(),
                            Descripcion = DR["Descripcion"].ToString()
                        });
                    }
                }
            }
            return cambios;
        }
        public int AgregarEscenario(EscenarioBE pEscenario)
        {
            string query = @"INSERT INTO Escenario (CodigoLocal, Nombre, Descripcion, Objetivo, FechaCreacion, Estado)
                             VALUES (@CodigoLocal, @Nombre, @Descripcion, @Objetivo, @FechaCreacion, @Estado); SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoLocal", pEscenario.CodigoLocal);
                CM.Parameters.AddWithValue("@Nombre", pEscenario.Nombre);
                CM.Parameters.AddWithValue("@Descripcion", (object)pEscenario.Descripcion ?? DBNull.Value);
                CM.Parameters.AddWithValue("@Objetivo", (object)pEscenario.Objetivo ?? DBNull.Value);
                CM.Parameters.AddWithValue("@FechaCreacion", pEscenario.FechaCreacion);
                CM.Parameters.AddWithValue("@Estado", pEscenario.Estado);
                CO.Open();
                return (int)CM.ExecuteScalar();
            }
        }
        public void AgregarCambio(EscenarioCambio pCambio)
        {
            string query = @"INSERT INTO EscenarioCambio (CodigoEscenario, TipoCambio, Descripcion) VALUES (@CodigoEscenario, @TipoCambio, @Descripcion)";

            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoEscenario", pCambio.CodigoEscenario);
                CM.Parameters.AddWithValue("@TipoCambio", pCambio.TipoCambio);
                CM.Parameters.AddWithValue("@Descripcion", pCambio.Descripcion);
                CO.Open();
                CM.ExecuteNonQuery();
            }
        }
        public void EliminarEscenario(int pCodigoEscenario)
        {
            using (SqlConnection CO = acceso.NuevaConexion())
            {
                CO.Open();
                using (SqlCommand cmCambios = new SqlCommand("DELETE FROM EscenarioCambio WHERE CodigoEscenario = @CodigoEscenario", CO))
                {
                    cmCambios.Parameters.AddWithValue("@CodigoEscenario", pCodigoEscenario);
                    cmCambios.ExecuteNonQuery();
                }
                using (SqlCommand cmEscenario = new SqlCommand("DELETE FROM Escenario WHERE CodigoEscenario = @CodigoEscenario", CO))
                {
                    cmEscenario.Parameters.AddWithValue("@CodigoEscenario", pCodigoEscenario);
                    cmEscenario.ExecuteNonQuery();
                }
            }
        }
    }
}
