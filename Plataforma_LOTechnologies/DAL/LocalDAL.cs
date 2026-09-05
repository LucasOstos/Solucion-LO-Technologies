using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;

namespace DAL
{
    public class LocalDAL
    {
        private AccesoDatos acceso = new AccesoDatos();

        public List<LocalListado> ObtenerLocalesListado(string pBusqueda, int? pCodigoEmpresa, string pEstado)
        {
            List<LocalListado> locales = new List<LocalListado>();
            string query = @"SELECT l.CodigoLocal, l.CodigoEmpresa, e.RazonSocial AS NombreEmpresa, l.Nombre, l.Direccion, l.TipoLocal, l.SuperficieAprox, l.Estado FROM Local l
                              INNER JOIN Empresa e ON l.CodigoEmpresa = e.CodigoEmpresa WHERE (@Busqueda = '' OR l.Nombre LIKE @BusquedaLike OR l.Direccion LIKE @BusquedaLike)
                                AND (@CodigoEmpresa IS NULL OR l.CodigoEmpresa = @CodigoEmpresa) AND (@Estado = '' OR l.Estado = @Estado) ORDER BY l.Nombre";

            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                string busqueda = pBusqueda ?? "";
                CM.Parameters.AddWithValue("@Busqueda", busqueda);
                CM.Parameters.AddWithValue("@BusquedaLike", "%" + busqueda + "%");
                CM.Parameters.AddWithValue("@CodigoEmpresa", (object)pCodigoEmpresa ?? DBNull.Value);
                CM.Parameters.AddWithValue("@Estado", pEstado ?? "");
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    while (DR.Read())
                    {
                        locales.Add(new LocalListado
                        {
                            CodigoLocal = int.Parse(DR["CodigoLocal"].ToString()),
                            CodigoEmpresa = int.Parse(DR["CodigoEmpresa"].ToString()),
                            NombreEmpresa = DR["NombreEmpresa"].ToString(),
                            Nombre = DR["Nombre"].ToString(),
                            Direccion = DR["Direccion"].ToString(),
                            TipoLocal = DR["TipoLocal"] == DBNull.Value ? "" : DR["TipoLocal"].ToString(),
                            SuperficieAprox = DR["SuperficieAprox"] == DBNull.Value ? (decimal?)null : decimal.Parse(DR["SuperficieAprox"].ToString()),
                            Estado = DR["Estado"].ToString() == "Activo" ? 1 : 0
                        });
                    }
                }
            }
            return locales;
        }
        public LocalBE ObtenerLocalPorCodigo(int pCodigoLocal)
        {
            LocalBE local = null;
            string query = @"SELECT CodigoLocal, CodigoEmpresa, Nombre, Direccion, TipoLocal, SuperficieAprox, Telefono, Observaciones, Estado FROM Local WHERE CodigoLocal = @CodigoLocal";

            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoLocal", pCodigoLocal);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    if (DR.Read())
                    {
                        local = new LocalBE(int.Parse(DR["CodigoLocal"].ToString()), int.Parse(DR["CodigoEmpresa"].ToString()), DR["Nombre"].ToString(), DR["Direccion"].ToString(),
                                            DR["TipoLocal"] == DBNull.Value ? "" : DR["TipoLocal"].ToString(), DR["SuperficieAprox"] == DBNull.Value ? (decimal?)null : decimal.Parse(DR["SuperficieAprox"].ToString()),
                                            DR["Telefono"] == DBNull.Value ? "" : DR["Telefono"].ToString(), DR["Observaciones"] == DBNull.Value ? "" : DR["Observaciones"].ToString(), DR["Estado"].ToString());
                    }
                }
            }
            return local;
        }
        public int AgregarLocal(LocalBE pLocal)
        {
            string query = @"INSERT INTO Local (CodigoEmpresa, Nombre, Direccion, TipoLocal, SuperficieAprox, Telefono, Observaciones, Estado)
                              VALUES (@CodigoEmpresa, @Nombre, @Direccion, @TipoLocal, @SuperficieAprox, @Telefono, @Observaciones, @Estado);
                              SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoEmpresa", pLocal.CodigoEmpresa);
                CM.Parameters.AddWithValue("@Nombre", pLocal.Nombre);
                CM.Parameters.AddWithValue("@Direccion", pLocal.Direccion);
                CM.Parameters.AddWithValue("@TipoLocal", (object)pLocal.TipoLocal ?? DBNull.Value);
                CM.Parameters.AddWithValue("@SuperficieAprox", (object)pLocal.SuperficieAprox ?? DBNull.Value);
                CM.Parameters.AddWithValue("@Telefono", (object)pLocal.Telefono ?? DBNull.Value);
                CM.Parameters.AddWithValue("@Observaciones", (object)pLocal.Observaciones ?? DBNull.Value);
                CM.Parameters.AddWithValue("@Estado", pLocal.Estado);
                CO.Open();
                return (int)CM.ExecuteScalar();
            }
        }
        public void ModificarLocal(LocalBE pLocal)
        {
            string query = @"UPDATE Local SET CodigoEmpresa = @CodigoEmpresa, Nombre = @Nombre, Direccion = @Direccion, TipoLocal = @TipoLocal, SuperficieAprox = @SuperficieAprox, Telefono = @Telefono, Observaciones = @Observaciones WHERE CodigoLocal = @CodigoLocal";

            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoLocal", pLocal.CodigoLocal);
                CM.Parameters.AddWithValue("@CodigoEmpresa", pLocal.CodigoEmpresa);
                CM.Parameters.AddWithValue("@Nombre", pLocal.Nombre);
                CM.Parameters.AddWithValue("@Direccion", pLocal.Direccion);
                CM.Parameters.AddWithValue("@TipoLocal", (object)pLocal.TipoLocal ?? DBNull.Value);
                CM.Parameters.AddWithValue("@SuperficieAprox", (object)pLocal.SuperficieAprox ?? DBNull.Value);
                CM.Parameters.AddWithValue("@Telefono", (object)pLocal.Telefono ?? DBNull.Value);
                CM.Parameters.AddWithValue("@Observaciones", (object)pLocal.Observaciones ?? DBNull.Value);
                CO.Open();
                CM.ExecuteNonQuery();
            }
        }
        public void CambiarEstado(int pCodigoLocal, string pNuevoEstado)
        {
            string query = "UPDATE Local SET Estado = @Estado WHERE CodigoLocal = @CodigoLocal";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@Estado", pNuevoEstado);
                CM.Parameters.AddWithValue("@CodigoLocal", pCodigoLocal);
                CO.Open();
                CM.ExecuteNonQuery();
            }
        }
    }
}
