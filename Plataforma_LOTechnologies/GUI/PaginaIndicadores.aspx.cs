using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BE;
using BLL;
using SERVICIO.Logica;

public partial class PaginaIndicadores : PaginaSegura
{
    protected override string PermisoRequerido
    {
        get { return PermisoNombres.VisualizarIndicadores; }
    }
    protected override string ClaveTitulo
    {
        get { return "indicadores.titulo"; }
    }
    private LocalBLL localBLL = new LocalBLL();
    private StockBLL stockBLL = new StockBLL();
    private IndicadorBLL indicadorBLL = new IndicadorBLL();
    private RecomendacionBLL recomendacionBLL = new RecomendacionBLL();
    private VentaBLL ventaBLL = new VentaBLL();
    private SectorBLL sectorBLL = new SectorBLL();
    private LayoutBLL layoutBLL = new LayoutBLL();
    private int CodigoLocalActual
    {
        get { return string.IsNullOrEmpty(ddlLocal.SelectedValue) ? 0 : int.Parse(ddlLocal.SelectedValue); }
    }
    protected void Page_Load(object sender, EventArgs e)
    {        
        if (!IsPostBack)
        {
            CargarDropdownLocales();
            AplicarFiltros();
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
    private const int MESES_EVOLUCION = 6;
    private void AplicarFiltros()
    {
        if (CodigoLocalActual == 0) return;
        int meses = MesesDelFiltro(ddlPeriodo.SelectedValue);
        string periodoDesde = PeriodoDesde(meses);
        litRangoPeriodo.Text = HttpUtility.HtmlEncode(Traducir("stock.rangoPeriodo", DescribirPeriodo(meses)));
        List<IndicadorSectorListado> indicadores = indicadorBLL.CalcularIndicadoresPorSector(CodigoLocalActual, periodoDesde);
        decimal ventasLocal = MostrarResumenLocal(periodoDesde);
        decimal ventasAnteriorLocal = indicadorBLL.CompletarVariacion(CodigoLocalActual, meses, indicadores);
        litVariacionLocal.Text = MostrarVariacion(IndicadorBLL.Variacion(ventasLocal, ventasAnteriorLocal), ventasLocal, ventasAnteriorLocal)
            + " " + HttpUtility.HtmlEncode(Traducir("indicadores.vsAnterior"));
        MostrarEvolucion();
        if (indicadores.Count == 0)
        {
            rptIndicadoresSector.Visible = false;
            lblSinIndicadores.Visible = true;
            rptRecomendaciones.Visible = false;
            lblSinRecomendaciones.Visible = true;
            pnlMapa.Visible = false;
            return;
        }
        rptIndicadoresSector.Visible = true;
        lblSinIndicadores.Visible = false;
        rptIndicadoresSector.DataSource = indicadores;
        rptIndicadoresSector.DataBind();
        MostrarMapa(indicadores);
        if (meses == 1) indicadorBLL.GuardarIndicadores(CodigoLocalActual, DateTime.Now.ToString("yyyy-MM"), indicadores);
        List<StockListado> stock = stockBLL.ObtenerStockPorLocal(CodigoLocalActual);
        List<RecomendacionBE> recomendaciones = recomendacionBLL.GenerarRecomendaciones(CodigoLocalActual, indicadores, stock);
        if (recomendaciones.Count == 0)
        {
            rptRecomendaciones.Visible = false;
            lblSinRecomendaciones.Visible = true;
        }
        else
        {
            rptRecomendaciones.Visible = true;
            lblSinRecomendaciones.Visible = false;
            rptRecomendaciones.DataSource = recomendaciones;
            rptRecomendaciones.DataBind();
        }
        if (meses == 1) recomendacionBLL.GuardarRecomendaciones(CodigoLocalActual, recomendaciones);
    }
    protected string UrlCrearEscenario(object pDato)
    {
        RecomendacionBE recomendacion = pDato as RecomendacionBE;
        if (recomendacion == null || recomendacion.CodigoSector == null || string.IsNullOrEmpty(recomendacion.AccionEscenario)) return null;
        if (!Sesion.Instancia.TienePermiso(PermisoNombres.GestionarEscenarios)) return null;
        return ResolveUrl($"~/PaginaEscenarios.aspx?local={CodigoLocalActual}&sector={recomendacion.CodigoSector}&accion={HttpUtility.UrlEncode(recomendacion.AccionEscenario)}");
    }    
    protected string MostrarVariacion(object pVariacion, object pActual, object pAnterior)
    {
        decimal actual = pActual == null ? 0 : Convert.ToDecimal(pActual);
        decimal anterior = pAnterior == null ? 0 : Convert.ToDecimal(pAnterior);
        if (pVariacion == null)
        {
            if (anterior <= 0 && actual > 0) return "<span class='ind-variacion ind-variacion-nuevo'>" + HttpUtility.HtmlEncode(Traducir("indicadores.nuevo")) + "</span>";
            return "-";
        }
        decimal variacion = Convert.ToDecimal(pVariacion);
        if (variacion == 0) return "<span class='ind-variacion'>= 0%</span>";
        bool sube = variacion > 0;
        string texto = (sube ? "&#9650; +" : "&#9660; ") + variacion.ToString("0.#") + "%";
        return "<span class='ind-variacion " + (sube ? "ind-variacion-sube" : "ind-variacion-baja") + "'>" + texto + "</span>";
    }
    protected string MostrarCobertura(object pCobertura, object pStock)
    {
        if (pCobertura != null) return HttpUtility.HtmlEncode(Traducir("indicadores.dias", Convert.ToDecimal(pCobertura).ToString("#,##0")));
        int stock = pStock == null ? 0 : Convert.ToInt32(pStock);
        return stock > 0 ? HttpUtility.HtmlEncode(Traducir("indicadores.sinVentasCobertura")) : "-";
    }    
    private void MostrarEvolucion()
    {
        List<VentaMensual> meses = indicadorBLL.ObtenerEvolucionVentas(CodigoLocalActual, MESES_EVOLUCION);
        decimal maximo = meses.Max(m => m.Monto);
        if (maximo <= 0)
        {
            pnlEvolucion.Visible = false;
            return;
        }
        var sb = new StringBuilder("<div class='ind-evolucion' role='list'>");
        foreach (VentaMensual mes in meses)
        {
            decimal alto = Math.Round(mes.Monto / maximo * 100, 1);
            string etiqueta = DateTime.ParseExact(mes.Periodo + "-01", "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture).ToString("MM/yyyy");
            string titulo = etiqueta + ": " + mes.Monto.ToString("$#,##0") + (mes.EnCurso ? " (" + Traducir("indicadores.enCurso") + ")" : "");
            sb.Append("<div class='ind-evolucion-mes' role='listitem' title='").Append(HttpUtility.HtmlAttributeEncode(titulo)).Append("'>")
              .Append("<span class='ind-evolucion-valor'>").Append(mes.Monto.ToString("$#,##0")).Append("</span>")
              .Append("<span class='ind-evolucion-pista'><span class='ind-evolucion-barra").Append(mes.EnCurso ? " ind-evolucion-en-curso" : "")
              .Append("' style='height:").Append(alto.ToString(System.Globalization.CultureInfo.InvariantCulture)).Append("%'></span></span>")
              .Append("<span class='ind-evolucion-etiqueta'>").Append(etiqueta);
            if (mes.EnCurso) sb.Append("<br /><span class='texto-secundario'>").Append(HttpUtility.HtmlEncode(Traducir("indicadores.enCurso"))).Append("</span>");
            sb.Append("</span></div>");
        }
        sb.Append("</div>");
        litEvolucion.Text = sb.ToString();
        pnlEvolucion.Visible = true;
    }
    protected string TextoRecomendacion(object pDato, bool pEsMotivo)
    {
        RecomendacionBE recomendacion = pDato as RecomendacionBE;
        if (recomendacion == null) return "";
        string clave = pEsMotivo ? recomendacion.ClaveMotivo : recomendacion.ClaveDescripcion;
        string texto = pEsMotivo ? recomendacion.Motivo : recomendacion.Descripcion;
        return string.IsNullOrEmpty(clave) ? texto : Traducir(clave, recomendacion.Parametros ?? new object[0]);
    }
    private void MostrarMapa(List<IndicadorSectorListado> pIndicadores)
    {
        Dictionary<int, IndicadorSectorListado> porSector = pIndicadores.ToDictionary(i => i.CodigoSector);
        string indicador = ddlIndicadorMapa.SelectedValue;
        var datos = new List<SectorMapa>();
        foreach (SectorListado sector in sectorBLL.ObtenerSectoresPorLocal(CodigoLocalActual))
        {
            IndicadorSectorListado ind;
            porSector.TryGetValue(sector.CodigoSector, out ind);
            decimal? valor = ind == null ? null : ValorIndicador(ind, indicador);
            bool esMoneda = indicador == "ventas" || indicador == "ventasM2";
            datos.Add(new SectorMapa
            {
                Nombre = sector.Nombre,
                Ubicacion = sector.Ubicacion,
                Valor = valor,
                ValorTexto = indicador == "indice" ? valor?.ToString("0") : esMoneda ? MapaLocal.FormatearMoneda(valor) : MapaLocal.FormatearPorcentaje(valor)
            });
        }
        LayoutBE vigente = layoutBLL.ObtenerLayoutVigente(CodigoLocalActual);
        litMapa.Text = MapaLocal.Generar(MapaLocal.ObtenerUrlImagen(vigente, ResolveUrl), vigente?.Nombre, datos, Traducir);
        pnlMapa.Visible = datos.Count > 0;
    }
    private static decimal? ValorIndicador(IndicadorSectorListado pInd, string pIndicador)
    {
        switch (pIndicador)
        {
            case "ventasM2": return pInd.VentasPorM2;
            case "margen": return pInd.Margen;
            case "circulacion": return pInd.Circulacion;
            case "rotacion": return pInd.Rotacion;
            case "indice": return pInd.IndiceGlobal;
            default: return pInd.Ventas;
        }
    }
    private decimal MostrarResumenLocal(string pPeriodoDesde)
    {
        decimal ventas = ventaBLL.ObtenerTotalVentas(CodigoLocalActual, pPeriodoDesde);
        decimal? margen = ventaBLL.ObtenerMargenTotal(CodigoLocalActual, pPeriodoDesde);
        LocalBE local = localBLL.ObtenerLocalPorCodigo(CodigoLocalActual);
        decimal? superficie = local == null ? null : local.SuperficieAprox;
        litVentasPeriodo.Text = ventas.ToString("$#,##0");
        litMargenPeriodo.Text = margen == null ? "-" : margen.Value.ToString("0.#") + "%";
        lblSinCostos.Visible = margen == null;
        litVentasM2Local.Text = superficie == null || superficie <= 0 ? "-" : (ventas / superficie.Value).ToString("$#,##0");
        lblSinSuperficie.Visible = superficie == null || superficie <= 0;
        pnlResumenLocal.Visible = true;
        return ventas;
    }
    #endregion

    protected void Filtro_Changed(object sender, EventArgs e)
    {
        AplicarFiltros();
    }
    protected void btnAplicar_Click(object sender, EventArgs e)
    {
        AplicarFiltros();
    }
}