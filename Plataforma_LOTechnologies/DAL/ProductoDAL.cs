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
        public List<ProductoListado> ObtenerProductosListado(string pBusqueda, int? pCodigoSector)
        {
            List<ProductoListado> productos = new List<ProductoListado>();
            string query = @"SELECT p.CodigoProducto, p.NumeroSerie, p.Nombre, p.Precio, p.Estado, c.Nombre AS NombreCategoria, s.Nombre AS NombreSector FROM Producto p
                             INNER JOIN Categoria c ON p.CodigoCategoria = c.CodigoCategoria LEFT JOIN Sector s ON p.CodigoSector = s.CodigoSector WHERE (@Busqueda = '' OR p.Nombre LIKE @BusquedaLike)
                             AND (@CodigoSector IS NULL OR p.CodigoSector = @CodigoSector) ORDER BY p.Nombre";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                string busqueda = pBusqueda ?? "";
                CM.Parameters.AddWithValue("@Busqueda", busqueda);
                CM.Parameters.AddWithValue("@BusquedaLike", "%" + busqueda + "%");
                CM.Parameters.AddWithValue("@CodigoSector", (object)pCodigoSector ?? DBNull.Value);
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
                            NombreSector = DR["NombreSector"] == DBNull.Value ? "-" : DR["NombreSector"].ToString(),
                            Precio = DR["Precio"] == DBNull.Value ? (decimal?)null : decimal.Parse(DR["Precio"].ToString()),
                            Stock = 0,
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
            string query = @"SELECT CodigoProducto, CodigoCategoria, CodigoSector, NumeroSerie, Nombre, Descripcion, Precio, Estado FROM Producto WHERE CodigoProducto = @CodigoProducto";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoProducto", pCodigoProducto);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    if (DR.Read())
                    {
                        producto = new ProductoBE(int.Parse(DR["CodigoProducto"].ToString()), int.Parse(DR["CodigoCategoria"].ToString()),
                                                  DR["CodigoSector"] == DBNull.Value ? (int?)null : int.Parse(DR["CodigoSector"].ToString()), int.Parse(DR["NumeroSerie"].ToString()),
                                                  DR["Nombre"].ToString(), DR["Descripcion"] == DBNull.Value ? "" : DR["Descripcion"].ToString(),
                                                  DR["Precio"] == DBNull.Value ? (decimal?)null : decimal.Parse(DR["Precio"].ToString()), bool.Parse(DR["Estado"].ToString()));                        
                    }
                }
            }
            return producto;
        }
        public int ObtenerProximoNumeroSerie()
        {
            string query = "SELECT ISNULL(MAX(NumeroSerie), 0) + 1 FROM Producto";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CO.Open();
                return (int)CM.ExecuteScalar();
            }
        }
        public int AgregarProducto(ProductoBE pProducto)
        {
            string query = @"INSERT INTO Producto (CodigoCategoria, CodigoSector, NumeroSerie, Nombre, Descripcion, Precio, Estado) VALUES (@CodigoCategoria, @CodigoSector, @NumeroSerie, @Nombre, @Descripcion, @Precio, @Estado);
                             SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoCategoria", pProducto.CodigoCategoria);
                CM.Parameters.AddWithValue("@CodigoSector", (object)pProducto.CodigoSector ?? DBNull.Value);
                CM.Parameters.AddWithValue("@NumeroSerie", pProducto.NumeroSerie);
                CM.Parameters.AddWithValue("@Nombre", pProducto.Nombre);
                CM.Parameters.AddWithValue("@Descripcion", (object)pProducto.Descripcion ?? DBNull.Value);
                CM.Parameters.AddWithValue("@Precio", (object)pProducto.Precio ?? DBNull.Value);
                CM.Parameters.AddWithValue("@Estado", pProducto.Estado);
                CO.Open();
                return (int)CM.ExecuteScalar();
            }
        }
        public void ModificarProducto(ProductoBE pProducto)
        {
            string query = @"UPDATE Producto SET CodigoCategoria = @CodigoCategoria, CodigoSector = @CodigoSector, Nombre = @Nombre, Descripcion = @Descripcion, Precio = @Precio
                             WHERE CodigoProducto = @CodigoProducto";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoProducto", pProducto.CodigoProducto);
                CM.Parameters.AddWithValue("@CodigoCategoria", pProducto.CodigoCategoria);
                CM.Parameters.AddWithValue("@CodigoSector", (object)pProducto.CodigoSector ?? DBNull.Value);
                CM.Parameters.AddWithValue("@Nombre", pProducto.Nombre);
                CM.Parameters.AddWithValue("@Descripcion", (object)pProducto.Descripcion ?? DBNull.Value);
                CM.Parameters.AddWithValue("@Precio", (object)pProducto.Precio ?? DBNull.Value);
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
    }
}
