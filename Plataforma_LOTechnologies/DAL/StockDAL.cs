using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;

namespace DAL
{
    public class StockDAL
    {
        private AccesoDatos acceso = new AccesoDatos();
        public List<StockListado> ObtenerStockPorLocal(int pCodigoLocal)
        {
            List<StockListado> stocks = new List<StockListado>();
            string query = @"SELECT st.CodigoStock, st.CodigoProducto, p.Nombre AS Producto, c.Nombre AS Categoria, st.CantidadDisponible, st.Minimo, st.Maximo FROM Stock st
                              INNER JOIN Producto p ON st.CodigoProducto = p.CodigoProducto
                              INNER JOIN Categoria c ON p.CodigoCategoria = c.CodigoCategoria
                              WHERE st.CodigoLocal = @CodigoLocal ORDER BY p.Nombre";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoLocal", pCodigoLocal);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    while (DR.Read())
                    {
                        int cantidad = int.Parse(DR["CantidadDisponible"].ToString());
                        int? minimo = DR["Minimo"] == DBNull.Value ? (int?)null : int.Parse(DR["Minimo"].ToString());
                        int? maximo = DR["Maximo"] == DBNull.Value ? (int?)null : int.Parse(DR["Maximo"].ToString());
                        stocks.Add(new StockListado
                        {
                            CodigoStock = int.Parse(DR["CodigoStock"].ToString()),
                            CodigoProducto = int.Parse(DR["CodigoProducto"].ToString()),
                            Producto = DR["Producto"].ToString(),
                            Categoria = DR["Categoria"].ToString(),
                            CantidadDisponible = cantidad,
                            Minimo = minimo,
                            Maximo = maximo,
                            EstadoStock = CalcularEstadoStock(cantidad, minimo),
                            ClaseEstadoStock = CalcularClaseEstadoStock(cantidad, minimo),
                            PorcentajeStock = CalcularPorcentaje(cantidad, maximo)
                        });
                    }
                }
            }
            return stocks;
        }
        private string CalcularEstadoStock(int cantidad, int? minimo)
        {
            if (minimo == null) return "Normal";
            if (cantidad <= 0) return "Crítico";
            if (cantidad <= minimo) return "Bajo";
            return "Normal";
        }
        private string CalcularClaseEstadoStock(int cantidad, int? minimo)
        {
            string estado = CalcularEstadoStock(cantidad, minimo);
            switch (estado)
            {
                case "Crítico": return "etiqueta-inactivo";
                case "Bajo": return "etiqueta-bloqueado";
                default: return "etiqueta-activo";
            }
        }
        private int CalcularPorcentaje(int cantidad, int? maximo)
        {
            if (maximo == null || maximo == 0) return 100;
            int porcentaje = (int)((cantidad / (decimal)maximo) * 100);
            return Math.Min(porcentaje, 100);
        }
        public StockBE ObtenerStockPorCodigo(int pCodigoStock)
        {
            StockBE stock = null;
            string query = @"SELECT CodigoStock, CodigoLocal, CodigoProducto, CantidadDisponible, Minimo, Maximo, Periodo, FechaActualizacion FROM Stock WHERE CodigoStock = @CodigoStock";

            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoStock", pCodigoStock);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    if (DR.Read())
                    {
                        stock = new StockBE(int.Parse(DR["CodigoStock"].ToString()), int.Parse(DR["CodigoLocal"].ToString()), int.Parse(DR["CodigoProducto"].ToString()),
                                            int.Parse(DR["CantidadDisponible"].ToString()), DR["Minimo"] == DBNull.Value ? (int?)null : int.Parse(DR["Minimo"].ToString()),
                                            DR["Maximo"] == DBNull.Value ? (int?)null : int.Parse(DR["Maximo"].ToString()), DR["Periodo"].ToString(), Convert.ToDateTime(DR["FechaActualizacion"]));                        
                    }
                }
            }
            return stock;
        }
        public StockBE ObtenerStockPorProducto(int pCodigoLocal, int pCodigoProducto)
        {
            StockBE stock = null;
            string query = @"SELECT CodigoStock, CodigoLocal, CodigoProducto, CantidadDisponible, Minimo, Maximo, Periodo, FechaActualizacion
                             FROM Stock WHERE CodigoLocal = @CodigoLocal AND CodigoProducto = @CodigoProducto";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoLocal", pCodigoLocal);
                CM.Parameters.AddWithValue("@CodigoProducto", pCodigoProducto);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    if (DR.Read())
                    {
                        stock = new StockBE(int.Parse(DR["CodigoStock"].ToString()), int.Parse(DR["CodigoLocal"].ToString()), int.Parse(DR["CodigoProducto"].ToString()),
                                            int.Parse(DR["CantidadDisponible"].ToString()), DR["Minimo"] == DBNull.Value ? (int?)null : int.Parse(DR["Minimo"].ToString()),
                                            DR["Maximo"] == DBNull.Value ? (int?)null : int.Parse(DR["Maximo"].ToString()), DR["Periodo"].ToString(), Convert.ToDateTime(DR["FechaActualizacion"]));
                    }
                }
            }
            return stock;
        }
        public int AgregarStock(StockBE pStock)
        {
            string query = @"INSERT INTO Stock (CodigoLocal, CodigoProducto, CantidadDisponible, Minimo, Maximo, Periodo, FechaActualizacion)
                             VALUES (@CodigoLocal, @CodigoProducto, @CantidadDisponible, @Minimo, @Maximo, @Periodo, @FechaActualizacion);
                             SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoLocal", pStock.CodigoLocal);
                CM.Parameters.AddWithValue("@CodigoProducto", pStock.CodigoProducto);
                CM.Parameters.AddWithValue("@CantidadDisponible", pStock.CantidadDisponible);
                CM.Parameters.AddWithValue("@Minimo", (object)pStock.Minimo ?? DBNull.Value);
                CM.Parameters.AddWithValue("@Maximo", (object)pStock.Maximo ?? DBNull.Value);
                CM.Parameters.AddWithValue("@Periodo", pStock.Periodo);
                CM.Parameters.AddWithValue("@FechaActualizacion", pStock.FechaActualizacion);
                CO.Open();
                return (int)CM.ExecuteScalar();
            }
        }
        public void ActualizarStock(StockBE pStock)
        {
            string query = @"UPDATE Stock SET CantidadDisponible = @CantidadDisponible, Minimo = @Minimo, Maximo = @Maximo, Periodo = @Periodo, FechaActualizacion = @FechaActualizacion
                             WHERE CodigoStock = @CodigoStock";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoStock", pStock.CodigoStock);
                CM.Parameters.AddWithValue("@CantidadDisponible", pStock.CantidadDisponible);
                CM.Parameters.AddWithValue("@Minimo", (object)pStock.Minimo ?? DBNull.Value);
                CM.Parameters.AddWithValue("@Maximo", (object)pStock.Maximo ?? DBNull.Value);
                CM.Parameters.AddWithValue("@Periodo", pStock.Periodo);
                CM.Parameters.AddWithValue("@FechaActualizacion", pStock.FechaActualizacion);
                CO.Open();
                CM.ExecuteNonQuery();
            }
        }
    }
}
