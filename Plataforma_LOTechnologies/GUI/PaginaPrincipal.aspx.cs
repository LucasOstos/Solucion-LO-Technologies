using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BE;
using BLL;
using SERVICIO.Logica;

public partial class PaginaPrincipal : PaginaSegura
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CargarHeroUsuario();
        }
    }
    #region Funciones
    private void CargarHeroUsuario()
    {
        Usuario usuario = Sesion.Instancia.Usuario;

        litNombreUsuario.Text = $"{usuario.Nombre} {usuario.Apellido}";
        litRolUsuario.Text = usuario.Perfil.Nombre;
        litInicialesUsuario.Text = ObtenerIniciales(usuario.Nombre, usuario.Apellido);
    }
    private string ObtenerIniciales(string nombre, string apellido)
    {
        string inicialNombre = string.IsNullOrEmpty(nombre) ? "" : nombre.Substring(0, 1);
        string inicialApellido = string.IsNullOrEmpty(apellido) ? "" : apellido.Substring(0, 1);
        return (inicialNombre + inicialApellido).ToUpper();
    }
    private void MostrarMensajeContrasenia(string texto, bool esExito)
    {
        lblMensajeContrasenia.Text = texto;
        lblMensajeContrasenia.CssClass = esExito ? "mensaje" : "mensaje mensaje-error";
        lblMensajeContrasenia.Visible = true;
    }
    private void MostrarMensajePrincipal(string texto, bool esExito)
    {
        lblMensajePrincipal.Text = texto;
        lblMensajePrincipal.CssClass = esExito ? "mensaje" : "mensaje mensaje-error";
        lblMensajePrincipal.Visible = true;
    }
    #endregion
    protected void btnLogout_Click(object sender, EventArgs e)
    {
        Usuario usuario = Sesion.Instancia.Usuario;
        SucesoServicio.Instancia.RegistrarSuceso($"{usuario.Nombre} {usuario.Apellido}", "Cierre de Sesión", "Acceso", 1);
        Sesion.Instancia.Logout();
        Response.Redirect("PaginaLogin.aspx");
    }
    protected void btnCancelarContrasenia_Click(object sender, EventArgs e)
    {
        pnlCambiarContrasenia.Style["display"] = "none";
    }
    protected void btnGuardarContrasenia_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid)
        {
            pnlCambiarContrasenia.Style["display"] = "flex";
            return;
        }
        UsuarioBLL usuarioBLL = new UsuarioBLL();
        Usuario usuario = Sesion.Instancia.Usuario;
        LoginResultado validacionActual = usuarioBLL.ValidarLogin(usuario.Email, tbContraseniaActual.Text.Trim(), pEscribir: false);
        if (validacionActual.Resultado != ResultadoLogin.Exitoso)
        {
            MostrarMensajeContrasenia("La contraseña actual no es correcta.", esExito: false);
            pnlCambiarContrasenia.Style["display"] = "flex";
            return;
        }
        usuarioBLL.CambiarContrasenia(usuario.DNI, tbContraseniaNueva.Text.Trim());
        SucesoServicio.Instancia.RegistrarSuceso($"{usuario.Nombre} {usuario.Apellido}", "Cambio de contraseña", "Acceso", 1);
        pnlCambiarContrasenia.Style["display"] = "none";
        MostrarMensajePrincipal("Contraseña actualizada correctamente.", esExito: true);
        tbContraseniaActual.Text = "";
        tbContraseniaNueva.Text = "";
        tbConfirmarContrasenia.Text = "";
    }   
}