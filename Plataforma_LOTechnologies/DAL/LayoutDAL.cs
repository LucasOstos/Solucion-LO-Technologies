using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;

namespace DAL
{
    public class LayoutDAL
    {
        private AccesoDatos acceso = new AccesoDatos();
        public List<LayoutBE> ObtenerLayoutsPorLocal(int pCodigoLocal)
        {
            List<LayoutBE> layouts = new List<LayoutBE>();
            string query = @"SELECT CodigoLayout, CodigoLocal, Nombre, Descripcion, NombreArchivo, TamanioArchivo, Fecha, Estado FROM Layout WHERE CodigoLocal = @CodigoLocal ORDER BY Fecha DESC";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoLocal", pCodigoLocal);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    while (DR.Read())
                    {
                        LayoutBE layout = new LayoutBE(int.Parse(DR["CodigoLayout"].ToString()), int.Parse(DR["CodigoLocal"].ToString()), DR["Nombre"].ToString(), DR["Descripcion"].ToString(),
                                                       DR["NombreArchivo"].ToString(), DR["TamanioArchivo"].ToString(), Convert.ToDateTime(DR["Fecha"]), DR["Estado"].ToString());
                        layouts.Add(layout);
                    }
                }
            }
            return layouts;
        }
        public LayoutBE ObtenerLayoutPorCodigo(int pCodigoLayout)
        {
            LayoutBE layout = null;
            string query = @"SELECT CodigoLayout, CodigoLocal, Nombre, Descripcion, NombreArchivo, TamanioArchivo, Fecha, Estado FROM Layout WHERE CodigoLayout = @CodigoLayout";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoLayout", pCodigoLayout);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    if (DR.Read())
                    {
                        layout = new LayoutBE(int.Parse(DR["CodigoLayout"].ToString()), int.Parse(DR["CodigoLocal"].ToString()), DR["Nombre"].ToString(), DR["Descripcion"].ToString(),
                                                       DR["NombreArchivo"].ToString(), DR["TamanioArchivo"].ToString(), Convert.ToDateTime(DR["Fecha"]), DR["Estado"].ToString());
                    }
                }
            }
            return layout;
        }
        public int AgregarLayout(LayoutBE pLayout)
        {
            string query = @"INSERT INTO Layout (CodigoLocal, Nombre, Descripcion, NombreArchivo, TamanioArchivo, Fecha, Estado) VALUES (@CodigoLocal, @Nombre, @Descripcion, @NombreArchivo, @TamanioArchivo, @Fecha, @Estado);
                             SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoLocal", pLayout.CodigoLocal);
                CM.Parameters.AddWithValue("@Nombre", pLayout.Nombre);
                CM.Parameters.AddWithValue("@Descripcion", pLayout.Descripcion);
                CM.Parameters.AddWithValue("@NombreArchivo", pLayout.NombreArchivo);
                CM.Parameters.AddWithValue("@TamanioArchivo", pLayout.TamanioArchivo);
                CM.Parameters.AddWithValue("@Fecha", pLayout.Fecha);
                CM.Parameters.AddWithValue("@Estado", pLayout.Estado);
                CO.Open();
                return (int)CM.ExecuteScalar();
            }
        }
        public LayoutBE ObtenerLayoutVigente(int pCodigoLocal)
        {
            string query = @"SELECT TOP 1 CodigoLayout FROM Layout WHERE CodigoLocal = @CodigoLocal AND Estado = 'Activo' ORDER BY Fecha DESC, CodigoLayout DESC";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoLocal", pCodigoLocal);
                CO.Open();
                object codigo = CM.ExecuteScalar();
                return codigo == null || codigo == DBNull.Value ? null : ObtenerLayoutPorCodigo((int)codigo);
            }
        }
        public List<int> DesactivarOtros(int pCodigoLocal, int pCodigoLayoutVigente)
        {
            var codigos = new List<int>();
            string query = @"UPDATE Layout SET Estado = 'Inactivo' OUTPUT INSERTED.CodigoLayout WHERE CodigoLocal = @CodigoLocal AND Estado = 'Activo' AND CodigoLayout <> @CodigoLayout";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoLocal", pCodigoLocal);
                CM.Parameters.AddWithValue("@CodigoLayout", pCodigoLayoutVigente);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    while (DR.Read()) codigos.Add(DR.GetInt32(0));
                }
            }
            return codigos;
        }
        public void CambiarEstado(int pCodigoLayout, string pEstado)
        {
            string query = "UPDATE Layout SET Estado = @Estado WHERE CodigoLayout = @CodigoLayout";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@Estado", pEstado);
                CM.Parameters.AddWithValue("@CodigoLayout", pCodigoLayout);
                CO.Open();
                CM.ExecuteNonQuery();
            }
        }
        public void EliminarLayout(int pCodigoLayout)
        {
            string query = "DELETE FROM Layout WHERE CodigoLayout = @CodigoLayout";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoLayout", pCodigoLayout);
                CO.Open();
                CM.ExecuteNonQuery();
            }
        }
    }
}
