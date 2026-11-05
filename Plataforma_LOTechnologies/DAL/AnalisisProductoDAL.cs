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
        // Productos ACTIVOS de la empresa del local que están ubicados en el local, o que tuvieron ventas o tienen stock en él
        public List<ProductoMetrica> ObtenerMetricasPorLocal(int pCodigoLocal, DateTime pDesde, DateTime pHasta)
        {
            var metricas = new List<ProductoMetrica>();
            string query = @"SELECT p.CodigoProducto, p.NumeroSerie, p.Nombre AS NombreProducto, c.Nombre AS NombreCategoria, p.Costo,
                             s.CodigoSector AS CodigoSectorActual, s.Nombre AS NombreSectorActual, s.Circulacion,
                             ISNULL(v.Unidades, 0) AS Ventas, ISNULL(v.Importe, 0) AS Importe, ISNULL(st.CantidadDisponible, 0) AS Stock
                             FROM Producto p
                             INNER JOIN Categoria c ON p.CodigoCategoria = c.CodigoCategoria
                             INNER JOIN Local l ON l.CodigoLocal = @CodigoLocal AND l.CodigoEmpresa = p.CodigoEmpresa
                             LEFT JOIN ProductoUbicacion pu ON pu.CodigoProducto = p.CodigoProducto AND pu.CodigoLocal = @CodigoLocal
                             LEFT JOIN Sector s ON s.CodigoSector = pu.CodigoSector
                             LEFT JOIN (SELECT CodigoProducto, SUM(Cantidad) AS Unidades, SUM(ImporteTotal) AS Importe FROM Venta
                                        WHERE CodigoLocal = @CodigoLocal AND Fecha >= @Desde AND Fecha < @HastaExclusivo
                                        GROUP BY CodigoProducto) v ON v.CodigoProducto = p.CodigoProducto
                             LEFT JOIN Stock st ON st.CodigoProducto = p.CodigoProducto AND st.CodigoLocal = @CodigoLocal
                             WHERE p.Estado = 1 AND (pu.CodigoUbicacion IS NOT NULL OR v.Unidades IS NOT NULL OR st.CodigoStock IS NOT NULL)";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoLocal", pCodigoLocal);
                CM.Parameters.AddWithValue("@Desde", pDesde.Date);
                CM.Parameters.AddWithValue("@HastaExclusivo", pHasta.Date.AddDays(1));
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    while (DR.Read())
                    {
                        metricas.Add(new ProductoMetrica
                        {
                            CodigoProducto = int.Parse(DR["CodigoProducto"].ToString()),
                            NumeroSerie = int.Parse(DR["NumeroSerie"].ToString()),
                            NombreProducto = DR["NombreProducto"].ToString(),
                            NombreCategoria = DR["NombreCategoria"].ToString(),
                            Costo = DR["Costo"] == DBNull.Value ? (decimal?)null : decimal.Parse(DR["Costo"].ToString()),
                            CodigoSectorActual = DR["CodigoSectorActual"] == DBNull.Value ? (int?)null : int.Parse(DR["CodigoSectorActual"].ToString()),
                            NombreSectorActual = DR["NombreSectorActual"] == DBNull.Value ? null : DR["NombreSectorActual"].ToString(),
                            CirculacionActual = DR["Circulacion"] == DBNull.Value ? 0 : decimal.Parse(DR["Circulacion"].ToString()),
                            Ventas = int.Parse(DR["Ventas"].ToString()),
                            Importe = decimal.Parse(DR["Importe"].ToString()),
                            Stock = int.Parse(DR["Stock"].ToString())
                        });
                    }
                }
            }
            return metricas;
        }
    }
}
