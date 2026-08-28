using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class AccesoDatos
    {
        private string connectionString = "Data Source=.;Initial Catalog=BD_LOTechnologies;Integrated Security=True;MultipleActiveResultSets=True";

        public SqlConnection NuevaConexion()
        {
            return new SqlConnection(connectionString);
        }

        public DataTable Tabla(string pNombreTabla, string pColumnaOrden = null)
        {
            string query = string.IsNullOrEmpty(pColumnaOrden) ? $"SELECT * FROM [{pNombreTabla}]" : $"SELECT * FROM [{pNombreTabla}] ORDER BY [{pColumnaOrden}]";
            DataTable DT = new DataTable();
            using(SqlConnection CO = NuevaConexion())   
            using(SqlDataAdapter DA = new SqlDataAdapter(query, CO))
            {
                DA.Fill(DT);
            }
            return DT;
        }
    }
}
