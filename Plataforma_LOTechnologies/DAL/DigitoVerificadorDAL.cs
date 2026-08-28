using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DigitoVerificadorDAL
    {
        private AccesoDatos acceso = new AccesoDatos();
        public DataTable ObtenerTabla(string pNombreTabla, string pColumnaOrden)
        {
            return acceso.Tabla(pNombreTabla, pColumnaOrden);
        }
        public void ActualizarDVH(string pNombreTabla, string nombreColumnaPK, object valorPK, string pDVH)
        {
            string query = $"UPDATE [{pNombreTabla}] SET DVH = @DVH WHERE [{nombreColumnaPK}] = @PK";
            using(SqlConnection CO = acceso.NuevaConexion())
            using(SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@DVH", pDVH);
                CM.Parameters.AddWithValue("@PK", valorPK);
                CO.Open();
                CM.ExecuteNonQuery();
            }
        }
        public void GuardarDVV(string pNombreTabla, string pDVV, int pCantidadRegistros)
        {
            string query = @"MERGE DigitoVerificador AS destino USING (SELECT @NombreTabla AS NombreTabla) AS origen ON destino.NombreTabla = origen.NombreTabla
                             WHEN MATCHED THEN UPDATE SET DVV = @DVV, CantidadRegistros = @CantidadRegistros
                             WHEN NOT MATCHED THEN INSERT(NombreTabla, DVV, CantidadRegistros) VALUES(@NombreTabla, @DVV, @CantidadRegistros);";
            using(SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@NombreTabla", pNombreTabla);
                CM.Parameters.AddWithValue("@DVV", pDVV);
                CM.Parameters.AddWithValue("@CantidadRegistros", pCantidadRegistros);
                CO.Open();
                CM.ExecuteNonQuery();
            }
        }
        public (string pDVV, int pCantidadRegistros)? ObtenerDVV(string pNombreTabla)
        {
            string query = "SELECT DVV, CantidadRegistros FROM DigitoVerificador WHERE NombreTabla = @NombreTabla";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@NombreTabla", pNombreTabla);
                CO.Open();
                using(SqlDataReader DR = CM.ExecuteReader())
                {
                    if (DR.Read())
                    {
                        return (DR["DVV"].ToString(), int.Parse(DR["CantidadRegistros"].ToString()));
                    }
                }
            }
            return null;
        }
    }
}
