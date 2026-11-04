using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BE;
using BLL;
using SERVICIO.Logica;

public partial class PaginaComparacionEscenarios : PaginaSegura
{
    private LocalBLL localBLL = new LocalBLL();
    private EscenarioBLL escenarioBLL = new EscenarioBLL();
    private ComparacionBLL comparacionBLL = new ComparacionBLL();
    private IndicadorBLL indicadorBLL = new IndicadorBLL();
    private LayoutBLL layoutBLL = new LayoutBLL();
    private SectorBLL sectorBLL = new SectorBLL();
    private int CodigoLocalActual
    {
        get { return string.IsNullOrEmpty(ddlLocal.SelectedValue) ? 0 : int.Parse(ddlLocal.SelectedValue); }
    }
    protected override string PermisoRequerido
    {
        get { return PermisoNombres.CompararEscenarios; }
    }
    protected override string ClaveTitulo
    {
        get { return "comparacion.titulo"; }
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
    private void CargarDropdownEscenarios()
    {
        if (CodigoLocalActual == 0) return;
        List<EscenarioListado> escenarios = escenarioBLL.ObtenerEscenariosPorLocal(CodigoLocalActual);
        ddlEscenarioA.Items.Clear();
        ddlEscenarioB.Items.Clear();
        foreach (EscenarioListado esc in escenarios)
        {
            ddlEscenarioA.Items.Add(new ListItem(esc.Nombre, esc.CodigoEscenario.ToString()));
            ddlEscenarioB.Items.Add(new ListItem(esc.Nombre, esc.CodigoEscenario.ToString()));
        }
    }
    private void MostrarMapas(EscenarioBE pEscenarioA, EscenarioBE pEscenarioB, List<IndicadorSectorListado> pProyectadosA, List<IndicadorSectorListado> pProyectadosB)
    {
        List<SectorListado> sectores = sectorBLL.ObtenerSectoresPorLocal(CodigoLocalActual);
        List<decimal> todas = pProyectadosA.Concat(pProyectadosB).Select(p => p.Ventas).ToList();
        decimal minimo = todas.Count > 0 ? todas.Min() : 0;
        decimal maximo = todas.Count > 0 ? todas.Max() : 0;
        LayoutBE vigente = layoutBLL.ObtenerLayoutVigente(CodigoLocalActual);
        LayoutBE layoutA = ObtenerLayoutBase(pEscenarioA) ?? vigente;
        LayoutBE layoutB = ObtenerLayoutBase(pEscenarioB) ?? vigente;
        litLayoutA.Text = HttpUtility.HtmlEncode(ObtenerLayoutBase(pEscenarioA)?.Nombre ?? Traducir("escenarios.sinLayout"));
        litLayoutB.Text = HttpUtility.HtmlEncode(ObtenerLayoutBase(pEscenarioB)?.Nombre ?? Traducir("escenarios.sinLayout"));
        litTituloMapaA.Text = HttpUtility.HtmlEncode(pEscenarioA.Nombre);
        litTituloMapaB.Text = HttpUtility.HtmlEncode(pEscenarioB.Nombre);
        litMapaA.Text = MapaLocal.Generar(MapaLocal.ObtenerUrlImagen(layoutA, ResolveUrl), layoutA?.Nombre, ArmarDatosMapa(sectores, pProyectadosA), Traducir, minimo, maximo);
        litMapaB.Text = MapaLocal.Generar(MapaLocal.ObtenerUrlImagen(layoutB, ResolveUrl), layoutB?.Nombre, ArmarDatosMapa(sectores, pProyectadosB), Traducir, minimo, maximo);
    }
    private LayoutBE ObtenerLayoutBase(EscenarioBE pEscenario)
    {
        return pEscenario.CodigoLayout == null ? null : layoutBLL.ObtenerLayoutPorCodigo(pEscenario.CodigoLayout.Value);
    }
    private static List<SectorMapa> ArmarDatosMapa(List<SectorListado> pSectores, List<IndicadorSectorListado> pProyectados)
    {
        Dictionary<int, IndicadorSectorListado> porSector = pProyectados.ToDictionary(p => p.CodigoSector);
        return pSectores.Select(s =>
        {
            IndicadorSectorListado ind;
            decimal? ventas = porSector.TryGetValue(s.CodigoSector, out ind) ? ind.Ventas : (decimal?)null;
            return new SectorMapa { Nombre = s.Nombre, Ubicacion = s.Ubicacion, Valor = ventas, ValorTexto = MapaLocal.FormatearMoneda(ventas) };
        }).ToList();
    }
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CargarDropdownLocales();
            int codigoLocalQuery, codigoEscenarioAQuery;
            if (int.TryParse(Request.QueryString["local"], out codigoLocalQuery) && ddlLocal.Items.FindByValue(codigoLocalQuery.ToString()) != null)
            {
                ddlLocal.SelectedValue = codigoLocalQuery.ToString();
            }
            CargarDropdownEscenarios();
            if (int.TryParse(Request.QueryString["escenarioA"], out codigoEscenarioAQuery) && ddlEscenarioA.Items.FindByValue(codigoEscenarioAQuery.ToString()) != null)
            {
                ddlEscenarioA.SelectedValue = codigoEscenarioAQuery.ToString();
            }
        }
    }    
    protected void ddlLocal_SelectedIndexChanged(object sender, EventArgs e)
    {
        CargarDropdownEscenarios();
        pnlResultados.Visible = false;
    }

    protected void btnComparar_Click(object sender, EventArgs e)
    {
        if (ddlEscenarioA.Items.Count == 0 || ddlEscenarioB.Items.Count == 0)
        {
            MostrarMensaje(Traducir("comun.completeDatos"), esExito: false);
            return;
        }
        int codigoEscenarioA = int.Parse(ddlEscenarioA.SelectedValue);
        int codigoEscenarioB = int.Parse(ddlEscenarioB.SelectedValue);
        if (codigoEscenarioA == codigoEscenarioB)
        {
            MostrarMensaje(Traducir("comparacion.errDistintos"), esExito: false);
            return;
        }
        EscenarioBE escenarioA = escenarioBLL.ObtenerEscenarioPorCodigo(codigoEscenarioA);
        EscenarioBE escenarioB = escenarioBLL.ObtenerEscenarioPorCodigo(codigoEscenarioB);
        if (escenarioA.CodigoLocal != escenarioB.CodigoLocal)
        {
            MostrarMensaje(Traducir("comparacion.errMismoLocal"), esExito: false);
            return;
        }
        List<EscenarioCambio> cambiosA = escenarioBLL.ObtenerCambiosPorEscenario(codigoEscenarioA);
        List<EscenarioCambio> cambiosB = escenarioBLL.ObtenerCambiosPorEscenario(codigoEscenarioB);
        litNombreEscenarioA.Text = escenarioA.Nombre;
        litNombreEscenarioB.Text = escenarioB.Nombre;
        string periodoDesde = ddlPeriodo.SelectedValue == "3_meses" ? DateTime.Now.AddMonths(-3).ToString("yyyy-MM") : DateTime.Now.AddMonths(-1).ToString("yyyy-MM");
        List<IndicadorSectorListado> indicadoresReales = indicadorBLL.CalcularIndicadoresPorSector(CodigoLocalActual, periodoDesde);
        if (indicadoresReales.Count == 0)
        {
            MostrarMensaje(Traducir("comparacion.errFaltanDatos"), esExito: false);
            return;
        }
        List<IndicadorSectorListado> proyectadosA = indicadorBLL.ProyectarIndicadores(indicadoresReales, cambiosA);
        List<IndicadorSectorListado> proyectadosB = indicadorBLL.ProyectarIndicadores(indicadoresReales, cambiosB);
        rptIndicadoresA.DataSource = ArmarResumenProyectado(proyectadosA);
        rptIndicadoresA.DataBind();
        rptIndicadoresB.DataSource = ArmarResumenProyectado(proyectadosB);
        rptIndicadoresB.DataBind();
        rptDiferencias.DataSource = ArmarDiferenciasPorSector(proyectadosA, proyectadosB);
        rptDiferencias.DataBind();
        MostrarMapas(escenarioA, escenarioB, proyectadosA, proyectadosB);
        ViewState["CodigoEscenarioA"] = codigoEscenarioA;
        ViewState["CodigoEscenarioB"] = codigoEscenarioB;
        pnlResultados.Visible = true;
    }
    private List<IndicadorComparado> ArmarResumenProyectado(List<IndicadorSectorListado> proyectados)
    {
        decimal ventasTotales = proyectados.Sum(p => p.Ventas);
        decimal circulacionPromedio = proyectados.Count > 0 ? proyectados.Average(p => p.Circulacion) : 0;
        decimal rotacionPromedio = proyectados.Count > 0 ? proyectados.Average(p => p.Rotacion) : 0;
        decimal indiceGlobalPromedio = proyectados.Count > 0 ? (decimal)proyectados.Average(p => p.IndiceGlobal) : 0;
        var conSuperficie = proyectados.Where(p => p.Superficie != null && p.Superficie > 0).ToList();
        decimal m2 = conSuperficie.Sum(p => p.Superficie.Value);
        string ventasPorM2 = m2 > 0 ? (conSuperficie.Sum(p => p.Ventas) / m2).ToString("$#,##0") : "-";
        return new List<IndicadorComparado>
        {
            new IndicadorComparado { Nombre = Traducir("comparacion.ventasEstimadas"), Valor = ventasTotales.ToString("$#,##0") },
            new IndicadorComparado { Nombre = Traducir("indicadores.colVentasM2"), Valor = ventasPorM2 },
            new IndicadorComparado { Nombre = Traducir("comparacion.circulacionPromedio"), Valor = $"{circulacionPromedio:0.0}%" },
            new IndicadorComparado { Nombre = Traducir("comparacion.rotacionPromedio"), Valor = $"{rotacionPromedio:0.0}%" },
            new IndicadorComparado { Nombre = Traducir("comparacion.indicePromedio"), Valor = indiceGlobalPromedio.ToString("0") }
        };
    }
    private List<DiferenciaComparacion> ArmarDiferenciasPorSector(List<IndicadorSectorListado> proyectadosA, List<IndicadorSectorListado> proyectadosB)
    {
        var filas = new List<DiferenciaComparacion>();
        foreach (var indB in proyectadosB)
        {
            var indA = proyectadosA.FirstOrDefault(p => p.CodigoSector == indB.CodigoSector);
            if (indA == null) continue;
            decimal diferenciaPorc = indA.Ventas != 0 ? ((indB.Ventas - indA.Ventas) / indA.Ventas) * 100 : 0;
            bool mejora = indB.Ventas > indA.Ventas;
            filas.Add(new DiferenciaComparacion
            {
                Indicador = indB.Sector,
                ValorActual = indA.Ventas.ToString("$#,##0"),
                ValorPropuesto = indB.Ventas.ToString("$#,##0"),
                Diferencia = (diferenciaPorc >= 0 ? "+" : "") + diferenciaPorc.ToString("0.0") + "%",
                ClaseDiferencia = mejora ? "etiqueta-activo" : (diferenciaPorc == 0 ? "etiqueta-info" : "etiqueta-inactivo")
            });
        }
        return filas;
    }
    protected void btnGuardarComparacion_Click(object sender, EventArgs e)
    {
        if (ViewState["CodigoEscenarioA"] == null || ViewState["CodigoEscenarioB"] == null)
        {
            MostrarMensaje(Traducir("comparacion.errSinComparacion"), esExito: false);
            return;
        }
        ComparacionBE comparacion = new ComparacionBE((int)ViewState["CodigoEscenarioA"], (int)ViewState["CodigoEscenarioB"], DateTime.Now, "Comparación basada en cambios propuestos por sector.", "");
        comparacionBLL.GuardarComparacion(comparacion);
        MostrarMensaje(Traducir("comparacion.msgGuardada"), esExito: true);
    }

    private void MostrarMensaje(string texto, bool esExito)
    {
        lblMensaje.Text = texto;
        lblMensaje.CssClass = esExito ? "mensaje" : "mensaje mensaje-error";
        lblMensaje.Visible = true;
    }
}