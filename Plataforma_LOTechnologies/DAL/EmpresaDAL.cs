using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;

namespace DAL
{
    public class EmpresaDAL
    {
        private AccesoDatos acceso = new AccesoDatos();

        public List<EmpresaBE> ObtenerEmpresas()
        {
            List<EmpresaBE> empresas = new List<EmpresaBE>();
            string query = "SELECT CodigoEmpresa, RazonSocial, CUIT, Correo, Telefono, Direccion, Estado FROM Empresa WHERE Estado = 1";

            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    while (DR.Read())
                    {
                        EmpresaBE empresa = new EmpresaBE(int.Parse(DR["CodigoEmpresa"].ToString()), DR["RazonSocial"].ToString(),
                                            DR["CUIT"].ToString(), DR["Correo"].ToString(), LeerTelefono(DR), DR["Direccion"].ToString(), bool.Parse(DR["Estado"].ToString()));
                        empresas.Add(empresa);
                    }
                }
            }
            return empresas;
        }
        public List<EmpresaBE> ObtenerEmpresasListado(string pBusqueda, bool? pEstado)
        {
            List<EmpresaBE> empresas = new List<EmpresaBE>();
            string query = @"SELECT e.CodigoEmpresa, e.RazonSocial, e.CUIT, e.Correo, e.Telefono, e.Direccion, e.Estado,
                      (SELECT COUNT(*) FROM Local l WHERE l.CodigoEmpresa = e.CodigoEmpresa) AS CantidadLocales,
                      (SELECT COUNT(*) FROM Usuario u WHERE u.UsuarioCodigoEmpresa = e.CodigoEmpresa AND u.UsuarioEstadoActivo = 1) AS CantidadUsuarios
                      FROM Empresa e
                      WHERE (@Busqueda = '' OR e.RazonSocial LIKE @BusquedaLike OR REPLACE(e.CUIT, '-', '') LIKE @BusquedaCuit)
                      AND (@Estado IS NULL OR e.Estado = @Estado) ORDER BY e.RazonSocial";

            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                string busqueda = pBusqueda ?? "";
                CM.Parameters.AddWithValue("@Busqueda", busqueda);
                CM.Parameters.AddWithValue("@BusquedaLike", "%" + busqueda + "%");
                CM.Parameters.AddWithValue("@BusquedaCuit", "%" + busqueda.Replace("-", "") + "%");
                CM.Parameters.AddWithValue("@Estado", (object)pEstado ?? DBNull.Value);

                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    while (DR.Read())
                    {
                        EmpresaBE empresa = new EmpresaBE(
                            int.Parse(DR["CodigoEmpresa"].ToString()),
                            DR["RazonSocial"].ToString(),
                            DR["CUIT"].ToString(),
                            DR["Correo"].ToString(),
                            LeerTelefono(DR),
                            DR["Direccion"].ToString(),
                            bool.Parse(DR["Estado"].ToString()));
                        empresa.CantidadLocales = int.Parse(DR["CantidadLocales"].ToString());
                        empresa.CantidadUsuarios = int.Parse(DR["CantidadUsuarios"].ToString());
                        empresas.Add(empresa);
                    }
                }
            }
            return empresas;
        }
        private static string LeerTelefono(SqlDataReader DR)
        {
            return DR["Telefono"] == DBNull.Value ? "" : DR["Telefono"].ToString();
        }
        public bool ExisteCUIT(string pCUIT, int pExcluirCodigoEmpresa = 0)
        {
            string query = "SELECT COUNT(1) FROM Empresa WHERE REPLACE(CUIT, '-', '') = @CUIT AND CodigoEmpresa <> @Excluir";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CUIT", (pCUIT ?? "").Replace("-", ""));
                CM.Parameters.AddWithValue("@Excluir", pExcluirCodigoEmpresa);
                CO.Open();
                return (int)CM.ExecuteScalar() > 0;
            }
        }
        public int AgregarEmpresa(EmpresaBE pEmpresa)
        {
            string query = @"INSERT INTO Empresa (RazonSocial, CUIT, Correo, Telefono, Direccion, Estado)
                             VALUES (@RazonSocial, @CUIT, @Correo, @Telefono, @Direccion, @Estado); SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@RazonSocial", pEmpresa.RazonSocial);
                CM.Parameters.AddWithValue("@CUIT", pEmpresa.CUIT);
                CM.Parameters.AddWithValue("@Correo", pEmpresa.Correo);
                CM.Parameters.AddWithValue("@Telefono", string.IsNullOrEmpty(pEmpresa.Telefono) ? (object)DBNull.Value : pEmpresa.Telefono);
                CM.Parameters.AddWithValue("@Direccion", pEmpresa.Direccion);
                CM.Parameters.AddWithValue("@Estado", pEmpresa.Estado);
                CO.Open();
                return (int)CM.ExecuteScalar();
            }
        }
        public void ModificarEmpresa(EmpresaBE pEmpresa)
        {
            string query = @"UPDATE Empresa SET RazonSocial = @RazonSocial, CUIT = @CUIT, Correo = @Correo, Telefono = @Telefono, Direccion = @Direccion WHERE CodigoEmpresa = @CodigoEmpresa";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoEmpresa", pEmpresa.CodigoEmpresa);
                CM.Parameters.AddWithValue("@RazonSocial", pEmpresa.RazonSocial);
                CM.Parameters.AddWithValue("@CUIT", pEmpresa.CUIT);
                CM.Parameters.AddWithValue("@Correo", pEmpresa.Correo);
                CM.Parameters.AddWithValue("@Telefono", string.IsNullOrEmpty(pEmpresa.Telefono) ? (object)DBNull.Value : pEmpresa.Telefono);
                CM.Parameters.AddWithValue("@Direccion", pEmpresa.Direccion);
                CO.Open();
                CM.ExecuteNonQuery();
            }
        }
        public int ContarUsuariosActivos(int pCodigoEmpresa)
        {
            string query = "SELECT COUNT(*) FROM Usuario WHERE UsuarioCodigoEmpresa = @CodigoEmpresa AND UsuarioEstadoActivo = 1";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoEmpresa", pCodigoEmpresa);
                CO.Open();
                return (int)CM.ExecuteScalar();
            }
        }
        public void CambiarEstado(int pCodigoEmpresa, bool pNuevoEstado)
        {
            string query = "UPDATE Empresa SET Estado = @Estado WHERE CodigoEmpresa = @CodigoEmpresa";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@Estado", pNuevoEstado);
                CM.Parameters.AddWithValue("@CodigoEmpresa", pCodigoEmpresa);
                CO.Open();
                CM.ExecuteNonQuery();
            }
        }
        public EmpresaBE ObtenerEmpresaPorCodigo(int pCodigoEmpresa)
        {
            EmpresaBE empresa = null;
            string query = "SELECT CodigoEmpresa, RazonSocial, CUIT, Correo, Telefono, Direccion, Estado FROM Empresa WHERE CodigoEmpresa = @CodigoEmpresa";

            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoEmpresa", pCodigoEmpresa);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    if (DR.Read())
                    {
                        empresa = new EmpresaBE(
                            int.Parse(DR["CodigoEmpresa"].ToString()),
                            DR["RazonSocial"].ToString(),
                            DR["CUIT"].ToString(),
                            DR["Correo"].ToString(),
                            LeerTelefono(DR),
                            DR["Direccion"].ToString(),
                            bool.Parse(DR["Estado"].ToString()));
                    }
                }
            }
            return empresa;
        }
    }
}
