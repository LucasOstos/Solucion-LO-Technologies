using System;
using System.Collections.Generic;
using System.Linq;
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
    private string ObtenerPeriodoDesde()
    {
        return PeriodoDesde(MesesDelFiltro(ddlPeriodo.SelectedValue));
    }
    private void AplicarFiltros()
    {
        if (CodigoLocalActual == 0) return;
        string periodoDesde = ObtenerPeriodoDesde();
        string periodoActual = DateTime.Now.ToString("yyyy-MM");
        List<IndicadorSectorListado> indicadores = indicadorBLL.CalcularIndicadoresPorSector(CodigoLocalActual, periodoDesde);
        MostrarResumenLocal(periodoDesde);
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
        indicadorBLL.GuardarIndicadores(CodigoLocalActual, periodoActual, indicadores);
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
            recomendacionBLL.GuardarRecomendaciones(recomendaciones);
        }
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
    private void MostrarResumenLocal(string pPeriodoDesde)
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