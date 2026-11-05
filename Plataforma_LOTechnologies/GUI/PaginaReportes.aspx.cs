using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms.DataVisualization.Charting;
using BE;
using BLL;
using DAL;
using iTextSharp.text;
using iTextSharp.text.pdf;
using SERVICIO.Logica;
using PdfImage = iTextSharp.text.Image;
using WebListItem = System.Web.UI.WebControls.ListItem;

public partial class PaginaReportes : PaginaSegura
{
    protected override string PermisoRequerido
    {
        get { return PermisoNombres.GenerarReportes; }
    }
    protected override string ClaveTitulo
    {
        get { return "reportes.titulo"; }
    }
    private const int MESES_MAXIMOS = 24;
    private const int MESES_PREDETERMINADOS = 3;
    private const string TIPO_COMPARACION = "Comparación";
    private Idioma idiomaReporte;
    private int codigoIdiomaPredeterminado;
    private LocalBLL localBLL = new LocalBLL();
    private EscenarioBLL escenarioBLL = new EscenarioBLL();
    private IndicadorBLL indicadorBLL = new IndicadorBLL();
    private StockBLL stockBLL = new StockBLL();
    private VentaBLL ventaBLL = new VentaBLL();
    private ProductoBLL productoBLL = new ProductoBLL();
    private ReporteBLL reporteBLL = new ReporteBLL();
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
            CargarDropdownIdiomas();
            int codigoLocalQuery;
            if (int.TryParse(Request.QueryString["local"], out codigoLocalQuery) && ddlLocal.Items.FindByValue(codigoLocalQuery.ToString()) != null)
            {
                ddlLocal.SelectedValue = codigoLocalQuery.ToString();
            }
            CargarDropdownEscenarios();
            string escenarioBaseQuery = Request.QueryString["base"], escenarioPropuestoQuery = Request.QueryString["propuesto"];
            if (escenarioBaseQuery != null && ddlEscenarioBase.Items.FindByValue(escenarioBaseQuery) != null) ddlEscenarioBase.SelectedValue = escenarioBaseQuery;
            if (escenarioPropuestoQuery != null && ddlEscenarioPropuesto.Items.FindByValue(escenarioPropuestoQuery) != null) ddlEscenarioPropuesto.SelectedValue = escenarioPropuestoQuery;
            int codigoComparacionQuery;
            if (int.TryParse(Request.QueryString["comparacion"], out codigoComparacionQuery))
            {
                ViewState["CodigoComparacion"] = codigoComparacionQuery;
                // Viene de "Generar reporte de esta comparación": se preselecciona el tipo Comparación
                ddlTipoReporte.SelectedValue = TIPO_COMPARACION;
                AplicarSeccionesDelTipo(TIPO_COMPARACION);
            }
            DateTime mesActual = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            tbPeriodoDesde.Text = mesActual.AddMonths(-(MESES_PREDETERMINADOS - 1)).ToString("yyyy-MM");
            tbPeriodoHasta.Text = mesActual.ToString("yyyy-MM");
            CargarReportesRecientes();
        }
    }

    #region Funciones
    private void CargarDropdownLocales()
    {
        List<LocalListado> locales = localBLL.ObtenerLocalesListado("", null, "Activo");
        ddlLocal.Items.Clear();
        foreach (LocalListado local in locales)
        {
            ddlLocal.Items.Add(new WebListItem(local.Nombre, local.CodigoLocal.ToString()));
        }
    }
    private void CargarDropdownIdiomas()
    {
        ddlIdiomaReporte.Items.Clear();
        foreach (Idioma idioma in GestorIdioma.Instancia.ObtenerIdiomasActivos())
        {
            if (!FuentesReporte.IdiomaSoportado(idioma.Codigo)) continue;
            ddlIdiomaReporte.Items.Add(new WebListItem(idioma.NombreNativo, idioma.CodigoIdioma.ToString()));
        }
        Idioma actual = IdiomaActual;
        if (actual != null && ddlIdiomaReporte.Items.FindByValue(actual.CodigoIdioma.ToString()) != null)
        {
            ddlIdiomaReporte.SelectedValue = actual.CodigoIdioma.ToString();
        }
    }
    private void CargarDropdownEscenarios()
    {
        if (CodigoLocalActual == 0) return;
        List<EscenarioListado> escenarios = escenarioBLL.ObtenerEscenariosPorLocal(CodigoLocalActual);
        ddlEscenarioBase.Items.Clear();
        ddlEscenarioPropuesto.Items.Clear();
        ddlEscenarioBase.Items.Add(new WebListItem(Traducir("reportes.ninguno"), ""));
        // La situación actual (sin cambios) también puede ser la base de la comparación
        ddlEscenarioBase.Items.Add(new WebListItem(Traducir("escenarios.situacionActual"), "0"));
        ddlEscenarioPropuesto.Items.Add(new WebListItem(Traducir("reportes.ninguno"), ""));
        foreach (EscenarioListado esc in escenarios)
        {
            ddlEscenarioBase.Items.Add(new WebListItem(esc.Nombre, esc.CodigoEscenario.ToString()));
            ddlEscenarioPropuesto.Items.Add(new WebListItem(esc.Nombre, esc.CodigoEscenario.ToString()));
        }
    }
    private void CargarReportesRecientes()
    {
        if (CodigoLocalActual == 0) return;
        List<ReporteBE> reportes = reporteBLL.ObtenerReportesPorLocal(CodigoLocalActual);
        rptReportesRecientes.Visible = reportes.Count > 0;
        lblSinReportes.Visible = reportes.Count == 0;
        rptReportesRecientes.DataSource = reportes;
        rptReportesRecientes.DataBind();
    }
    // Mismas secciones que preselecciona ReportesJS.js al cambiar el tipo (acá se usa al entrar desde la comparación)
    private void AplicarSeccionesDelTipo(string pTipo)
    {
        bool completo = pTipo == "Completo", comparacion = pTipo == TIPO_COMPARACION, indicadores = pTipo == "Indicadores", stock = pTipo == "Stock";
        chkResumenEjecutivo.Checked = true;
        chkEvolucionVentas.Checked = completo || indicadores;
        chkIndicadoresPrincipales.Checked = completo || indicadores || comparacion;
        chkRecomendaciones.Checked = completo || indicadores || stock;
        chkComparacionEscenarios.Checked = completo || comparacion;
        chkOportunidades.Checked = completo || stock;
        chkEstadoStock.Checked = completo || stock;
        chkHistorialVentas.Checked = completo || indicadores;
        chkDetalleProductos.Checked = completo;
    }
    private bool AlgunaSeccionElegida()
    {
        return chkResumenEjecutivo.Checked || chkEvolucionVentas.Checked || chkIndicadoresPrincipales.Checked || chkRecomendaciones.Checked
            || chkComparacionEscenarios.Checked || chkOportunidades.Checked || chkEstadoStock.Checked || chkHistorialVentas.Checked || chkDetalleProductos.Checked;
    }
    // Acepta "2026-09" (lo que envía el campo mes) o "09/2026" (navegadores que lo muestran como texto)
    private static bool LeerMes(string pTexto, out DateTime pMes)
    {
        string[] formatos = { "yyyy-MM", "MM/yyyy", "M/yyyy", "yyyy-MM-dd" };
        if (DateTime.TryParseExact((pTexto ?? "").Trim(), formatos, CultureInfo.InvariantCulture, DateTimeStyles.None, out pMes))
        {
            pMes = new DateTime(pMes.Year, pMes.Month, 1);
            return true;
        }
        return false;
    }
    private void PrepararIdiomaReporte()
    {
        Idioma predeterminado = GestorIdioma.Instancia.IdiomaPredeterminado;
        codigoIdiomaPredeterminado = predeterminado == null ? 0 : predeterminado.CodigoIdioma;
        int codigoElegido;
        int.TryParse(ddlIdiomaReporte.SelectedValue, out codigoElegido);
        idiomaReporte = GestorIdioma.Instancia.ObtenerIdiomasActivos().FirstOrDefault(i => i.CodigoIdioma == codigoElegido && FuentesReporte.IdiomaSoportado(i.Codigo));
        if (idiomaReporte == null) idiomaReporte = IdiomaActual;
        if (idiomaReporte != null && !FuentesReporte.IdiomaSoportado(idiomaReporte.Codigo)) idiomaReporte = predeterminado;
    }
    private CultureInfo CulturaReporte()
    {
        try { return idiomaReporte == null ? CultureInfo.CurrentCulture : new CultureInfo(idiomaReporte.Codigo); }
        catch (CultureNotFoundException) { return CultureInfo.CurrentCulture; }
    }
    private string T(string pClave)
    {
        if (idiomaReporte == null) return Traducir(pClave);
        return Traductor.Instancia.Traducir(idiomaReporte.CodigoIdioma, codigoIdiomaPredeterminado, pClave);
    }
    private string T(string pClave, params object[] pValores)
    {
        string texto = T(pClave);
        if (pValores == null || pValores.Length == 0) return texto;
        try { return string.Format(texto, pValores); }
        catch (FormatException) { return texto; }
    }
    private string TValor(string pValor)
    {
        string clave = ValoresTraducibles.ObtenerClave(pValor);
        return clave == null ? (pValor ?? "") : T(clave);
    }
    private string TRecomendacion(RecomendacionBE pRecomendacion, bool pEsMotivo)
    {
        string clave = pEsMotivo ? pRecomendacion.ClaveMotivo : pRecomendacion.ClaveDescripcion;
        string texto = pEsMotivo ? pRecomendacion.Motivo : pRecomendacion.Descripcion;
        return string.IsNullOrEmpty(clave) ? texto : T(clave, pRecomendacion.Parametros ?? new object[0]);
    }
    private string TituloDelTipo(string pTipo)
    {
        switch (pTipo)
        {
            case TIPO_COMPARACION: return T("pdf.tituloComparacion");
            case "Indicadores": return T("pdf.tituloIndicadores");
            case "Stock": return T("pdf.tituloStock");
            default: return T("pdf.tituloCompleto");
        }
    }
    private static string DescribirMeses(DateTime pDesde, DateTime pHasta)
    {
        return pDesde == pHasta ? pDesde.ToString("MM/yyyy") : pDesde.ToString("MM/yyyy") + " – " + pHasta.ToString("MM/yyyy");
    }
    // Junta todo lo que va en el PDF. pFin = último día con datos (hoy si el período llega al mes en curso).
    private DatosReporte ArmarDatos(List<IndicadorSectorListado> pIndicadores, DateTime pDesde, DateTime pHasta, DateTime pFin, LocalBE pLocal)
    {
        string periodoDesde = pDesde.ToString("yyyy-MM"), periodoHasta = pHasta.ToString("yyyy-MM");
        List<StockListado> stock = stockBLL.ObtenerStockPorLocal(CodigoLocalActual);
        var datos = new DatosReporte
        {
            TituloTipo = TituloDelTipo(ddlTipoReporte.SelectedValue),
            NombreLocal = pLocal?.Nombre,
            TextoPeriodo = DescribirMeses(pDesde, pHasta),
            FechaGeneracion = DateTime.Now,
            IncluirResumen = chkResumenEjecutivo.Checked,
            IncluirEvolucion = chkEvolucionVentas.Checked,
            IncluirIndicadores = chkIndicadoresPrincipales.Checked,
            IncluirRecomendaciones = chkRecomendaciones.Checked,
            IncluirComparacion = chkComparacionEscenarios.Checked,
            IncluirOportunidades = chkOportunidades.Checked,
            IncluirStock = chkEstadoStock.Checked,
            IncluirHistorial = chkHistorialVentas.Checked,
            IncluirProductos = chkDetalleProductos.Checked,
            Indicadores = pIndicadores,
            Stock = stock,
            AlertasStock = stock.Count(s => s.EstadoStock == "Crítico" || s.EstadoStock == "Bajo")
        };
        if (datos.IncluirResumen)
        {
            datos.VentasPeriodo = ventaBLL.ObtenerTotalVentasEntreFechas(CodigoLocalActual, pDesde, pFin);
            decimal ventasAnteriores = indicadorBLL.CompletarVariacion(CodigoLocalActual, pDesde, pFin, pIndicadores);
            datos.VariacionVentas = IndicadorBLL.Variacion(datos.VentasPeriodo, ventasAnteriores);
            datos.Margen = ventaBLL.ObtenerMargenTotal(CodigoLocalActual, periodoDesde, periodoHasta);
            datos.SuperficieLocal = pLocal?.SuperficieAprox;
            LayoutBE vigente = layoutBLL.ObtenerLayoutVigente(CodigoLocalActual);
            if (vigente != null)
            {
                datos.NombrePlano = vigente.Nombre;
                if (MapaLocal.ObtenerUrlImagen(vigente, ResolveUrl) != null) datos.RutaPlano = Server.MapPath("~/Subidas/Layouts/" + vigente.NombreArchivo);
            }
        }
        if (datos.IncluirEvolucion) datos.Evolucion = indicadorBLL.ObtenerEvolucionVentas(CodigoLocalActual, pDesde, pHasta);
        if (datos.IncluirRecomendaciones)
        {
            datos.Recomendaciones = new RecomendacionBLL().GenerarRecomendaciones(CodigoLocalActual, pIndicadores, stock)
                .Select(r => new FilaRecomendacion { Prioridad = r.Prioridad, TextoPrioridad = TValor(r.Prioridad), Titulo = TRecomendacion(r, false), Detalle = TRecomendacion(r, true) })
                .ToList();
        }
        if (datos.IncluirComparacion) datos.Comparacion = ArmarComparacion(pIndicadores);
        if (datos.IncluirOportunidades) datos.Oportunidades = ArmarOportunidades(pDesde, pFin);
        if (datos.IncluirHistorial) datos.Historial = ventaBLL.ObtenerVentasAgrupadas(CodigoLocalActual, periodoDesde, VentaDAL.AGRUPAR_SECTOR, periodoHasta);
        if (datos.IncluirProductos) datos.Productos = productoBLL.ObtenerProductosListado(CodigoLocalActual, "", null);
        return datos;
    }
    // La comparación usa los indicadores del mismo período del reporte
    private DatosComparacionReporte ArmarComparacion(List<IndicadorSectorListado> pIndicadores)
    {
        if (string.IsNullOrEmpty(ddlEscenarioBase.SelectedValue) || string.IsNullOrEmpty(ddlEscenarioPropuesto.SelectedValue))
        {
            return new DatosComparacionReporte { Error = T("pdf.errEscenarios") };
        }
        int codigoBase = int.Parse(ddlEscenarioBase.SelectedValue);
        int codigoPropuesto = int.Parse(ddlEscenarioPropuesto.SelectedValue);
        // Base 0 = situación actual (sin cambios)
        EscenarioBE escenarioBase = codigoBase == 0 ? null : escenarioBLL.ObtenerEscenarioPorCodigo(codigoBase);
        EscenarioBE escenarioPropuesto = escenarioBLL.ObtenerEscenarioPorCodigo(codigoPropuesto);
        if (escenarioPropuesto == null || escenarioPropuesto.CodigoLocal != CodigoLocalActual || (escenarioBase != null && escenarioBase.CodigoLocal != escenarioPropuesto.CodigoLocal))
        {
            return new DatosComparacionReporte { Error = T("comparacion.errMismoLocal") };
        }
        List<EscenarioCambio> cambiosBase = escenarioBase == null ? new List<EscenarioCambio>() : escenarioBLL.ObtenerCambiosPorEscenario(codigoBase);
        List<EscenarioCambio> cambiosPropuesto = escenarioBLL.ObtenerCambiosPorEscenario(codigoPropuesto);
        int? layoutBase = escenarioBase == null ? layoutBLL.ObtenerLayoutVigente(CodigoLocalActual)?.CodigoLayout : escenarioBase.CodigoLayout;
        var comparacion = new DatosComparacionReporte
        {
            NombreBase = escenarioBase == null ? T("escenarios.situacionActual") : escenarioBase.Nombre,
            NombrePropuesto = escenarioPropuesto.Nombre,
            LayoutBase = NombreLayout(layoutBase),
            LayoutPropuesto = NombreLayout(escenarioPropuesto.CodigoLayout),
            Base = indicadorBLL.ProyectarIndicadores(pIndicadores, cambiosBase),
            Propuesto = indicadorBLL.ProyectarIndicadores(pIndicadores, cambiosPropuesto),
            CambiosPropuesto = cambiosPropuesto.Select(DescribirCambio).Where(c => c != "").ToList()
        };
        ArmarConclusion(comparacion);
        return comparacion;
    }
    // "Hogar: Mover al frente" -> "Hogar: <acción traducida>"
    private string DescribirCambio(EscenarioCambio pCambio)
    {
        string descripcion = pCambio.Descripcion ?? "";
        int posicion = descripcion.IndexOf(": ");
        return posicion > 0 ? descripcion.Substring(0, posicion) + ": " + TValor(descripcion.Substring(posicion + 2)) : TValor(descripcion);
    }
    // Mismo criterio que la pantalla de comparación
    private void ArmarConclusion(DatosComparacionReporte pComparacion)
    {
        decimal ventasBase = pComparacion.Base.Sum(p => p.Ventas), ventasPropuesto = pComparacion.Propuesto.Sum(p => p.Ventas);
        int indiceBase = pComparacion.Base.Count > 0 ? (int)Math.Round(pComparacion.Base.Average(p => p.IndiceGlobal), 0) : 0;
        int indicePropuesto = pComparacion.Propuesto.Count > 0 ? (int)Math.Round(pComparacion.Propuesto.Average(p => p.IndiceGlobal), 0) : 0;
        decimal variacionVentas = IndicadorBLL.Variacion(ventasPropuesto, ventasBase) ?? 0;
        int diferenciaIndice = indicePropuesto - indiceBase;
        if (variacionVentas == 0 && diferenciaIndice == 0)
        {
            pComparacion.ConclusionTitulo = T("comparacion.conclusionIgual", pComparacion.NombreBase, pComparacion.NombrePropuesto);
            pComparacion.ConclusionDetalle = "";
            return;
        }
        bool convienePropuesto = variacionVentas > 0 || (variacionVentas == 0 && diferenciaIndice > 0);
        pComparacion.ConclusionTitulo = T("comparacion.conclusionConviene", convienePropuesto ? pComparacion.NombrePropuesto : pComparacion.NombreBase);
        pComparacion.ConclusionDetalle = T("comparacion.conclusionDetalle", pComparacion.NombrePropuesto, (variacionVentas > 0 ? "+" : "") + variacionVentas.ToString("0.#") + "%",
                                           (diferenciaIndice > 0 ? "+" : "") + diferenciaIndice, pComparacion.NombreBase);
    }
    private List<FilaOportunidad> ArmarOportunidades(DateTime pDesde, DateTime pFin)
    {
        string[] orden = { "Alta", "Media", "Baja" };
        return new AnalisisProductoBLL().DetectarOportunidades(CodigoLocalActual, pDesde, pFin)
            .Where(o => o.Tipo != null)
            .OrderBy(o => Array.IndexOf(orden, o.Prioridad) < 0 ? 9 : Array.IndexOf(orden, o.Prioridad)).ThenByDescending(o => o.Importe)
            .Select(o => new FilaOportunidad
            {
                Producto = o.NombreProducto,
                Sector = o.SectorActual,
                Tipo = o.Tipo,
                TextoTipo = TextoTipoOportunidad(o.Tipo),
                Prioridad = o.Prioridad,
                TextoPrioridad = TextoPrioridadOportunidad(o.Prioridad),
                Motivo = string.IsNullOrEmpty(o.ClaveMotivo) ? "" : T(o.ClaveMotivo, o.Parametros ?? new object[0])
            }).ToList();
    }
    private string TextoTipoOportunidad(string pTipo)
    {
        switch (pTipo)
        {
            case ProductoAnalisisListado.TIPO_ESTRELLA: return T("analisis.tipoEstrellaEscondida");
            case ProductoAnalisisListado.TIPO_LUGAR_PRIVILEGIADO: return T("analisis.tipoLugarPrivilegiado");
            case ProductoAnalisisListado.TIPO_SIN_MOVIMIENTO: return T("analisis.tipoSinMovimiento");
            case ProductoAnalisisListado.TIPO_RIESGO_QUIEBRE: return T("analisis.tipoRiesgoQuiebre");
            case ProductoAnalisisListado.TIPO_SIN_UBICAR: return T("analisis.tipoSinUbicar");
            case ProductoAnalisisListado.TIPO_COBERTURA_EXCESIVA: return T("analisis.tipoCoberturaExcesiva");
            default: return pTipo ?? "";
        }
    }
    private string TextoPrioridadOportunidad(string pPrioridad)
    {
        switch (pPrioridad)
        {
            case "Alta": return T("analisis.prioridadAlta");
            case "Media": return T("analisis.prioridadMedia");
            case "Baja": return T("analisis.prioridadBaja");
            default: return "";
        }
    }
    private string NombreLayout(int? pCodigoLayout)
    {
        LayoutBE layout = pCodigoLayout == null ? null : layoutBLL.ObtenerLayoutPorCodigo(pCodigoLayout.Value);
        return layout?.Nombre ?? T("escenarios.sinLayout");
    }
    private void MostrarMensaje(string texto, bool esExito)
    {
        lblMensaje.Text = texto;
        lblMensaje.CssClass = esExito ? "mensaje" : "mensaje mensaje-error";
        lblMensaje.Visible = true;
    }
    #endregion
    protected void ddlLocal_SelectedIndexChanged(object sender, EventArgs e)
    {
        ViewState["CodigoComparacion"] = null;
        CargarDropdownEscenarios();
        CargarReportesRecientes();
    }
    protected void btnGenerarReporte_Click(object sender, EventArgs e)
    {
        if (CodigoLocalActual == 0)
        {
            MostrarMensaje(Traducir("stock.errSeleccionarLocal"), esExito: false);
            return;
        }
        DateTime desde, hasta;
        if (!LeerMes(tbPeriodoDesde.Text, out desde) || !LeerMes(tbPeriodoHasta.Text, out hasta))
        {
            MostrarMensaje(Traducir("reportes.errPeriodo"), esExito: false);
            return;
        }
        DateTime mesActual = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        if (desde > hasta)
        {
            MostrarMensaje(Traducir("reportes.errPeriodoOrden"), esExito: false);
            return;
        }
        if (hasta > mesActual)
        {
            MostrarMensaje(Traducir("reportes.errPeriodoFuturo"), esExito: false);
            return;
        }
        if ((hasta.Year - desde.Year) * 12 + hasta.Month - desde.Month + 1 > MESES_MAXIMOS)
        {
            MostrarMensaje(Traducir("reportes.errPeriodoMaximo", MESES_MAXIMOS), esExito: false);
            return;
        }
        if (!AlgunaSeccionElegida())
        {
            MostrarMensaje(Traducir("reportes.errSinSecciones"), esExito: false);
            return;
        }
        if (ddlTipoReporte.SelectedValue == TIPO_COMPARACION && (string.IsNullOrEmpty(ddlEscenarioBase.SelectedValue) || string.IsNullOrEmpty(ddlEscenarioPropuesto.SelectedValue)))
        {
            MostrarMensaje(Traducir("pdf.errEscenarios"), esExito: false);
            return;
        }
        // Si el período llega al mes en curso, los datos van hasta hoy
        DateTime fin = hasta == mesActual ? DateTime.Today : hasta.AddMonths(1).AddDays(-1);
        try
        {
            List<IndicadorSectorListado> indicadores = indicadorBLL.CalcularIndicadoresPorSector(CodigoLocalActual, desde.ToString("yyyy-MM"), hasta.ToString("yyyy-MM"));
            if (indicadores.Count == 0)
            {
                MostrarMensaje(Traducir("reportes.errSinDatos"), esExito: false);
                return;
            }
            LocalBE local = localBLL.ObtenerLocalPorCodigo(CodigoLocalActual);
            PrepararIdiomaReporte();
            DatosReporte datos = ArmarDatos(indicadores, desde, hasta, fin, local);
            FuentesReporte fuentes = FuentesReporte.Preparar(idiomaReporte == null ? "es" : idiomaReporte.Codigo, Server.MapPath("~/"));
            var constructor = new ConstructorReportePdf(T, new GraficosReporte(fuentes.FamiliaGrafico), fuentes, CulturaReporte());
            byte[] pdf = constructor.Construir(datos);

            string nombreArchivo = $"reporte_{CodigoLocalActual}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            string carpetaDestino = Server.MapPath("~/Subidas/Reportes/");
            if (!Directory.Exists(carpetaDestino)) Directory.CreateDirectory(carpetaDestino);
            File.WriteAllBytes(Path.Combine(carpetaDestino, nombreArchivo), pdf);

            int codigoEscenarioElegido;
            int? codigoEscenario = int.TryParse(ddlEscenarioPropuesto.SelectedValue, out codigoEscenarioElegido) && codigoEscenarioElegido > 0 ? codigoEscenarioElegido
                                 : int.TryParse(ddlEscenarioBase.SelectedValue, out codigoEscenarioElegido) && codigoEscenarioElegido > 0 ? codigoEscenarioElegido : (int?)null;
            int? codigoComparacion = chkComparacionEscenarios.Checked ? ViewState["CodigoComparacion"] as int? : null;
            string titulo = $"{TraducirValor(ddlTipoReporte.SelectedValue)} — {local?.Nombre}";
            var reporte = new ReporteBE(CodigoLocalActual, codigoEscenario, codigoComparacion, titulo, ddlTipoReporte.SelectedValue, DescribirMeses(desde, hasta),
                                        $"Subidas/Reportes/{nombreArchivo}", desde, fin, DateTime.Now);
            reporteBLL.CrearReporte(reporte);
            MostrarMensaje(Traducir("reportes.msgGenerado"), esExito: true);
        }
        catch (ExcepcionTraducible ex)
        {
            MostrarMensaje(TraducirError(ex), esExito: false);
        }
        CargarReportesRecientes();
    }
    protected void rptReportesRecientes_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int codigoReporte = int.Parse(e.CommandArgument.ToString());
        if (e.CommandName == "EliminarReporte")
        {
            try
            {
                string ruta = reporteBLL.EliminarReporte(codigoReporte);
                BorrarArchivo(ruta);
                MostrarMensaje(Traducir("reportes.msgEliminado"), esExito: true);
            }
            catch (ExcepcionTraducible ex)
            {
                MostrarMensaje(TraducirError(ex), esExito: false);
            }
            CargarReportesRecientes();
            return;
        }
        ReporteBE reporte = reporteBLL.ObtenerReportePorCodigo(codigoReporte);
        if (reporte == null) return;
        if (e.CommandName == "VerReporte" || e.CommandName == "DescargarReporte")
        {
            Response.Redirect("~/" + reporte.RutaArchivo);
        }
    }
    private void BorrarArchivo(string pRutaRelativa)
    {
        if (string.IsNullOrEmpty(pRutaRelativa)) return;
        try
        {
            string carpeta = Path.GetFullPath(Server.MapPath("~/Subidas/Reportes/"));
            string ruta = Path.GetFullPath(Server.MapPath("~/" + pRutaRelativa));
            if (ruta.StartsWith(carpeta, StringComparison.OrdinalIgnoreCase) && File.Exists(ruta)) File.Delete(ruta);
        }
        catch (Exception) { } // si el archivo no se puede borrar, el registro ya no aparece igual
    }
}