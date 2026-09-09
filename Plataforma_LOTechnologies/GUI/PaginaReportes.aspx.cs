using System;
using System.Collections.Generic;
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

public partial class PaginaReportes : System.Web.UI.Page
{
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
        if (!Sesion.Instancia.IsLogueado())
        {
            Response.Redirect("PaginaLogin.aspx");
            return;
        }

        if (!IsPostBack)
        {
            CargarDropdownLocales();
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
    private void CargarDropdownEscenarios()
    {
        if (CodigoLocalActual == 0) return;
        List<EscenarioListado> escenarios = escenarioBLL.ObtenerEscenariosPorLocal(CodigoLocalActual);
        ddlEscenarioBase.Items.Clear();
        ddlEscenarioPropuesto.Items.Clear();
        ddlEscenarioBase.Items.Add(new WebListItem("— Ninguno —", ""));
        ddlEscenarioPropuesto.Items.Add(new WebListItem("— Ninguno —", ""));
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
    private void GenerarPdf(string rutaCompleta, string titulo, List<IndicadorSectorListado> indicadores, DateTime periodoDesde, DateTime periodoHasta)
    {
        using (var stream = new FileStream(rutaCompleta, FileMode.Create))
        {
            var documento = new Document(PageSize.A4, 40, 40, 50, 50);
            PdfWriter.GetInstance(documento, stream);
            documento.Open();
            var fuenteTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
            var fuenteSubtitulo = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.GRAY);
            var fuenteSeccion = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 13);
            var fuenteNormal = FontFactory.GetFont(FontFactory.HELVETICA, 10);
            documento.Add(new Paragraph(titulo, fuenteTitulo));
            documento.Add(new Paragraph($"Período: {periodoDesde:dd/MM/yyyy} - {periodoHasta:dd/MM/yyyy}", fuenteSubtitulo));
            documento.Add(new Paragraph($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}", fuenteSubtitulo));
            documento.Add(new Paragraph(" "));
            if (chkResumenEjecutivo.Checked)
            {
                documento.Add(new Paragraph("Resumen Ejecutivo", fuenteSeccion));
                decimal ventasTotales = indicadores.Sum(i => i.Ventas);
                decimal circulacionPromedio = indicadores.Count > 0 ? indicadores.Average(i => i.Circulacion) : 0;
                documento.Add(new Paragraph($"Ventas totales del período: {ventasTotales:$#,##0}", fuenteNormal));
                documento.Add(new Paragraph($"Circulación promedio: {circulacionPromedio:0.0}%", fuenteNormal));
                documento.Add(new Paragraph($"Sectores analizados: {indicadores.Count}", fuenteNormal));
                documento.Add(new Paragraph(" "));
            }
            if (chkIndicadoresPrincipales.Checked || chkDetallePorSector.Checked)
            {
                documento.Add(new Paragraph("Indicadores por Sector", fuenteSeccion));
                var tabla = new PdfPTable(5) { WidthPercentage = 100 };
                tabla.AddCell("Sector"); tabla.AddCell("Ventas"); tabla.AddCell("Circulación"); tabla.AddCell("Rotación"); tabla.AddCell("Índice Global");
                foreach (var ind in indicadores)
                {
                    tabla.AddCell(ind.Sector);
                    tabla.AddCell(ind.Ventas.ToString("$#,##0"));
                    tabla.AddCell($"{ind.Circulacion:0.0}%");
                    tabla.AddCell($"{ind.Rotacion:0.0}%");
                    tabla.AddCell(ind.IndiceGlobal.ToString());
                }
                documento.Add(tabla);
                documento.Add(new Paragraph(" "));
                byte[] graficoVentas = GenerarGraficoBarras(
                    "Ventas por Sector",
                    indicadores.Select(i => i.Sector).ToList(),
                    indicadores.Select(i => (double)i.Ventas).ToList());
                PdfImage imgVentas = PdfImage.GetInstance(graficoVentas);
                imgVentas.ScaleToFit(480, 260);
                documento.Add(imgVentas);
                documento.Add(new Paragraph(" "));
            }
            if (chkEstadoStock.Checked)
            {
                documento.Add(new Paragraph("Estado de Stock", fuenteSeccion));
                List<StockListado> stock = stockBLL.ObtenerStockPorLocal(CodigoLocalActual);
                var tabla = new PdfPTable(4) { WidthPercentage = 100 };
                tabla.AddCell("Producto"); tabla.AddCell("Disponible"); tabla.AddCell("Mínimo"); tabla.AddCell("Estado");
                foreach (var st in stock)
                {
                    tabla.AddCell(st.Producto);
                    tabla.AddCell(st.CantidadDisponible.ToString());
                    tabla.AddCell(st.Minimo?.ToString() ?? "-");
                    tabla.AddCell(st.EstadoStock);
                }
                documento.Add(tabla);
                documento.Add(new Paragraph(" "));
            }
            if (chkRecomendaciones.Checked)
            {
                documento.Add(new Paragraph("Recomendaciones", fuenteSeccion));
                List<StockListado> stockParaRecomendaciones = stockBLL.ObtenerStockPorLocal(CodigoLocalActual);
                var recomendacionBLL = new RecomendacionBLL();
                List<RecomendacionBE> recomendaciones = recomendacionBLL.GenerarRecomendaciones(CodigoLocalActual, indicadores, stockParaRecomendaciones);
                if (recomendaciones.Count == 0)
                {
                    documento.Add(new Paragraph("No hay recomendaciones para este período.", fuenteNormal));
                }
                foreach (var rec in recomendaciones)
                {
                    documento.Add(new Paragraph($"[{rec.Prioridad}] {rec.Descripcion}", fuenteNormal));
                    documento.Add(new Paragraph(rec.Motivo, fuenteSubtitulo));
                }
                documento.Add(new Paragraph(" "));
            }
            if (chkHistorialVentas.Checked)
            {
                documento.Add(new Paragraph("Historial de Ventas", fuenteSeccion));
                List<VentaListado> ventas = ventaBLL.ObtenerVentasAgrupadas(CodigoLocalActual, periodoDesde.ToString("yyyy-MM"));
                var tabla = new PdfPTable(4) { WidthPercentage = 100 };
                tabla.AddCell("Período"); tabla.AddCell("Sector"); tabla.AddCell("Unidades"); tabla.AddCell("Monto");
                foreach (var v in ventas)
                {
                    tabla.AddCell(v.Periodo);
                    tabla.AddCell(v.Sector);
                    tabla.AddCell(v.Unidades.ToString());
                    tabla.AddCell(v.Monto.ToString("$#,##0"));
                }
                documento.Add(tabla);
            }
            if (chkComparacionEscenarios.Checked)
            {
                AgregarSeccionComparacion(documento, fuenteSeccion, fuenteNormal, fuenteSubtitulo);
            }

            if (chkDetalleProductos.Checked)
            {
                AgregarSeccionProductos(documento, fuenteSeccion);
            }            
            documento.Close();
        }
    }
    private void AgregarSeccionComparacion(Document documento, Font fuenteSeccion, Font fuenteNormal, Font fuenteSubtitulo)
    {
        documento.Add(new Paragraph("Comparación de Escenarios", fuenteSeccion));
        if (string.IsNullOrEmpty(ddlEscenarioBase.SelectedValue) || string.IsNullOrEmpty(ddlEscenarioPropuesto.SelectedValue))
        {
            documento.Add(new Paragraph("Seleccioná un Escenario Base y un Escenario Propuesto para incluir esta sección.", fuenteSubtitulo));
            documento.Add(new Paragraph(" "));
            return;
        }
        int codigoBase = int.Parse(ddlEscenarioBase.SelectedValue);
        int codigoPropuesto = int.Parse(ddlEscenarioPropuesto.SelectedValue);
        EscenarioBE escenarioBase = escenarioBLL.ObtenerEscenarioPorCodigo(codigoBase);
        EscenarioBE escenarioPropuesto = escenarioBLL.ObtenerEscenarioPorCodigo(codigoPropuesto);
        if (escenarioBase.CodigoLocal != escenarioPropuesto.CodigoLocal)
        {
            documento.Add(new Paragraph("Los escenarios seleccionados deben pertenecer al mismo local.", fuenteSubtitulo));
            documento.Add(new Paragraph(" "));
            return;
        }
        List<EscenarioCambio> cambiosBase = escenarioBLL.ObtenerCambiosPorEscenario(codigoBase);
        List<EscenarioCambio> cambiosPropuesto = escenarioBLL.ObtenerCambiosPorEscenario(codigoPropuesto);
        string periodoDesdeStr = DateTime.Now.AddMonths(-1).ToString("yyyy-MM");
        List<IndicadorSectorListado> indicadoresReales = indicadorBLL.CalcularIndicadoresPorSector(CodigoLocalActual, periodoDesdeStr);
        List<IndicadorSectorListado> proyectadosBase = indicadorBLL.ProyectarIndicadores(indicadoresReales, cambiosBase);
        List<IndicadorSectorListado> proyectadosPropuesto = indicadorBLL.ProyectarIndicadores(indicadoresReales, cambiosPropuesto);
        documento.Add(new Paragraph($"{escenarioBase.Nombre} (base) vs. {escenarioPropuesto.Nombre} (propuesto)", fuenteNormal));
        var tabla = new PdfPTable(4) { WidthPercentage = 100 };
        tabla.AddCell("Sector"); tabla.AddCell("Ventas Base"); tabla.AddCell("Ventas Propuesto"); tabla.AddCell("Diferencia");
        foreach (var indPropuesto in proyectadosPropuesto)
        {
            var indBase = proyectadosBase.FirstOrDefault(p => p.CodigoSector == indPropuesto.CodigoSector);
            if (indBase == null) continue;

            decimal diferenciaPorc = indBase.Ventas != 0 ? ((indPropuesto.Ventas - indBase.Ventas) / indBase.Ventas) * 100 : 0;

            tabla.AddCell(indPropuesto.Sector);
            tabla.AddCell(indBase.Ventas.ToString("$#,##0"));
            tabla.AddCell(indPropuesto.Ventas.ToString("$#,##0"));
            tabla.AddCell((diferenciaPorc >= 0 ? "+" : "") + diferenciaPorc.ToString("0.0") + "%");
        }
        documento.Add(tabla);
        documento.Add(new Paragraph(" "));
        byte[] graficoComparacion = GenerarGraficoComparacion(proyectadosBase, proyectadosPropuesto);
        PdfImage imgComparacion = PdfImage.GetInstance(graficoComparacion);
        imgComparacion.ScaleToFit(480, 260);
        documento.Add(imgComparacion);
        documento.Add(new Paragraph(" "));
    }
    private void AgregarSeccionProductos(Document documento, Font fuenteSeccion)
    {
        documento.Add(new Paragraph("Detalle de Productos", fuenteSeccion));
        List<SectorListado> sectores = sectorBLL.ObtenerSectoresPorLocal(CodigoLocalActual);
        var tabla = new PdfPTable(5) { WidthPercentage = 100 };
        tabla.AddCell("Código"); tabla.AddCell("Producto"); tabla.AddCell("Categoría"); tabla.AddCell("Sector"); tabla.AddCell("Precio");
        foreach (var sector in sectores)
        {
            List<ProductoListado> productos = productoBLL.ObtenerProductosListado("", sector.CodigoSector);
            foreach (var prod in productos)
            {
                tabla.AddCell(prod.CodigoInterno);
                tabla.AddCell(prod.Nombre);
                tabla.AddCell(prod.NombreCategoria);
                tabla.AddCell(sector.Nombre);
                tabla.AddCell(prod.Precio?.ToString("$#,##0.00") ?? "-");
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
            chart.Titles.Add(new Title(titulo));
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
            chart.Titles.Add(new Title("Ventas estimadas: Base vs. Propuesto"));
            var serieBase = new Series("Base") { ChartType = SeriesChartType.Column, Color = System.Drawing.ColorTranslator.FromHtml("#9aa4b5") };
            var seriePropuesto = new Series("Propuesto") { ChartType = SeriesChartType.Column, Color = System.Drawing.ColorTranslator.FromHtml("#2f6bff") };
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
            chart.Legends.Add(new Legend());
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
            MostrarMensaje("Seleccioná un local.", esExito: false);
            return;
        }
        DateTime periodoDesde, periodoHasta;
        if (!DateTime.TryParse(tbPeriodoDesde.Text, out periodoDesde) || !DateTime.TryParse(tbPeriodoHasta.Text, out periodoHasta))
        {
            MostrarMensaje("Complete el período desde y hasta.", esExito: false);
            return;
        }
        string periodoDesdeStr = periodoDesde.ToString("yyyy-MM");
        List<IndicadorSectorListado> indicadores = indicadorBLL.CalcularIndicadoresPorSector(CodigoLocalActual, periodoDesdeStr);
        if (indicadores.Count == 0)
        {
            MostrarMensaje("No hay datos suficientes para generar el reporte.", esExito: false);
            return;
        }
        LocalListado local = localBLL.ObtenerLocalesListado("", null, null).Find(l => l.CodigoLocal == CodigoLocalActual);
        string titulo = $"{ddlTipoReporte.SelectedItem.Text} — {local?.Nombre}";
        string nombreArchivo = $"reporte_{CodigoLocalActual}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
        string carpetaDestino = Server.MapPath("~/Subidas/Reportes/");
        if (!Directory.Exists(carpetaDestino)) Directory.CreateDirectory(carpetaDestino);
        string rutaCompleta = Path.Combine(carpetaDestino, nombreArchivo);
        GenerarPdf(rutaCompleta, titulo, indicadores, periodoDesde, periodoHasta);
        int? codigoEscenarioBase = string.IsNullOrEmpty(ddlEscenarioBase.SelectedValue) ? (int?)null : int.Parse(ddlEscenarioBase.SelectedValue);
        var reporte = new ReporteBE(CodigoLocalActual, codigoEscenarioBase, null, titulo, ddlTipoReporte.SelectedValue, "", $"Subidas/Reportes/{nombreArchivo}", periodoDesde, periodoHasta, DateTime.Now);        
        reporteBLL.CrearReporte(reporte);
        MostrarMensaje("Reporte generado correctamente.", esExito: true);
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