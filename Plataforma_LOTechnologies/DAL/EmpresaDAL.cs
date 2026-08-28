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
            string query = "SELECT CodigoEmpresa, RazonSocial, CUIT, Telefono, Direccion, Estado FROM Empresa WHERE Estado = 1";

            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    while (DR.Read())
                    {
                        EmpresaBE empresa = new EmpresaBE(int.Parse(DR["CodigoEmpresa"].ToString()), DR["RazonSocial"].ToString(),
                                            DR["CUIT"].ToString(), int.Parse(DR["Telefono"].ToString()), DR["Direccion"].ToString(), bool.Parse(DR["Estado"].ToString()));
                        empresas.Add(empresa);
                    }
                }
            }
            return empresas;
        }
    }
}
