using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;

namespace DAL
{
    public class IndicadorDAL
    {
        private AccesoDatos acceso = new AccesoDatos();
        public List<IndicadorSectorListado> CalcularIndicadoresPorSector(int pCodigoLocal, string pPeriodoDesde)
        {
            var indicadores = new List<IndicadorSectorListado>();
            string query = @"SELECT s.CodigoSector, s.Nombre AS Sector, s.Circulacion, s.Superficie, ISNULL((SELECT SUM(v.ImporteTotal) FROM Venta v
                            INNER JOIN ProductoUbicacion pu ON pu.CodigoProducto = v.CodigoProducto AND pu.CodigoLocal = v.CodigoLocal WHERE pu.CodigoSector = s.CodigoSector
                            AND v.CodigoLocal = @CodigoLocal AND v.Periodo >= @PeriodoDesde), 0) AS Ventas, ISNULL((SELECT SUM(v.Cantidad) FROM Venta v
                            INNER JOIN ProductoUbicacion pu ON pu.CodigoProducto = v.CodigoProducto AND pu.CodigoLocal = v.CodigoLocal WHERE pu.CodigoSector = s.CodigoSector
                            AND v.CodigoLocal = @CodigoLocal AND v.Periodo >= @PeriodoDesde), 0) AS UnidadesVendidas, ISNULL((SELECT SUM(st.CantidadDisponible) FROM Stock st
                            INNER JOIN ProductoUbicacion pu ON pu.CodigoProducto = st.CodigoProducto AND pu.CodigoLocal = st.CodigoLocal
                            WHERE pu.CodigoSector = s.CodigoSector AND st.CodigoLocal = @CodigoLocal), 0) AS StockDisponible,
                            ISNULL((SELECT SUM(v.ImporteTotal) FROM Venta v INNER JOIN ProductoUbicacion pu ON pu.CodigoProducto = v.CodigoProducto AND pu.CodigoLocal = v.CodigoLocal
                            INNER JOIN Producto p ON p.CodigoProducto = v.CodigoProducto WHERE pu.CodigoSector = s.CodigoSector AND p.Costo IS NOT NULL
                            AND v.CodigoLocal = @CodigoLocal AND v.Periodo >= @PeriodoDesde), 0) AS VentasConCosto,
                            ISNULL((SELECT SUM(v.Cantidad * p.Costo) FROM Venta v INNER JOIN ProductoUbicacion pu ON pu.CodigoProducto = v.CodigoProducto AND pu.CodigoLocal = v.CodigoLocal
                            INNER JOIN Producto p ON p.CodigoProducto = v.CodigoProducto WHERE pu.CodigoSector = s.CodigoSector AND p.Costo IS NOT NULL
                            AND v.CodigoLocal = @CodigoLocal AND v.Periodo >= @PeriodoDesde), 0) AS CostoVendido FROM Sector s
                            WHERE s.CodigoLocal = @CodigoLocal";
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
                        decimal ventas = decimal.Parse(DR["Ventas"].ToString());
                        int unidadesVendidas = int.Parse(DR["UnidadesVendidas"].ToString());
                        int stockDisponible = int.Parse(DR["StockDisponible"].ToString());
                        decimal circulacion = DR["Circulacion"] == DBNull.Value ? 0 : decimal.Parse(DR["Circulacion"].ToString());
                        decimal? superficie = DR["Superficie"] == DBNull.Value ? (decimal?)null : decimal.Parse(DR["Superficie"].ToString());
                        decimal ventasConCosto = decimal.Parse(DR["VentasConCosto"].ToString());
                        decimal costoVendido = decimal.Parse(DR["CostoVendido"].ToString());
                        indicadores.Add(new IndicadorSectorListado
                        {
                            CodigoSector = int.Parse(DR["CodigoSector"].ToString()),
                            Sector = DR["Sector"].ToString(),
                            Ventas = ventas,
                            Margen = CalcularMargen(ventasConCosto, costoVendido),
                            Superficie = superficie,
                            VentasPorM2 = CalcularVentasPorM2(ventas, superficie),
                            Circulacion = circulacion,
                            UnidadesVendidas = unidadesVendidas,
                            StockDisponible = stockDisponible
                        });
                    }
                }
            }
            return indicadores;
        }
        public static decimal? CalcularMargen(decimal pVentasConCosto, decimal pCostoVendido)
        {
            if (pVentasConCosto <= 0) return null;
            return Math.Round((pVentasConCosto - pCostoVendido) / pVentasConCosto * 100, 1);
        }
        public static decimal? CalcularVentasPorM2(decimal pVentas, decimal? pSuperficie)
        {
            if (pSuperficie == null || pSuperficie <= 0) return null;
            return Math.Round(pVentas / pSuperficie.Value, 0);
        }
        // Ventas por sector entre dos fechas (para comparar con el mismo período anterior)
        public Dictionary<int, decimal> ObtenerVentasPorSectorEntreFechas(int pCodigoLocal, DateTime pDesde, DateTime pHasta)
        {
            var ventas = new Dictionary<int, decimal>();
            string query = @"SELECT pu.CodigoSector, SUM(v.ImporteTotal) AS Ventas FROM Venta v
                             INNER JOIN ProductoUbicacion pu ON pu.CodigoProducto = v.CodigoProducto AND pu.CodigoLocal = v.CodigoLocal
                             WHERE v.CodigoLocal = @CodigoLocal AND v.Fecha >= @Desde AND v.Fecha < @HastaExclusivo
                             GROUP BY pu.CodigoSector";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoLocal", pCodigoLocal);
                CM.Parameters.AddWithValue("@Desde", pDesde.Date);
                CM.Parameters.AddWithValue("@HastaExclusivo", pHasta.Date.AddDays(1));
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    while (DR.Read()) ventas[int.Parse(DR["CodigoSector"].ToString())] = decimal.Parse(DR["Ventas"].ToString());
                }
            }
            return ventas;
        }
        public List<IndicadorBE> ObtenerIndicadoresDelPeriodo(int pCodigoLocal, string pPeriodo)
        {
            var indicadores = new List<IndicadorBE>();
            string query = @"SELECT CodigoIndicador, CodigoLocal, CodigoSector, TipoIndicador, Nombre, Fecha, Periodo, Valor, UnidadMedida FROM Indicador
                             WHERE CodigoLocal = @CodigoLocal AND Periodo = @Periodo AND CodigoEscenario IS NULL AND CodigoProducto IS NULL";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoLocal", pCodigoLocal);
                CM.Parameters.AddWithValue("@Periodo", pPeriodo);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    while (DR.Read())
                    {
                        indicadores.Add(new IndicadorBE
                        {
                            CodigoIndicador = int.Parse(DR["CodigoIndicador"].ToString()),
                            CodigoLocal = int.Parse(DR["CodigoLocal"].ToString()),
                            CodigoSector = DR["CodigoSector"] == DBNull.Value ? (int?)null : int.Parse(DR["CodigoSector"].ToString()),
                            TipoIndicador = DR["TipoIndicador"].ToString(),
                            Nombre = DR["Nombre"].ToString(),
                            Fecha = Convert.ToDateTime(DR["Fecha"]),
                            Periodo = DR["Periodo"].ToString(),
                            Valor = decimal.Parse(DR["Valor"].ToString()),
                            UnidadMedida = DR["UnidadMedida"] == DBNull.Value ? null : DR["UnidadMedida"].ToString()
                        });
                    }
                }
            }
            return indicadores;
        }
        public void ActualizarIndicador(int pCodigoIndicador, decimal pValor, DateTime pFecha)
        {
            string query = "UPDATE Indicador SET Valor = @Valor, Fecha = @Fecha WHERE CodigoIndicador = @CodigoIndicador";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoIndicador", pCodigoIndicador);
                CM.Parameters.AddWithValue("@Valor", pValor);
                CM.Parameters.AddWithValue("@Fecha", pFecha);
                CO.Open();
                CM.ExecuteNonQuery();
            }
        }
        public int AgregarIndicador(IndicadorBE pIndicador)
        {
            string query = @"INSERT INTO Indicador (CodigoLocal, CodigoSector, CodigoProducto, CodigoEscenario, Nombre, TipoIndicador, Fecha, Periodo, Valor, UnidadMedida)
                              VALUES (@CodigoLocal, @CodigoSector, @CodigoProducto, @CodigoEscenario, @Nombre, @TipoIndicador, @Fecha, @Periodo, @Valor, @UnidadMedida);
                              SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoLocal", pIndicador.CodigoLocal);
                CM.Parameters.AddWithValue("@CodigoSector", (object)pIndicador.CodigoSector ?? DBNull.Value);
                CM.Parameters.AddWithValue("@CodigoProducto", (object)pIndicador.CodigoProducto ?? DBNull.Value);
                CM.Parameters.AddWithValue("@CodigoEscenario", (object)pIndicador.CodigoEscenario ?? DBNull.Value);
                CM.Parameters.AddWithValue("@Nombre", pIndicador.Nombre);
                CM.Parameters.AddWithValue("@TipoIndicador", pIndicador.TipoIndicador);
                CM.Parameters.AddWithValue("@Fecha", pIndicador.Fecha);
                CM.Parameters.AddWithValue("@Periodo", pIndicador.Periodo);
                CM.Parameters.AddWithValue("@Valor", pIndicador.Valor);
                CM.Parameters.AddWithValue("@UnidadMedida", (object)pIndicador.UnidadMedida ?? DBNull.Value);
                CO.Open();
                return (int)CM.ExecuteScalar();
            }
        }
    }
}
