using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;

namespace DAL
{
    public class SucesoDAL
    {
        private AccesoDatos acceso = new AccesoDatos();
        public void RegistrarSuceso(Suceso pSuceso)
        {
            string query = "INSERT INTO Suceso (FechaSuceso, UsuarioSuceso, DescripcionSuceso, TipoSuceso, CriticidadSuceso)" +
                           "VALUES (@FechaSuceso, @UsuarioSuceso, @DescripcionSuceso, @TipoSuceso, @CriticidadSuceso)";
            using(SqlConnection CO = acceso.NuevaConexion())
            using(SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@FechaSuceso", pSuceso.Fecha);
                CM.Parameters.AddWithValue("@UsuarioSuceso", pSuceso.Usuario);
                CM.Parameters.AddWithValue("@DescripcionSuceso", pSuceso.Descripcion);
                CM.Parameters.AddWithValue("@TipoSuceso", pSuceso.TipoSuceso);
                CM.Parameters.AddWithValue("@CriticidadSuceso", pSuceso.Criticidad);
                CO.Open();
                CM.ExecuteNonQuery();
            }
        }
        public List<Suceso> LeerSucesosUltimosTresDias()
        {
            List<Suceso> sucesos = new List<Suceso>();
            string query = "SELECT * FROM Suceso WHERE Fecha >= DATEADD(DAY, -3, GETDATE()) ORDER BY Fecha DESC";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CO.Open();
                using(SqlDataReader DR = CM.ExecuteReader())
                {
                    while (DR.Read())
                    {
                        Suceso suceso = new Suceso(int.Parse(DR[0].ToString()), DateTime.Parse(DR[1].ToString()), DR[2].ToString(), DR[3].ToString(), DR[4].ToString(), int.Parse(DR[5].ToString()));
                        sucesos.Add(suceso);
                    }
                }
            }
            return sucesos;
        }
        public List<Suceso> LeerSucesos()
        {
            List<Suceso> sucesos = new List<Suceso>();
            string query = "SELECT * FROM Suceso ORDER BY Fecha DESC";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    while (DR.Read())
                    {
                        Suceso suceso = new Suceso(int.Parse(DR[0].ToString()), DateTime.Parse(DR[1].ToString()), DR[2].ToString(), DR[3].ToString(), DR[4].ToString(), int.Parse(DR[5].ToString()));
                        sucesos.Add(suceso);
                    }
                }
            }
            return sucesos;
        }
    }
}
