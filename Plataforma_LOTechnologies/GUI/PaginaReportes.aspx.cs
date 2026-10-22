using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms.DataVisualization.Charting;
using BE;
using BLL;
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
    private static readonly string[] IDIOMAS_SIN_PDF = { "ar", "he", "fa", "ur" };
    private Idioma idiomaReporte;
    private int codigoIdiomaPredeterminado;
    private BaseFont fuenteBase;
    private BaseFont fuenteBaseNegrita;
    private string familiaGrafico = "Arial";
    private LocalBLL localBLL = new LocalBLL();
    private EscenarioBLL escenarioBLL = new EscenarioBLL();
    private IndicadorBLL indicadorBLL = new IndicadorBLL();
    private StockBLL stockBLL = new StockBLL();
    private VentaBLL ventaBLL = new VentaBLL();
    private ProductoBLL productoBLL = new ProductoBLL();
    private SectorBLL sectorBLL = new SectorBLL();
    private ReporteBLL reporteBLL = new ReporteBLL();
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
            CargarDropdownEscenarios();
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
            if (IDIOMAS_SIN_PDF.Contains(idioma.Codigo)) continue;
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
        if (reportes.Count == 0)
        {
            rptReportesRecientes.Visible = false;
            lblSinReportes.Visible = true;
        }
        else
        {
            rptReportesRecientes.Visible = true;
            lblSinReportes.Visible = false;
            rptReportesRecientes.DataSource = reportes;
            rptReportesRecientes.DataBind();
        }
    }
    private void PrepararIdiomaReporte()
    {
        Idioma predeterminado = GestorIdioma.Instancia.IdiomaPredeterminado;
        codigoIdiomaPredeterminado = predeterminado == null ? 0 : predeterminado.CodigoIdioma;
        int codigoElegido;
        int.TryParse(ddlIdiomaReporte.SelectedValue, out codigoElegido);
        idiomaReporte = GestorIdioma.Instancia.ObtenerIdiomasActivos().FirstOrDefault(i => i.CodigoIdioma == codigoElegido && !IDIOMAS_SIN_PDF.Contains(i.Codigo));
        if (idiomaReporte == null) idiomaReporte = IdiomaActual;
        if (idiomaReporte != null && IDIOMAS_SIN_PDF.Contains(idiomaReporte.Codigo)) idiomaReporte = predeterminado;
        PrepararFuentes(idiomaReporte == null ? "es" : idiomaReporte.Codigo);
    }
    private string T(string pClave)
    {
        if (idiomaReporte == null) return Traducir(pClave);
        return Traductor.Instancia.Traducir(idiomaReporte.CodigoIdioma, codigoIdiomaPredeterminado, pClave);
    }
    private string T(string pClave, params object[] pValores)
    {
        string texto = T(pClave);
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
    private void PrepararFuentes(string pCodigoIdioma)
    {
        string carpeta = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
        string[] normales, negritas;
        if (pCodigoIdioma == "ja")
        {
            normales = new[] { "msgothic.ttc,0", "meiryo.ttc,0", "YuGothM.ttc,0" };
            negritas = new[] { "msgothic.ttc,0", "meiryob.ttc,0", "YuGothB.ttc,0" };
            familiaGrafico = ObtenerFamiliaInstalada("MS Gothic", "Meiryo", "Yu Gothic") ?? "Arial";
        }
        else
        {
            normales = new[] { "arial.ttf", "segoeui.ttf", "tahoma.ttf" };
            negritas = new[] { "arialbd.ttf", "segoeuib.ttf", "tahomabd.ttf" };
            familiaGrafico = "Arial";
        }
        fuenteBase = CrearFuenteBase(carpeta, normales);
        fuenteBaseNegrita = CrearFuenteBase(carpeta, negritas) ?? fuenteBase;
    }
    private static BaseFont CrearFuenteBase(string pCarpeta, string[] pArchivos)
    {
        foreach (string archivo in pArchivos)
        {
            string nombre = archivo.Split(',')[0];
            if (!File.Exists(Path.Combine(pCarpeta, nombre))) continue;
            try
            {
                return BaseFont.CreateFont(Path.Combine(pCarpeta, archivo), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            }
            catch (Exception) { }
        }
        return null;
    }
    private static string ObtenerFamiliaInstalada(params string[] pFamilias)
    {
        using (var instaladas = new InstalledFontCollection())
        {
            foreach (string familia in pFamilias)
            {
                if (instaladas.Families.Any(f => string.Equals(f.Name, familia, StringComparison.OrdinalIgnoreCase))) return familia;
            }
        }
        return null;
    }
    private Font CrearFuente(float pTamanio, bool pNegrita, BaseColor pColor = null)
    {
        BaseFont baseFuente = pNegrita ? fuenteBaseNegrita : fuenteBase;
        if (baseFuente == null)
        {
            return FontFactory.GetFont(pNegrita ? FontFactory.HELVETICA_BOLD : FontFactory.HELVETICA, pTamanio, pColor ?? BaseColor.BLACK);
        }
        return new Font(baseFuente, pTamanio, Font.NORMAL, pColor ?? BaseColor.BLACK);
    }
    private static void AgregarFila(PdfPTable pTabla, Font pFuente, params string[] pTextos)
    {
        foreach (string texto in pTextos)
        {
            pTabla.AddCell(new PdfPCell(new Phrase(texto ?? "", pFuente)));
        }
    }
    private void GenerarPdf(string rutaCompleta, string titulo, List<IndicadorSectorListado> indicadores, DateTime periodoDesde, DateTime periodoHasta)
    {
        using (var stream = new FileStream(rutaCompleta, FileMode.Create))
        {
            var documento = new Document(PageSize.A4, 40, 40, 50, 50);
            PdfWriter.GetInstance(documento, stream);
            documento.Open();
            var fuenteTitulo = CrearFuente(18, true);
            var fuenteSubtitulo = CrearFuente(10, false, BaseColor.GRAY);
            var fuenteSeccion = CrearFuente(13, true);
            var fuenteNormal = CrearFuente(10, false);
            var fuenteEncabezado = CrearFuente(10, true);
            documento.Add(new Paragraph(titulo, fuenteTitulo));
            documento.Add(new Paragraph(T("pdf.periodo", periodoDesde.ToString("dd/MM/yyyy"), periodoHasta.ToString("dd/MM/yyyy")), fuenteSubtitulo));
            documento.Add(new Paragraph(T("pdf.generado", DateTime.Now.ToString("dd/MM/yyyy HH:mm")), fuenteSubtitulo));
            documento.Add(new Paragraph(" "));
            if (chkResumenEjecutivo.Checked)
            {
                documento.Add(new Paragraph(T("reportes.resumen"), fuenteSeccion));
                decimal ventasTotales = indicadores.Sum(i => i.Ventas);
                decimal circulacionPromedio = indicadores.Count > 0 ? indicadores.Average(i => i.Circulacion) : 0;
                documento.Add(new Paragraph(T("pdf.ventasTotales", ventasTotales.ToString("$#,##0")), fuenteNormal));
                documento.Add(new Paragraph(T("pdf.circulacionPromedio", circulacionPromedio.ToString("0.0")), fuenteNormal));
                documento.Add(new Paragraph(T("pdf.sectoresAnalizados", indicadores.Count), fuenteNormal));
                documento.Add(new Paragraph(" "));
            }
            if (chkIndicadoresPrincipales.Checked || chkDetallePorSector.Checked)
            {
                documento.Add(new Paragraph(T("indicadores.porSector"), fuenteSeccion));
                var tabla = new PdfPTable(5) { WidthPercentage = 100 };
                AgregarFila(tabla, fuenteEncabezado, T("layout.colSector"), T("indicadores.colVentas"), T("layout.colCirculacion"), T("indicadores.colRotacion"), T("indicadores.colIndice"));
                foreach (var ind in indicadores)
                {
                    AgregarFila(tabla, fuenteNormal, ind.Sector, ind.Ventas.ToString("$#,##0"), $"{ind.Circulacion:0.0}%", $"{ind.Rotacion:0.0}%", ind.IndiceGlobal.ToString());
                }
                documento.Add(tabla);
                documento.Add(new Paragraph(" "));
                byte[] graficoVentas = GenerarGraficoBarras(
                    T("stock.ventasPorSector"),
                    indicadores.Select(i => i.Sector).ToList(),
                    indicadores.Select(i => (double)i.Ventas).ToList());
                PdfImage imgVentas = PdfImage.GetInstance(graficoVentas);
                imgVentas.ScaleToFit(480, 260);
                documento.Add(imgVentas);
                documento.Add(new Paragraph(" "));
            }
            if (chkEstadoStock.Checked)
            {
                documento.Add(new Paragraph(T("reportes.estadoStock"), fuenteSeccion));
                List<StockListado> stock = stockBLL.ObtenerStockPorLocal(CodigoLocalActual);
                var tabla = new PdfPTable(4) { WidthPercentage = 100 };
                AgregarFila(tabla, fuenteEncabezado, T("productos.colProducto"), T("pdf.colDisponible"), T("stock.colMinimo"), T("comun.estado"));
                foreach (var st in stock)
                {
                    AgregarFila(tabla, fuenteNormal, st.Producto, st.CantidadDisponible.ToString(), st.Minimo?.ToString() ?? "-", TValor(st.EstadoStock));
                }
                documento.Add(tabla);
                documento.Add(new Paragraph(" "));
            }
            if (chkRecomendaciones.Checked)
            {
                documento.Add(new Paragraph(T("reportes.recomendaciones"), fuenteSeccion));
                List<StockListado> stockParaRecomendaciones = stockBLL.ObtenerStockPorLocal(CodigoLocalActual);
                var recomendacionBLL = new RecomendacionBLL();
                List<RecomendacionBE> recomendaciones = recomendacionBLL.GenerarRecomendaciones(CodigoLocalActual, indicadores, stockParaRecomendaciones);
                if (recomendaciones.Count == 0)
                {
                    documento.Add(new Paragraph(T("pdf.sinRecomendaciones"), fuenteNormal));
                }
                foreach (var rec in recomendaciones)
                {
                    documento.Add(new Paragraph($"[{TValor(rec.Prioridad)}] {TRecomendacion(rec, false)}", fuenteNormal));
                    documento.Add(new Paragraph(TRecomendacion(rec, true), fuenteSubtitulo));
                }
                documento.Add(new Paragraph(" "));
            }
            if (chkHistorialVentas.Checked)
            {
                documento.Add(new Paragraph(T("reportes.historialVentas"), fuenteSeccion));
                List<VentaListado> ventas = ventaBLL.ObtenerVentasAgrupadas(CodigoLocalActual, periodoDesde.ToString("yyyy-MM"));
                var tabla = new PdfPTable(4) { WidthPercentage = 100 };
                AgregarFila(tabla, fuenteEncabezado, T("stock.colPeriodo"), T("layout.colSector"), T("stock.colUnidades"), T("stock.colMonto"));
                foreach (var v in ventas)
                {
                    AgregarFila(tabla, fuenteNormal, v.Periodo, v.Sector, v.Unidades.ToString(), v.Monto.ToString("$#,##0"));
                }
                documento.Add(tabla);
            }
            if (chkComparacionEscenarios.Checked)
            {
                AgregarSeccionComparacion(documento, fuenteSeccion, fuenteNormal, fuenteSubtitulo, fuenteEncabezado);
            }

            if (chkDetalleProductos.Checked)
            {
                AgregarSeccionProductos(documento, fuenteSeccion, fuenteNormal, fuenteEncabezado);
            }
            documento.Close();
        }
    }
    private void AgregarSeccionComparacion(Document documento, Font fuenteSeccion, Font fuenteNormal, Font fuenteSubtitulo, Font fuenteEncabezado)
    {
        documento.Add(new Paragraph(T("principal.comparacion.titulo"), fuenteSeccion));
        if (string.IsNullOrEmpty(ddlEscenarioBase.SelectedValue) || string.IsNullOrEmpty(ddlEscenarioPropuesto.SelectedValue))
        {
            documento.Add(new Paragraph(T("pdf.errEscenarios"), fuenteSubtitulo));
            documento.Add(new Paragraph(" "));
            return;
        }
        int codigoBase = int.Parse(ddlEscenarioBase.SelectedValue);
        int codigoPropuesto = int.Parse(ddlEscenarioPropuesto.SelectedValue);
        EscenarioBE escenarioBase = escenarioBLL.ObtenerEscenarioPorCodigo(codigoBase);
        EscenarioBE escenarioPropuesto = escenarioBLL.ObtenerEscenarioPorCodigo(codigoPropuesto);
        if (escenarioBase.CodigoLocal != escenarioPropuesto.CodigoLocal)
        {
            documento.Add(new Paragraph(T("comparacion.errMismoLocal"), fuenteSubtitulo));
            documento.Add(new Paragraph(" "));
            return;
        }
        List<EscenarioCambio> cambiosBase = escenarioBLL.ObtenerCambiosPorEscenario(codigoBase);
        List<EscenarioCambio> cambiosPropuesto = escenarioBLL.ObtenerCambiosPorEscenario(codigoPropuesto);
        string periodoDesdeStr = DateTime.Now.AddMonths(-1).ToString("yyyy-MM");
        List<IndicadorSectorListado> indicadoresReales = indicadorBLL.CalcularIndicadoresPorSector(CodigoLocalActual, periodoDesdeStr);
        List<IndicadorSectorListado> proyectadosBase = indicadorBLL.ProyectarIndicadores(indicadoresReales, cambiosBase);
        List<IndicadorSectorListado> proyectadosPropuesto = indicadorBLL.ProyectarIndicadores(indicadoresReales, cambiosPropuesto);
        documento.Add(new Paragraph(T("pdf.baseVsPropuesto", escenarioBase.Nombre, escenarioPropuesto.Nombre), fuenteNormal));
        var tabla = new PdfPTable(4) { WidthPercentage = 100 };
        AgregarFila(tabla, fuenteEncabezado, T("layout.colSector"), T("pdf.ventasBase"), T("pdf.ventasPropuesto"), T("comparacion.colDiferencia"));
        foreach (var indPropuesto in proyectadosPropuesto)
        {
            var indBase = proyectadosBase.FirstOrDefault(p => p.CodigoSector == indPropuesto.CodigoSector);
            if (indBase == null) continue;

            decimal diferenciaPorc = indBase.Ventas != 0 ? ((indPropuesto.Ventas - indBase.Ventas) / indBase.Ventas) * 100 : 0;

            AgregarFila(tabla, fuenteNormal, indPropuesto.Sector, indBase.Ventas.ToString("$#,##0"), indPropuesto.Ventas.ToString("$#,##0"), (diferenciaPorc >= 0 ? "+" : "") + diferenciaPorc.ToString("0.0") + "%");
        }
        documento.Add(tabla);
        documento.Add(new Paragraph(" "));
        byte[] graficoComparacion = GenerarGraficoComparacion(proyectadosBase, proyectadosPropuesto);
        PdfImage imgComparacion = PdfImage.GetInstance(graficoComparacion);
        imgComparacion.ScaleToFit(480, 260);
        documento.Add(imgComparacion);
        documento.Add(new Paragraph(" "));
    }
    private void AgregarSeccionProductos(Document documento, Font fuenteSeccion, Font fuenteNormal, Font fuenteEncabezado)
    {
        documento.Add(new Paragraph(T("reportes.detalleProductos"), fuenteSeccion));
        List<SectorListado> sectores = sectorBLL.ObtenerSectoresPorLocal(CodigoLocalActual);
        var tabla = new PdfPTable(5) { WidthPercentage = 100 };
        AgregarFila(tabla, fuenteEncabezado, T("productos.colCodigo"), T("productos.colProducto"), T("productos.colCategoria"), T("layout.colSector"), T("productos.colPrecio"));
        foreach (var sector in sectores)
        {
            List<ProductoListado> productos = productoBLL.ObtenerProductosListado(CodigoLocalActual, "", sector.CodigoSector);
            foreach (var prod in productos)
            {
                AgregarFila(tabla, fuenteNormal, prod.CodigoInterno, prod.Nombre, prod.NombreCategoria, sector.Nombre, prod.Precio?.ToString("$#,##0.00") ?? "-");
            }
        }
        documento.Add(tabla);
        documento.Add(new Paragraph(" "));
    }
    private byte[] GenerarGraficoBarras(string titulo, List<string> etiquetas, List<double> valores)
    {
        using (var chart = new Chart())
        {
            chart.Width = 700;
            chart.Height = 380;
            var area = new ChartArea();
            chart.ChartAreas.Add(area);
            chart.Titles.Add(new Title(titulo) { Font = new System.Drawing.Font(familiaGrafico, 13, System.Drawing.FontStyle.Bold) });
            area.AxisX.LabelStyle.Font = new System.Drawing.Font(familiaGrafico, 9);
            area.AxisY.LabelStyle.Font = new System.Drawing.Font(familiaGrafico, 9);
            var serie = new Series
            {
                ChartType = SeriesChartType.Column,
                Color = System.Drawing.ColorTranslator.FromHtml("#2f6bff")
            };
            for (int i = 0; i < etiquetas.Count; i++)
            {
                serie.Points.AddXY(etiquetas[i], valores[i]);
            }
            chart.Series.Add(serie);
            using (var ms = new MemoryStream())
            {
                chart.SaveImage(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }
    }
    private byte[] GenerarGraficoComparacion(List<IndicadorSectorListado> baseInd, List<IndicadorSectorListado> propuestoInd)
    {
        using (var chart = new Chart())
        {
            chart.Width = 700;
            chart.Height = 380;
            var area = new ChartArea();
            chart.ChartAreas.Add(area);
            chart.Titles.Add(new Title(T("pdf.graficoComparacion")) { Font = new System.Drawing.Font(familiaGrafico, 13, System.Drawing.FontStyle.Bold) });
            area.AxisX.LabelStyle.Font = new System.Drawing.Font(familiaGrafico, 9);
            area.AxisY.LabelStyle.Font = new System.Drawing.Font(familiaGrafico, 9);
            var serieBase = new Series(T("valor.base")) { ChartType = SeriesChartType.Column, Color = System.Drawing.ColorTranslator.FromHtml("#9aa4b5") };
            var seriePropuesto = new Series(T("comparacion.colPropuesto")) { ChartType = SeriesChartType.Column, Color = System.Drawing.ColorTranslator.FromHtml("#2f6bff") };
            foreach (var ind in baseInd)
            {
                serieBase.Points.AddXY(ind.Sector, (double)ind.Ventas);
            }
            foreach (var ind in propuestoInd)
            {
                seriePropuesto.Points.AddXY(ind.Sector, (double)ind.Ventas);
            }
            chart.Series.Add(serieBase);
            chart.Series.Add(seriePropuesto);
            chart.Legends.Add(new Legend { Font = new System.Drawing.Font(familiaGrafico, 9) });
            using (var ms = new MemoryStream())
            {
                chart.SaveImage(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }
    }
    private void MostrarMensaje(string texto, bool esExito)
    {
        lblMensaje.Text = texto;
        lblMensaje.CssClass = esExito ? "mensaje" : "mensaje mensaje-error";
        lblMensaje.Visible = true;
    }
    #endregion

    protected void btnGenerarReporte_Click(object sender, EventArgs e)
    {
        if (CodigoLocalActual == 0)
        {
            MostrarMensaje(Traducir("stock.errSeleccionarLocal"), esExito: false);
            return;
        }
        DateTime periodoDesde, periodoHasta;
        if (!DateTime.TryParse(tbPeriodoDesde.Text, out periodoDesde) || !DateTime.TryParse(tbPeriodoHasta.Text, out periodoHasta))
        {
            MostrarMensaje(Traducir("reportes.errPeriodo"), esExito: false);
            return;
        }
        string periodoDesdeStr = periodoDesde.ToString("yyyy-MM");
        List<IndicadorSectorListado> indicadores = indicadorBLL.CalcularIndicadoresPorSector(CodigoLocalActual, periodoDesdeStr);
        if (indicadores.Count == 0)
        {
            MostrarMensaje(Traducir("reportes.errSinDatos"), esExito: false);
            return;
        }
        LocalListado local = localBLL.ObtenerLocalesListado("", null, null).Find(l => l.CodigoLocal == CodigoLocalActual);
        PrepararIdiomaReporte();
        string titulo = $"{TValor(ddlTipoReporte.SelectedValue)} — {local?.Nombre}";
        string nombreArchivo = $"reporte_{CodigoLocalActual}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
        string carpetaDestino = Server.MapPath("~/Subidas/Reportes/");
        if (!Directory.Exists(carpetaDestino)) Directory.CreateDirectory(carpetaDestino);
        string rutaCompleta = Path.Combine(carpetaDestino, nombreArchivo);
        GenerarPdf(rutaCompleta, titulo, indicadores, periodoDesde, periodoHasta);
        int? codigoEscenarioBase = string.IsNullOrEmpty(ddlEscenarioBase.SelectedValue) ? (int?)null : int.Parse(ddlEscenarioBase.SelectedValue);
        var reporte = new ReporteBE(CodigoLocalActual, codigoEscenarioBase, null, titulo, ddlTipoReporte.SelectedValue, "", $"Subidas/Reportes/{nombreArchivo}", periodoDesde, periodoHasta, DateTime.Now);
        reporteBLL.CrearReporte(reporte);
        MostrarMensaje(Traducir("reportes.msgGenerado"), esExito: true);
        CargarReportesRecientes();
    }

    protected void rptReportesRecientes_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int codigoReporte = int.Parse(e.CommandArgument.ToString());
        ReporteBE reporte = reporteBLL.ObtenerReportePorCodigo(codigoReporte);
        if (e.CommandName == "VerReporte" || e.CommandName == "DescargarReporte")
        {
            Response.Redirect("~/" + reporte.RutaArchivo);
        }
    }
}