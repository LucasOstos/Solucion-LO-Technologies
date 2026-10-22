using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;

namespace DAL
{
    public class TraduccionDAL
    {
        private AccesoDatos acceso = new AccesoDatos();
        public Dictionary<string, string> ObtenerTextos(int pCodigoIdioma)
        {
            Dictionary<string, string> textos = new Dictionary<string, string>();
            string query = "SELECT Clave, Texto FROM Traduccion WHERE CodigoIdioma = @CodigoIdioma";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoIdioma", pCodigoIdioma);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    while (DR.Read())
                    {
                        textos[DR["Clave"].ToString()] = DR["Texto"].ToString();
                    }
                }
            }
            return textos;
        }
        public int ContarPendientes(int pCodigoIdioma)
        {
            string query = "SELECT COUNT(1) FROM Traduccion WHERE CodigoIdioma = @CodigoIdioma AND Pendiente = 1";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoIdioma", pCodigoIdioma);
                CO.Open();
                return Convert.ToInt32(CM.ExecuteScalar());
            }
        }
        public List<TraduccionComparada> ObtenerComparadas(int pCodigoIdioma, int pCodigoIdiomaBase)
        {
            List<TraduccionComparada> lista = new List<TraduccionComparada>();
            string query = @"SELECT t.CodigoTraduccion, t.Clave, t.Modulo, t.Texto, t.Pendiente, b.Texto AS TextoBase
                             FROM Traduccion t
                             LEFT JOIN Traduccion b ON b.Clave = t.Clave AND b.CodigoIdioma = @CodigoIdiomaBase
                             WHERE t.CodigoIdioma = @CodigoIdioma
                             ORDER BY t.Modulo, t.Clave";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoIdioma", pCodigoIdioma);
                CM.Parameters.AddWithValue("@CodigoIdiomaBase", pCodigoIdiomaBase);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    while (DR.Read())
                    {
                        lista.Add(new TraduccionComparada
                        {
                            CodigoTraduccion = Convert.ToInt32(DR["CodigoTraduccion"]),
                            Clave = DR["Clave"].ToString(),
                            Modulo = DR["Modulo"].ToString(),
                            TextoTraducido = DR["Texto"].ToString(),
                            Pendiente = Convert.ToBoolean(DR["Pendiente"]),
                            TextoBase = DR["TextoBase"] == DBNull.Value ? "" : DR["TextoBase"].ToString()
                        });
                    }
                }
            }
            return lista;
        }
        public void ActualizarTextos(int pCodigoIdioma, List<Traduccion> pTraducciones)
        {
            string query = "UPDATE Traduccion SET Texto = @Texto, Pendiente = @Pendiente WHERE CodigoTraduccion = @CodigoTraduccion AND CodigoIdioma = @CodigoIdioma";
            using (SqlConnection CO = acceso.NuevaConexion())
            {
                CO.Open();
                using (SqlTransaction TR = CO.BeginTransaction())
                {
                    try
                    {
                        foreach (Traduccion traduccion in pTraducciones)
                        {
                            using (SqlCommand CM = new SqlCommand(query, CO, TR))
                            {
                                CM.Parameters.AddWithValue("@Texto", traduccion.Texto);
                                CM.Parameters.AddWithValue("@Pendiente", traduccion.Pendiente);
                                CM.Parameters.AddWithValue("@CodigoTraduccion", traduccion.CodigoTraduccion);
                                CM.Parameters.AddWithValue("@CodigoIdioma", pCodigoIdioma);
                                CM.ExecuteNonQuery();
                            }
                        }
                        TR.Commit();
                    }
                    catch
                    {
                        TR.Rollback();
                        throw;
                    }
                }
            }
        }
        public List<int> Agregar(List<Traduccion> pTraducciones)
        {
            using (SqlConnection CO = acceso.NuevaConexion())
            {
                CO.Open();
                using (SqlTransaction TR = CO.BeginTransaction())
                {
                    try
                    {
                        List<int> codigos = Insertar(pTraducciones, CO, TR);
                        TR.Commit();
                        return codigos;
                    }
                    catch
                    {
                        TR.Rollback();
                        throw;
                    }
                }
            }
        }
        internal static List<int> Insertar(List<Traduccion> pTraducciones, SqlConnection CO, SqlTransaction TR)
        {
            List<int> codigos = new List<int>();
            string query = @"INSERT INTO Traduccion (CodigoIdioma, Clave, Modulo, Texto, Pendiente)
                             VALUES (@CodigoIdioma, @Clave, @Modulo, @Texto, @Pendiente);
                             SELECT CAST(SCOPE_IDENTITY() AS INT);";
            foreach (Traduccion traduccion in pTraducciones)
            {
                using (SqlCommand CM = new SqlCommand(query, CO, TR))
                {
                    CM.Parameters.AddWithValue("@CodigoIdioma", traduccion.CodigoIdioma);
                    CM.Parameters.AddWithValue("@Clave", traduccion.Clave);
                    CM.Parameters.AddWithValue("@Modulo", traduccion.Modulo);
                    CM.Parameters.AddWithValue("@Texto", traduccion.Texto);
                    CM.Parameters.AddWithValue("@Pendiente", traduccion.Pendiente);
                    codigos.Add(Convert.ToInt32(CM.ExecuteScalar()));
                }
            }
            return codigos;
        }
    }
}
