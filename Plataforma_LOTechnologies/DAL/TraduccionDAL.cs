using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
