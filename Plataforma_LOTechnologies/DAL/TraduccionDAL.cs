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
