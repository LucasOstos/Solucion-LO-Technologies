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
            string query = @"SELECT e.CodigoEscenario, e.CodigoLocal, e.Nombre, e.Descripcion, e.Estado, e.CodigoLayout, l.Nombre AS NombreLayout,
                             (SELECT COUNT(*) FROM EscenarioCambio ec WHERE ec.CodigoEscenario = e.CodigoEscenario) AS CantidadCambios FROM Escenario e
                             LEFT JOIN Layout l ON l.CodigoLayout = e.CodigoLayout WHERE e.CodigoLocal = @CodigoLocal ORDER BY e.FechaCreacion DESC";
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
                            // Las estimaciones las calcula la GUI con IndicadorBLL.ProyectarIndicadores
                            VentasEstimadas = 0,
                            CirculacionEstimada = 0,
                            SatisfaccionEstimada = 0,
                            CodigoLayout = DR["CodigoLayout"] == DBNull.Value ? (int?)null : int.Parse(DR["CodigoLayout"].ToString()),
                            NombreLayout = DR["NombreLayout"] == DBNull.Value ? null : DR["NombreLayout"].ToString()
                        });
                    }
                }
            }
            return escenarios;
        }
        public EscenarioBE ObtenerEscenarioPorCodigo(int pCodigoEscenario)
        {
            EscenarioBE escenario = null;
            string query = @"SELECT CodigoEscenario, CodigoLocal, Nombre, Descripcion, Objetivo, FechaCreacion, Estado, CodigoLayout FROM Escenario WHERE CodigoEscenario = @CodigoEscenario";
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
                        escenario.CodigoLayout = DR["CodigoLayout"] == DBNull.Value ? (int?)null : int.Parse(DR["CodigoLayout"].ToString());
                    }
                }
            }
            return escenario;
        }
        public List<EscenarioCambio> ObtenerCambiosPorEscenario(int pCodigoEscenario)
        {
            List<EscenarioCambio> cambios = new List<EscenarioCambio>();
            string query = "SELECT CodigoCambio, CodigoEscenario, TipoCambio, Descripcion, CodigoSector, Accion FROM EscenarioCambio WHERE CodigoEscenario = @CodigoEscenario";
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
                            Descripcion = DR["Descripcion"].ToString(),
                            CodigoSector = DR["CodigoSector"] == DBNull.Value ? (int?)null : int.Parse(DR["CodigoSector"].ToString()),
                            Accion = DR["Accion"] == DBNull.Value ? null : DR["Accion"].ToString()
                        });
                    }
                }
            }
            return cambios;
        }
        // Escenario y cambios en una transacción, se guarda todo o nada
        public int AgregarEscenarioConCambios(EscenarioBE pEscenario, List<EscenarioCambio> pCambios)
        {
            string queryEscenario = @"INSERT INTO Escenario (CodigoLocal, Nombre, Descripcion, Objetivo, FechaCreacion, Estado, CodigoLayout)
                                      VALUES (@CodigoLocal, @Nombre, @Descripcion, @Objetivo, @FechaCreacion, @Estado, @CodigoLayout); SELECT CAST(SCOPE_IDENTITY() AS INT);";

            string queryCambio = @"INSERT INTO EscenarioCambio (CodigoEscenario, TipoCambio, Descripcion, CodigoSector, Accion) VALUES (@CodigoEscenario, @TipoCambio, @Descripcion, @CodigoSector, @Accion)";
            using (SqlConnection CO = acceso.NuevaConexion())
            {
                CO.Open();
                using (SqlTransaction TR = CO.BeginTransaction())
                {
                    try
                    {
                        int codigoNuevo;
                        using (SqlCommand CM = new SqlCommand(queryEscenario, CO, TR))
                        {
                            CM.Parameters.AddWithValue("@CodigoLocal", pEscenario.CodigoLocal);
                            CM.Parameters.AddWithValue("@Nombre", pEscenario.Nombre);
                            CM.Parameters.AddWithValue("@Descripcion", (object)pEscenario.Descripcion ?? DBNull.Value);
                            CM.Parameters.AddWithValue("@Objetivo", (object)pEscenario.Objetivo ?? DBNull.Value);
                            CM.Parameters.AddWithValue("@FechaCreacion", pEscenario.FechaCreacion);
                            CM.Parameters.AddWithValue("@Estado", pEscenario.Estado);
                            CM.Parameters.AddWithValue("@CodigoLayout", (object)pEscenario.CodigoLayout ?? DBNull.Value);
                            codigoNuevo = (int)CM.ExecuteScalar();
                        }
                        foreach (EscenarioCambio cambio in pCambios)
                        {
                            using (SqlCommand CM = new SqlCommand(queryCambio, CO, TR))
                            {
                                CM.Parameters.AddWithValue("@CodigoEscenario", codigoNuevo);
                                CM.Parameters.AddWithValue("@TipoCambio", cambio.TipoCambio);
                                CM.Parameters.AddWithValue("@Descripcion", cambio.Descripcion);
                                CM.Parameters.AddWithValue("@CodigoSector", (object)cambio.CodigoSector ?? DBNull.Value);
                                CM.Parameters.AddWithValue("@Accion", (object)cambio.Accion ?? DBNull.Value);
                                CM.ExecuteNonQuery();
                            }
                            cambio.CodigoEscenario = codigoNuevo;
                        }
                        TR.Commit();
                        return codigoNuevo;
                    }
                    catch
                    {
                        TR.Rollback();
                        throw;
                    }
                }
            }
        }
        public bool ExisteNombre(int pCodigoLocal, string pNombre)
        {
            string query = "SELECT COUNT(1) FROM Escenario WHERE CodigoLocal = @CodigoLocal AND UPPER(LTRIM(RTRIM(Nombre))) = UPPER(@Nombre)";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoLocal", pCodigoLocal);
                CM.Parameters.AddWithValue("@Nombre", (pNombre ?? "").Trim());
                CO.Open();
                return (int)CM.ExecuteScalar() > 0;
            }
        }
        public void CambiarEstado(int pCodigoEscenario, string pEstado)
        {
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand("UPDATE Escenario SET Estado = @Estado WHERE CodigoEscenario = @CodigoEscenario", CO))
            {
                CM.Parameters.AddWithValue("@Estado", pEstado);
                CM.Parameters.AddWithValue("@CodigoEscenario", pCodigoEscenario);
                CO.Open();
                CM.ExecuteNonQuery();
            }
        }
        public int AgregarEscenario(EscenarioBE pEscenario)
        {
            string query = @"INSERT INTO Escenario (CodigoLocal, Nombre, Descripcion, Objetivo, FechaCreacion, Estado, CodigoLayout)
                             VALUES (@CodigoLocal, @Nombre, @Descripcion, @Objetivo, @FechaCreacion, @Estado, @CodigoLayout); SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoLocal", pEscenario.CodigoLocal);
                CM.Parameters.AddWithValue("@Nombre", pEscenario.Nombre);
                CM.Parameters.AddWithValue("@Descripcion", (object)pEscenario.Descripcion ?? DBNull.Value);
                CM.Parameters.AddWithValue("@Objetivo", (object)pEscenario.Objetivo ?? DBNull.Value);
                CM.Parameters.AddWithValue("@FechaCreacion", pEscenario.FechaCreacion);
                CM.Parameters.AddWithValue("@Estado", pEscenario.Estado);
                CM.Parameters.AddWithValue("@CodigoLayout", (object)pEscenario.CodigoLayout ?? DBNull.Value);
                CO.Open();
                return (int)CM.ExecuteScalar();
            }
        }
        public void AgregarCambio(EscenarioCambio pCambio)
        {
            string query = @"INSERT INTO EscenarioCambio (CodigoEscenario, TipoCambio, Descripcion, CodigoSector, Accion) VALUES (@CodigoEscenario, @TipoCambio, @Descripcion, @CodigoSector, @Accion)";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoEscenario", pCambio.CodigoEscenario);
                CM.Parameters.AddWithValue("@TipoCambio", pCambio.TipoCambio);
                CM.Parameters.AddWithValue("@Descripcion", pCambio.Descripcion);
                CM.Parameters.AddWithValue("@CodigoSector", (object)pCambio.CodigoSector ?? DBNull.Value);
                CM.Parameters.AddWithValue("@Accion", (object)pCambio.Accion ?? DBNull.Value);
                CO.Open();
                CM.ExecuteNonQuery();
            }
        }
        public List<int> QuitarLayoutBase(int pCodigoLayout)
        {
            var codigos = new List<int>();
            string query = @"UPDATE Escenario SET CodigoLayout = NULL OUTPUT INSERTED.CodigoEscenario WHERE CodigoLayout = @CodigoLayout";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoLayout", pCodigoLayout);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    while (DR.Read()) codigos.Add(DR.GetInt32(0));
                }
            }
            return codigos;
        }
        // Borra el escenario y todo lo que depende de él en una transacción
        public ResultadoEliminarEscenario EliminarEscenario(int pCodigoEscenario)
        {
            var resultado = new ResultadoEliminarEscenario();
            string[] borrados =
            {
                "DELETE FROM Comparacion WHERE CodigoEscenarioA = @Codigo OR CodigoEscenarioB = @Codigo",
                "DELETE FROM Indicador WHERE CodigoEscenario = @Codigo",
                "DELETE FROM Recomendacion WHERE CodigoEscenario = @Codigo",
                "DELETE FROM EscenarioCambio WHERE CodigoEscenario = @Codigo",
                "DELETE FROM Escenario WHERE CodigoEscenario = @Codigo"
            };
            using (SqlConnection CO = acceso.NuevaConexion())
            {
                CO.Open();
                using (SqlTransaction TR = CO.BeginTransaction())
                {
                    try
                    {
                        string queryReportes = @"UPDATE Reporte SET CodigoComparacion = NULL OUTPUT INSERTED.CodigoReporte
                                                 WHERE CodigoComparacion IN (SELECT CodigoComparacion FROM Comparacion WHERE CodigoEscenarioA = @Codigo OR CodigoEscenarioB = @Codigo);
                                                 UPDATE Reporte SET CodigoEscenario = NULL OUTPUT INSERTED.CodigoReporte WHERE CodigoEscenario = @Codigo;";
                        using (SqlCommand CM = new SqlCommand(queryReportes, CO, TR))
                        {
                            CM.Parameters.AddWithValue("@Codigo", pCodigoEscenario);
                            using (SqlDataReader DR = CM.ExecuteReader())
                            {
                                do
                                {
                                    while (DR.Read()) resultado.ReportesModificados.Add(DR.GetInt32(0));
                                } while (DR.NextResult());
                            }
                        }
                        int[] cantidades = new int[borrados.Length];
                        for (int i = 0; i < borrados.Length; i++)
                        {
                            using (SqlCommand CM = new SqlCommand(borrados[i], CO, TR))
                            {
                                CM.Parameters.AddWithValue("@Codigo", pCodigoEscenario);
                                cantidades[i] = CM.ExecuteNonQuery();
                            }
                        }
                        TR.Commit();
                        resultado.ComparacionesBorradas = cantidades[0];
                        resultado.IndicadoresBorrados = cantidades[1];
                        resultado.RecomendacionesBorradas = cantidades[2];
                        resultado.ReportesModificados = resultado.ReportesModificados.Distinct().ToList();
                        return resultado;
                    }
                    catch
                    {
                        TR.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
