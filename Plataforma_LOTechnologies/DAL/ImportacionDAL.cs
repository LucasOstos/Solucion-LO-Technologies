using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;

namespace DAL
{
    public class ImportacionDAL
    {
        private AccesoDatos acceso = new AccesoDatos();
        public Dictionary<int, ProductoImportable> ObtenerProductosImportables(int pCodigoEmpresa, int pCodigoLocal)
        {
            var productos = new Dictionary<int, ProductoImportable>();
            string query = @"SELECT p.CodigoProducto, p.NumeroSerie, p.Estado, p.Precio,
                             CASE WHEN EXISTS (SELECT 1 FROM ProductoUbicacion pu WHERE pu.CodigoProducto = p.CodigoProducto AND pu.CodigoLocal = @CodigoLocal)
                             THEN 1 ELSE 0 END AS Ubicado FROM Producto p WHERE p.CodigoEmpresa = @CodigoEmpresa";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoEmpresa", pCodigoEmpresa);
                CM.Parameters.AddWithValue("@CodigoLocal", pCodigoLocal);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    while (DR.Read())
                    {
                        var producto = new ProductoImportable
                        {
                            CodigoProducto = int.Parse(DR["CodigoProducto"].ToString()),
                            NumeroSerie = int.Parse(DR["NumeroSerie"].ToString()),
                            Activo = bool.Parse(DR["Estado"].ToString()),
                            Precio = DR["Precio"] == DBNull.Value ? (decimal?)null : decimal.Parse(DR["Precio"].ToString()),
                            UbicadoEnLocal = int.Parse(DR["Ubicado"].ToString()) == 1
                        };
                        productos[producto.NumeroSerie] = producto;
                    }
                }
            }
            return productos;
        }
        //Registra la importación y todas sus ventas en una sola transacción
        public int RegistrarVentas(ImportacionBE pImportacion, List<VentaBE> pVentas, List<int> pCodigosVenta)
        {
            string queryVenta = @"INSERT INTO Venta (CodigoLocal, CodigoProducto, Fecha, Cantidad, PrecioUnitario, ImporteTotal, Periodo, CodigoImportacion)
                                  VALUES (@CodigoLocal, @CodigoProducto, @Fecha, @Cantidad, @PrecioUnitario, @ImporteTotal, @Periodo, @CodigoImportacion);
                                  SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using (SqlConnection CO = acceso.NuevaConexion())
            {
                CO.Open();
                using (SqlTransaction TR = CO.BeginTransaction())
                {
                    try
                    {
                        int codigoImportacion = InsertarImportacion(CO, TR, pImportacion);
                        foreach (VentaBE venta in pVentas)
                        {
                            using (SqlCommand CM = new SqlCommand(queryVenta, CO, TR))
                            {
                                CM.Parameters.AddWithValue("@CodigoLocal", venta.CodigoLocal);
                                CM.Parameters.AddWithValue("@CodigoProducto", venta.CodigoProducto);
                                CM.Parameters.AddWithValue("@Fecha", venta.Fecha);
                                CM.Parameters.AddWithValue("@Cantidad", venta.Cantidad);
                                CM.Parameters.AddWithValue("@PrecioUnitario", venta.PrecioUnitario);
                                CM.Parameters.AddWithValue("@ImporteTotal", venta.ImporteTotal);
                                CM.Parameters.AddWithValue("@Periodo", venta.Periodo);
                                CM.Parameters.AddWithValue("@CodigoImportacion", codigoImportacion);
                                pCodigosVenta.Add((int)CM.ExecuteScalar());
                            }
                        }
                        TR.Commit();
                        return codigoImportacion;
                    }
                    catch
                    {
                        TR.Rollback();
                        pCodigosVenta.Clear();
                        throw;
                    }
                }
            }
        }
        //Stock: un registro por producto y local. Si existe se actualiza, si no se crea.
        public int RegistrarStock(ImportacionBE pImportacion, List<StockBE> pStocks, List<int> pCodigosStock)
        {
            string queryActualizar = @"UPDATE Stock SET CantidadDisponible = @CantidadDisponible, Minimo = @Minimo, Maximo = @Maximo,
                                       Periodo = @Periodo, FechaActualizacion = @FechaActualizacion OUTPUT INSERTED.CodigoStock WHERE CodigoLocal = @CodigoLocal AND CodigoProducto = @CodigoProducto";

            string queryInsertar = @"INSERT INTO Stock (CodigoLocal, CodigoProducto, CantidadDisponible, Minimo, Maximo, Periodo, FechaActualizacion)
                                     VALUES (@CodigoLocal, @CodigoProducto, @CantidadDisponible, @Minimo, @Maximo, @Periodo, @FechaActualizacion);
                                     SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using (SqlConnection CO = acceso.NuevaConexion())
            {
                CO.Open();
                using (SqlTransaction TR = CO.BeginTransaction())
                {
                    try
                    {
                        int codigoImportacion = InsertarImportacion(CO, TR, pImportacion);
                        foreach (StockBE stock in pStocks)
                        {
                            var actualizados = new List<int>();
                            using (SqlCommand CM = new SqlCommand(queryActualizar, CO, TR))
                            {
                                CargarParametrosStock(CM, stock);
                                using (SqlDataReader DR = CM.ExecuteReader())
                                {
                                    while (DR.Read()) actualizados.Add(DR.GetInt32(0));
                                }
                            }
                            if (actualizados.Count == 0)
                            {
                                using (SqlCommand CM = new SqlCommand(queryInsertar, CO, TR))
                                {
                                    CargarParametrosStock(CM, stock);
                                    actualizados.Add((int)CM.ExecuteScalar());
                                }
                            }
                            pCodigosStock.AddRange(actualizados);
                        }
                        TR.Commit();
                        return codigoImportacion;
                    }
                    catch
                    {
                        TR.Rollback();
                        pCodigosStock.Clear();
                        throw;
                    }
                }
            }
        }
        private static void CargarParametrosStock(SqlCommand pComando, StockBE pStock)
        {
            pComando.Parameters.AddWithValue("@CodigoLocal", pStock.CodigoLocal);
            pComando.Parameters.AddWithValue("@CodigoProducto", pStock.CodigoProducto);
            pComando.Parameters.AddWithValue("@CantidadDisponible", pStock.CantidadDisponible);
            pComando.Parameters.AddWithValue("@Minimo", (object)pStock.Minimo ?? DBNull.Value);
            pComando.Parameters.AddWithValue("@Maximo", (object)pStock.Maximo ?? DBNull.Value);
            pComando.Parameters.AddWithValue("@Periodo", pStock.Periodo);
            pComando.Parameters.AddWithValue("@FechaActualizacion", pStock.FechaActualizacion);
        }
        private static int InsertarImportacion(SqlConnection pConexion, SqlTransaction pTransaccion, ImportacionBE pImportacion)
        {
            string query = @"INSERT INTO Importacion (CodigoLocal, Tipo, NombreArchivo, Fecha, Usuario, CantidadFilas, Estado)
                             VALUES (@CodigoLocal, @Tipo, @NombreArchivo, @Fecha, @Usuario, @CantidadFilas, @Estado); SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using (SqlCommand CM = new SqlCommand(query, pConexion, pTransaccion))
            {
                CM.Parameters.AddWithValue("@CodigoLocal", pImportacion.CodigoLocal);
                CM.Parameters.AddWithValue("@Tipo", pImportacion.Tipo);
                CM.Parameters.AddWithValue("@NombreArchivo", pImportacion.NombreArchivo);
                CM.Parameters.AddWithValue("@Fecha", pImportacion.Fecha);
                CM.Parameters.AddWithValue("@Usuario", pImportacion.Usuario);
                CM.Parameters.AddWithValue("@CantidadFilas", pImportacion.CantidadFilas);
                CM.Parameters.AddWithValue("@Estado", pImportacion.Estado);
                return (int)CM.ExecuteScalar();
            }
        }
        public List<ImportacionBE> ObtenerImportacionesPorLocal(int pCodigoLocal, int pCantidad)
        {
            var importaciones = new List<ImportacionBE>();
            string query = @"SELECT TOP (@Cantidad) CodigoImportacion, CodigoLocal, Tipo, NombreArchivo, Fecha, Usuario, CantidadFilas, Estado
                             FROM Importacion WHERE CodigoLocal = @CodigoLocal ORDER BY Fecha DESC, CodigoImportacion DESC";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoLocal", pCodigoLocal);
                CM.Parameters.AddWithValue("@Cantidad", pCantidad);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    while (DR.Read()) importaciones.Add(LeerImportacion(DR));
                }
            }
            return importaciones;
        }
        public ImportacionBE ObtenerImportacionPorCodigo(int pCodigoImportacion)
        {
            string query = @"SELECT CodigoImportacion, CodigoLocal, Tipo, NombreArchivo, Fecha, Usuario, CantidadFilas, Estado
                             FROM Importacion WHERE CodigoImportacion = @CodigoImportacion";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoImportacion", pCodigoImportacion);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    return DR.Read() ? LeerImportacion(DR) : null;
                }
            }
        }
        //Borra las ventas que entraron con la importacion. Devuelve cuantas ventas se borraron.
        public int DeshacerImportacion(int pCodigoImportacion)
        {
            using (SqlConnection CO = acceso.NuevaConexion())
            {
                CO.Open();
                using (SqlTransaction TR = CO.BeginTransaction())
                {
                    try
                    {
                        int borradas;
                        using (SqlCommand CM = new SqlCommand("DELETE FROM Venta WHERE CodigoImportacion = @CodigoImportacion", CO, TR))
                        {
                            CM.Parameters.AddWithValue("@CodigoImportacion", pCodigoImportacion);
                            borradas = CM.ExecuteNonQuery();
                        }
                        using (SqlCommand CM = new SqlCommand("UPDATE Importacion SET Estado = @Estado WHERE CodigoImportacion = @CodigoImportacion", CO, TR))
                        {
                            CM.Parameters.AddWithValue("@Estado", ImportacionBE.ESTADO_DESHECHA);
                            CM.Parameters.AddWithValue("@CodigoImportacion", pCodigoImportacion);
                            CM.ExecuteNonQuery();
                        }
                        TR.Commit();
                        return borradas;
                    }
                    catch
                    {
                        TR.Rollback();
                        throw;
                    }
                }
            }
        }
        private static ImportacionBE LeerImportacion(SqlDataReader DR)
        {
            return new ImportacionBE
            {
                CodigoImportacion = int.Parse(DR["CodigoImportacion"].ToString()),
                CodigoLocal = int.Parse(DR["CodigoLocal"].ToString()),
                Tipo = DR["Tipo"].ToString(),
                NombreArchivo = DR["NombreArchivo"].ToString(),
                Fecha = Convert.ToDateTime(DR["Fecha"]),
                Usuario = DR["Usuario"].ToString(),
                CantidadFilas = int.Parse(DR["CantidadFilas"].ToString()),
                Estado = DR["Estado"].ToString()
            };
        }
    }
}
