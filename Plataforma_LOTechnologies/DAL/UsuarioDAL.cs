using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;

namespace DAL
{
    public class UsuarioDAL
    {
        private AccesoDatos acceso = new AccesoDatos();
        private const string SELECT_USUARIO = @"SELECT u.UsuarioDNI, u.UsuarioNombre, u.UsuarioApellido, u.UsuarioEmail, u.UsuarioContrasenia,
                                                u.UsuarioCodigoPerfil, p.Nombre_Perfil, u.UsuarioEstadoActivo, u.UsuarioEstadoBloqueado, u.UsuarioIntentosAcceso,
                                                u.UsuarioUltimoAcceso, u.UsuarioIdioma, u.UsuarioCodigoEmpresa FROM Usuario u INNER JOIN Perfil p ON u.UsuarioCodigoPerfil = p.Cod_Perfil";
        private Usuario MapearUsuario(SqlDataReader DR)
        {
            return new Usuario(
                Convert.ToInt32(DR["UsuarioDNI"]),
                DR["UsuarioNombre"].ToString(),
                DR["UsuarioApellido"].ToString(),
                DR["UsuarioEmail"].ToString(),
                DR["UsuarioContrasenia"].ToString(),
                new Perfil(Convert.ToInt32(DR["UsuarioCodigoPerfil"]), DR["Nombre_Perfil"].ToString()),
                Convert.ToBoolean(DR["UsuarioEstadoActivo"]),
                Convert.ToBoolean(DR["UsuarioEstadoBloqueado"]),
                Convert.ToInt32(DR["UsuarioIntentosAcceso"]),
                Convert.ToDateTime(DR["UsuarioUltimoAcceso"]),
                Convert.ToInt32(DR["UsuarioIdioma"]),
                DR["UsuarioCodigoEmpresa"] == DBNull.Value ? 0 : Convert.ToInt32(DR["UsuarioCodigoEmpresa"]));
        }
        public List<Usuario> LeerUsuarios()
        {
            List<Usuario> listaUsuarios = new List<Usuario>();
            string query = SELECT_USUARIO;

            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    while (DR.Read())
                    {                        
                        listaUsuarios.Add(MapearUsuario(DR));
                    }
                }
            }
            return listaUsuarios;
        }
        public List<Usuario> LeerUsuariosBloqueados()
        {
            List<Usuario> listaUsuarios = new List<Usuario>();
            string query = SELECT_USUARIO + " WHERE u.UsuarioEstadoBloqueado = 1";

            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    while (DR.Read())
                    {
                        listaUsuarios.Add(MapearUsuario(DR));
                    }
                }
            }
            return listaUsuarios;
        }
        public Usuario ObtenerUsuario(string pEmail)
        {
            Usuario usuario = null;
            string query = SELECT_USUARIO + " WHERE u.UsuarioEmail = @UsuarioEmail";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@UsuarioEmail", pEmail);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    if (DR.Read())
                    {
                        usuario = MapearUsuario(DR);
                    }
                }
            }
            return usuario;
        }
        public Usuario ObtenerUsuarioPorDNI(int pDNI)
        {
            Usuario usuario = null;
            string query = SELECT_USUARIO + " WHERE u.UsuarioDNI = @UsuarioDNI";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@UsuarioDNI", pDNI);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    if (DR.Read())
                    {
                        usuario = MapearUsuario(DR);
                    }
                }
            }
            return usuario;
        }
        public void ActualizarIntentos(int pIntentos, int pDNI)
        {
            string query = "UPDATE Usuario SET UsuarioIntentosAcceso = @UsuarioIntentosAcceso WHERE UsuarioDNI = @UsuarioDNI";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@UsuarioIntentosAcceso", pIntentos);
                CM.Parameters.AddWithValue("@UsuarioDNI", pDNI);
                CO.Open();
                CM.ExecuteNonQuery();
            }
        }
        public void ActualizarUltimoAcceso(DateTime pFecha, int pDNI)
        {
            string query = "UPDATE Usuario SET UsuarioUltimoAcceso = @UsuarioUltimoAcceso WHERE UsuarioDNI = @UsuarioDNI";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@UsuarioUltimoAcceso", pFecha);
                CM.Parameters.AddWithValue("@UsuarioDNI", pDNI);
                CO.Open();
                CM.ExecuteNonQuery();
            }
        }
        public void BloquearUsuario(bool pEstadoBloqueado, int pDNI)
        {
            string query = "UPDATE Usuario SET UsuarioEstadoBloqueado = @UsuarioEstadoBloqueado WHERE UsuarioDNI = @UsuarioDNI";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@UsuarioEstadoBloqueado", pEstadoBloqueado);
                CM.Parameters.AddWithValue("@UsuarioDNI", pDNI);
                CO.Open();
                CM.ExecuteNonQuery();
            }
        }
        public void DesbloquearSinContrasenia(int pDNI)
        {
            string query = "UPDATE Usuario SET UsuarioEstadoBloqueado = 0, UsuarioIntentosAcceso = 0 WHERE UsuarioDNI = @UsuarioDNI";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@UsuarioDNI", pDNI);
                CO.Open();
                CM.ExecuteNonQuery();
            }
        }
        public void CambiarContrasenia(int pDNI, string pNuevaContrasenia)
        {
            string query = "UPDATE Usuario SET UsuarioContrasenia = @UsuarioContrasenia WHERE UsuarioDNI = @UsuarioDNI";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@UsuarioContrasenia", pNuevaContrasenia);
                CM.Parameters.AddWithValue("@UsuarioDNI", pDNI);
                CO.Open();
                CM.ExecuteNonQuery();
            }
        }
        public List<UsuarioListado> ObtenerUsuariosListado(string pBusqueda, int? pCodigoPerfil, int? pCodigoEmpresa)
        {
            List<UsuarioListado> usuarios = new List<UsuarioListado>();
            string query = @"SELECT u.UsuarioDNI, u.UsuarioNombre, u.UsuarioApellido, u.UsuarioEmail, u.UsuarioCodigoPerfil, p.Nombre_Perfil,
                            u.UsuarioEstadoActivo, u.UsuarioEstadoBloqueado, u.UsuarioUltimoAcceso, u.UsuarioCodigoEmpresa, e.RazonSocial AS NombreEmpresa FROM Usuario u
                            INNER JOIN Perfil p ON u.UsuarioCodigoPerfil = p.Cod_Perfil LEFT JOIN Empresa e ON u.UsuarioCodigoEmpresa = e.CodigoEmpresa
                            WHERE (@Busqueda = '' OR u.UsuarioNombre LIKE @BusquedaLike OR u.UsuarioApellido LIKE @BusquedaLike OR u.UsuarioEmail LIKE @BusquedaLike
                            OR (u.UsuarioNombre + ' ' + u.UsuarioApellido) LIKE @BusquedaLike OR (u.UsuarioApellido + ' ' + u.UsuarioNombre) LIKE @BusquedaLike)
                            AND (@CodigoPerfil IS NULL OR u.UsuarioCodigoPerfil = @CodigoPerfil) AND (@CodigoEmpresa IS NULL OR u.UsuarioCodigoEmpresa = @CodigoEmpresa) ORDER BY u.UsuarioApellido, u.UsuarioNombre";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                string busqueda = pBusqueda ?? "";
                CM.Parameters.AddWithValue("@Busqueda", busqueda);
                CM.Parameters.AddWithValue("@BusquedaLike", "%" + busqueda + "%");
                CM.Parameters.AddWithValue("@CodigoPerfil", (object)pCodigoPerfil ?? DBNull.Value);
                CM.Parameters.AddWithValue("@CodigoEmpresa", (object)pCodigoEmpresa ?? DBNull.Value);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    while (DR.Read())
                    {
                        usuarios.Add(new UsuarioListado
                        {
                            DNI = Convert.ToInt32(DR["UsuarioDNI"]),
                            Nombre = DR["UsuarioNombre"].ToString(),
                            Apellido = DR["UsuarioApellido"].ToString(),
                            Email = DR["UsuarioEmail"].ToString(),
                            CodigoPerfil = Convert.ToInt32(DR["UsuarioCodigoPerfil"]),
                            NombrePerfil = DR["Nombre_Perfil"].ToString(),
                            estadoActivo = Convert.ToBoolean(DR["UsuarioEstadoActivo"]),
                            estadoBloqueado = Convert.ToBoolean(DR["UsuarioEstadoBloqueado"]),
                            ultimoAcceso = Convert.ToDateTime(DR["UsuarioUltimoAcceso"]),
                            CodigoEmpresa = DR["UsuarioCodigoEmpresa"] == DBNull.Value ? (int?)null : Convert.ToInt32(DR["UsuarioCodigoEmpresa"]),
                            NombreEmpresa = DR["NombreEmpresa"] == DBNull.Value ? "-" : DR["NombreEmpresa"].ToString()
                        });
                    }
                }
            }
            return usuarios;
        }
        public bool ExisteDNI(int pDNI)
        {
            string query = "SELECT COUNT(1) FROM Usuario WHERE UsuarioDNI = @DNI";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@DNI", pDNI);
                CO.Open();
                return (int)CM.ExecuteScalar() > 0;
            }
        }
        public void AgregarUsuario(Usuario pUsuario)
        {
            string query = @"INSERT INTO Usuario (UsuarioDNI, UsuarioNombre, UsuarioApellido, UsuarioEmail, UsuarioContrasenia, UsuarioCodigoPerfil, UsuarioEstadoActivo, UsuarioEstadoBloqueado, UsuarioIntentosAcceso, UsuarioUltimoAcceso, UsuarioIdioma, UsuarioCodigoEmpresa)
                   VALUES (@DNI, @Nombre, @Apellido, @Email, @Contrasenia, @Rol, @estadoActivo, 0, 0, @ultimoAcceso, @Idioma, @CodigoEmpresa)";

            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@DNI", pUsuario.DNI);
                CM.Parameters.AddWithValue("@Nombre", pUsuario.Nombre);
                CM.Parameters.AddWithValue("@Apellido", pUsuario.Apellido);
                CM.Parameters.AddWithValue("@Email", pUsuario.Email);
                CM.Parameters.AddWithValue("@Contrasenia", pUsuario.Contrasenia);
                CM.Parameters.AddWithValue("@Rol", pUsuario.Perfil.Codigo);
                CM.Parameters.AddWithValue("@estadoActivo", pUsuario.estadoActivo);
                CM.Parameters.AddWithValue("@ultimoAcceso", DateTime.Now);
                CM.Parameters.AddWithValue("@Idioma", 1);
                CM.Parameters.AddWithValue("@CodigoEmpresa", (object)pUsuario.CodigoEmpresa ?? DBNull.Value);
                CO.Open();
                CM.ExecuteNonQuery();
            }
        }
        public void ModificarUsuario(Usuario pUsuario)
        {
            string query = @"UPDATE Usuario SET UsuarioNombre = @Nombre, UsuarioApellido = @Apellido, UsuarioEmail = @Email, UsuarioCodigoPerfil = @Rol, UsuarioEstadoActivo = @estadoActivo, UsuarioCodigoEmpresa = @CodigoEmpresa WHERE UsuarioDNI = @DNI";

            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@DNI", pUsuario.DNI);
                CM.Parameters.AddWithValue("@Nombre", pUsuario.Nombre);
                CM.Parameters.AddWithValue("@Apellido", pUsuario.Apellido);
                CM.Parameters.AddWithValue("@Email", pUsuario.Email);
                CM.Parameters.AddWithValue("@Rol", pUsuario.Perfil.Codigo);
                CM.Parameters.AddWithValue("@estadoActivo", pUsuario.estadoActivo);
                CM.Parameters.AddWithValue("@CodigoEmpresa", (object)pUsuario.CodigoEmpresa ?? DBNull.Value);
                CO.Open();
                CM.ExecuteNonQuery();
            }
        }
        public void CambiarEstadoActivo(int pDNI, bool pNuevoEstado)
        {
            string query = "UPDATE Usuario SET UsuarioEstadoActivo = @estadoActivo WHERE UsuarioDNI = @DNI";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@estadoActivo", pNuevoEstado);
                CM.Parameters.AddWithValue("@DNI", pDNI);
                CO.Open();
                CM.ExecuteNonQuery();
            }
        }
    }
}
