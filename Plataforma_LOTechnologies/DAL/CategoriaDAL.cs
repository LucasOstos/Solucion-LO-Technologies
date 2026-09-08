using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;

namespace DAL
{
    public class CategoriaDAL
    {
        private AccesoDatos acceso = new AccesoDatos();
        public List<CategoriaBE> ObtenerCategorias()
        {
            List<CategoriaBE> categorias = new List<CategoriaBE>();
            string query = "SELECT CodigoCategoria, Nombre, Descripcion, Estado FROM Categoria WHERE Estado = 1 ORDER BY Nombre";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    while (DR.Read())
                    {
                        CategoriaBE categoria = new CategoriaBE(int.Parse(DR["CodigoCategoria"].ToString()), DR["Nombre"].ToString(), DR["Descripcion"] == DBNull.Value ? "" : DR["Descripcion"].ToString(),
                                                                bool.Parse(DR["Estado"].ToString()));
                        categorias.Add(categoria);
                    }
                }
            }
            return categorias;
        }
        public CategoriaBE ObtenerCategoriaPorCodigo(int pCodigoCategoria)
        {
            CategoriaBE categoria = null;
            string query = "SELECT CodigoCategoria, Nombre, Descripcion, Estado FROM Categoria WHERE CodigoCategoria = @CodigoCategoria";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoCategoria", pCodigoCategoria);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    if (DR.Read())
                    {
                        categoria = new CategoriaBE(int.Parse(DR["CodigoCategoria"].ToString()), DR["Nombre"].ToString(), DR["Descripcion"] == DBNull.Value ? "" : DR["Descripcion"].ToString(), 
                                                    bool.Parse(DR["Estado"].ToString()));
                    }
                }
            }
            return categoria;
        }
        public int AgregarCategoria(CategoriaBE pCategoria)
        {
            string query = @"INSERT INTO Categoria (Nombre, Descripcion, Estado) VALUES (@Nombre, @Descripcion, @Estado); SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@Nombre", pCategoria.Nombre);
                CM.Parameters.AddWithValue("@Descripcion", (object)pCategoria.Descripcion ?? DBNull.Value);
                CM.Parameters.AddWithValue("@Estado", pCategoria.Estado);
                CO.Open();
                return (int)CM.ExecuteScalar();
            }
        }
        public void ModificarCategoria(CategoriaBE pCategoria)
        {
            string query = "UPDATE Categoria SET Nombre = @Nombre, Descripcion = @Descripcion WHERE CodigoCategoria = @CodigoCategoria";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoCategoria", pCategoria.CodigoCategoria);
                CM.Parameters.AddWithValue("@Nombre", pCategoria.Nombre);
                CM.Parameters.AddWithValue("@Descripcion", (object)pCategoria.Descripcion ?? DBNull.Value);
                CO.Open();
                CM.ExecuteNonQuery();
            }
        }
    }
}
