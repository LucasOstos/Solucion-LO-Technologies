using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BE;
using BLL;
using SERVICIO.Logica;

public partial class PaginaAnalisisProductos : PaginaSegura
{
    protected override string PermisoRequerido
    {
        get { return PermisoNombres.AnalizarProductos; }
    }
    protected override string ClaveTitulo
    {
        get { return "analisis.titulo"; }
    }
    private LocalBLL localBLL = new LocalBLL();
    private AnalisisProductoBLL analisisBLL = new AnalisisProductoBLL();
    private List<ProductoAnalisisListado> ultimoResultado
    {
        get { return (List<ProductoAnalisisListado>)(ViewState["UltimoResultado"] ?? new List<ProductoAnalisisListado>()); }
        set { ViewState["UltimoResultado"] = value; }
    }
    private int CodigoLocalAnalizado
    {
        get { return ViewState["CodigoLocalAnalizado"] == null ? 0 : (int)ViewState["CodigoLocalAnalizado"]; }
        set { ViewState["CodigoLocalAnalizado"] = value; }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        tbPeriodoHasta.Attributes["max"] = DateTime.Today.ToString("yyyy-MM-dd");
        tbPeriodoDesde.Attributes["max"] = DateTime.Today.ToString("yyyy-MM-dd");
        if (!IsPostBack)
        {
            CargarDropdownLocales();
            // Por defecto: los últimos 3 meses (el actual y los 2 anteriores) hasta hoy
            tbPeriodoDesde.Text = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-2).ToString("yyyy-MM-dd");
            tbPeriodoHasta.Text = DateTime.Today.ToString("yyyy-MM-dd");
        }
    }

    #region Funciones
    private void CargarDropdownLocales()
    {
        List<LocalListado> locales = localBLL.ObtenerLocalesListado("", null, "Activo");
        ddlLocal.Items.Clear();
        foreach (LocalListado local in locales)
        {
            ddlLocal.Items.Add(new ListItem(local.Nombre, local.CodigoLocal.ToString()));
        }
    }
    private void MostrarResultados(List<ProductoAnalisisListado> resultado)
    {
        litProductosAnalizados.Text = resultado.Select(r => r.CodigoProducto).Distinct().Count().ToString();
        litPrioridadAlta.Text = resultado.Count(r => r.Prioridad == "Alta").ToString();
        litPrioridadMedia.Text = resultado.Count(r => r.Prioridad == "Media").ToString();
        litPrioridadBaja.Text = resultado.Count(r => r.Prioridad == "Baja").ToString();
        AplicarFiltroYBindear();
        pnlResultados.Visible = true;
    }
    private void AplicarFiltroYBindear()
    {
        string busqueda = tbBuscarProducto.Text.Trim().ToLower();
        string prioridad = ddlFiltroPrioridad.SelectedValue;
        string tipo = ddlFiltroTipo.SelectedValue;
        var filtrados = ultimoResultado.Where(r => string.IsNullOrEmpty(busqueda) || r.NombreProducto.ToLower().Contains(busqueda)
            || r.NombreCategoria.ToLower().Contains(busqueda) || (r.CodigoInterno ?? "").ToLower().Contains(busqueda)).ToList();
        var candidatos = filtrados.Where(r => r.Tipo != null);
        if (!string.IsNullOrEmpty(prioridad))
        {
            candidatos = candidatos.Where(r => r.Prioridad == prioridad);
        }
        if (!string.IsNullOrEmpty(tipo))
        {
            candidatos = candidatos.Where(r => r.Tipo == tipo);
        }
        var listaCandidatos = candidatos.ToList();
        if (listaCandidatos.Count == 0)
        {
            rptCandidatos.Visible = false;
            lblSinCandidatos.Visible = true;
        }
        else
        {
            rptCandidatos.Visible = true;
            lblSinCandidatos.Visible = false;
            rptCandidatos.DataSource = listaCandidatos;
            rptCandidatos.DataBind();
        }
        var sinRecomendacion = filtrados.Where(r => r.Tipo == null).ToList();
        rptSinRecomendacion.DataSource = sinRecomendacion;
        rptSinRecomendacion.DataBind();
        lblSinAnalizados.Visible = sinRecomendacion.Count == 0;
    }
    protected string TraducirTipo(object pTipo)
    {
        switch (pTipo as string)
        {
            case ProductoAnalisisListado.TIPO_ESTRELLA: return Traducir("analisis.tipoEstrellaEscondida");
            case ProductoAnalisisListado.TIPO_LUGAR_PRIVILEGIADO: return Traducir("analisis.tipoLugarPrivilegiado");
            case ProductoAnalisisListado.TIPO_SIN_MOVIMIENTO: return Traducir("analisis.tipoSinMovimiento");
            case ProductoAnalisisListado.TIPO_RIESGO_QUIEBRE: return Traducir("analisis.tipoRiesgoQuiebre");
            case ProductoAnalisisListado.TIPO_SIN_UBICAR: return Traducir("analisis.tipoSinUbicar");
            default: return "";
        }
    }
    protected string MostrarMotivo(object pDato)
    {
        ProductoAnalisisListado item = pDato as ProductoAnalisisListado;
        if (item == null || string.IsNullOrEmpty(item.ClaveMotivo)) return "";
        return Traducir(item.ClaveMotivo, item.Parametros ?? new object[0]);
    }
    protected string MostrarSector(object pSector)
    {
        string sector = pSector as string;
        return string.IsNullOrEmpty(sector) ? Traducir("productos.sinUbicar") : sector;
    }
    protected string MostrarMargen(object pMargen)
    {
        return pMargen == null ? "-" : Convert.ToDecimal(pMargen).ToString("0.#") + "%";
    }
    protected string MostrarCobertura(object pCobertura)
    {
        return pCobertura == null ? "-" : Traducir("indicadores.dias", Convert.ToDecimal(pCobertura).ToString("#,##0"));
    }
    protected string UrlCrearEscenario(object pDato)
    {
        ProductoAnalisisListado item = pDato as ProductoAnalisisListado;
        if (item == null || item.CodigoSectorSugerido == null || string.IsNullOrEmpty(item.AccionEscenario) || CodigoLocalAnalizado == 0) return null;
        if (!Sesion.Instancia.TienePermiso(PermisoNombres.GestionarEscenarios)) return null;
        return ResolveUrl($"~/PaginaEscenarios.aspx?local={CodigoLocalAnalizado}&sector={item.CodigoSectorSugerido}&accion={HttpUtility.UrlEncode(item.AccionEscenario)}");
    }
    private void MostrarMensaje(string texto, bool esExito)
    {
        lblMensaje.Text = texto;
        lblMensaje.CssClass = esExito ? "mensaje" : "mensaje mensaje-error";
        lblMensaje.Visible = true;
    }
    #endregion

    protected void Filtro_Changed(object sender, EventArgs e)
    {
        if (ultimoResultado.Count > 0)
        {
            AplicarFiltroYBindear();
            pnlResultados.Visible = true;
        }
    }

    protected void btnDetectar_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(ddlLocal.SelectedValue))
        {
            MostrarMensaje(Traducir("stock.errSeleccionarLocal"), esExito: false);
            return;
        }
        DateTime periodoDesde, periodoHasta;
        if (!DateTime.TryParse(tbPeriodoDesde.Text, out periodoDesde) || !DateTime.TryParse(tbPeriodoHasta.Text, out periodoHasta))
        {
            MostrarMensaje(Traducir("analisis.errPeriodo"), esExito: false);
            pnlResultados.Visible = false;
            return;
        }
        if (periodoDesde > periodoHasta)
        {
            MostrarMensaje(Traducir("analisis.errPeriodo"), esExito: false);
            pnlResultados.Visible = false;
            return;
        }
        if (periodoDesde.Date > DateTime.Today)
        {
            MostrarMensaje(Traducir("analisis.errPeriodo"), esExito: false);
            pnlResultados.Visible = false;
            return;
        }
        int codigoLocal = int.Parse(ddlLocal.SelectedValue);
        // Las ventas se filtran por fecha desde y hasta inclusive (hasta no puede pasar de hoy)
        List<ProductoAnalisisListado> resultado = analisisBLL.DetectarOportunidades(codigoLocal, periodoDesde, periodoHasta);
        CodigoLocalAnalizado = codigoLocal;
        if (resultado.Count == 0)
        {
            MostrarMensaje(Traducir("analisis.errSinDatos"), esExito: false);
            pnlResultados.Visible = false;
            return;
        }
        ultimoResultado = resultado;
        MostrarResultados(resultado);
    }
}