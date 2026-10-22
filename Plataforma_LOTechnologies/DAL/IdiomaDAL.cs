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
                    while (DR.Read()) idiomas.Add(ArmarIdioma(DR));
                }
            }
            return idiomas;
        }
        public List<Idioma> ListarIdiomasConCompletitud()
        {
            List<Idioma> idiomas = new List<Idioma>();
            string query = @"SELECT i.CodigoIdioma, i.Nombre, i.NombreNativo, i.Codigo, i.Predeterminado, i.Estado,
                             (SELECT COUNT(*) FROM Traduccion t WHERE t.CodigoIdioma = i.CodigoIdioma AND t.Pendiente = 0) AS Revisadas,
                             (SELECT COUNT(*) FROM Traduccion t INNER JOIN Idioma p ON t.CodigoIdioma = p.CodigoIdioma WHERE p.Predeterminado = 1) AS Total FROM Idioma i
                             ORDER BY i.Predeterminado DESC, i.Nombre";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    while (DR.Read())
                    {
                        Idioma idioma = ArmarIdioma(DR);
                        int revisadas = Convert.ToInt32(DR["Revisadas"]);
                        int total = Convert.ToInt32(DR["Total"]);
                        idioma.Completitud = total == 0 ? 0 : Math.Min(100, revisadas * 100 / total);
                        idiomas.Add(idioma);
                    }
                }
            }
            return idiomas;
        }
        public int AgregarConTraducciones(Idioma pIdioma, List<Traduccion> pTraducciones, out List<int> pCodigosTraduccion)
        {
            pCodigosTraduccion = new List<int>();
            string queryIdioma = @"INSERT INTO Idioma (Nombre, NombreNativo, Codigo, Predeterminado, Estado) VALUES (@Nombre, @NombreNativo, @Codigo, 0, @Estado); SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using (SqlConnection CO = acceso.NuevaConexion())
            {
                CO.Open();
                using (SqlTransaction TR = CO.BeginTransaction())
                {
                    try
                    {
                        int codigoIdioma;
                        using (SqlCommand CM = new SqlCommand(queryIdioma, CO, TR))
                        {
                            CM.Parameters.AddWithValue("@Nombre", pIdioma.Nombre);
                            CM.Parameters.AddWithValue("@NombreNativo", pIdioma.NombreNativo);
                            CM.Parameters.AddWithValue("@Codigo", pIdioma.Codigo);
                            CM.Parameters.AddWithValue("@Estado", pIdioma.Estado);
                            codigoIdioma = Convert.ToInt32(CM.ExecuteScalar());
                        }
                        foreach (Traduccion traduccion in pTraducciones) traduccion.CodigoIdioma = codigoIdioma;
                        pCodigosTraduccion = TraduccionDAL.Insertar(pTraducciones, CO, TR);
                        TR.Commit();
                        return codigoIdioma;
                    }
                    catch
                    {
                        TR.Rollback();
                        pCodigosTraduccion = new List<int>();
                        throw;
                    }
                }
            }
        }
        public void CambiarEstado(int pCodigoIdioma, bool pEstado)
        {
            string query = "UPDATE Idioma SET Estado = @Estado WHERE CodigoIdioma = @CodigoIdioma";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@Estado", pEstado);
                CM.Parameters.AddWithValue("@CodigoIdioma", pCodigoIdioma);
                CO.Open();
                CM.ExecuteNonQuery();
            }
        }
        public int ContarUsuarios(int pCodigoIdioma)
        {
            string query = "SELECT COUNT(1) FROM Usuario WHERE UsuarioIdioma = @CodigoIdioma";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoIdioma", pCodigoIdioma);
                CO.Open();
                return Convert.ToInt32(CM.ExecuteScalar());
            }
        }
        private Idioma ArmarIdioma(SqlDataReader DR)
        {
            return new Idioma(
                Convert.ToInt32(DR["CodigoIdioma"]),
                DR["Nombre"].ToString(),
                DR["NombreNativo"].ToString(),
                DR["Codigo"].ToString(),
                Convert.ToBoolean(DR["Predeterminado"]),
                Convert.ToBoolean(DR["Estado"]));
        }
    }
}
