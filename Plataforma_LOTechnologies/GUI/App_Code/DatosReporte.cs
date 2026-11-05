using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BE;

/// <summary>
/// Descripción breve de DatosReporte
/// </summary>
public class DatosReporte
{
    public string TituloTipo { get; set; } // "Reporte completo"
    public string NombreLocal { get; set; }
    public string TextoPeriodo { get; set; } // "08/2026 – 11/2026"
    public DateTime FechaGeneracion { get; set; }
    public bool IncluirResumen { get; set; }
    public bool IncluirEvolucion { get; set; }
    public bool IncluirIndicadores { get; set; }
    public bool IncluirRecomendaciones { get; set; }
    public bool IncluirComparacion { get; set; }
    public bool IncluirOportunidades { get; set; }
    public bool IncluirStock { get; set; }
    public bool IncluirHistorial { get; set; }
    public bool IncluirProductos { get; set; }

    // Resumen ejecutivo
    public decimal VentasPeriodo { get; set; }
    public decimal? VariacionVentas { get; set; } // % contra el período anterior (null = sin datos)
    public decimal? Margen { get; set; }
    public decimal? SuperficieLocal { get; set; }
    public int AlertasStock { get; set; } // productos con stock crítico o bajo
    public string NombrePlano { get; set; }
    public string RutaPlano { get; set; } // ruta física de la imagen (null = sin imagen)
    public List<IndicadorSectorListado> Indicadores { get; set; }
    public List<VentaMensual> Evolucion { get; set; }
    public List<FilaRecomendacion> Recomendaciones { get; set; }
    public DatosComparacionReporte Comparacion { get; set; }
    public List<FilaOportunidad> Oportunidades { get; set; }
    public List<StockListado> Stock { get; set; }
    public List<VentaListado> Historial { get; set; }
    public List<ProductoListado> Productos { get; set; }
}
public class FilaRecomendacion
{
    public string Prioridad { get; set; }
    public string TextoPrioridad { get; set; }
    public string Titulo { get; set; }
    public string Detalle { get; set; }
}
public class FilaOportunidad
{
    public string Producto { get; set; }
    public string Sector { get; set; }
    public string Tipo { get; set; }
    public string TextoTipo { get; set; }
    public string Prioridad { get; set; } // Alta / Media / Baja
    public string TextoPrioridad { get; set; }
    public string Motivo { get; set; }
}
public class DatosComparacionReporte
{
    public string Error { get; set; }
    public string NombreBase { get; set; }
    public string NombrePropuesto { get; set; }
    public string LayoutBase { get; set; }
    public string LayoutPropuesto { get; set; }
    public List<IndicadorSectorListado> Base { get; set; }
    public List<IndicadorSectorListado> Propuesto { get; set; }
    public List<string> CambiosPropuesto { get; set; }
    public string ConclusionTitulo { get; set; }
    public string ConclusionDetalle { get; set; }
}
public interface IGeneradorGraficos
{
    byte[] Barras(List<string> pEtiquetas, List<double> pValores, int pDestacado, bool pHorizontal, int pAncho, int pAlto);
    byte[] Comparacion(List<string> pEtiquetas, List<double> pBase, List<double> pPropuesto, string pNombreBase, string pNombrePropuesto, int pAncho, int pAlto);
}