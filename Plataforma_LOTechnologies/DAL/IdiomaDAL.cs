using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;

namespace DAL
{
    public class IdiomaDAL
    {
        private AccesoDatos acceso = new AccesoDatos();
        public List<Idioma> ListarIdiomas()
        {
            List<Idioma> idiomas = new List<Idioma>();
            string query = "SELECT CodigoIdioma, Nombre, NombreNativo, Codigo, Predeterminado, Estado FROM Idioma ORDER BY Predeterminado DESC, Nombre";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    while (DR.Read())
                    {
                        idiomas.Add(new Idioma(
                            Convert.ToInt32(DR["CodigoIdioma"]),
                            DR["Nombre"].ToString(),
                            DR["NombreNativo"].ToString(),
                            DR["Codigo"].ToString(),
                            Convert.ToBoolean(DR["Predeterminado"]),
                            Convert.ToBoolean(DR["Estado"])));
                    }
                }
            }
            return idiomas;
        }
    }
}
