using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;

namespace DAL
{
    public class TokenRecuperacionDAL
    {
        private AccesoDatos accesoDatos = new AccesoDatos();
        public Guid GenerarToken(int pDNI, int pMinutosMAX)
        {
            InvalidarTokensAnteriores(pDNI);
            Guid nuevoToken = Guid.NewGuid();
            string query = @"INSERT INTO TokenRecuperacion (IdToken, UsuarioDNI, FechaCreacion, FechaExpiracion, TokenUsado)
                           VALUES (@IdToken, @UsuarioDNI, @FechaCreacion, @FechaExpiracion, 0)";
            using (SqlConnection CO = accesoDatos.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@IdToken", nuevoToken);
                CM.Parameters.AddWithValue("@UsuarioDNI", pDNI);
                CM.Parameters.AddWithValue("@FechaCreacion", DateTime.Now);
                CM.Parameters.AddWithValue("@FechaExpiracion", DateTime.Now.AddMinutes(pMinutosMAX));
                CO.Open();
                CM.ExecuteNonQuery();
            }
            return nuevoToken;
        }
        private void InvalidarTokensAnteriores(int pDNI)
        {
            string query = "UPDATE TokenRecuperacion SET TokenUsado = 1 WHERE UsuarioDNI = @UsuarioDNI AND TokenUsado = 0";
            using(SqlConnection CO = accesoDatos.NuevaConexion())
            using(SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@UsuarioDNI", pDNI);
                CO.Open();
                CM.ExecuteNonQuery();
            }
        }
        public TokenRecuperacion ObtenerToken(Guid IdToken)
        {
            TokenRecuperacion token = null;
            string query = "SELECT IdToken, UsuarioDNI, FechaCreacion, FechaExpiracion, TokenUsado FROM TokenRecuperacion WHERE IdToken = @IdToken";
            using (SqlConnection CO = accesoDatos.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@IdToken", IdToken);
                CO.Open();
                using(SqlDataReader DR = CM.ExecuteReader())
                {
                    if (DR.Read())
                    {
                        token = new TokenRecuperacion
                        {
                            IdToken = (Guid)DR["IdToken"],
                            UsuarioDNI = int.Parse(DR["UsuarioDNI"].ToString()),
                            FechaCreacion = DateTime.Parse(DR["FechaCreacion"].ToString()),
                            FechaExpiracion = DateTime.Parse(DR["FechaExpiracion"].ToString()),
                            TokenUsado = bool.Parse(DR["TokenUsado"].ToString())
                        };
                    }
                }
            }
            return token;
        }
        public void MarcarUsado(Guid IdToken)
        {
            string query = "UPDATE TokenRecuperacion SET TokenUsado = 1 WHERE IdToken = @IdToken";
            using (SqlConnection CO = accesoDatos.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@IdToken", IdToken);
                CO.Open();
                CM.ExecuteNonQuery();
            }
        }
    }
}
