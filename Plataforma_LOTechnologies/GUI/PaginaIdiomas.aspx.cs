using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BE;
using BLL;

public partial class PaginaIdiomas : PaginaSegura
{
    private readonly IdiomaBLL idiomaBLL = new IdiomaBLL();
    protected override string PermisoRequerido
    {
        get { return PermisoNombres.GestionarIdiomas; }
    }
    protected override string ClaveTitulo
    {
        get { return "idiomas.titulo"; }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CargarIdiomas();
        }
    }
    #region Funciones
    private void CargarIdiomas()
    {
        rptIdiomas.DataSource = idiomaBLL.ObtenerIdiomas();
        rptIdiomas.DataBind();
        btnAgregarIdioma.Enabled = idiomaBLL.ObtenerIdiomasParaAgregar().Count > 0;
        btnAgregarIdioma.ToolTip = btnAgregarIdioma.Enabled ? "" : Traducir("idiomas.todosAgregados");
    }
    private void MostrarMensaje(string pTexto, bool pEsExito)
    {
        lblMensaje.Text = pTexto;
        lblMensaje.CssClass = pEsExito ? "mensaje" : "mensaje mensaje-error";
        lblMensaje.Visible = true;
    }
    private void MostrarModal(bool pMostrar)
    {
        pnlModalIdioma.Style["display"] = pMostrar ? "flex" : "none";
    }
    public string ObtenerClaseCompletitud(object completitud)
    {
        int valor = Convert.ToInt32(completitud);
        if (valor >= 90) return "idi-completitud-alta";
        if (valor >= 60) return "idi-completitud-media";
        return "idi-completitud-baja";
    }
    #endregion

    protected void btnAgregarIdioma_Click(object sender, EventArgs e)
    {
        ddlNuevoIdioma.Items.Clear();
        foreach (Idioma idioma in idiomaBLL.ObtenerIdiomasParaAgregar())
        {
            ddlNuevoIdioma.Items.Add(new ListItem($"{NombreIdioma(idioma)} — {idioma.NombreNativo}", idioma.Codigo));
        }
        if (ddlNuevoIdioma.Items.Count == 0)
        {
            MostrarMensaje(Traducir("idiomas.todosAgregados"), false);
            return;
        }
        MostrarModal(true);
    }
    protected void btnCancelarIdioma_Click(object sender, EventArgs e)
    {
        MostrarModal(false);
    }

    protected void btnGuardarIdioma_Click(object sender, EventArgs e)
    {
        string codigo = ddlNuevoIdioma.SelectedValue;
        string nombre = NombreIdioma(new Idioma { Codigo = codigo, Nombre = codigo });
        int pendientes;
        ResultadoAgregarIdioma resultado = idiomaBLL.AgregarIdioma(codigo, out pendientes);
        MostrarModal(false);
        switch (resultado)
        {
            case ResultadoAgregarIdioma.Exitoso:
                MostrarMensaje(pendientes == 0 ? Traducir("idiomas.msgAgregadoCompleto", nombre)
                : Traducir("idiomas.msgAgregadoPendientes", nombre, pendientes), true);
                break;
            case ResultadoAgregarIdioma.YaExiste:
                MostrarMensaje(Traducir("idiomas.errYaExiste", nombre), false);
                break;
            default:
                MostrarMensaje(Traducir("idiomas.errNoCatalogo"), false);
                break;
        }
        CargarIdiomas();
    }

    protected void rptIdiomas_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int codigoIdioma = Convert.ToInt32(e.CommandArgument);
        if (e.CommandName == "EditarTraducciones")
        {
            Response.Redirect($"PaginaEditarTraducciones.aspx?idioma={codigoIdioma}", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }
        if (e.CommandName == "Activar" || e.CommandName == "Desactivar")
        {
            CambiarEstado(codigoIdioma, e.CommandName == "Activar");
        }
    }
    protected void btnSincronizar_Click(object sender, EventArgs e)
    {
        ResultadoSincronizacion resultado = idiomaBLL.SincronizarClaves();
        if (resultado.ClavesAgregadas == 0)
        {
            MostrarMensaje(Traducir("idiomas.msgSincronizadoNada"), true);
        }
        else
        {
            string pendientes = resultado.ClavesPendientes == 0 ? "" : " " + Traducir("idiomas.msgSincronizadoPendientes", resultado.ClavesPendientes);
            MostrarMensaje(Traducir("idiomas.msgSincronizado", resultado.ClavesAgregadas, resultado.IdiomasActualizados) + pendientes, true);
        }
        CargarIdiomas();
    }
    private void CambiarEstado(int pCodigoIdioma, bool pActivar)
    {
        int usuarios, pendientes;
        ResultadoCambioEstadoIdioma resultado = idiomaBLL.CambiarEstado(pCodigoIdioma, pActivar, out usuarios, out pendientes);
        switch (resultado)
        {
            case ResultadoCambioEstadoIdioma.Exitoso:
                if (!pActivar) MostrarMensaje(Traducir("idiomas.msgDesactivado"), true);
                else if (pendientes == 0) MostrarMensaje(Traducir("idiomas.msgActivado"), true);
                else MostrarMensaje(Traducir("idiomas.msgActivadoPendientes", pendientes), true);
                break;
            case ResultadoCambioEstadoIdioma.EsPredeterminado:
                MostrarMensaje(Traducir("idiomas.errPredeterminado"), false);
                break;
            case ResultadoCambioEstadoIdioma.EnUso:
                MostrarMensaje(Traducir("idiomas.errEnUso", usuarios), false);
                break;
            default:
                MostrarMensaje(Traducir("idiomas.errNoExiste"), false);
                break;
        }
        CargarIdiomas();
    }
}