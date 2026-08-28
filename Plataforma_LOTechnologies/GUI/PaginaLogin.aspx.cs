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
        UsuarioBLL accesoUsuarioBLL = new UsuarioBLL();
        DigitoVerificador digitosBLL = new DigitoVerificador();
        bool integridadOK = digitosBLL.ValidarIntegridadDatos();
        string email = tbEmail.Text.Trim();
        string contrasenia = tbContrasenia.Text.Trim();
        LoginResultado resultado = accesoUsuarioBLL.ValidarLogin(email, contrasenia, pEscribir: integridadOK);

        if (!integridadOK)
        {
            if(resultado.Resultado == ResultadoLogin.Exitoso && resultado.Usuario.Rol == 1)
            {
                Sesion.Instancia.Login(resultado.Usuario);
                Response.Redirect("PaginaDigitosVerificadores.aspx");
            }
            else { Sesion.Instancia.Logout(); MostrarError("El sistema no está disponible"); } return;
        }
        switch (resultado.Resultado)
        {
            case ResultadoLogin.Exitoso:
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