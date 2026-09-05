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
                                            DR["CUIT"].ToString(), DR["Correo"].ToString(), int.Parse(DR["Telefono"].ToString()), DR["Direccion"].ToString(), bool.Parse(DR["Estado"].ToString()));
                        empresas.Add(empresa);
                    }
                }
            }
            return empresas;
        }
        public List<EmpresaBE> ObtenerEmpresasListado(string pBusqueda, bool? pEstado)
        {
            List<EmpresaBE> empresas = new List<EmpresaBE>();
            string query = @"SELECT CodigoEmpresa, RazonSocial, CUIT, Correo, Telefono, Direccion, Estado
                      FROM Empresa
                      WHERE (@Busqueda = '' OR RazonSocial LIKE @BusquedaLike OR CUIT LIKE @BusquedaLike)
                        AND (@Estado IS NULL OR Estado = @Estado)
                      ORDER BY RazonSocial";

            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                string busqueda = pBusqueda ?? "";
                CM.Parameters.AddWithValue("@Busqueda", busqueda);
                CM.Parameters.AddWithValue("@BusquedaLike", "%" + busqueda + "%");
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
                            DR["Telefono"] == DBNull.Value ? 0 : int.Parse(DR["Telefono"].ToString()),
                            DR["Direccion"].ToString(),
                            bool.Parse(DR["Estado"].ToString()));
                        empresas.Add(empresa);
                    }
                }
            }
            return empresas;
        }
        public bool ExisteCUIT(string pCUIT)
        {
            string query = "SELECT COUNT(1) FROM Empresa WHERE CUIT = @CUIT";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CUIT", pCUIT);
                CO.Open();
                return (int)CM.ExecuteScalar() > 0;
            }
        }
        public int AgregarEmpresa(EmpresaBE pEmpresa)
        {
            string query = @"INSERT INTO Empresa (RazonSocial, CUIT, Correo, Telefono, Direccion, Estado)
                      VALUES (@RazonSocial, @CUIT, @Correo, @Telefono, @Direccion, @Estado);
                      SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@RazonSocial", pEmpresa.RazonSocial);
                CM.Parameters.AddWithValue("@CUIT", pEmpresa.CUIT);
                CM.Parameters.AddWithValue("@Correo", pEmpresa.Correo);
                CM.Parameters.AddWithValue("@Telefono", pEmpresa.Telefono);
                CM.Parameters.AddWithValue("@Direccion", pEmpresa.Direccion);
                CM.Parameters.AddWithValue("@Estado", pEmpresa.Estado);
                CO.Open();
                return (int)CM.ExecuteScalar();
            }
        }
        public void ModificarEmpresa(EmpresaBE pEmpresa)
        {
            string query = @"UPDATE Empresa SET RazonSocial = @RazonSocial, CUIT = @CUIT, Correo = @Correo,
                      Telefono = @Telefono, Direccion = @Direccion
                      WHERE CodigoEmpresa = @CodigoEmpresa";

            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@CodigoEmpresa", pEmpresa.CodigoEmpresa);
                CM.Parameters.AddWithValue("@RazonSocial", pEmpresa.RazonSocial);
                CM.Parameters.AddWithValue("@CUIT", pEmpresa.CUIT);
                CM.Parameters.AddWithValue("@Correo", pEmpresa.Correo);
                CM.Parameters.AddWithValue("@Telefono", pEmpresa.Telefono);
                CM.Parameters.AddWithValue("@Direccion", pEmpresa.Direccion);
                CO.Open();
                CM.ExecuteNonQuery();
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
                            DR["Telefono"] == DBNull.Value ? 0 : int.Parse(DR["Telefono"].ToString()),
                            DR["Direccion"].ToString(),
                            bool.Parse(DR["Estado"].ToString()));
                    }
                }
            }
            return empresa;
        }
    }
}
