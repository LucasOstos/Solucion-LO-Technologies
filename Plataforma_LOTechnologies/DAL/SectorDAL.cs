using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;

namespace DAL
{
    public class SectorDAL
    {
        private AccesoDatos acceso = new AccesoDatos();

        public List<SectorListado> ObtenerSectoresPorLocal(int pCodigoLocal)
        {
            List<SectorListado> sectores = new List<SectorListado>();
            string query = @"SELECT s.CodigoSector, s.CodigoLocal, s.Nombre, s.Tipo, s.Ubicacion, s.Circulacion, s.Descripcion, s.Estado,
                            (SELECT COUNT(*) FROM Producto p WHERE p.CodigoSector = s.CodigoSector) AS CantidadProductos FROM Sector s WHERE s.CodigoLocal = @CodigoLocal ORDER BY s.Nombre";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoLocal", pCodigoLocal);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    while (DR.Read())
                    {
                        sectores.Add(new SectorListado
                        {
                            CodigoSector = int.Parse(DR["CodigoSector"].ToString()),
                            CodigoLocal = int.Parse(DR["CodigoLocal"].ToString()),
                            Nombre = DR["Nombre"].ToString(),
                            Tipo = DR["Tipo"] == DBNull.Value ? "" : DR["Tipo"].ToString(),
                            Ubicacion = DR["Ubicacion"] == DBNull.Value ? "" : DR["Ubicacion"].ToString(),
                            Circulacion = DR["Circulacion"] == DBNull.Value ? (decimal?)null : decimal.Parse(DR["Circulacion"].ToString()),
                            Descripcion = DR["Descripcion"] == DBNull.Value ? "" : DR["Descripcion"].ToString(),
                            Estado = bool.Parse(DR["Estado"].ToString()),
                            CantidadProductos = int.Parse(DR["CantidadProductos"].ToString())
                        });
                    }
                }
            }
            return sectores;
        }
        public SectorBE ObtenerSectorPorCodigo(int pCodigoSector)
        {
            SectorBE sector = null;
            string query = @"SELECT CodigoSector, CodigoLocal, Nombre, Tipo, Ubicacion, Circulacion, Descripcion, Estado FROM Sector WHERE CodigoSector = @CodigoSector";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoSector", pCodigoSector);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    if (DR.Read())
                    {
                        sector = new SectorBE(int.Parse(DR["CodigoSector"].ToString()), int.Parse(DR["CodigoLocal"].ToString()), DR["Nombre"].ToString(),
                                          DR["Tipo"] == DBNull.Value ? "" : DR["Tipo"].ToString(), DR["Ubicacion"] == DBNull.Value ? "" : DR["Ubicacion"].ToString(),
                                          DR["Circulacion"] == DBNull.Value ? (decimal?)null : decimal.Parse(DR["Circulacion"].ToString()),
                                          DR["Descripcion"] == DBNull.Value ? "" : DR["Descripcion"].ToString(), bool.Parse(DR["Estado"].ToString()));                        
                    }
                }
            }
            return sector;
        }
        public int AgregarSector(SectorBE pSector)
        {
            string query = @"INSERT INTO Sector (CodigoLocal, Nombre, Tipo, Ubicacion, Circulacion, Descripcion, Estado)
                             VALUES (@CodigoLocal, @Nombre, @Tipo, @Ubicacion, @Circulacion, @Descripcion, @Estado);
                             SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoLocal", pSector.CodigoLocal);
                CM.Parameters.AddWithValue("@Nombre", pSector.Nombre);
                CM.Parameters.AddWithValue("@Tipo", (object)pSector.Tipo ?? DBNull.Value);
                CM.Parameters.AddWithValue("@Ubicacion", (object)pSector.Ubicacion ?? DBNull.Value);
                CM.Parameters.AddWithValue("@Circulacion", (object)pSector.Circulacion ?? DBNull.Value);
                CM.Parameters.AddWithValue("@Descripcion", (object)pSector.Descripcion ?? DBNull.Value);
                CM.Parameters.AddWithValue("@Estado", pSector.Estado);
                CO.Open();
                return (int)CM.ExecuteScalar();
            }
        }
        public void ModificarSector(SectorBE pSector)
        {
            string query = @"UPDATE Sector SET Nombre = @Nombre, Tipo = @Tipo, Ubicacion = @Ubicacion, Circulacion = @Circulacion, Descripcion = @Descripcion WHERE CodigoSector = @CodigoSector";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoSector", pSector.CodigoSector);
                CM.Parameters.AddWithValue("@Nombre", pSector.Nombre);
                CM.Parameters.AddWithValue("@Tipo", (object)pSector.Tipo ?? DBNull.Value);
                CM.Parameters.AddWithValue("@Ubicacion", (object)pSector.Ubicacion ?? DBNull.Value);
                CM.Parameters.AddWithValue("@Circulacion", (object)pSector.Circulacion ?? DBNull.Value);
                CM.Parameters.AddWithValue("@Descripcion", (object)pSector.Descripcion ?? DBNull.Value);
                CO.Open();
                CM.ExecuteNonQuery();
            }
        }
    }
}
