using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;

namespace DAL
{
    public class VentaDAL
    {
        private AccesoDatos acceso = new AccesoDatos();

        public List<VentaListado> ObtenerVentasAgrupadas(int pCodigoLocal, string pPeriodoDesde)
        {
            List<VentaListado> ventas = new List<VentaListado>();
            string query = @"SELECT v.Periodo, s.Nombre AS Sector, c.Nombre AS Categoria, SUM(v.Cantidad) AS Unidades, SUM(v.ImporteTotal) AS Monto FROM Venta v
                             INNER JOIN Producto p ON v.CodigoProducto = p.CodigoProducto INNER JOIN Categoria c ON p.CodigoCategoria = c.CodigoCategoria
                             LEFT JOIN Sector s ON p.CodigoSector = s.CodigoSector WHERE v.CodigoLocal = @CodigoLocal AND v.Periodo >= @PeriodoDesde
                             GROUP BY v.Periodo, s.Nombre, c.Nombre ORDER BY v.Periodo DESC";
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
                        ventas.Add(new VentaListado
                        {
                            Periodo = DR["Periodo"].ToString(),
                            Sector = DR["Sector"] == DBNull.Value ? "-" : DR["Sector"].ToString(),
                            Categoria = DR["Categoria"].ToString(),
                            Unidades = int.Parse(DR["Unidades"].ToString()),
                            Monto = decimal.Parse(DR["Monto"].ToString()),
                            Margen = 0
                        });
                    }
                }
            }
            return ventas;
        }
        public decimal ObtenerTotalVentas(int pCodigoLocal, string pPeriodoDesde)
        {
            return ObtenerEscalarDecimal(pCodigoLocal, pPeriodoDesde, "SUM(ImporteTotal)");
        }
        public decimal ObtenerTicketPromedio(int pCodigoLocal, string pPeriodoDesde)
        {
            string query = @"SELECT AVG(ImporteTotal) FROM Venta WHERE CodigoLocal = @CodigoLocal AND Periodo >= @PeriodoDesde";
            return EjecutarEscalarDecimal(query, pCodigoLocal, pPeriodoDesde);
        }
        public int ObtenerUnidadesVendidas(int pCodigoLocal, string pPeriodoDesde)
        {
            string query = @"SELECT ISNULL(SUM(Cantidad), 0) FROM Venta WHERE CodigoLocal = @CodigoLocal AND Periodo >= @PeriodoDesde";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoLocal", pCodigoLocal);
                CM.Parameters.AddWithValue("@PeriodoDesde", pPeriodoDesde);
                CO.Open();
                object resultado = CM.ExecuteScalar();
                return resultado == DBNull.Value ? 0 : Convert.ToInt32(resultado);
            }
        }
        private decimal ObtenerEscalarDecimal(int pCodigoLocal, string pPeriodoDesde, string pAgregacion)
        {
            string query = $@"SELECT ISNULL({pAgregacion}, 0) FROM Venta WHERE CodigoLocal = @CodigoLocal AND Periodo >= @PeriodoDesde";
            return EjecutarEscalarDecimal(query, pCodigoLocal, pPeriodoDesde);
        }
        private decimal EjecutarEscalarDecimal(string query, int pCodigoLocal, string pPeriodoDesde)
        {
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoLocal", pCodigoLocal);
                CM.Parameters.AddWithValue("@PeriodoDesde", pPeriodoDesde);
                CO.Open();
                object resultado = CM.ExecuteScalar();
                return resultado == DBNull.Value ? 0 : Convert.ToDecimal(resultado);
            }
        }
        public int AgregarVenta(VentaBE pVenta)
        {
            string query = @"INSERT INTO Venta (CodigoLocal, CodigoProducto, Fecha, Cantidad, PrecioUnitario, ImporteTotal, Periodo)
                             VALUES (@CodigoLocal, @CodigoProducto, @Fecha, @Cantidad, @PrecioUnitario, @ImporteTotal, @Periodo);
                             SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoLocal", pVenta.CodigoLocal);
                CM.Parameters.AddWithValue("@CodigoProducto", pVenta.CodigoProducto);
                CM.Parameters.AddWithValue("@Fecha", pVenta.Fecha);
                CM.Parameters.AddWithValue("@Cantidad", pVenta.Cantidad);
                CM.Parameters.AddWithValue("@PrecioUnitario", pVenta.PrecioUnitario);
                CM.Parameters.AddWithValue("@ImporteTotal", pVenta.ImporteTotal);
                CM.Parameters.AddWithValue("@Periodo", pVenta.Periodo);
                CO.Open();
                return (int)CM.ExecuteScalar();
            }
        }
    }
}
