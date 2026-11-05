using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Windows.Forms.DataVisualization.Charting;

/// <summary>
/// Descripción breve de GraficosReporte
/// </summary>
public class GraficosReporte : IGeneradorGraficos
{
    private static readonly Color AZUL = ColorTranslator.FromHtml("#3b82f6");
    private static readonly Color AZUL_OSCURO = ColorTranslator.FromHtml("#17223a");
    private static readonly Color GRIS_BASE = ColorTranslator.FromHtml("#a7b0bf");
    private static readonly Color TEXTO = ColorTranslator.FromHtml("#374151");
    private static readonly Color TEXTO_SUAVE = ColorTranslator.FromHtml("#6b7280");
    private static readonly Color GRILLA = ColorTranslator.FromHtml("#e5e9f0");
    private static readonly Color EJE = ColorTranslator.FromHtml("#c9d1dd");
    private const float TAMANIO_ETIQUETA = 17f;
    private const float TAMANIO_VALOR = 17f;
    private readonly string familia;
    public GraficosReporte(string pFamilia)
    {
        familia = string.IsNullOrEmpty(pFamilia) ? "Arial" : pFamilia;
    }
    public byte[] Barras(List<string> pEtiquetas, List<double> pValores, int pDestacado, bool pHorizontal, int pAncho, int pAlto)
    {
        using (Chart grafico = NuevoGrafico(pAncho, pAlto))
        {
            ChartArea area = NuevaArea(grafico, pValores.DefaultIfEmpty(0).Max(), pHorizontal);
            var serie = new Series
            {
                ChartType = pHorizontal ? SeriesChartType.Bar : SeriesChartType.Column,
                Color = AZUL,
                IsValueShownAsLabel = pEtiquetas.Count <= 14,
                LabelFormat = FormatoValor(pValores.DefaultIfEmpty(0).Max(), pEtiquetas.Count > 6),
                Font = new Font(familia, TAMANIO_VALOR, FontStyle.Bold),
                LabelForeColor = TEXTO
            };
            serie["PointWidth"] = pHorizontal ? "0.6" : "0.55";
            for (int i = 0; i < pEtiquetas.Count; i++)
            {
                int indice = serie.Points.AddXY(pEtiquetas[i], pValores[i]);
                if (i == pDestacado) serie.Points[indice].Color = AZUL_OSCURO;
            }
            grafico.Series.Add(serie);
            return Guardar(grafico);
        }
    }
    public byte[] Comparacion(List<string> pEtiquetas, List<double> pBase, List<double> pPropuesto, string pNombreBase, string pNombrePropuesto, int pAncho, int pAlto)
    {
        using (Chart grafico = NuevoGrafico(pAncho, pAlto))
        {
            NuevaArea(grafico, pBase.Concat(pPropuesto).DefaultIfEmpty(0).Max(), false);
            var serieBase = new Series(pNombreBase) { ChartType = SeriesChartType.Column, Color = GRIS_BASE };
            var seriePropuesto = new Series(pNombrePropuesto) { ChartType = SeriesChartType.Column, Color = AZUL };
            serieBase["PointWidth"] = "0.7";
            seriePropuesto["PointWidth"] = "0.7";
            for (int i = 0; i < pEtiquetas.Count; i++)
            {
                serieBase.Points.AddXY(pEtiquetas[i], pBase[i]);
                seriePropuesto.Points.AddXY(pEtiquetas[i], pPropuesto[i]);
            }
            grafico.Series.Add(serieBase);
            grafico.Series.Add(seriePropuesto);
            grafico.Legends.Add(new Legend
            {
                Docking = Docking.Top,
                Alignment = StringAlignment.Far,
                Font = new Font(familia, TAMANIO_ETIQUETA),
                ForeColor = TEXTO,
                BackColor = Color.Transparent,
                IsTextAutoFit = false
            });
            return Guardar(grafico);
        }
    }
    #region Estilo común
    private static Chart NuevoGrafico(int pAncho, int pAlto)
    {
        return new Chart
        {
            Width = pAncho,
            Height = pAlto,
            BackColor = Color.White,
            BorderlineWidth = 0,
            AntiAliasing = AntiAliasingStyles.All,
            TextAntiAliasingQuality = TextAntiAliasingQuality.High
        };
    }
    private ChartArea NuevaArea(Chart pGrafico, double pMaximo, bool pHorizontal)
    {
        var area = new ChartArea("principal") { BackColor = Color.White };
        // Eje de categorías (en barras horizontales queda vertical)
        area.AxisX.MajorGrid.Enabled = false;
        area.AxisX.MajorTickMark.Enabled = false;
        area.AxisX.LineColor = EJE;
        area.AxisX.LineWidth = 2;
        area.AxisX.Interval = 1;
        area.AxisX.LabelStyle.Font = new Font(familia, TAMANIO_ETIQUETA);
        area.AxisX.LabelStyle.ForeColor = TEXTO;
        area.AxisX.IsLabelAutoFit = true;
        area.AxisX.LabelAutoFitMinFontSize = 12;
        area.AxisX.LabelAutoFitMaxFontSize = (int)TAMANIO_ETIQUETA;
        area.AxisX.LabelAutoFitStyle = LabelAutoFitStyles.DecreaseFont | LabelAutoFitStyles.LabelsAngleStep45 | LabelAutoFitStyles.WordWrap;
        // Eje de valores: grilla suave, sin línea, formato de moneda abreviado
        double maximo = MaximoRedondo(pMaximo * (pHorizontal ? 1.15 : 1));
        area.AxisY.Minimum = 0;
        area.AxisY.Maximum = maximo;
        area.AxisY.Interval = maximo / 4;
        area.AxisY.MajorGrid.LineColor = GRILLA;
        area.AxisY.MajorGrid.LineWidth = 2;
        area.AxisY.MajorTickMark.Enabled = false;
        area.AxisY.LineColor = Color.Transparent;
        area.AxisY.LabelStyle.Format = FormatoValor(maximo, true);
        area.AxisY.LabelStyle.Font = new Font(familia, TAMANIO_ETIQUETA - 1);
        area.AxisY.LabelStyle.ForeColor = TEXTO_SUAVE;
        area.AxisY.IsLabelAutoFit = false;
        if (pHorizontal) area.AxisY.MajorGrid.Enabled = true;
        pGrafico.ChartAreas.Add(area);
        return area;
    }
    // Deja lugar arriba para la etiqueta del valor y usa un máximo "redondo" (1, 2, 2,5 o 5 por potencia de 10)
    private static double MaximoRedondo(double pMaximo)
    {
        if (pMaximo <= 0) return 1;
        double objetivo = pMaximo * 1.15;
        double potencia = Math.Pow(10, Math.Floor(Math.Log10(objetivo)));
        foreach (double paso in new[] { 1, 2, 2.5, 4, 5, 10 })
        {
            if (paso * potencia >= objetivo) return paso * potencia;
        }
        return 10 * potencia;
    }
    // $1,5M / $750k / $9.500
    private static string FormatoValor(double pMaximo, bool pAbreviado)
    {
        if (!pAbreviado) return "$#,##0";
        if (pMaximo >= 1000000) return "$#,##0.#,,M";
        if (pMaximo >= 10000) return "$#,##0,k";
        return "$#,##0";
    }
    private static byte[] Guardar(Chart pGrafico)
    {
        using (var memoria = new MemoryStream())
        {
            pGrafico.SaveImage(memoria, ChartImageFormat.Png);
            return memoria.ToArray();
        }
    }
    #endregion
}