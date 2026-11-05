using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using BE;
using BLL;
using iTextSharp.text;
using iTextSharp.text.pdf;

/// <summary>
/// Descripción breve de ConstructorReportePdf
/// </summary>
public class ConstructorReportePdf
{
    #region Estilo
    private static readonly BaseColor AZUL_OSCURO = new BaseColor(0x17, 0x22, 0x3a);
    private static readonly BaseColor AZUL = new BaseColor(0x3b, 0x82, 0xf6);
    private static readonly BaseColor AZUL_CLARO = new BaseColor(0x8f, 0xb3, 0xff);
    private static readonly BaseColor TEXTO = new BaseColor(0x1f, 0x29, 0x37);
    private static readonly BaseColor GRIS = new BaseColor(0x6b, 0x72, 0x80);
    private static readonly BaseColor GRIS_CLARO = new BaseColor(0xc9, 0xd3, 0xe6);
    private static readonly BaseColor LINEA = new BaseColor(0xe5, 0xe9, 0xf0);
    private static readonly BaseColor CEBRA = new BaseColor(0xf4, 0xf6, 0xfa);
    private static readonly BaseColor FONDO_TARJETA = new BaseColor(0xf7, 0xf9, 0xfc);
    private static readonly BaseColor BORDE_TARJETA = new BaseColor(0xdb, 0xe2, 0xee);
    private static readonly BaseColor FONDO_CONCLUSION = new BaseColor(0xee, 0xf4, 0xff);
    private static readonly BaseColor VERDE = new BaseColor(0x16, 0xa3, 0x4a);
    private static readonly BaseColor VERDE_TEXTO = new BaseColor(0x15, 0x80, 0x3d);
    private static readonly BaseColor AMARILLO = new BaseColor(0xd9, 0x77, 0x06);
    private static readonly BaseColor ROJO = new BaseColor(0xdc, 0x26, 0x26);
    private static readonly BaseColor ROJO_TEXTO = new BaseColor(0xb9, 0x1c, 0x1c);
    private static readonly BaseColor ROJO_OSCURO = new BaseColor(0x99, 0x1b, 0x1b);
    private static readonly BaseColor AZUL_ETIQUETA = new BaseColor(0x25, 0x63, 0xeb);

    private const float MARGEN_LATERAL = 40;
    private const float MARGEN_SUPERIOR_PRIMERA = 118;
    private const float MARGEN_SUPERIOR = 54;
    private const float MARGEN_INFERIOR = 50;
    private const float ALTO_BANDA_PRIMERA = 96;
    private const float ALTO_BANDA = 26;
    private const float ESPACIO_ENTRE_SECCIONES = 20;
    private const float ESPACIO_MINIMO_SECCION = 120; // si queda menos lugar, la sección empieza en la página siguiente
    private const int ESCALA_GRAFICO = 3;              // los gráficos se dibujan al triple para que se vean nítidos
    #endregion

    private readonly Func<string, object[], string> traducir;
    private readonly IGeneradorGraficos graficos;
    private readonly FuentesReporte fuentes;
    private readonly CultureInfo cultura;
    private Document documento;
    private PdfWriter escritor;
    private DatosReporte datos;

    public ConstructorReportePdf(Func<string, object[], string> pTraducir, IGeneradorGraficos pGraficos, FuentesReporte pFuentes, CultureInfo pCultura)
    {
        traducir = pTraducir;
        graficos = pGraficos;
        fuentes = pFuentes;
        cultura = pCultura ?? CultureInfo.CurrentCulture;
    }
    public byte[] Construir(DatosReporte pDatos)
    {
        datos = pDatos;
        byte[] contenido;
        using (var memoria = new MemoryStream())
        {
            documento = new Document(PageSize.A4, MARGEN_LATERAL, MARGEN_LATERAL, MARGEN_SUPERIOR_PRIMERA, MARGEN_INFERIOR);
            escritor = PdfWriter.GetInstance(documento, memoria);
            documento.AddTitle(datos.TituloTipo + " - " + datos.NombreLocal);
            documento.AddAuthor("LO Technologies");
            documento.AddCreator("LO Technologies");
            documento.AddSubject(datos.TextoPeriodo);
            documento.Open();
            // Desde la segunda página el encabezado es más chico (los márgenes nuevos aplican a la página siguiente)
            documento.SetMargins(MARGEN_LATERAL, MARGEN_LATERAL, MARGEN_SUPERIOR, MARGEN_INFERIOR);

            if (datos.IncluirResumen) AgregarResumen();
            if (datos.IncluirEvolucion) AgregarEvolucion();
            if (datos.IncluirIndicadores) AgregarIndicadores();
            if (datos.IncluirRecomendaciones) AgregarRecomendaciones();
            if (datos.IncluirComparacion) AgregarComparacion();
            if (datos.IncluirOportunidades) AgregarOportunidades();
            if (datos.IncluirStock) AgregarStock();
            if (datos.IncluirHistorial) AgregarHistorial();
            if (datos.IncluirProductos) AgregarProductos();
            documento.Close();
            contenido = memoria.ToArray();
        }
        return AgregarEncabezadosYPies(contenido);
    }

    #region Secciones
    private void AgregarResumen()
    {
        Seccion(T("reportes.resumen"));
        List<IndicadorSectorListado> indicadores = datos.Indicadores ?? new List<IndicadorSectorListado>();
        int indicePromedio = indicadores.Count > 0 ? (int)Math.Round(indicadores.Average(i => i.IndiceGlobal), 0) : 0;
        Kpi variacion = datos.VariacionVentas == null
            ? new Kpi(T("indicadores.ventasPeriodo"), Moneda(datos.VentasPeriodo), T("pdf.sinPeriodoAnterior"), GRIS)
            : new Kpi(T("indicadores.ventasPeriodo"), Moneda(datos.VentasPeriodo), T("pdf.vsPeriodoAnterior", Porcentaje(datos.VariacionVentas.Value, true)), ColorVariacion(datos.VariacionVentas.Value));
        Kpi margen = datos.Margen == null
            ? new Kpi(T("stock.colMargen"), "–", T("pdf.sinCostos"), GRIS)
            : new Kpi(T("stock.colMargen"), Porcentaje(datos.Margen.Value, false), T("pdf.margenDetalle"), GRIS);
        Kpi ventasM2 = datos.SuperficieLocal == null || datos.SuperficieLocal <= 0
            ? new Kpi(T("indicadores.ventasM2Local"), "–", T("pdf.sinSuperficie"), GRIS)
            : new Kpi(T("indicadores.ventasM2Local"), Moneda(datos.VentasPeriodo / datos.SuperficieLocal.Value), T("pdf.superficieLocal", datos.SuperficieLocal.Value.ToString("#,##0.#")), GRIS);
        Kpi indice = new Kpi(T("comparacion.indicePromedio"), indicadores.Count > 0 ? indicePromedio.ToString() : "–",
                             indicadores.Count > 0 ? TextoIndice(indicePromedio) : T("pdf.sinSectores"), indicadores.Count > 0 ? ColorIndice(indicePromedio) : GRIS);
        Tarjetas(variacion, margen, ventasM2, indice);

        // Línea de destacados
        var destacados = new List<string>();
        if (indicadores.Count > 0)
        {
            IndicadorSectorListado mejor = indicadores.OrderByDescending(i => i.IndiceGlobal).First();
            destacados.Add(T("pdf.mejorSector", mejor.Sector, mejor.IndiceGlobal));
            if (indicadores.Count > 1)
            {
                IndicadorSectorListado peor = indicadores.OrderBy(i => i.IndiceGlobal).First();
                destacados.Add(T("pdf.sectorRevisar", peor.Sector, peor.IndiceGlobal));
            }
        }
        if (datos.AlertasStock > 0) destacados.Add(T("pdf.alertasStock", datos.AlertasStock));
        if (destacados.Count > 0)
        {
            var linea = new Paragraph(string.Join("  ·  ", destacados), Fuente(8.5f, false, TEXTO)) { SpacingBefore = 8 };
            linea.SetLeading(0, 1.3f);
            documento.Add(linea);
        }
        AgregarPlano();
    }
    private void AgregarPlano()
    {
        if (string.IsNullOrEmpty(datos.NombrePlano)) return;
        if (!string.IsNullOrEmpty(datos.RutaPlano) && File.Exists(datos.RutaPlano))
        {
            try
            {
                iTextSharp.text.Image plano = iTextSharp.text.Image.GetInstance(datos.RutaPlano);
                plano.ScaleToFit(260, 150);
                plano.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                plano.SpacingBefore = 12;
                plano.Border = Rectangle.BOX;
                plano.BorderColor = BORDE_TARJETA;
                plano.BorderWidth = 0.7f;
                documento.Add(plano);
            }
            catch (Exception) { }
        }
        documento.Add(new Paragraph(T("pdf.planoVigente", datos.NombrePlano), Fuente(7.5f, false, GRIS)) { Alignment = Element.ALIGN_CENTER, SpacingBefore = 4 });
    }
    private void AgregarEvolucion()
    {
        Seccion(T("indicadores.evolucion"));
        List<VentaMensual> meses = datos.Evolucion ?? new List<VentaMensual>();
        if (meses.Count == 0 || meses.All(m => m.Monto == 0))
        {
            SinDatos(T("pdf.sinVentas"));
            return;
        }
        int enCurso = meses.FindIndex(m => m.EnCurso);
        List<string> etiquetas = meses.Select(m => MostrarPeriodo(m.Periodo) + (m.EnCurso ? "*" : "")).ToList();
        AgregarGrafico(graficos.Barras(etiquetas, meses.Select(m => (double)m.Monto).ToList(), enCurso, false, 515 * ESCALA_GRAFICO, 165 * ESCALA_GRAFICO), 515, 165);
        if (enCurso >= 0) Nota(T("pdf.notaEnCurso"));
    }
    private void AgregarIndicadores()
    {
        Seccion(T("indicadores.porSector"));
        List<IndicadorSectorListado> indicadores = (datos.Indicadores ?? new List<IndicadorSectorListado>()).OrderByDescending(i => i.IndiceGlobal).ToList();
        if (indicadores.Count == 0)
        {
            SinDatos(T("pdf.sinSectores"));
            return;
        }
        float[] anchos = { 22, 14, 9, 12, 11, 10, 11, 9 };
        PdfPTable tabla = Tabla(anchos);
        Encabezados(tabla, anchos, T("layout.colSector"), "<" + T("indicadores.colVentas"), "<" + T("stock.colMargen"), "<" + T("indicadores.colVentasM2"),
                    "<" + T("layout.colCirculacion"), "<" + T("indicadores.colRotacion"), "<" + T("indicadores.colCobertura"), "<" + T("pdf.colIndice"));
        int fila = 0;
        foreach (IndicadorSectorListado ind in indicadores)
        {
            tabla.AddCell(Celda(ind.Sector, fila));
            tabla.AddCell(Celda(Moneda(ind.Ventas), fila, Element.ALIGN_RIGHT));
            tabla.AddCell(Celda(ind.Margen == null ? "–" : Porcentaje(ind.Margen.Value, false), fila, Element.ALIGN_RIGHT));
            tabla.AddCell(Celda(ind.VentasPorM2 == null ? "–" : Moneda(ind.VentasPorM2.Value), fila, Element.ALIGN_RIGHT));
            tabla.AddCell(Celda(Porcentaje(ind.Circulacion, false), fila, Element.ALIGN_RIGHT));
            tabla.AddCell(Celda(Porcentaje(ind.Rotacion, false), fila, Element.ALIGN_RIGHT));
            tabla.AddCell(Celda(ind.CoberturaDias == null ? "–" : T("pdf.dias", ind.CoberturaDias.Value.ToString("#,##0")), fila, Element.ALIGN_RIGHT));
            tabla.AddCell(CeldaEtiqueta(ind.IndiceGlobal.ToString(), ColorIndice(ind.IndiceGlobal), fila, Element.ALIGN_RIGHT, 24, 3));
            fila++;
        }
        documento.Add(tabla);
        Nota(T("indicadores.ayudaIndice"));

        Subtitulo(T("stock.ventasPorSector"));
        // Barras horizontales: el gráfico dibuja de abajo hacia arriba, se invierte para que el mejor quede arriba
        List<IndicadorSectorListado> porVentas = indicadores.OrderBy(i => i.Ventas).ToList();
        int alto = Math.Min(260, 28 + porVentas.Count * 24);
        AgregarGrafico(graficos.Barras(porVentas.Select(i => i.Sector).ToList(), porVentas.Select(i => (double)i.Ventas).ToList(), -1, true,
                                       515 * ESCALA_GRAFICO, alto * ESCALA_GRAFICO), 515, alto);
    }
    private void AgregarRecomendaciones()
    {
        Seccion(T("reportes.recomendaciones"));
        if (datos.Recomendaciones == null || datos.Recomendaciones.Count == 0)
        {
            SinDatos(T("pdf.sinRecomendaciones"));
            return;
        }
        var tabla = new PdfPTable(new float[] { 70, 445 }) { WidthPercentage = 100, SplitLate = false };
        foreach (FilaRecomendacion rec in datos.Recomendaciones)
        {
            var etiqueta = new PdfPCell(new Phrase(" ")) { Border = Rectangle.BOTTOM_BORDER, BorderColorBottom = LINEA, BorderWidthBottom = 0.5f, MinimumHeight = 22 };
            etiqueta.CellEvent = new EventoEtiqueta(Mayusculas(rec.TextoPrioridad), Fuente(6.8f, true, BaseColor.WHITE), ColorPrioridad(rec.Prioridad), Element.ALIGN_LEFT, 54, 5.5f, true);
            tabla.AddCell(etiqueta);
            var texto = new PdfPCell { Border = Rectangle.BOTTOM_BORDER, BorderColorBottom = LINEA, BorderWidthBottom = 0.5f, PaddingTop = 2, PaddingBottom = 7 };
            var titulo = new Paragraph(rec.Titulo, Fuente(8.8f, true, AZUL_OSCURO));
            titulo.SetLeading(0, 1.25f);
            texto.AddElement(titulo);
            var detalle = new Paragraph(rec.Detalle, Fuente(8, false, TEXTO));
            detalle.SetLeading(0, 1.25f);
            texto.AddElement(detalle);
            tabla.AddCell(texto);
        }
        documento.Add(tabla);
    }
    private void AgregarComparacion()
    {
        Seccion(T("principal.comparacion.titulo"));
        DatosComparacionReporte comp = datos.Comparacion;
        if (comp == null || !string.IsNullOrEmpty(comp.Error))
        {
            SinDatos(comp == null ? T("pdf.errEscenarios") : comp.Error);
            return;
        }
        var encabezado = new Paragraph();
        encabezado.Add(new Chunk(T("pdf.baseVsPropuesto", comp.NombreBase, comp.NombrePropuesto), Fuente(9, true, TEXTO)));
        encabezado.Add(new Chunk("   ·   " + T("pdf.layoutBase", comp.LayoutBase, comp.LayoutPropuesto), Fuente(8, false, GRIS)));
        encabezado.SpacingAfter = 8;
        documento.Add(encabezado);

        decimal ventasBase = comp.Base.Sum(i => i.Ventas), ventasPropuesto = comp.Propuesto.Sum(i => i.Ventas);
        int indiceBase = comp.Base.Count > 0 ? (int)Math.Round(comp.Base.Average(i => i.IndiceGlobal), 0) : 0;
        int indicePropuesto = comp.Propuesto.Count > 0 ? (int)Math.Round(comp.Propuesto.Average(i => i.IndiceGlobal), 0) : 0;
        var filas = new List<Tuple<IndicadorSectorListado, IndicadorSectorListado>>();
        foreach (IndicadorSectorListado propuesto in comp.Propuesto)
        {
            IndicadorSectorListado base_ = comp.Base.FirstOrDefault(b => b.CodigoSector == propuesto.CodigoSector);
            if (base_ != null) filas.Add(Tuple.Create(base_, propuesto));
        }
        int mejoran = filas.Count(f => f.Item2.Ventas > f.Item1.Ventas);
        int empeoran = filas.Count(f => f.Item2.Ventas < f.Item1.Ventas);
        decimal? variacion = IndicadorBLL.Variacion(ventasPropuesto, ventasBase);
        Tarjetas(
            new Kpi(T("comparacion.ventasEstimadas"), Moneda(ventasPropuesto),
                    variacion == null ? T("pdf.sinBase") : T("pdf.vsBase", Porcentaje(variacion.Value, true) + " (" + Signo(ventasPropuesto - ventasBase) + Moneda(ventasPropuesto - ventasBase) + ")"),
                    variacion == null ? GRIS : ColorVariacion(variacion.Value)),
            new Kpi(T("comparacion.indicePromedio"), indicePropuesto.ToString(), T("pdf.puntosVsBase", Signo(indicePropuesto - indiceBase) + (indicePropuesto - indiceBase)),
                    ColorVariacion(indicePropuesto - indiceBase)),
            new Kpi(T("pdf.sectoresMejoran"), T("pdf.xDeY", mejoran, filas.Count), T("pdf.empeoran", empeoran), empeoran > 0 ? ROJO_TEXTO : GRIS));

        float[] anchos = { 25, 19, 19, 15, 22 };
        PdfPTable tabla = Tabla(anchos);
        tabla.SpacingBefore = 8;
        Encabezados(tabla, anchos, T("layout.colSector"), "<" + T("pdf.ventasBase"), "<" + T("pdf.ventasPropuesto"), "<" + T("comparacion.colDiferencia"), "<" + T("pdf.indiceBaseAPropuesto"));
        int fila = 0;
        foreach (var par in filas)
        {
            decimal diferencia = par.Item1.Ventas != 0 ? (par.Item2.Ventas - par.Item1.Ventas) / par.Item1.Ventas * 100 : 0;
            tabla.AddCell(Celda(par.Item2.Sector, fila));
            tabla.AddCell(Celda(Moneda(par.Item1.Ventas), fila, Element.ALIGN_RIGHT));
            tabla.AddCell(Celda(Moneda(par.Item2.Ventas), fila, Element.ALIGN_RIGHT));
            tabla.AddCell(Celda(Porcentaje(diferencia, true), fila, Element.ALIGN_RIGHT, Fuente(8.5f, diferencia != 0, ColorVariacion(diferencia))));
            tabla.AddCell(Celda(par.Item1.IndiceGlobal + "  →  " + par.Item2.IndiceGlobal, fila, Element.ALIGN_RIGHT));
            fila++;
        }
        documento.Add(tabla);

        if (filas.Count > 0)
        {
            AgregarGrafico(graficos.Comparacion(filas.Select(f => f.Item2.Sector).ToList(), filas.Select(f => (double)f.Item1.Ventas).ToList(),
                                                filas.Select(f => (double)f.Item2.Ventas).ToList(), comp.NombreBase, comp.NombrePropuesto,
                                                515 * ESCALA_GRAFICO, 170 * ESCALA_GRAFICO), 515, 170);
        }
        // Conclusión
        var caja = new PdfPTable(1) { WidthPercentage = 100, SpacingBefore = 8 };
        var celda = new PdfPCell { Border = Rectangle.NO_BORDER, PaddingLeft = 12, PaddingRight = 10, PaddingTop = 6, PaddingBottom = 9 };
        celda.CellEvent = new EventoTarjeta(FONDO_CONCLUSION, null, AZUL, 0, 0, 0);
        var conclusion = new Paragraph();
        conclusion.Add(new Chunk(T("comparacion.conclusion") + ": ", Fuente(8.8f, true, AZUL_OSCURO)));
        conclusion.Add(new Chunk(comp.ConclusionTitulo + " ", Fuente(8.8f, true, TEXTO)));
        conclusion.Add(new Chunk(comp.ConclusionDetalle ?? "", Fuente(8.5f, false, TEXTO)));
        conclusion.SetLeading(0, 1.3f);
        celda.AddElement(conclusion);
        if (comp.CambiosPropuesto != null && comp.CambiosPropuesto.Count > 0)
        {
            var cambios = new Paragraph(T("pdf.cambiosDe", comp.NombrePropuesto) + " " + string.Join(" · ", comp.CambiosPropuesto), Fuente(7.8f, false, GRIS)) { SpacingBefore = 3 };
            cambios.SetLeading(0, 1.3f);
            celda.AddElement(cambios);
        }
        caja.AddCell(celda);
        documento.Add(caja);
        Nota(T("pdf.notaSimulacion"));
    }
    private void AgregarOportunidades()
    {
        Seccion(T("pdf.oportunidades"));
        if (datos.Oportunidades == null || datos.Oportunidades.Count == 0)
        {
            SinDatos(T("pdf.sinOportunidades"));
            return;
        }
        float[] anchos = { 20, 14, 20, 46 };
        PdfPTable tabla = Tabla(anchos);
        Encabezados(tabla, anchos, T("productos.colProducto"), T("layout.colSector"), T("pdf.colTipo"), T("pdf.colMotivo"));
        int fila = 0;
        foreach (FilaOportunidad op in datos.Oportunidades)
        {
            var producto = Celda("", fila);
            var frase = new Phrase();
            frase.Add(new Chunk(op.Producto, Fuente(8.5f, true, TEXTO)));
            if (!string.IsNullOrEmpty(op.TextoPrioridad)) frase.Add(new Chunk("\n" + op.TextoPrioridad, Fuente(7, false, ColorPrioridad(op.Prioridad))));
            producto.Phrase = frase;
            tabla.AddCell(producto);
            tabla.AddCell(Celda(string.IsNullOrEmpty(op.Sector) ? "–" : op.Sector, fila));
            tabla.AddCell(CeldaEtiqueta(op.TextoTipo, ColorTipo(op.Tipo), fila, Element.ALIGN_LEFT, 0, 5.5f, AnchoColumna(anchos, 2)));
            tabla.AddCell(Celda(op.Motivo, fila, Element.ALIGN_LEFT, Fuente(7.8f, false, TEXTO)));
            fila++;
        }
        documento.Add(tabla);
    }
    private void AgregarStock()
    {
        Seccion(T("reportes.estadoStock"));
        if (datos.Stock == null || datos.Stock.Count == 0)
        {
            SinDatos(T("pdf.sinStock"));
            return;
        }
        string[] orden = { "Crítico", "Bajo", "Sobrestock", "Normal" };
        List<StockListado> stock = datos.Stock.OrderBy(s => Array.IndexOf(orden, s.EstadoStock) < 0 ? 9 : Array.IndexOf(orden, s.EstadoStock)).ThenBy(s => s.Producto).ToList();
        float[] anchos = { 30, 20, 12, 11, 11, 16 };
        PdfPTable tabla = Tabla(anchos);
        Encabezados(tabla, anchos, T("productos.colProducto"), T("productos.colCategoria"), "<" + T("pdf.colDisponible"), "<" + T("stock.colMinimo"), "<" + T("stock.colMaximo"), T("comun.estado"));
        int fila = 0;
        foreach (StockListado st in stock)
        {
            tabla.AddCell(Celda(st.Producto, fila));
            tabla.AddCell(Celda(st.Categoria, fila));
            tabla.AddCell(Celda(st.CantidadDisponible.ToString("#,##0"), fila, Element.ALIGN_RIGHT));
            tabla.AddCell(Celda(st.Minimo == null ? "–" : st.Minimo.Value.ToString("#,##0"), fila, Element.ALIGN_RIGHT));
            tabla.AddCell(Celda(st.Maximo == null ? "–" : st.Maximo.Value.ToString("#,##0"), fila, Element.ALIGN_RIGHT));
            tabla.AddCell(CeldaEtiqueta(TValor(st.EstadoStock), ColorEstadoStock(st.EstadoStock), fila, Element.ALIGN_LEFT, 0, 5.5f, AnchoColumna(anchos, 5)));
            fila++;
        }
        documento.Add(tabla);
    }
    private void AgregarHistorial()
    {
        Seccion(T("reportes.historialVentas"));
        if (datos.Historial == null || datos.Historial.Count == 0)
        {
            SinDatos(T("pdf.sinVentas"));
            return;
        }
        float[] anchos = { 16, 36, 14, 20, 14 };
        PdfPTable tabla = Tabla(anchos);
        Encabezados(tabla, anchos, T("stock.colPeriodo"), T("layout.colSector"), "<" + T("stock.colUnidades"), "<" + T("stock.colMonto"), "<" + T("stock.colMargen"));
        int fila = 0;
        foreach (VentaListado v in datos.Historial)
        {
            tabla.AddCell(Celda(MostrarPeriodo(v.Periodo), fila));
            tabla.AddCell(Celda(string.IsNullOrEmpty(v.Sector) || v.Sector == "-" ? T("pdf.sinUbicar") : v.Sector, fila));
            tabla.AddCell(Celda(v.Unidades.ToString("#,##0"), fila, Element.ALIGN_RIGHT));
            tabla.AddCell(Celda(Moneda(v.Monto), fila, Element.ALIGN_RIGHT));
            tabla.AddCell(Celda(v.Margen == null ? "–" : Porcentaje(v.Margen.Value, false), fila, Element.ALIGN_RIGHT));
            fila++;
        }
        documento.Add(tabla);
    }
    private void AgregarProductos()
    {
        Seccion(T("reportes.detalleProductos"));
        if (datos.Productos == null || datos.Productos.Count == 0)
        {
            SinDatos(T("pdf.sinProductos"));
            return;
        }
        float[] anchos = { 13, 30, 20, 17, 12, 8 };
        PdfPTable tabla = Tabla(anchos);
        Encabezados(tabla, anchos, T("productos.colCodigo"), T("productos.colProducto"), T("productos.colCategoria"), T("layout.colSector"), "<" + T("productos.colPrecio"), "<" + T("productos.colStock"));
        int fila = 0;
        foreach (ProductoListado p in datos.Productos)
        {
            tabla.AddCell(Celda(p.CodigoInterno, fila, Element.ALIGN_LEFT, Fuente(8, false, GRIS)));
            tabla.AddCell(Celda(p.Nombre, fila));
            tabla.AddCell(Celda(p.NombreCategoria, fila));
            tabla.AddCell(Celda(string.IsNullOrEmpty(p.NombreSector) ? T("pdf.sinUbicar") : p.NombreSector, fila));
            tabla.AddCell(Celda(p.Precio == null ? "–" : p.Precio.Value.ToString("$#,##0.00"), fila, Element.ALIGN_RIGHT));
            tabla.AddCell(Celda(p.Stock.ToString("#,##0"), fila, Element.ALIGN_RIGHT));
            fila++;
        }
        documento.Add(tabla);
    }
    #endregion

    #region Bloques
    private void Seccion(string pTitulo)
    {
        // No dejar un título solo al pie de la página
        if (escritor.GetVerticalPosition(true) - documento.BottomMargin < ESPACIO_MINIMO_SECCION) documento.NewPage();
        var tabla = new PdfPTable(1) { WidthPercentage = 100, SpacingBefore = ESPACIO_ENTRE_SECCIONES, SpacingAfter = 9 };
        tabla.AddCell(new PdfPCell(new Phrase(pTitulo, Fuente(12.5f, true, AZUL_OSCURO)))
        {
            Border = Rectangle.BOTTOM_BORDER,
            BorderColorBottom = AZUL,
            BorderWidthBottom = 1.5f,
            PaddingLeft = 0,
            PaddingBottom = 5
        });
        documento.Add(tabla);
    }
    private void Subtitulo(string pTexto)
    {
        documento.Add(new Paragraph(pTexto, Fuente(9, true, AZUL_OSCURO)) { SpacingBefore = 12, SpacingAfter = 2 });
    }
    private void Nota(string pTexto)
    {
        var nota = new Paragraph(pTexto, Fuente(7, false, GRIS)) { SpacingBefore = 4 };
        nota.SetLeading(0, 1.3f);
        documento.Add(nota);
    }
    private void SinDatos(string pTexto)
    {
        documento.Add(new Paragraph(pTexto, Fuente(8.5f, false, GRIS)));
    }
    private void AgregarGrafico(byte[] pImagen, float pAncho, float pAlto)
    {
        if (pImagen == null) return;
        iTextSharp.text.Image imagen = iTextSharp.text.Image.GetInstance(pImagen);
        imagen.ScaleToFit(pAncho, pAlto);
        imagen.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
        imagen.SpacingBefore = 6;
        documento.Add(imagen);
    }
    private class Kpi
    {
        public string Titulo, Valor, Detalle;
        public BaseColor ColorDetalle;
        public Kpi(string pTitulo, string pValor, string pDetalle, BaseColor pColorDetalle)
        {
            Titulo = pTitulo; Valor = pValor; Detalle = pDetalle; ColorDetalle = pColorDetalle;
        }
    }
    private void Tarjetas(params Kpi[] pKpis)
    {
        const float SEPARACION = 3.5f;
        var tabla = new PdfPTable(pKpis.Length) { WidthPercentage = 100 };
        for (int i = 0; i < pKpis.Length; i++)
        {
            Kpi kpi = pKpis[i];
            float izquierda = i == 0 ? 0 : SEPARACION, derecha = i == pKpis.Length - 1 ? 0 : SEPARACION;
            var celda = new PdfPCell { Border = Rectangle.NO_BORDER, PaddingLeft = izquierda + 9, PaddingRight = derecha + 7, PaddingTop = 5, PaddingBottom = 9 };
            celda.CellEvent = new EventoTarjeta(FONDO_TARJETA, BORDE_TARJETA, null, izquierda, derecha, 4);
            var titulo = new Paragraph(Mayusculas(kpi.Titulo), Fuente(6.6f, false, GRIS));
            titulo.SetLeading(0, 1.25f);
            celda.AddElement(titulo);
            var valor = new Paragraph(kpi.Valor, Fuente(14.5f, true, AZUL_OSCURO)) { SpacingBefore = 1 };
            valor.SetLeading(0, 1.2f);
            celda.AddElement(valor);
            var detalle = new Paragraph(kpi.Detalle, Fuente(7, kpi.ColorDetalle != GRIS, kpi.ColorDetalle));
            detalle.SetLeading(0, 1.25f);
            celda.AddElement(detalle);
            tabla.AddCell(celda);
        }
        documento.Add(tabla);
    }
    private static PdfPTable Tabla(float[] pAnchos)
    {
        return new PdfPTable(pAnchos) { WidthPercentage = 100, HeaderRows = 1, SplitLate = false, SpacingAfter = 2 };
    }
    // Un encabezado que empieza con "<" va alineado a la derecha (columnas numéricas)
    private void Encabezados(PdfPTable pTabla, float[] pAnchos, params string[] pTextos)
    {
        foreach (string texto in pTextos)
        {
            bool derecha = texto.StartsWith("<");
            pTabla.AddCell(new PdfPCell(new Phrase(derecha ? texto.Substring(1) : texto, Fuente(7.6f, true, BaseColor.WHITE)))
            {
                BackgroundColor = AZUL_OSCURO,
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = derecha ? Element.ALIGN_RIGHT : Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                PaddingTop = 4.5f,
                PaddingBottom = 6,
                PaddingLeft = 5,
                PaddingRight = 5
            });
        }
    }
    private PdfPCell Celda(string pTexto, int pFila, int pAlineacion = Element.ALIGN_LEFT, Font pFuente = null)
    {
        var celda = new PdfPCell(new Phrase(pTexto ?? "", pFuente ?? Fuente(8.5f, false, TEXTO)))
        {
            BackgroundColor = pFila % 2 == 1 ? CEBRA : BaseColor.WHITE,
            Border = Rectangle.BOTTOM_BORDER,
            BorderColorBottom = LINEA,
            BorderWidthBottom = 0.5f,
            HorizontalAlignment = pAlineacion,
            VerticalAlignment = Element.ALIGN_MIDDLE,
            PaddingTop = 4,
            PaddingBottom = 6,
            PaddingLeft = 5,
            PaddingRight = 5,
            MinimumHeight = 20
        };
        celda.SetLeading(0, 1.2f);
        return celda;
    }
    // Etiqueta de color (estado, prioridad, índice). Si el texto no entra en la columna se muestra como texto de color.
    private PdfPCell CeldaEtiqueta(string pTexto, BaseColor pColor, int pFila, int pAlineacion, float pAnchoMinimo, float pRadio, float pAnchoColumna = 0)
    {
        Font fuente = Fuente(7, true, BaseColor.WHITE);
        if (pAnchoColumna > 0 && AnchoTexto(pTexto, fuente) + 10 > pAnchoColumna - 10)
        {
            return Celda(pTexto, pFila, pAlineacion, Fuente(8, true, pColor));
        }
        PdfPCell celda = Celda(" ", pFila, pAlineacion);
        celda.CellEvent = new EventoEtiqueta(pTexto, fuente, pColor, pAlineacion, pAnchoMinimo, pRadio, false);
        return celda;
    }
    private static float AnchoColumna(float[] pAnchos, int pIndice)
    {
        return (PageSize.A4.Width - 2 * MARGEN_LATERAL) * pAnchos[pIndice] / pAnchos.Sum();
    }
    private static float AnchoTexto(string pTexto, Font pFuente)
    {
        BaseFont baseFuente = pFuente.BaseFont ?? pFuente.GetCalculatedBaseFont(false);
        return baseFuente.GetWidthPoint(pTexto ?? "", pFuente.Size);
    }
    #endregion

    #region Encabezado y pie
    private byte[] AgregarEncabezadosYPies(byte[] pContenido)
    {
        var lector = new PdfReader(pContenido);
        using (var salida = new MemoryStream())
        {
            var estampador = new PdfStamper(lector, salida);
            int paginas = lector.NumberOfPages;
            for (int pagina = 1; pagina <= paginas; pagina++)
            {
                PdfContentByte lienzo = estampador.GetOverContent(pagina);
                Rectangle tamanio = lector.GetPageSize(pagina);
                if (pagina == 1) DibujarEncabezadoPrincipal(lienzo, tamanio);
                else DibujarEncabezadoSecundario(lienzo, tamanio);
                DibujarPie(lienzo, tamanio, pagina, paginas);
            }
            estampador.Close();
            lector.Close();
            return salida.ToArray();
        }
    }
    private void DibujarEncabezadoPrincipal(PdfContentByte pLienzo, Rectangle pTamanio)
    {
        float arriba = pTamanio.Height;
        pLienzo.SaveState();
        pLienzo.SetColorFill(AZUL_OSCURO);
        pLienzo.Rectangle(0, arriba - ALTO_BANDA_PRIMERA, pTamanio.Width, ALTO_BANDA_PRIMERA);
        pLienzo.Fill();
        pLienzo.SetColorFill(AZUL);
        pLienzo.Rectangle(0, arriba - ALTO_BANDA_PRIMERA - 3, pTamanio.Width, 3);
        pLienzo.Fill();
        pLienzo.RestoreState();

        var marca = new Chunk("LO TECHNOLOGIES", Fuente(8, true, AZUL_CLARO));
        marca.SetCharacterSpacing(1.6f);
        ColumnText.ShowTextAligned(pLienzo, Element.ALIGN_LEFT, new Phrase(marca), MARGEN_LATERAL, arriba - 28, 0);
        ColumnText.ShowTextAligned(pLienzo, Element.ALIGN_LEFT, new Phrase(datos.TituloTipo, Fuente(19, true, BaseColor.WHITE)), MARGEN_LATERAL, arriba - 54, 0);

        var meta = new Phrase();
        AgregarDato(meta, T("pdf.metaLocal"), datos.NombreLocal);
        AgregarDato(meta, T("pdf.metaPeriodo"), datos.TextoPeriodo);
        AgregarDato(meta, T("pdf.metaGenerado"), datos.FechaGeneracion.ToString("dd/MM/yyyy HH:mm"));
        ColumnText.ShowTextAligned(pLienzo, Element.ALIGN_LEFT, meta, MARGEN_LATERAL, arriba - 77, 0);
    }
    private void AgregarDato(Phrase pFrase, string pEtiqueta, string pValor)
    {
        if (pFrase.Count > 0) pFrase.Add(new Chunk("        ", Fuente(8.5f, false, GRIS_CLARO)));
        pFrase.Add(new Chunk(pEtiqueta + ": ", Fuente(8.5f, false, GRIS_CLARO)));
        pFrase.Add(new Chunk(pValor ?? "", Fuente(8.5f, true, BaseColor.WHITE)));
    }
    private void DibujarEncabezadoSecundario(PdfContentByte pLienzo, Rectangle pTamanio)
    {
        float arriba = pTamanio.Height;
        pLienzo.SaveState();
        pLienzo.SetColorFill(AZUL_OSCURO);
        pLienzo.Rectangle(0, arriba - ALTO_BANDA, pTamanio.Width, ALTO_BANDA);
        pLienzo.Fill();
        pLienzo.SetColorFill(AZUL);
        pLienzo.Rectangle(0, arriba - ALTO_BANDA - 2, pTamanio.Width, 2);
        pLienzo.Fill();
        pLienzo.RestoreState();
        var marca = new Chunk("LO TECHNOLOGIES", Fuente(7.5f, true, BaseColor.WHITE));
        marca.SetCharacterSpacing(1.2f);
        ColumnText.ShowTextAligned(pLienzo, Element.ALIGN_LEFT, new Phrase(marca), MARGEN_LATERAL, arriba - 16.5f, 0);
        ColumnText.ShowTextAligned(pLienzo, Element.ALIGN_RIGHT, new Phrase(datos.TituloTipo + "  ·  " + datos.NombreLocal + "  ·  " + datos.TextoPeriodo, Fuente(7.5f, false, GRIS_CLARO)),
                                   pTamanio.Width - MARGEN_LATERAL, arriba - 16.5f, 0);
    }
    private void DibujarPie(PdfContentByte pLienzo, Rectangle pTamanio, int pPagina, int pPaginas)
    {
        pLienzo.SaveState();
        pLienzo.SetColorStroke(BORDE_TARJETA);
        pLienzo.SetLineWidth(0.6f);
        pLienzo.MoveTo(MARGEN_LATERAL, 34);
        pLienzo.LineTo(pTamanio.Width - MARGEN_LATERAL, 34);
        pLienzo.Stroke();
        pLienzo.RestoreState();
        Font fuente = Fuente(7.2f, false, GRIS);
        ColumnText.ShowTextAligned(pLienzo, Element.ALIGN_LEFT, new Phrase("LO Technologies  ·  " + datos.NombreLocal + "  ·  " + datos.TituloTipo, fuente), MARGEN_LATERAL, 22, 0);
        ColumnText.ShowTextAligned(pLienzo, Element.ALIGN_RIGHT, new Phrase(T("pdf.pagina", pPagina, pPaginas), fuente), pTamanio.Width - MARGEN_LATERAL, 22, 0);
    }
    #endregion

    #region Formato y colores
    private string T(string pClave, params object[] pValores)
    {
        return traducir(pClave, pValores);
    }
    private string TValor(string pValor)
    {
        string clave = ValoresTraducibles.ObtenerClave(pValor);
        return clave == null ? (pValor ?? "") : T(clave);
    }
    private Font Fuente(float pTamanio, bool pNegrita, BaseColor pColor)
    {
        BaseFont baseFuente = pNegrita ? fuentes.Negrita : fuentes.Normal;
        if (baseFuente == null) return FontFactory.GetFont(pNegrita ? FontFactory.HELVETICA_BOLD : FontFactory.HELVETICA, pTamanio, pColor);
        return new Font(baseFuente, pTamanio, Font.NORMAL, pColor);
    }
    private string Mayusculas(string pTexto)
    {
        return (pTexto ?? "").ToUpper(cultura);
    }
    private static string Moneda(decimal pValor)
    {
        return pValor.ToString("$#,##0");
    }
    private static string Porcentaje(decimal pValor, bool pConSigno)
    {
        return (pConSigno ? Signo(pValor) : "") + pValor.ToString("#,##0.#") + "%";
    }
    private static string Signo(decimal pValor)
    {
        return pValor > 0 ? "+" : "";
    }
    // "2026-11" -> "11/2026"
    private static string MostrarPeriodo(string pPeriodo)
    {
        DateTime fecha;
        return DateTime.TryParseExact(pPeriodo + "-01", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha) ? fecha.ToString("MM/yyyy") : pPeriodo;
    }
    private string TextoIndice(int pIndice)
    {
        if (pIndice >= IndicadorBLL.INDICE_BUENO) return T("pdf.indiceBueno");
        if (pIndice >= IndicadorBLL.INDICE_REGULAR) return T("pdf.indiceRegular");
        return T("pdf.indiceBajo");
    }
    private static BaseColor ColorIndice(int pIndice)
    {
        if (pIndice >= IndicadorBLL.INDICE_BUENO) return VERDE;
        if (pIndice >= IndicadorBLL.INDICE_REGULAR) return AMARILLO;
        return ROJO;
    }
    private static BaseColor ColorVariacion(decimal pValor)
    {
        return pValor > 0 ? VERDE_TEXTO : pValor < 0 ? ROJO_TEXTO : GRIS;
    }
    private static BaseColor ColorPrioridad(string pPrioridad)
    {
        switch (pPrioridad)
        {
            case "Alto": case "Alta": return ROJO;
            case "Medio": case "Media": return AMARILLO;
            case "Positivo": return VERDE;
            default: return GRIS;
        }
    }
    private static BaseColor ColorEstadoStock(string pEstado)
    {
        switch (pEstado)
        {
            case "Crítico": return ROJO_OSCURO;
            case "Bajo": return ROJO;
            case "Sobrestock": return AMARILLO;
            default: return VERDE;
        }
    }
    private static BaseColor ColorTipo(string pTipo)
    {
        switch (pTipo)
        {
            case ProductoAnalisisListado.TIPO_ESTRELLA: return AZUL_ETIQUETA;
            case ProductoAnalisisListado.TIPO_RIESGO_QUIEBRE: return ROJO;
            case ProductoAnalisisListado.TIPO_LUGAR_PRIVILEGIADO:
            case ProductoAnalisisListado.TIPO_COBERTURA_EXCESIVA: return AMARILLO;
            default: return GRIS;
        }
    }
    #endregion
}
public class EventoTarjeta : IPdfPCellEvent
{
    private readonly BaseColor fondo, borde, acento;
    private readonly float margenIzquierdo, margenDerecho, radio;
    public EventoTarjeta(BaseColor pFondo, BaseColor pBorde, BaseColor pAcento, float pMargenIzquierdo, float pMargenDerecho, float pRadio)
    {
        fondo = pFondo; borde = pBorde; acento = pAcento;
        margenIzquierdo = pMargenIzquierdo; margenDerecho = pMargenDerecho; radio = pRadio;
    }
    public void CellLayout(PdfPCell pCelda, Rectangle pPosicion, PdfContentByte[] pLienzos)
    {
        PdfContentByte lienzo = pLienzos[PdfPTable.BACKGROUNDCANVAS];
        float izquierda = pPosicion.Left + margenIzquierdo, ancho = pPosicion.Width - margenIzquierdo - margenDerecho;
        lienzo.SaveState();
        lienzo.SetColorFill(fondo);
        if (borde != null)
        {
            lienzo.SetColorStroke(borde);
            lienzo.SetLineWidth(0.7f);
        }
        if (radio > 0) lienzo.RoundRectangle(izquierda, pPosicion.Bottom, ancho, pPosicion.Height, radio);
        else lienzo.Rectangle(izquierda, pPosicion.Bottom, ancho, pPosicion.Height);
        if (borde != null) lienzo.FillStroke(); else lienzo.Fill();
        if (acento != null)
        {
            lienzo.SetColorFill(acento);
            lienzo.Rectangle(izquierda, pPosicion.Bottom, 3, pPosicion.Height);
            lienzo.Fill();
        }
        lienzo.RestoreState();
    }
}
public class EventoEtiqueta : IPdfPCellEvent
{
    private const float RELLENO_HORIZONTAL = 5.5f;
    private const float RELLENO_CELDA = 5;
    private readonly string texto;
    private readonly Font fuente;
    private readonly BaseColor color;
    private readonly int alineacion;
    private readonly float anchoMinimo, radio;
    private readonly bool arriba;
    public EventoEtiqueta(string pTexto, Font pFuente, BaseColor pColor, int pAlineacion, float pAnchoMinimo, float pRadio, bool pArriba)
    {
        texto = pTexto ?? ""; fuente = pFuente; color = pColor; alineacion = pAlineacion;
        anchoMinimo = pAnchoMinimo; radio = pRadio; arriba = pArriba;
    }
    public void CellLayout(PdfPCell pCelda, Rectangle pPosicion, PdfContentByte[] pLienzos)
    {
        BaseFont baseFuente = fuente.BaseFont ?? fuente.GetCalculatedBaseFont(false);
        float ancho = Math.Max(anchoMinimo, baseFuente.GetWidthPoint(texto, fuente.Size) + 2 * RELLENO_HORIZONTAL);
        float alto = fuente.Size + 5.5f;
        float centroY = arriba ? pPosicion.Top - 5 - alto / 2 : (pPosicion.Top + pPosicion.Bottom) / 2;
        float x;
        if (alineacion == Element.ALIGN_RIGHT) x = pPosicion.Right - RELLENO_CELDA - ancho;
        else if (alineacion == Element.ALIGN_CENTER) x = (pPosicion.Left + pPosicion.Right - ancho) / 2;
        else x = pPosicion.Left + RELLENO_CELDA;

        PdfContentByte fondo = pLienzos[PdfPTable.BACKGROUNDCANVAS];
        fondo.SaveState();
        fondo.SetColorFill(color);
        fondo.RoundRectangle(x, centroY - alto / 2, ancho, alto, Math.Min(radio, alto / 2));
        fondo.Fill();
        fondo.RestoreState();
        ColumnText.ShowTextAligned(pLienzos[PdfPTable.TEXTCANVAS], Element.ALIGN_CENTER, new Phrase(texto, fuente), x + ancho / 2, centroY - fuente.Size * 0.35f, 0);
    }
}