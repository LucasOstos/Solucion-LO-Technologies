using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BE;
using BLL;

public partial class PaginaEditarTraducciones : PaginaSegura
{
    private readonly IdiomaBLL idiomaBLL = new IdiomaBLL();
    private Idioma idioma;
    protected override string PermisoRequerido
    {
        get { return PermisoNombres.GestionarIdiomas; }
    }
    protected bool EsIdiomaBase
    {
        get { return idioma != null && idioma.Predeterminado; }
    }
    protected string NombreIdiomaBase
    {
        get { return NombreIdioma(SERVICIO.Logica.GestorIdioma.Instancia.IdiomaPredeterminado); }
    }
    protected override string ClaveTitulo
    {
        get { return "traducciones.titulo"; }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        int codigoIdioma;
        idioma = int.TryParse(Request.QueryString["idioma"], out codigoIdioma) ? idiomaBLL.ObtenerIdioma(codigoIdioma) : null;
        if (idioma == null)
        {
            Response.Redirect("~/PaginaIdiomas.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }
        if (!IsPostBack)
        {
            List<TraduccionComparada> todas = idiomaBLL.ObtenerTraducciones(idioma.CodigoIdioma);
            CargarModulos(todas);
            lblAvisoBase.Visible = EsIdiomaBase;
            spnSoloPendientes.Visible = !EsIdiomaBase;
            chkSoloPendientes.Checked = !EsIdiomaBase && todas.Any(t => t.Pendiente);
            CargarTraducciones(todas);
        }
    }
    #region Funciones
    private void CargarModulos(List<TraduccionComparada> pTodas)
    {
        ddlModulo.Items.Clear();
        ddlModulo.Items.Add(new ListItem(Traducir("traducciones.todosModulos"), ""));
        foreach (string modulo in pTodas.Select(t => t.Modulo).Distinct().OrderBy(m => m))
        {
            ddlModulo.Items.Add(new ListItem(modulo, modulo));
        }
    }
    private void CargarTraducciones(List<TraduccionComparada> pTodas = null)
    {
        List<TraduccionComparada> todas = pTodas ?? idiomaBLL.ObtenerTraducciones(idioma.CodigoIdioma);
        int pendientes = todas.Count(t => t.Pendiente);

        litTitulo.Text = HttpUtility.HtmlEncode(Traducir("traducciones.encabezado", NombreIdioma(idioma)))
                       + $" <span class=\"trd-titulo-nativo\" lang=\"{HttpUtility.HtmlAttributeEncode(idioma.Codigo)}\">{HttpUtility.HtmlEncode(idioma.NombreNativo)}</span>";
        litResumen.Text = EsIdiomaBase ? Traducir("traducciones.resumenBase", todas.Count)
            : Traducir("traducciones.resumen", todas.Count, pendientes, Traducir(idioma.Estado ? "comun.activo" : "comun.inactivo"));

        List<TraduccionComparada> filtradas = idiomaBLL.FiltrarTraducciones(todas, tbBuscar.Text, ddlModulo.SelectedValue, chkSoloPendientes.Checked);
        rptTraducciones.DataSource = filtradas;
        rptTraducciones.DataBind();
        rptTraducciones.Visible = filtradas.Count > 0;
        lblSinResultados.Visible = filtradas.Count == 0;
        lblSinResultados.Text = chkSoloPendientes.Checked && pendientes == 0 && tbBuscar.Text.Trim().Length == 0 && ddlModulo.SelectedValue == ""
            ? Traducir("traducciones.sinPendientes") : Traducir("traducciones.sinResultados");
        btnGuardar.Visible = filtradas.Count > 0;
    }
    private void MostrarMensaje(string pTexto, bool pEsExito)
    {
        lblMensaje.Text = pTexto;
        lblMensaje.CssClass = pEsExito ? "mensaje" : "mensaje mensaje-error";
        lblMensaje.Visible = true;
    }
    private string ListarClaves(List<string> pClaves)
    {
        string lista = string.Join(", ", pClaves.Take(5));
        return pClaves.Count > 5 ? Traducir("traducciones.yMas", lista, pClaves.Count - 5) : lista;
    }
    #endregion

    protected void rptTraducciones_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;
        TraduccionComparada traduccion = (TraduccionComparada)e.Item.DataItem;
        TextBox tbTexto = (TextBox)e.Item.FindControl("tbTexto");
        tbTexto.Attributes["lang"] = idioma.Codigo;
        tbTexto.Attributes["dir"] = "auto";
        tbTexto.Attributes["aria-label"] = Traducir("traducciones.ariaTraduccion", traduccion.Clave);
    }
    protected void btnBuscar_Click(object sender, EventArgs e)
    {
        CargarTraducciones();
    }
    protected void btnGuardar_Click(object sender, EventArgs e)
    {
        List<Traduccion> editadas = new List<Traduccion>();
        foreach (RepeaterItem item in rptTraducciones.Items)
        {
            HiddenField hfCodigo = (HiddenField)item.FindControl("hfCodigo");
            TextBox tbTexto = (TextBox)item.FindControl("tbTexto");
            CheckBox chkRevisada = (CheckBox)item.FindControl("chkRevisada");
            int codigo;
            if (!int.TryParse(hfCodigo.Value, out codigo)) continue;
            editadas.Add(new Traduccion { CodigoTraduccion = codigo, Texto = tbTexto.Text, Pendiente = !EsIdiomaBase && !chkRevisada.Checked });
        }

        ResultadoGuardarTraducciones resultado = idiomaBLL.GuardarTraducciones(idioma.CodigoIdioma, editadas);
        if (!resultado.Exitoso)
        {
            // No se guardó nada, se dejan los textos como los escribió para que los corrija
            List<string> problemas = new List<string>();
            if (resultado.ClavesVacias.Count > 0) problemas.Add(Traducir("traducciones.errVacio", ListarClaves(resultado.ClavesVacias)));
            if (resultado.ClavesMuyLargas.Count > 0) problemas.Add(Traducir("traducciones.errLargo", IdiomaBLL.LARGO_MAXIMO_TEXTO, ListarClaves(resultado.ClavesMuyLargas)));
            if (resultado.ClavesMarcadores.Count > 0) problemas.Add(Traducir("traducciones.errMarcadores", ListarClaves(resultado.ClavesMarcadores)));
            MostrarMensaje(Traducir("traducciones.errNoGuardado") + " " + string.Join(" ", problemas), false);
            return;
        }
        MostrarMensaje(resultado.Modificadas == 0 ? Traducir("traducciones.msgSinCambios") : resultado.Modificadas == 1 ? Traducir("traducciones.msgGuardada") : Traducir("traducciones.msgGuardadas", resultado.Modificadas), true);
        CargarTraducciones();
    }
}