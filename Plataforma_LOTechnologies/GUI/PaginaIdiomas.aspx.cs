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
    }
    private void MostrarMensaje(string pTexto, bool pEsExito)
    {
        lblMensaje.Text = pTexto;
        lblMensaje.CssClass = pEsExito ? "mensaje" : "mensaje mensaje-error";
        lblMensaje.Visible = true;
    }
    private const string VALOR_OTRO = "otro";
    private void MostrarModal(bool pMostrar)
    {
        pnlModalIdioma.Style["display"] = pMostrar ? "flex" : "none";
        // Los campos de "Otro idioma" solo se ven si está elegido (ver IdiomasJS.js)
        pnlOtroIdioma.Style["display"] = ddlNuevoIdioma.SelectedValue == VALOR_OTRO ? "block" : "none";
    }
    private void MostrarMensajeModal(string pTexto)
    {
        lblMensajeModal.Text = pTexto;
        lblMensajeModal.Visible = true;
        MostrarModal(true);
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
        // Siempre está la opción de cargar un idioma que no está en la lista
        ddlNuevoIdioma.Items.Add(new ListItem(Traducir("idiomas.otro"), VALOR_OTRO));
        tbCodigoIso.Text = "";
        tbNombreIdioma.Text = "";
        tbNombreNativo.Text = "";
        MostrarModal(true);
    }
    protected void btnCancelarIdioma_Click(object sender, EventArgs e)
    {
        MostrarModal(false);
    }

    protected void btnGuardarIdioma_Click(object sender, EventArgs e)
    {
        bool esOtro = ddlNuevoIdioma.SelectedValue == VALOR_OTRO;
        string codigo = esOtro ? tbCodigoIso.Text.Trim() : ddlNuevoIdioma.SelectedValue;
        string nombre = esOtro ? tbNombreIdioma.Text.Trim() : NombreIdioma(new Idioma { Codigo = codigo, Nombre = codigo });
        int pendientes;
        ResultadoAgregarIdioma resultado = esOtro
            ? idiomaBLL.AgregarOtroIdioma(tbCodigoIso.Text, tbNombreIdioma.Text, tbNombreNativo.Text, out pendientes)
            : idiomaBLL.AgregarIdioma(codigo, out pendientes);
        switch (resultado)
        {
            case ResultadoAgregarIdioma.Exitoso:
                MostrarModal(false);
                if (esOtro && pendientes > 0) MostrarMensaje(Traducir("idiomas.msgAgregadoSinTraduccion", nombre, pendientes), true);
                else if (pendientes == 0) MostrarMensaje(Traducir("idiomas.msgAgregadoCompleto", nombre), true);
                else MostrarMensaje(Traducir("idiomas.msgAgregadoPendientes", nombre, pendientes), true);
                CargarIdiomas();
                break;
            // Los errores de "Otro idioma" se muestran en el modal, sin perder lo escrito
            case ResultadoAgregarIdioma.CodigoInvalido:
                MostrarMensajeModal(Traducir("idiomas.errCodigoInvalido"));
                break;
            case ResultadoAgregarIdioma.CodigoDesconocido:
                MostrarMensajeModal(Traducir("idiomas.errCodigoDesconocido", codigo));
                break;
            case ResultadoAgregarIdioma.NombreObligatorio:
                MostrarMensajeModal(Traducir("idiomas.errNombreObligatorio"));
                break;
            case ResultadoAgregarIdioma.NombreMuyLargo:
                MostrarMensajeModal(Traducir("idiomas.errNombreLargo", IdiomaBLL.LARGO_MAXIMO_NOMBRE));
                break;
            case ResultadoAgregarIdioma.YaExiste:
                if (esOtro) MostrarMensajeModal(Traducir("idiomas.errYaExiste", codigo));
                else { MostrarModal(false); MostrarMensaje(Traducir("idiomas.errYaExiste", nombre), false); CargarIdiomas(); }
                break;
            default:
                MostrarModal(false);
                MostrarMensaje(Traducir("idiomas.errNoCatalogo"), false);
                CargarIdiomas();
                break;
        }
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