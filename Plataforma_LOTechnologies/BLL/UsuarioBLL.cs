using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BE;
using DAL;
using SERVICIO.Logica;
namespace BLL
{
    #region Enums
    public class LoginResultado
    {
        public ResultadoLogin Resultado { get; set; }
        public Usuario Usuario { get; set; }
    }
    public enum ResultadoCambio { Exitoso, TokenInvalido, TokenExpirado, TokenYaUsado }
    public enum ResultadoGuardarUsuario { Exitoso, DniYaRegistrado, DatosInvalidos }
    #endregion
    public class UsuarioBLL
    {
        private UsuarioDAL accesoDAL = new UsuarioDAL();
        private TokenRecuperacionDAL tokenDAL = new TokenRecuperacionDAL();
        private DigitoVerificador digitos = new DigitoVerificador();
        public LoginResultado ValidarLogin(string pEmail, string pContrasenia, bool pEscribir)
        {
            Usuario usuario = accesoDAL.ObtenerUsuario(pEmail);
            if(usuario != null)
            {
                if (!usuario.estadoBloqueado)
                {
                    if (usuario.estadoActivo)
                    {
                        string contraseniaEncriptada = Encriptado.Instancia.EncriptarContraseña(pContrasenia);
                        if (usuario.Contrasenia == contraseniaEncriptada)
                        {
                            if (pEscribir)
                            {
                                usuario.intentosAcceso = 0;
                                usuario.ultimoAcceso = DateTime.Now;
                                accesoDAL.ActualizarIntentos(usuario.intentosAcceso, usuario.DNI);
                                accesoDAL.ActualizarUltimoAcceso(usuario.ultimoAcceso, usuario.DNI);
                                digitos.ActualizarDigitoFila("Usuario", usuario.DNI);
                            }
                            return new LoginResultado { Resultado = ResultadoLogin.Exitoso, Usuario = usuario };
                        }
                        else
                        {
                            if (pEscribir)
                            {
                                usuario.intentosAcceso++;
                                accesoDAL.ActualizarIntentos(usuario.intentosAcceso, usuario.DNI);
                                digitos.ActualizarDigitoFila("Usuario", usuario.DNI);
                                if (usuario.intentosAcceso >= 3)
                                {
                                    accesoDAL.BloquearUsuario(true, usuario.DNI);
                                    digitos.ActualizarDigitoFila("Usuario", usuario.DNI);
                                    return new LoginResultado { Resultado = ResultadoLogin.UsuarioRecienBloqueado };
                                }
                            }
                            return new LoginResultado { Resultado = ResultadoLogin.CredencialesIncorrectas };
                        }
                    }
                    else { return new LoginResultado { Resultado = ResultadoLogin.UsuarioDeshabilitado }; }
                }
                else { return new LoginResultado { Resultado = ResultadoLogin.UsuarioBloqueado }; }                                
            }
            else { return new LoginResultado { Resultado = ResultadoLogin.CredencialesIncorrectas }; }
        }
        public void SolicitarRecuperacion(string pEmail)
        {
            Usuario usuario = accesoDAL.ObtenerUsuario(pEmail);
            if (usuario == null) return;
            Guid token = tokenDAL.GenerarToken(usuario.DNI, 5);
            GmailServicio.Instancia.EnviarMailRecuperacion(usuario.Email, $"{usuario.Nombre} {usuario.Apellido}", token);
        }
        public ResultadoCambio ValidarToken(Guid pToken)
        {
            TokenRecuperacion token = tokenDAL.ObtenerToken(pToken);
            if (token == null) return ResultadoCambio.TokenInvalido;
            if (token.TokenUsado) return ResultadoCambio.TokenYaUsado;
            if(token.FechaExpiracion < DateTime.Now) return ResultadoCambio.TokenExpirado;
            return ResultadoCambio.Exitoso;
        }
        public ResultadoCambio CambiarContraseniaConToken(Guid pToken, string pNuevaContrasenia)
        {
            ResultadoCambio validacion = ValidarToken(pToken);
            if(validacion != ResultadoCambio.Exitoso) return validacion;
            TokenRecuperacion token = tokenDAL.ObtenerToken(pToken);
            string contraseniaEncriptada = Encriptado.Instancia.EncriptarContraseña(pNuevaContrasenia);
            accesoDAL.CambiarContrasenia(token.UsuarioDNI, contraseniaEncriptada);
            digitos.ActualizarDigitoFila("Usuario", token.UsuarioDNI);
            tokenDAL.MarcarUsado(pToken);
            return ResultadoCambio.Exitoso;
        }
        public Usuario ObtenerUsuario(string pEmail)
        {
            return accesoDAL.ObtenerUsuario(pEmail);
        }
        public List<Usuario> ObtenerUsuarios()
        {
            return accesoDAL.LeerUsuarios();
        }
        public List<Usuario> ObtenerBloqueados()
        {
            return accesoDAL.LeerUsuariosBloqueados();
        }
        public Usuario ObtenerUsuarioPorDNI(int pDNI)
        {
            return accesoDAL.ObtenerUsuarioPorDNI(pDNI);
        }
        public List<UsuarioListado> ObtenerUsuariosListado(string pBusqueda, int? pRol, int? pCodigoEmpresa)
        {
            return accesoDAL.ObtenerUsuariosListado(pBusqueda, pRol, pCodigoEmpresa);
        }
        public ResultadoGuardarUsuario CrearUsuario(Usuario pUsuario, string pPasswordPlano)
        {
            if (accesoDAL.ExisteDNI(pUsuario.DNI))
            {
                return ResultadoGuardarUsuario.DniYaRegistrado;
            }
            pUsuario.Contrasenia = Encriptado.Instancia.EncriptarContraseña(pPasswordPlano);
            pUsuario.Idioma = 1;

            accesoDAL.AgregarUsuario(pUsuario);
            digitos.ActualizarDigitoFila("Usuario", pUsuario.DNI);
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "Registrar usuario", "Usuarios", 1);
            return ResultadoGuardarUsuario.Exitoso;
        }
        public ResultadoGuardarUsuario ModificarUsuario(Usuario pUsuario)
        {
            accesoDAL.ModificarUsuario(pUsuario);
            digitos.ActualizarDigitoFila("Usuario", pUsuario.DNI);
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "Modificar usuario", "Usuarios", 1);
            return ResultadoGuardarUsuario.Exitoso;
        }
        public void CambiarEstadoActivo(int dni, bool nuevoEstado)
        {
            accesoDAL.CambiarEstadoActivo(dni, nuevoEstado);
            digitos.ActualizarDigitoFila("Usuario", dni);
            string accion = nuevoEstado ? "Activar usuario" : "Desactivar usuario";
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", accion, "Usuarios", 1);
        }
        public void DesbloquearUsuario(int pDNI)
        {
            Usuario usuario = accesoDAL.ObtenerUsuarioPorDNI(pDNI);
            accesoDAL.DesbloquearSinContrasenia(pDNI);
            digitos.ActualizarDigitoFila("Usuario", pDNI);
            Guid token = tokenDAL.GenerarToken(pDNI, 60);
            GmailServicio.Instancia.EnviarMailRecuperacion(usuario.Email, $"{usuario.Nombre} {usuario.Apellido}", token);
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "Desbloquear usuario", "Usuarios", 1);
        }
    }
}
