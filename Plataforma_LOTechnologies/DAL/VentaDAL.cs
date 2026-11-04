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
        public const string AGRUPAR_SECTOR = "sector";
        public const string AGRUPAR_CATEGORIA = "categoria";
        public const string AGRUPAR_PRODUCTO = "producto";
        // Ventas del período agrupadas por mes y por sector, categoría o producto.
        public List<VentaListado> ObtenerVentasAgrupadas(int pCodigoLocal, string pPeriodoDesde, string pAgrupacion)
        {
            List<VentaListado> ventas = new List<VentaListado>();
            string columnasGrupo;
            switch (pAgrupacion)
            {
                case AGRUPAR_CATEGORIA: columnasGrupo = "c.Nombre"; break;
                case AGRUPAR_PRODUCTO: columnasGrupo = "p.NumeroSerie, p.Nombre"; break;
                default: columnasGrupo = "s.Nombre"; pAgrupacion = AGRUPAR_SECTOR; break;
            }
            string query = $@"SELECT v.Periodo, {columnasGrupo}, SUM(v.Cantidad) AS Unidades, SUM(v.ImporteTotal) AS Monto,
                             SUM(CASE WHEN p.Costo IS NULL THEN 0 ELSE v.ImporteTotal END) AS VentasConCosto,
                             SUM(CASE WHEN p.Costo IS NULL THEN 0 ELSE v.Cantidad * p.Costo END) AS CostoVendido FROM Venta v
                             INNER JOIN Producto p ON v.CodigoProducto = p.CodigoProducto INNER JOIN Categoria c ON p.CodigoCategoria = c.CodigoCategoria
                             LEFT JOIN ProductoUbicacion pu ON pu.CodigoProducto = v.CodigoProducto AND pu.CodigoLocal = v.CodigoLocal
                             LEFT JOIN Sector s ON pu.CodigoSector = s.CodigoSector WHERE v.CodigoLocal = @CodigoLocal AND v.Periodo >= @PeriodoDesde
                             GROUP BY v.Periodo, {columnasGrupo} ORDER BY v.Periodo DESC, Monto DESC";
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
                        string grupo;
                        if (pAgrupacion == AGRUPAR_PRODUCTO) grupo = $"PROD-{int.Parse(DR["NumeroSerie"].ToString()):D3} - {DR["Nombre"]}";
                        else grupo = DR["Nombre"] == DBNull.Value ? null : DR["Nombre"].ToString(); // sector null = producto sin ubicar
                        ventas.Add(new VentaListado
                        {
                            Periodo = DR["Periodo"].ToString(),
                            Grupo = grupo,
                            Sector = pAgrupacion == AGRUPAR_SECTOR ? (grupo ?? "-") : null,
                            Categoria = pAgrupacion == AGRUPAR_CATEGORIA ? grupo : null,
                            Unidades = int.Parse(DR["Unidades"].ToString()),
                            Monto = decimal.Parse(DR["Monto"].ToString()),
                            Margen = IndicadorDAL.CalcularMargen(decimal.Parse(DR["VentasConCosto"].ToString()), decimal.Parse(DR["CostoVendido"].ToString()))
                        });
                    }
                }
            }
            return ventas;
        }
        //Margen del local en el período, sobre las ventas de productos con costo cargado (null = ninguno tiene costo)
        public decimal? ObtenerMargenTotal(int pCodigoLocal, string pPeriodoDesde)
        {
            string query = @"SELECT ISNULL(SUM(v.ImporteTotal), 0) AS VentasConCosto, ISNULL(SUM(v.Cantidad * p.Costo), 0) AS CostoVendido FROM Venta v
                             INNER JOIN Producto p ON p.CodigoProducto = v.CodigoProducto
                             WHERE v.CodigoLocal = @CodigoLocal AND v.Periodo >= @PeriodoDesde AND p.Costo IS NOT NULL";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoLocal", pCodigoLocal);
                CM.Parameters.AddWithValue("@PeriodoDesde", pPeriodoDesde);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    if (!DR.Read()) return null;
                    return IndicadorDAL.CalcularMargen(decimal.Parse(DR["VentasConCosto"].ToString()), decimal.Parse(DR["CostoVendido"].ToString()));
                }
            }
        }
        //Ventas del local mes por mes desde un período
        public Dictionary<string, decimal> ObtenerVentasPorMes(int pCodigoLocal, string pPeriodoDesde)
        {
            var meses = new Dictionary<string, decimal>();
            string query = @"SELECT Periodo, SUM(ImporteTotal) AS Monto FROM Venta
                             WHERE CodigoLocal = @CodigoLocal AND Periodo >= @PeriodoDesde GROUP BY Periodo";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoLocal", pCodigoLocal);
                CM.Parameters.AddWithValue("@PeriodoDesde", pPeriodoDesde);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    while (DR.Read()) meses[DR["Periodo"].ToString()] = decimal.Parse(DR["Monto"].ToString());
                }
            }
            return meses;
        }
        public decimal ObtenerTotalVentasEntreFechas(int pCodigoLocal, DateTime pDesde, DateTime pHasta)
        {
            string query = @"SELECT ISNULL(SUM(ImporteTotal), 0) FROM Venta
                             WHERE CodigoLocal = @CodigoLocal AND Fecha >= @Desde AND Fecha < @HastaExclusivo";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoLocal", pCodigoLocal);
                CM.Parameters.AddWithValue("@Desde", pDesde.Date);
                CM.Parameters.AddWithValue("@HastaExclusivo", pHasta.Date.AddDays(1));
                CO.Open();
                return Convert.ToDecimal(CM.ExecuteScalar());
            }
        }
        public decimal ObtenerTotalVentas(int pCodigoLocal, string pPeriodoDesde)
        {
            return ObtenerEscalarDecimal(pCodigoLocal, pPeriodoDesde, "SUM(ImporteTotal)");
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
        // Últimas ventas cargadas en el local (manuales o importadas), para poder corregir una carga equivocada
        public List<VentaDetalle> ObtenerUltimasVentas(int pCodigoLocal, int pCantidad)
        {
            var ventas = new List<VentaDetalle>();
            string query = @"SELECT TOP (@Cantidad) v.CodigoVenta, v.Fecha, p.NumeroSerie, p.Nombre, v.Cantidad, v.ImporteTotal, v.CodigoImportacion
                             FROM Venta v INNER JOIN Producto p ON p.CodigoProducto = v.CodigoProducto WHERE v.CodigoLocal = @CodigoLocal ORDER BY v.CodigoVenta DESC";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoLocal", pCodigoLocal);
                CM.Parameters.AddWithValue("@Cantidad", pCantidad);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    while (DR.Read())
                    {
                        ventas.Add(new VentaDetalle
                        {
                            CodigoVenta = int.Parse(DR["CodigoVenta"].ToString()),
                            Fecha = Convert.ToDateTime(DR["Fecha"]),
                            Producto = $"PROD-{int.Parse(DR["NumeroSerie"].ToString()):D3} - {DR["Nombre"]}",
                            Cantidad = int.Parse(DR["Cantidad"].ToString()),
                            ImporteTotal = decimal.Parse(DR["ImporteTotal"].ToString()),
                            CodigoImportacion = DR["CodigoImportacion"] == DBNull.Value ? (int?)null : int.Parse(DR["CodigoImportacion"].ToString())
                        });
                    }
                }
            }
            return ventas;
        }
        public void EliminarVenta(int pCodigoVenta)
        {
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand("DELETE FROM Venta WHERE CodigoVenta = @CodigoVenta", CO))
            {
                CM.Parameters.AddWithValue("@CodigoVenta", pCodigoVenta);
                CO.Open();
                CM.ExecuteNonQuery();
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
