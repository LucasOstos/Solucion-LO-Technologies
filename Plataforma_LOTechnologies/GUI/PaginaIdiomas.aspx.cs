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
        btnAgregarIdioma.ToolTip = btnAgregarIdioma.Enabled ? "" : "Todos los idiomas del catálogo ya están agregados";
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
            ddlNuevoIdioma.Items.Add(new ListItem($"{idioma.Nombre} — {idioma.NombreNativo}", idioma.Codigo));
        }
        if (ddlNuevoIdioma.Items.Count == 0)
        {
            MostrarMensaje("Todos los idiomas del catálogo ya están agregados.", false);
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
        string nombre = ddlNuevoIdioma.SelectedItem == null ? codigo : ddlNuevoIdioma.SelectedItem.Text;
        int pendientes;
        ResultadoAgregarIdioma resultado = idiomaBLL.AgregarIdioma(codigo, out pendientes);
        MostrarModal(false);
        switch (resultado)
        {
            case ResultadoAgregarIdioma.Exitoso:
                MostrarMensaje(pendientes == 0 ? $"Se agregó {nombre} con todas sus traducciones. Revisalas y activá el idioma."
                : $"Se agregó {nombre}. {pendientes} traducciones quedaron pendientes (con el texto en español): completalas desde \"Editar traducciones\" antes de activarlo.", true);
                break;
            case ResultadoAgregarIdioma.YaExiste:
                MostrarMensaje($"{nombre} ya estaba agregado.", false);
                break;
            default:
                MostrarMensaje("El idioma seleccionado no está en el catálogo.", false);
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
            MostrarMensaje("Todos los idiomas ya tienen todas las claves del catálogo.", true);
        }
        else
        {
            string pendientes = resultado.ClavesPendientes == 0 ? "" : $" {resultado.ClavesPendientes} quedaron pendientes (con el texto en español).";
            MostrarMensaje($"Se agregaron {resultado.ClavesAgregadas} traducciones en {resultado.IdiomasActualizados} idioma(s).{pendientes}", true);
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
                if (!pActivar) MostrarMensaje("Idioma desactivado: ya no aparece en el selector de idioma.", true);
                else if (pendientes == 0) MostrarMensaje("Idioma activado: los usuarios ya lo pueden elegir.", true);
                else MostrarMensaje($"Idioma activado. Tiene {pendientes} traducciones pendientes que se van a mostrar en español.", true);
                break;
            case ResultadoCambioEstadoIdioma.EsPredeterminado:
                MostrarMensaje("El idioma predeterminado no se puede desactivar.", false);
                break;
            case ResultadoCambioEstadoIdioma.EnUso:
                MostrarMensaje(usuarios == 1
                    ? "No se puede desactivar: 1 usuario tiene asignado este idioma."
                    : $"No se puede desactivar: {usuarios} usuarios tienen asignado este idioma.", false);
                break;
            default:
                MostrarMensaje("El idioma no existe.", false);
                break;
        }
        CargarIdiomas();
    }
}