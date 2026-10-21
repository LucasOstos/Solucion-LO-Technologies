using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SERVICIO.Logica;
using BE;
using BLL;

public partial class PaginaLogin : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack && Sesion.Instancia.IsLogueado())
        {
            Response.Redirect("PaginaPrincipal.aspx");
        }
    }

    protected void btnIngresar_Click(object sender, EventArgs e)
    {
        string email = tbEmail.Text.Trim();
        string contrasenia = tbContrasenia.Text.Trim();
        DigitoVerificador digitosBLL = new DigitoVerificador();
        Usuario superUsuario = SuperUsuarioServicio.Instancia.ValidarCredenciales(email, contrasenia);
        if (superUsuario != null)
        {
            bool integridad;
            try { integridad = digitosBLL.ValidarIntegridadDatos(); }
            catch { integridad = false; }

            Sesion.Instancia.Login(superUsuario);
            Sesion.Instancia.IntegridadComprometida = !integridad;
            //SuperUsuarioServicio.Instancia.RegistrarSinFallar($"{superUsuario.Nombre} {superUsuario.Apellido}", "Inicio de Sesión (superusuario)", "Acceso", 2);
            Response.Redirect(integridad ? "PaginaPrincipal.aspx" : "PaginaDigitosVerificadores.aspx");
            return;
        }
        bool integridadOK = digitosBLL.ValidarIntegridadDatos();
        if (!integridadOK)
        {
            Sesion.Instancia.Logout();
            MostrarError("El sistema no está disponible.");
            return;
        }
        UsuarioBLL accesoUsuarioBLL = new UsuarioBLL();
        LoginResultado resultado = accesoUsuarioBLL.ValidarLogin(email, contrasenia, pEscribir: true);
        /*UsuarioBLL accesoUsuarioBLL = new UsuarioBLL();
        DigitoVerificador digitosBLL = new DigitoVerificador();
        bool integridadOK = digitosBLL.ValidarIntegridadDatos();
        string email = tbEmail.Text.Trim();
        string contrasenia = tbContrasenia.Text.Trim();
        LoginResultado resultado = accesoUsuarioBLL.ValidarLogin(email, contrasenia, pEscribir: integridadOK);
        if (resultado.Resultado == ResultadoLogin.Exitoso)
        {
            resultado.Usuario.Perfil = new PermisoBLL().ObtenerPerfil(resultado.Usuario.Perfil.Codigo);
        }
        if (!integridadOK)
        {
            bool puedeRecalcular = resultado.Resultado == ResultadoLogin.Exitoso && resultado.Usuario.Perfil != null && resultado.Usuario.Perfil.TienePermiso(PermisoNombres.RecalcularDigitos);
            if (puedeRecalcular)
            {
                Sesion.Instancia.Login(resultado.Usuario);
                Sesion.Instancia.IntegridadComprometida = true;
                Response.Redirect("PaginaDigitosVerificadores.aspx");
            }
            else { Sesion.Instancia.Logout(); MostrarError("El sistema no está disponible"); }
            return;            
        }*/
        switch (resultado.Resultado)
        {
            case ResultadoLogin.Exitoso:
                if (resultado.Usuario.CodigoEmpresa <= 0)
                {
                    MostrarError("Tu usuario no tiene una empresa asociada. Contactá al administrador.");
                    return;
                }
                resultado.Usuario.Perfil = new PermisoBLL().ObtenerPerfil(resultado.Usuario.Perfil.Codigo);
                Sesion.Instancia.Login(resultado.Usuario);
                SucesoServicio.Instancia.RegistrarSuceso($"{resultado.Usuario.Nombre} {resultado.Usuario.Apellido}", "Inicio de Sesión", "Acceso", 1);
                Response.Redirect("PaginaPrincipal.aspx");                
                break;
            case ResultadoLogin.CredencialesIncorrectas:
                MostrarError("Credenciales incorrectas");
                break;
            case ResultadoLogin.UsuarioRecienBloqueado:
                MostrarError("El usuario ha sido bloqueado");
                break;
            case ResultadoLogin.UsuarioBloqueado:
                MostrarError("Usuario bloqueado");
                break;
            case ResultadoLogin.UsuarioDeshabilitado:
                MostrarError("Usuario deshabilitado");
                break;
        }
    }
    private void MostrarError(string pMensaje)
    {
        lbError.Text = pMensaje;
        lbError.Visible = true;
    }
}