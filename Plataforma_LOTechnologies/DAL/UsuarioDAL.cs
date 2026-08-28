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
        public List<Usuario> LeerUsuarios()
        {
            List<Usuario> listaUsuarios = new List<Usuario>();
            string query = "SELECT UsuarioDNI, UsuarioNombre, UsuarioApellido, UsuarioEmail, UsuarioContrasenia, UsuarioRol, UsuarioEstadoActivo," +
                           "UsuarioEstadoBloqueado, UsuarioIntentosAcceso, UsuarioUltimoAcceso, UsuarioIdioma, UsuarioCodigoEmpresa FROM Usuario";

            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    while (DR.Read())
                    {
                        Usuario usuario = new Usuario(int.Parse(DR[0].ToString()), DR[1].ToString(), DR[2].ToString(), DR[3].ToString(), DR[4].ToString(), int.Parse(DR[5].ToString()),
                                              bool.Parse(DR[6].ToString()), bool.Parse(DR[7].ToString()), int.Parse(DR[8].ToString()), DateTime.Parse(DR[9].ToString()), int.Parse(DR[10].ToString()), int.Parse(DR[11].ToString()));
                        listaUsuarios.Add(usuario);
                    }
                }
            }
            return listaUsuarios;
        }
        public List<Usuario> LeerUsuariosBloqueados()
        {
            List<Usuario> listaUsuarios = new List<Usuario>();
            string query = "SELECT UsuarioDNI, UsuarioNombre, UsuarioApellido, UsuarioEmail, UsuarioContrasenia, UsuarioRol, UsuarioEstadoActivo," +
                           "UsuarioEstadoBloqueado, UsuarioIntentosAcceso, UsuarioUltimoAcceso, UsuarioIdioma, UsuarioCodigoEmpresa FROM Usuario WHERE UsuarioEstadoBloqueado = 1";

            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    while (DR.Read())
                    {
                        Usuario usuario = new Usuario(int.Parse(DR[0].ToString()), DR[1].ToString(), DR[2].ToString(), DR[3].ToString(), DR[4].ToString(), int.Parse(DR[5].ToString()),
                                              bool.Parse(DR[6].ToString()), bool.Parse(DR[7].ToString()), int.Parse(DR[8].ToString()), DateTime.Parse(DR[9].ToString()), int.Parse(DR[10].ToString()), int.Parse(DR[11].ToString()));
                        listaUsuarios.Add(usuario);
                    }
                }
            }
            return listaUsuarios;
        }
        public Usuario ObtenerUsuario(string pEmail)
        {
            Usuario usuario = null;
            string query = "SELECT UsuarioDNI, UsuarioNombre, UsuarioApellido, UsuarioEmail, UsuarioContrasenia, UsuarioRol, UsuarioEstadoActivo," +
                           "UsuarioEstadoBloqueado, UsuarioIntentosAcceso, UsuarioUltimoAcceso, UsuarioIdioma, UsuarioCodigoEmpresa FROM Usuario WHERE UsuarioEmail = @UsuarioEmail";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@UsuarioEmail", pEmail);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    if (DR.Read())
                    {
                        usuario = new Usuario(int.Parse(DR[0].ToString()), DR[1].ToString(), DR[2].ToString(), DR[3].ToString(), DR[4].ToString(), int.Parse(DR[5].ToString()),
                                      bool.Parse(DR[6].ToString()), bool.Parse(DR[7].ToString()), int.Parse(DR[8].ToString()), DateTime.Parse(DR[9].ToString()), int.Parse(DR[10].ToString()), int.Parse(DR[11].ToString()));
                    }
                }
            }
            return usuario;
        }
        public Usuario ObtenerUsuarioPorDNI(int pDNI)
        {
            Usuario usuario = null;
            string query = "SELECT UsuarioDNI, UsuarioNombre, UsuarioApellido, UsuarioEmail, UsuarioContrasenia, UsuarioRol, UsuarioEstadoActivo," +
                           "UsuarioEstadoBloqueado, UsuarioIntentosAcceso, UsuarioUltimoAcceso, UsuarioIdioma, UsuarioCodigoEmpresa FROM Usuario WHERE UsuarioDNI = @UsuarioDNI";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@UsuarioDNI", pDNI);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    if (DR.Read())
                    {
                        usuario = new Usuario(int.Parse(DR[0].ToString()), DR[1].ToString(), DR[2].ToString(), DR[3].ToString(), DR[4].ToString(), int.Parse(DR[5].ToString()),
                                      bool.Parse(DR[6].ToString()), bool.Parse(DR[7].ToString()), int.Parse(DR[8].ToString()), DateTime.Parse(DR[9].ToString()), int.Parse(DR[10].ToString()), int.Parse(DR[11].ToString()));
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
        public List<UsuarioListado> ObtenerUsuariosListado(string pBusqueda, int? pRol, int? pCodigoEmpresa)
        {
            List<UsuarioListado> usuarios = new List<UsuarioListado>();
            string query = @"SELECT u.UsuarioDNI, u.UsuarioNombre, u.UsuarioApellido, u.UsuarioEmail, u.UsuarioRol, u.UsuarioEstadoActivo, u.UsuarioEstadoBloqueado, u.UsuarioUltimoAcceso, u.UsuarioCodigoEmpresa, e.RazonSocial AS NombreEmpresa FROM Usuario u
                   LEFT JOIN Empresa e ON u.UsuarioCodigoEmpresa = e.CodigoEmpresa WHERE (@Busqueda = '' OR u.UsuarioNombre LIKE @BusquedaLike OR u.UsuarioApellido LIKE @BusquedaLike OR u.UsuarioEmail LIKE @BusquedaLike OR (u.UsuarioNombre + ' ' + u.UsuarioApellido) LIKE @BusquedaLike OR (u.UsuarioApellido + ' ' + u.UsuarioNombre) LIKE @BusquedaLike)
                   AND (@Rol IS NULL OR u.UsuarioRol = @Rol) AND (@CodigoEmpresa IS NULL OR u.UsuarioCodigoEmpresa = @CodigoEmpresa) ORDER BY u.UsuarioApellido, u.UsuarioNombre";

            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                string busqueda = pBusqueda ?? "";
                CM.Parameters.AddWithValue("@Busqueda", busqueda);
                CM.Parameters.AddWithValue("@BusquedaLike", "%" + busqueda + "%");
                CM.Parameters.AddWithValue("@Rol", (object)pRol ?? DBNull.Value);
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
                            Rol = Convert.ToInt32(DR["UsuarioRol"]),
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
            string query = @"INSERT INTO Usuario (UsuarioDNI, UsuarioNombre, UsuarioApellido, UsuarioEmail, UsuarioContrasenia, UsuarioRol, UsuarioEstadoActivo, UsuarioEstadoBloqueado, UsuarioIntentosAcceso, UsuarioUltimoAcceso, UsuarioIdioma, UsuarioCodigoEmpresa)
                   VALUES (@DNI, @Nombre, @Apellido, @Email, @Contrasenia, @Rol, @estadoActivo, 0, 0, @ultimoAcceso, @Idioma, @CodigoEmpresa)";

            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@DNI", pUsuario.DNI);
                CM.Parameters.AddWithValue("@Nombre", pUsuario.Nombre);
                CM.Parameters.AddWithValue("@Apellido", pUsuario.Apellido);
                CM.Parameters.AddWithValue("@Email", pUsuario.Email);
                CM.Parameters.AddWithValue("@Contrasenia", pUsuario.Contrasenia);
                CM.Parameters.AddWithValue("@Rol", pUsuario.Rol);
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
            string query = @"UPDATE Usuario SET UsuarioNombre = @Nombre, UsuarioApellido = @Apellido, UsuarioEmail = @Email, UsuarioRol = @Rol, UsuarioEstadoActivo = @estadoActivo, UsuarioCodigoEmpresa = @CodigoEmpresa WHERE UsuarioDNI = @DNI";

            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@DNI", pUsuario.DNI);
                CM.Parameters.AddWithValue("@Nombre", pUsuario.Nombre);
                CM.Parameters.AddWithValue("@Apellido", pUsuario.Apellido);
                CM.Parameters.AddWithValue("@Email", pUsuario.Email);
                CM.Parameters.AddWithValue("@Rol", pUsuario.Rol);
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
