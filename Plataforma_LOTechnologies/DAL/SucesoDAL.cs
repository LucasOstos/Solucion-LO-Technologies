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
            string query = "SELECT * FROM Suceso WHERE FechaSuceso >= DATEADD(DAY, -3, GETDATE()) ORDER BY FechaSuceso DESC";
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
            string query = "SELECT * FROM Suceso ORDER BY FechaSuceso DESC";
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
        public List<Suceso> FiltrarSucesos(DateTime? pDesde, DateTime? pHasta, string pBusqueda, string pTipoSuceso, int? pCriticidad)
        {
            List<Suceso> sucesos = new List<Suceso>();
            string query = @"SELECT CodigoSuceso, FechaSuceso, UsuarioSuceso, DescripcionSuceso, TipoSuceso, CriticidadSuceso FROM Suceso WHERE 1=1";
            if (pDesde.HasValue) query += " AND FechaSuceso >= @Desde";
            if (pHasta.HasValue) query += " AND FechaSuceso <= @Hasta";
            if (!string.IsNullOrWhiteSpace(pBusqueda)) query += " AND (UsuarioSuceso LIKE @Busqueda OR DescripcionSuceso LIKE @Busqueda)";
            if (!string.IsNullOrWhiteSpace(pTipoSuceso)) query += " AND TipoSuceso = @TipoSuceso";
            if (pCriticidad.HasValue) query += " AND CriticidadSuceso = @Criticidad";
            query += " ORDER BY FechaSuceso DESC";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                if (pDesde.HasValue) CM.Parameters.AddWithValue("@Desde", pDesde.Value);
                if (pHasta.HasValue) CM.Parameters.AddWithValue("@Hasta", pHasta.Value);
                if (!string.IsNullOrWhiteSpace(pBusqueda)) CM.Parameters.AddWithValue("@Busqueda", "%" + pBusqueda + "%");
                if (!string.IsNullOrWhiteSpace(pTipoSuceso)) CM.Parameters.AddWithValue("@TipoSuceso", pTipoSuceso);
                if (pCriticidad.HasValue) CM.Parameters.AddWithValue("@Criticidad", pCriticidad.Value);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    while (DR.Read())
                    {
                        Suceso suceso = new Suceso(
                            int.Parse(DR[0].ToString()),
                            DateTime.Parse(DR[1].ToString()),
                            DR[2].ToString(),
                            DR[3].ToString(),
                            DR[4].ToString(),
                            int.Parse(DR[5].ToString()));
                        sucesos.Add(suceso);
                    }
                }
            }
            return sucesos;
        }
        public List<string> ObtenerTiposSuceso()
        {
            var tipos = new List<string>();
            string query = "SELECT DISTINCT TipoSuceso FROM Suceso ORDER BY TipoSuceso";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    while (DR.Read())
                    {
                        tipos.Add(DR["TipoSuceso"].ToString());
                    }
                }
            }
            return tipos;
        }
    }
}
