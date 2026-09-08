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
            string query = @"SELECT s.CodigoSector, s.Nombre AS Sector, s.Circulacion, ISNULL((SELECT SUM(v.ImporteTotal) FROM Venta v
                            INNER JOIN Producto p ON v.CodigoProducto = p.CodigoProducto WHERE p.CodigoSector = s.CodigoSector
                            AND v.CodigoLocal = @CodigoLocal AND v.Periodo >= @PeriodoDesde), 0) AS Ventas, ISNULL((SELECT SUM(v.Cantidad) FROM Venta v
                            INNER JOIN Producto p ON v.CodigoProducto = p.CodigoProducto WHERE p.CodigoSector = s.CodigoSector
                            AND v.CodigoLocal = @CodigoLocal AND v.Periodo >= @PeriodoDesde), 0) AS UnidadesVendidas, ISNULL((SELECT SUM(st.CantidadDisponible) FROM Stock st
                            INNER JOIN Producto p ON st.CodigoProducto = p.CodigoProducto
                            WHERE p.CodigoSector = s.CodigoSector AND st.CodigoLocal = @CodigoLocal), 0) AS StockDisponible FROM Sector s
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
                        decimal rotacion = stockDisponible > 0 ? Math.Min(Math.Round((unidadesVendidas / (decimal)stockDisponible) * 100, 1), 100) : 0;
                        int indiceGlobal = (int)Math.Round((circulacion + rotacion) / 2, 0);
                        indicadores.Add(new IndicadorSectorListado
                        {
                            CodigoSector = int.Parse(DR["CodigoSector"].ToString()),
                            Sector = DR["Sector"].ToString(),
                            Ventas = ventas,
                            Margen = 0,
                            Circulacion = circulacion,
                            Rotacion = rotacion,
                            IndiceGlobal = indiceGlobal,
                            ClaseIndiceGlobal = ClasificarIndice(indiceGlobal)
                        });
                    }
                }
            }
            return indicadores;
        }
        private string ClasificarIndice(int indice)
        {
            if (indice >= 80) return "etiqueta-activo";
            if (indice >= 60) return "etiqueta-bloqueado";
            return "etiqueta-inactivo";
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
