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
        // CUN 008: "comparar un escenario actual con un escenario propuesto": la situación actual (sin cambios) es la opción por defecto
        ddlEscenarioA.Items.Add(new ListItem(Traducir("escenarios.situacionActual"), SITUACION_ACTUAL.ToString()));
        foreach (EscenarioListado esc in escenarios.Where(e => e.Estado != EscenarioBE.ESTADO_DESCARTADO))
        {
            ddlEscenarioA.Items.Add(new ListItem(esc.Nombre, esc.CodigoEscenario.ToString()));
            ddlEscenarioB.Items.Add(new ListItem(esc.Nombre, esc.CodigoEscenario.ToString()));
        }
    }
    private const int SITUACION_ACTUAL = 0;
    // Escenario elegido en el combo; null = situación actual (sin cambios)
    private EscenarioBE ObtenerEscenario(int pCodigo)
    {
        return pCodigo == SITUACION_ACTUAL ? null : escenarioBLL.ObtenerEscenarioPorCodigo(pCodigo);
    }
    private string NombreEscenario(EscenarioBE pEscenario)
    {
        return pEscenario == null ? Traducir("escenarios.situacionActual") : pEscenario.Nombre;
    }
    protected string MostrarDelta(object pDelta)
    {
        decimal delta = pDelta == null ? 0 : Convert.ToDecimal(pDelta);
        return delta == 0 ? "-" : (delta > 0 ? "+" : "") + delta.ToString("0.#") + "%";
    }
    // Resumen en una oración: cuánto cambian ventas, índice global y ventas/m² del propuesto (B) frente al actual (A)
    private void ArmarConclusion(string pNombreA, string pNombreB, List<IndicadorSectorListado> pA, List<IndicadorSectorListado> pB)
    {
        decimal ventasA = pA.Sum(p => p.Ventas), ventasB = pB.Sum(p => p.Ventas);
        int indiceA = pA.Count > 0 ? (int)Math.Round(pA.Average(p => p.IndiceGlobal), 0) : 0;
        int indiceB = pB.Count > 0 ? (int)Math.Round(pB.Average(p => p.IndiceGlobal), 0) : 0;
        decimal? ventasM2A = VentasPorM2(pA), ventasM2B = VentasPorM2(pB);
        decimal variacionVentas = IndicadorBLL.Variacion(ventasB, ventasA) ?? 0;
        int diferenciaIndice = indiceB - indiceA;
        if (variacionVentas == 0 && diferenciaIndice == 0)
        {
            litConclusion.Text = HttpUtility.HtmlEncode(Traducir("comparacion.conclusionIgual", pNombreA, pNombreB));
        }
        else
        {
            bool convieneB = variacionVentas > 0 || (variacionVentas == 0 && diferenciaIndice > 0);
            litConclusion.Text = "<strong>" + HttpUtility.HtmlEncode(Traducir("comparacion.conclusionConviene", convieneB ? pNombreB : pNombreA)) + "</strong> "
                + HttpUtility.HtmlEncode(Traducir("comparacion.conclusionDetalle", pNombreB, Signo(variacionVentas) + variacionVentas.ToString("0.#") + "%",
                                                  Signo(diferenciaIndice) + diferenciaIndice, pNombreA));
        }
        decimal? variacionM2 = ventasM2A != null && ventasM2B != null ? IndicadorBLL.Variacion(ventasM2B.Value, ventasM2A.Value) : null;
        litConclusionDetalle.Text = variacionM2 == null ? ""
            : HttpUtility.HtmlEncode(Traducir("comparacion.conclusionM2", Signo(variacionM2.Value) + variacionM2.Value.ToString("0.#") + "%"));
    }
    private static string Signo(decimal pValor) { return pValor > 0 ? "+" : ""; }
    private static decimal? VentasPorM2(List<IndicadorSectorListado> pIndicadores)
    {
        var conSuperficie = pIndicadores.Where(p => p.Superficie != null && p.Superficie > 0).ToList();
        decimal m2 = conSuperficie.Sum(p => p.Superficie.Value);
        return m2 > 0 ? conSuperficie.Sum(p => p.Ventas) / m2 : (decimal?)null;
    }
    private void MostrarMapas(EscenarioBE pEscenarioA, EscenarioBE pEscenarioB, List<IndicadorSectorListado> pProyectadosA, List<IndicadorSectorListado> pProyectadosB,
                              List<EscenarioCambio> pCambiosA, List<EscenarioCambio> pCambiosB)
    {
        List<SectorListado> sectores = sectorBLL.ObtenerSectoresPorLocal(CodigoLocalActual);
        List<decimal> todas = pProyectadosA.Concat(pProyectadosB).Select(p => p.Ventas).ToList();
        decimal minimo = todas.Count > 0 ? todas.Min() : 0;
        decimal maximo = todas.Count > 0 ? todas.Max() : 0;
        LayoutBE vigente = layoutBLL.ObtenerLayoutVigente(CodigoLocalActual);
        // La situación actual usa el plano vigente
        LayoutBE layoutA = pEscenarioA == null ? vigente : (ObtenerLayoutBase(pEscenarioA) ?? vigente);
        LayoutBE layoutB = ObtenerLayoutBase(pEscenarioB) ?? vigente;
        litLayoutA.Text = HttpUtility.HtmlEncode(layoutA?.Nombre ?? Traducir("escenarios.sinLayout"));
        litLayoutB.Text = HttpUtility.HtmlEncode(ObtenerLayoutBase(pEscenarioB)?.Nombre ?? Traducir("escenarios.sinLayout"));
        litTituloMapaA.Text = HttpUtility.HtmlEncode(NombreEscenario(pEscenarioA));
        litTituloMapaB.Text = HttpUtility.HtmlEncode(NombreEscenario(pEscenarioB));
        litMapaA.Text = MapaLocal.Generar(MapaLocal.ObtenerUrlImagen(layoutA, ResolveUrl), layoutA?.Nombre, ArmarDatosMapa(sectores, pProyectadosA, pCambiosA), Traducir, minimo, maximo);
        litMapaB.Text = MapaLocal.Generar(MapaLocal.ObtenerUrlImagen(layoutB, ResolveUrl), layoutB?.Nombre, ArmarDatosMapa(sectores, pProyectadosB, pCambiosB), Traducir, minimo, maximo);
    }
    private LayoutBE ObtenerLayoutBase(EscenarioBE pEscenario)
    {
        return pEscenario?.CodigoLayout == null ? null : layoutBLL.ObtenerLayoutPorCodigo(pEscenario.CodigoLayout.Value);
    }
    // Los sectores con "Mover al frente" / "Mover al fondo" se dibujan en su NUEVA fila del mapa (misma columna)
    private static List<SectorMapa> ArmarDatosMapa(List<SectorListado> pSectores, List<IndicadorSectorListado> pProyectados, List<EscenarioCambio> pCambios)
    {
        Dictionary<int, IndicadorSectorListado> porSector = pProyectados.ToDictionary(p => p.CodigoSector);
        Dictionary<int, string> acciones = pCambios.Where(c => c.CodigoSector != null && !string.IsNullOrEmpty(c.Accion))
                                                   .GroupBy(c => c.CodigoSector.Value).ToDictionary(g => g.Key, g => g.Last().Accion);
        return pSectores.Select(s =>
        {
            IndicadorSectorListado ind;
            decimal? ventas = porSector.TryGetValue(s.CodigoSector, out ind) ? ind.Ventas : (decimal?)null;
            string accion;
            string ubicacion = s.Ubicacion;
            if (acciones.TryGetValue(s.CodigoSector, out accion)) ubicacion = MoverFila(s.Ubicacion, accion);
            return new SectorMapa { Nombre = s.Nombre, Ubicacion = ubicacion, Valor = ventas, ValorTexto = MapaLocal.FormatearMoneda(ventas) };
        }).ToList();
    }
    // "Centro - Izquierda" + "Mover al frente" -> "Frente - Izquierda"
    private static string MoverFila(string pUbicacion, string pAccion)
    {
        string fila = pAccion == "Mover al frente" ? "Frente" : pAccion == "Mover al fondo" ? "Fondo" : null;
        if (fila == null || string.IsNullOrEmpty(pUbicacion)) return pUbicacion;
        int separador = pUbicacion.IndexOf(" - ");
        string columna = separador >= 0 ? pUbicacion.Substring(separador + 3) : "Centro";
        return fila + " - " + columna;
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
            if (int.TryParse(Request.QueryString["escenarioA"], out codigoEscenarioAQuery) && ddlEscenarioB.Items.FindByValue(codigoEscenarioAQuery.ToString()) != null)
            {
                ddlEscenarioB.SelectedValue = codigoEscenarioAQuery.ToString();
                ddlEscenarioA.SelectedValue = SITUACION_ACTUAL.ToString();
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
        rptSupuestos.DataSource = IndicadorBLL.ObtenerSupuestos();
        rptSupuestos.DataBind();
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
        EscenarioBE escenarioA = ObtenerEscenario(codigoEscenarioA);
        EscenarioBE escenarioB = ObtenerEscenario(codigoEscenarioB);
        if ((escenarioA != null && escenarioA.CodigoLocal != CodigoLocalActual) || (escenarioB != null && escenarioB.CodigoLocal != CodigoLocalActual))
        {
            MostrarMensaje(Traducir("comparacion.errMismoLocal"), esExito: false);
            return;
        }
        List<EscenarioCambio> cambiosA = escenarioA == null ? new List<EscenarioCambio>() : escenarioBLL.ObtenerCambiosPorEscenario(codigoEscenarioA);
        List<EscenarioCambio> cambiosB = escenarioB == null ? new List<EscenarioCambio>() : escenarioBLL.ObtenerCambiosPorEscenario(codigoEscenarioB);
        litNombreEscenarioA.Text = HttpUtility.HtmlEncode(NombreEscenario(escenarioA));
        litNombreEscenarioB.Text = HttpUtility.HtmlEncode(NombreEscenario(escenarioB));
        string periodoDesde = PeriodoDesde(MesesDelFiltro(ddlPeriodo.SelectedValue));
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
        MostrarMapas(escenarioA, escenarioB, proyectadosA, proyectadosB, cambiosA, cambiosB);
        ArmarConclusion(NombreEscenario(escenarioA), NombreEscenario(escenarioB), proyectadosA, proyectadosB);
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
        int codigoA = (int)ViewState["CodigoEscenarioA"];
        int codigoB = (int)ViewState["CodigoEscenarioB"];
        string url = $"PaginaReportes.aspx?local={CodigoLocalActual}&base={codigoA}&propuesto={codigoB}";
        if (codigoA != SITUACION_ACTUAL && codigoB != SITUACION_ACTUAL)
        {
            ComparacionBE comparacion = new ComparacionBE(codigoA, codigoB, DateTime.Now, "Comparación basada en cambios propuestos por sector.", "");
            url += "&comparacion=" + comparacionBLL.GuardarComparacion(comparacion);
        }
        Response.Redirect(url);
    }
    private void MostrarMensaje(string texto, bool esExito)
    {
        lblMensaje.Text = texto;
        lblMensaje.CssClass = esExito ? "mensaje" : "mensaje mensaje-error";
        lblMensaje.Visible = true;
    }
}