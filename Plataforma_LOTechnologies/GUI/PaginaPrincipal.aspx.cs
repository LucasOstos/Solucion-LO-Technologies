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
    protected override string ClaveTitulo
    {
        get { return "principal.titulo"; }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CargarHeroUsuario();
            CargarIdiomas();
        }        
    }
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        List<Idioma> idiomas = GestorIdioma.Instancia.ObtenerIdiomas();
        foreach (ListItem item in ddlIdioma.Items)
        {
            Idioma idioma = idiomas.FirstOrDefault(i => i.CodigoIdioma.ToString() == item.Value);
            if (idioma != null) item.Attributes["lang"] = idioma.Codigo;
        }
    }
    #region Funciones
    private void CargarHeroUsuario()
    {
        Usuario usuario = Sesion.Instancia.Usuario;
        litNombreUsuario.Text = $"{usuario.Nombre} {usuario.Apellido}";
        litRolUsuario.Text = usuario.Perfil.Nombre;
        btnAbrirCambiarContrasenia.Visible = !usuario.EsSuperUsuario;
        litInicialesUsuario.Text = ObtenerIniciales(usuario.Nombre, usuario.Apellido);
    }
    private void CargarIdiomas()
    {
        ddlIdioma.Items.Clear();
        foreach (Idioma idioma in GestorIdioma.Instancia.ObtenerIdiomasActivos())
        {
            ddlIdioma.Items.Add(new ListItem(idioma.NombreNativo, idioma.CodigoIdioma.ToString()));
        }
        Idioma actual = IdiomaActual;
        if (actual != null && ddlIdioma.Items.FindByValue(actual.CodigoIdioma.ToString()) != null)
        {
            ddlIdioma.SelectedValue = actual.CodigoIdioma.ToString();
        }
        pnlIdioma.Visible = ddlIdioma.Items.Count > 1;
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
    protected void ddlIdioma_SelectedIndexChanged(object sender, EventArgs e)
    {
        int codigoIdioma;
        if (!int.TryParse(ddlIdioma.SelectedValue, out codigoIdioma)) return;
        try
        {
            GestorIdioma.Instancia.CambiarIdioma(codigoIdioma);
        }
        catch (ArgumentException)
        {
            CargarIdiomas();
        }
    }
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