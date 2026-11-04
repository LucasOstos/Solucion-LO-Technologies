using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;

namespace DAL
{
    public class ProductoDAL
    {
        private AccesoDatos acceso = new AccesoDatos();
        // Catálogo de la empresa con el sector que cada producto tiene en el local indicado (NULL = sin ubicar en ese local)
        public List<ProductoListado> ObtenerProductosListado(int pCodigoEmpresa, int pCodigoLocal, string pBusqueda, int? pCodigoSector)
        {
            List<ProductoListado> productos = new List<ProductoListado>();
            string query = @"SELECT p.CodigoProducto, p.NumeroSerie, p.Nombre, p.Precio, p.Estado, c.Nombre AS NombreCategoria, s.Nombre AS NombreSector,
                             ISNULL((SELECT SUM(st.CantidadDisponible) FROM Stock st WHERE st.CodigoProducto = p.CodigoProducto AND st.CodigoLocal = @CodigoLocal), 0) AS Stock
                             FROM Producto p INNER JOIN Categoria c ON p.CodigoCategoria = c.CodigoCategoria
                             LEFT JOIN ProductoUbicacion pu ON pu.CodigoProducto = p.CodigoProducto AND pu.CodigoLocal = @CodigoLocal
                             LEFT JOIN Sector s ON pu.CodigoSector = s.CodigoSector
                             WHERE p.CodigoEmpresa = @CodigoEmpresa AND (@Busqueda = '' OR p.Nombre LIKE @BusquedaLike)
                             AND (@CodigoSector IS NULL OR pu.CodigoSector = @CodigoSector) ORDER BY p.Nombre";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                string busqueda = pBusqueda ?? "";
                CM.Parameters.AddWithValue("@Busqueda", busqueda);
                CM.Parameters.AddWithValue("@BusquedaLike", "%" + busqueda + "%");
                CM.Parameters.AddWithValue("@CodigoSector", (object)pCodigoSector ?? DBNull.Value);
                CM.Parameters.AddWithValue("@CodigoEmpresa", pCodigoEmpresa);
                CM.Parameters.AddWithValue("@CodigoLocal", pCodigoLocal);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    while (DR.Read())
                    {
                        int numeroSerie = int.Parse(DR["NumeroSerie"].ToString());
                        productos.Add(new ProductoListado
                        {
                            CodigoProducto = int.Parse(DR["CodigoProducto"].ToString()),
                            CodigoInterno = $"PROD-{numeroSerie:D3}",
                            Nombre = DR["Nombre"].ToString(),
                            NombreCategoria = DR["NombreCategoria"].ToString(),
                            NombreSector = DR["NombreSector"] == DBNull.Value ? null : DR["NombreSector"].ToString(),
                            Precio = DR["Precio"] == DBNull.Value ? (decimal?)null : decimal.Parse(DR["Precio"].ToString()),
                            Stock = int.Parse(DR["Stock"].ToString()),
                            Estado = bool.Parse(DR["Estado"].ToString()) ? 1 : 0
                        });
                    }
                }
            }
            return productos;
        }
        public ProductoBE ObtenerProductoPorCodigo(int pCodigoProducto)
        {
            ProductoBE producto = null;
            string query = @"SELECT CodigoProducto, CodigoCategoria, NumeroSerie, Nombre, Descripcion, Precio, Estado, Costo FROM Producto WHERE CodigoProducto = @CodigoProducto";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoProducto", pCodigoProducto);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    if (DR.Read())
                    {
                        producto = new ProductoBE(int.Parse(DR["CodigoProducto"].ToString()), int.Parse(DR["CodigoCategoria"].ToString()), int.Parse(DR["NumeroSerie"].ToString()),
                                                  DR["Nombre"].ToString(), DR["Descripcion"] == DBNull.Value ? "" : DR["Descripcion"].ToString(),
                                                  DR["Precio"] == DBNull.Value ? (decimal?)null : decimal.Parse(DR["Precio"].ToString()), bool.Parse(DR["Estado"].ToString()),
                                                  DR["Costo"] == DBNull.Value ? (decimal?)null : decimal.Parse(DR["Costo"].ToString()));
                    }
                }
            }
            return producto;
        }
        public int ObtenerProximoNumeroSerie(int pCodigoEmpresa)
        {
            string query = "SELECT ISNULL(MAX(NumeroSerie), 0) + 1 FROM Producto WHERE CodigoEmpresa = @CodigoEmpresa";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoEmpresa", pCodigoEmpresa);
                CO.Open();
                return (int)CM.ExecuteScalar();
            }
        }
        public int AgregarProducto(ProductoBE pProducto)
        {
            string query = @"INSERT INTO Producto (CodigoCategoria, NumeroSerie, Nombre, Descripcion, Precio, Estado, CodigoEmpresa, Costo)
                             VALUES (@CodigoCategoria, @NumeroSerie, @Nombre, @Descripcion, @Precio, @Estado, @CodigoEmpresa, @Costo);
                             SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoCategoria", pProducto.CodigoCategoria);
                CM.Parameters.AddWithValue("@NumeroSerie", pProducto.NumeroSerie);
                CM.Parameters.AddWithValue("@Nombre", pProducto.Nombre);
                CM.Parameters.AddWithValue("@Descripcion", (object)pProducto.Descripcion ?? DBNull.Value);
                CM.Parameters.AddWithValue("@Precio", (object)pProducto.Precio ?? DBNull.Value);
                CM.Parameters.AddWithValue("@Costo", (object)pProducto.Costo ?? DBNull.Value);
                CM.Parameters.AddWithValue("@Estado", pProducto.Estado);
                CM.Parameters.AddWithValue("@CodigoEmpresa", pProducto.CodigoEmpresa);
                CO.Open();
                return (int)CM.ExecuteScalar();
            }
        }
        public void ModificarProducto(ProductoBE pProducto)
        {
            string query = @"UPDATE Producto SET CodigoCategoria = @CodigoCategoria, Nombre = @Nombre, Descripcion = @Descripcion, Precio = @Precio, Costo = @Costo
                             WHERE CodigoProducto = @CodigoProducto";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoProducto", pProducto.CodigoProducto);
                CM.Parameters.AddWithValue("@CodigoCategoria", pProducto.CodigoCategoria);
                CM.Parameters.AddWithValue("@Nombre", pProducto.Nombre);
                CM.Parameters.AddWithValue("@Descripcion", (object)pProducto.Descripcion ?? DBNull.Value);
                CM.Parameters.AddWithValue("@Precio", (object)pProducto.Precio ?? DBNull.Value);
                CM.Parameters.AddWithValue("@Costo", (object)pProducto.Costo ?? DBNull.Value);
                CO.Open();
                CM.ExecuteNonQuery();
            }
        }
        public void CambiarEstado(int pCodigoProducto, bool pNuevoEstado)
        {
            string query = "UPDATE Producto SET Estado = @Estado WHERE CodigoProducto = @CodigoProducto";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@Estado", pNuevoEstado);
                CM.Parameters.AddWithValue("@CodigoProducto", pCodigoProducto);
                CO.Open();
                CM.ExecuteNonQuery();
            }
        }

        #region Ubicación del producto en cada local (tabla ProductoUbicacion)
        public ProductoUbicacionBE ObtenerUbicacion(int pCodigoProducto, int pCodigoLocal)
        {
            string query = @"SELECT CodigoUbicacion, CodigoProducto, CodigoLocal, CodigoSector FROM ProductoUbicacion
                             WHERE CodigoProducto = @CodigoProducto AND CodigoLocal = @CodigoLocal";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoProducto", pCodigoProducto);
                CM.Parameters.AddWithValue("@CodigoLocal", pCodigoLocal);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    if (!DR.Read()) return null;
                    return new ProductoUbicacionBE(int.Parse(DR["CodigoUbicacion"].ToString()), int.Parse(DR["CodigoProducto"].ToString()),
                                                   int.Parse(DR["CodigoLocal"].ToString()), int.Parse(DR["CodigoSector"].ToString()));
                }
            }
        }
        public int AgregarUbicacion(ProductoUbicacionBE pUbicacion)
        {
            string query = @"INSERT INTO ProductoUbicacion (CodigoProducto, CodigoLocal, CodigoSector) VALUES (@CodigoProducto, @CodigoLocal, @CodigoSector);
                             SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoProducto", pUbicacion.CodigoProducto);
                CM.Parameters.AddWithValue("@CodigoLocal", pUbicacion.CodigoLocal);
                CM.Parameters.AddWithValue("@CodigoSector", pUbicacion.CodigoSector);
                CO.Open();
                return (int)CM.ExecuteScalar();
            }
        }
        public void ModificarUbicacion(int pCodigoUbicacion, int pCodigoSector)
        {
            string query = "UPDATE ProductoUbicacion SET CodigoSector = @CodigoSector WHERE CodigoUbicacion = @CodigoUbicacion";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoSector", pCodigoSector);
                CM.Parameters.AddWithValue("@CodigoUbicacion", pCodigoUbicacion);
                CO.Open();
                CM.ExecuteNonQuery();
            }
        }
        public void EliminarUbicacion(int pCodigoUbicacion)
        {
            string query = "DELETE FROM ProductoUbicacion WHERE CodigoUbicacion = @CodigoUbicacion";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoUbicacion", pCodigoUbicacion);
                CO.Open();
                CM.ExecuteNonQuery();
            }
        }
        #endregion
    }
}
