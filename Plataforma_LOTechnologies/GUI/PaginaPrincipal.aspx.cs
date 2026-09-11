using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BE;
using BLL;
using SERVICIO.Logica;

public partial class PaginaPrincipal : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Sesion.Instancia.IsLogueado())
        {
            Response.Redirect("PaginaLogin.aspx");
            return;
        }

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
        litRolUsuario.Text = ObtenerNombreRol(usuario.Rol);
        litInicialesUsuario.Text = ObtenerIniciales(usuario.Nombre, usuario.Apellido);
    }
    private string ObtenerNombreRol(int rol)
    {
        switch (rol)
        {
            case 1: return "Administrador";
            case 2: return "Analista";
            case 3: return "Gerente";
            default: return "";
        }
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
        MostrarMensajeContrasenia("Contraseña actualizada correctamente.", esExito: true);
        tbContraseniaActual.Text = "";
        tbContraseniaNueva.Text = "";
        tbConfirmarContrasenia.Text = "";
    }
    protected void btnUsuarios_Click(object sender, EventArgs e)
    {
        Response.Redirect("PaginaUsuarios.aspx");
    }

    protected void btnDigitos_Click(object sender, EventArgs e)
    {
        Response.Redirect("PaginaDigitosVerificadores.aspx");
    }
    protected void btnBackupRestore_Click(object sender, EventArgs e)
    {
        Response.Redirect("PaginaBackupRestore.aspx");
    }

    protected void btnAuditoria_Click(object sender, EventArgs e)
    {
        Response.Redirect("PaginaAuditorias.aspx");
    }

    protected void btnIdiomas_Click(object sender, EventArgs e)
    {
        Response.Redirect("PaginaIdiomas.aspx");
    }
    protected void btnEmpresas_Click(object sender, EventArgs e)
    {
        Response.Redirect("PaginaEmpresas.aspx");
    }
    protected void btnLocales_Click(object sender, EventArgs e)
    {
        Response.Redirect("PaginaLocales.aspx");
    }

    protected void btnLayoutSectores_Click(object sender, EventArgs e)
    {
        Response.Redirect("PaginaLayoutSectores.aspx");
    }
    protected void btnProductos_Click(object sender, EventArgs e)
    {
        Response.Redirect("PaginaProductos.aspx");
    }
    protected void btnStockVentas_Click(object sender, EventArgs e)
    {
        Response.Redirect("PaginaStockVentas.aspx");
    }
    protected void btnEscenarios_Click(object sender, EventArgs e)
    {
        Response.Redirect("PaginaEscenarios.aspx");
    }
    protected void btnComparacionEscenarios_Click(object sender, EventArgs e)
    {
        Response.Redirect("PaginaComparacionEscenarios.aspx");
    }
    protected void btnIndicadores_Click(object sender, EventArgs e)
    {
        Response.Redirect("PaginaIndicadores.aspx");
    }
    protected void btnAnalisisProductos_Click(object sender, EventArgs e)
    {
        Response.Redirect("PaginaAnalisisProductos.aspx");
    }
    protected void btnReportes_Click(object sender, EventArgs e)
    {
        Response.Redirect("PaginaReportes.aspx");
    }

    protected void btnPermisos_Click(object sender, EventArgs e)
    {
        Response.Redirect("PaginaPermisos.aspx");
    }
}