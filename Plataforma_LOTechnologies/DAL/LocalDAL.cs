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
            string query = @"SELECT l.CodigoLocal, l.CodigoEmpresa, e.RazonSocial AS NombreEmpresa, l.Nombre, l.Direccion, l.TipoLocal, l.SuperficieAprox, l.Estado,
                             CASE WHEN EXISTS (SELECT 1 FROM Layout la WHERE la.CodigoLocal = l.CodigoLocal AND la.Estado = 'Activo') THEN 1 ELSE 0 END AS TieneLayoutVigente,
                             (SELECT COUNT(*) FROM Sector s WHERE s.CodigoLocal = l.CodigoLocal AND s.Estado = 1) AS CantidadSectores,
                             (SELECT COUNT(*) FROM Sector s WHERE s.CodigoLocal = l.CodigoLocal AND s.Estado = 1 AND s.Superficie > 0 AND s.Circulacion IS NOT NULL) AS SectoresCompletos,
                             (SELECT ISNULL(SUM(s.Superficie), 0) FROM Sector s WHERE s.CodigoLocal = l.CodigoLocal AND s.Estado = 1) AS SuperficieSectores,
                             (SELECT COUNT(*) FROM ProductoUbicacion pu WHERE pu.CodigoLocal = l.CodigoLocal) AS ProductosUbicados,
                             (SELECT COUNT(*) FROM Stock st WHERE st.CodigoLocal = l.CodigoLocal) AS ProductosConStock,
                             (SELECT COUNT(*) FROM Venta v WHERE v.CodigoLocal = l.CodigoLocal AND v.Periodo = @PeriodoActual) AS VentasDelMes
                             FROM Local l
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
                CM.Parameters.AddWithValue("@PeriodoActual", DateTime.Now.ToString("yyyy-MM"));
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
                            Estado = DR["Estado"].ToString() == "Activo" ? 1 : 0,
                            TieneLayoutVigente = int.Parse(DR["TieneLayoutVigente"].ToString()) == 1,
                            CantidadSectores = int.Parse(DR["CantidadSectores"].ToString()),
                            SectoresCompletos = int.Parse(DR["SectoresCompletos"].ToString()),
                            SuperficieSectores = decimal.Parse(DR["SuperficieSectores"].ToString()),
                            ProductosUbicados = int.Parse(DR["ProductosUbicados"].ToString()),
                            ProductosConStock = int.Parse(DR["ProductosConStock"].ToString()),
                            VentasDelMes = int.Parse(DR["VentasDelMes"].ToString())
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
        public bool ExisteNombre(int pCodigoEmpresa, string pNombre, int pExcluirCodigoLocal)
        {
            string query = "SELECT COUNT(1) FROM Local WHERE CodigoEmpresa = @CodigoEmpresa AND UPPER(LTRIM(RTRIM(Nombre))) = UPPER(@Nombre) AND CodigoLocal <> @Excluir";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoEmpresa", pCodigoEmpresa);
                CM.Parameters.AddWithValue("@Nombre", (pNombre ?? "").Trim());
                CM.Parameters.AddWithValue("@Excluir", pExcluirCodigoLocal);
                CO.Open();
                return (int)CM.ExecuteScalar() > 0;
            }
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
