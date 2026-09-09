using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;

namespace DAL
{
    public class AnalisisProductoDAL
    {
        private AccesoDatos acceso = new AccesoDatos();
        public List<ProductoMetrica> ObtenerMetricasPorLocal(int pCodigoLocal, string pPeriodoDesde)
        {
            var metricas = new List<ProductoMetrica>();
            string query = @"SELECT p.CodigoProducto, p.Nombre AS NombreProducto, c.Nombre AS NombreCategoria,
                             s.CodigoSector AS CodigoSectorActual, s.Nombre AS NombreSectorActual, s.Circulacion, ISNULL((SELECT SUM(v.Cantidad) FROM Venta v
                             WHERE v.CodigoProducto = p.CodigoProducto AND v.CodigoLocal = @CodigoLocal AND v.Periodo >= @PeriodoDesde), 0) AS Ventas,
                             ISNULL((SELECT st.CantidadDisponible FROM Stock st WHERE st.CodigoProducto = p.CodigoProducto AND st.CodigoLocal = @CodigoLocal), 0) AS Stock
                             FROM Producto p INNER JOIN Categoria c ON p.CodigoCategoria = c.CodigoCategoria INNER JOIN Sector s ON p.CodigoSector = s.CodigoSector WHERE s.CodigoLocal = @CodigoLocal";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoLocal", pCodigoLocal);
                CM.Parameters.AddWithValue("@PeriodoDesde", pPeriodoDesde);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    while (DR.Read())
                    {
                        metricas.Add(new ProductoMetrica
                        {
                            CodigoProducto = int.Parse(DR["CodigoProducto"].ToString()),
                            NombreProducto = DR["NombreProducto"].ToString(),
                            NombreCategoria = DR["NombreCategoria"].ToString(),
                            CodigoSectorActual = int.Parse(DR["CodigoSectorActual"].ToString()),
                            NombreSectorActual = DR["NombreSectorActual"].ToString(),
                            CirculacionActual = DR["Circulacion"] == DBNull.Value ? 0 : decimal.Parse(DR["Circulacion"].ToString()),
                            Ventas = int.Parse(DR["Ventas"].ToString()),
                            Stock = int.Parse(DR["Stock"].ToString())
                        });
                    }
                }
            }
            return metricas;
        }
    }
}
