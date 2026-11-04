using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using BE;

/// <summary>
/// Descripción breve de MapaLocal
/// </summary>

public class SectorMapa
{
    public string Nombre { get; set; }
    public string Ubicacion { get; set; }
    public decimal? Valor { get; set; }
    public string ValorTexto { get; set; }
}
public static class MapaLocal
{
    private static readonly string[] FILAS = { "Fondo", "Centro", "Frente" };          // de arriba hacia abajo: el frente (entrada) abajo
    private static readonly string[] COLUMNAS = { "Izquierda", "Centro", "Derecha" };
    private static readonly string[] EXTENSIONES_IMAGEN = { ".png", ".jpg", ".jpeg" };
    public const int NIVELES = 5;
    public static string ObtenerUrlImagen(LayoutBE pLayout, Func<string, string> pResolverUrl)
    {
        if (pLayout == null || string.IsNullOrEmpty(pLayout.NombreArchivo)) return null;
        string extension = System.IO.Path.GetExtension(pLayout.NombreArchivo).ToLowerInvariant();
        if (!EXTENSIONES_IMAGEN.Contains(extension)) return null;
        return pResolverUrl("~/Subidas/Layouts/" + pLayout.NombreArchivo);
    }
    public static string Generar(string pUrlImagen, string pNombreLayout, List<SectorMapa> pSectores, Func<string, string> pTraducir,
                                 decimal? pMinimo = null, decimal? pMaximo = null)
    {
        List<decimal> valores = pSectores.Where(s => s.Valor != null).Select(s => s.Valor.Value).ToList();
        decimal minimo = pMinimo ?? (valores.Count > 0 ? valores.Min() : 0);
        decimal maximo = pMaximo ?? (valores.Count > 0 ? valores.Max() : 0);

        var sb = new StringBuilder();
        sb.Append("<div class='mapa-local'>");
        sb.Append("<div class='mapa-plano").Append(pUrlImagen == null ? " mapa-sin-imagen" : "").Append("'>");
        if (pUrlImagen != null)
        {
            sb.Append("<img class='mapa-imagen' src='").Append(HttpUtility.HtmlAttributeEncode(pUrlImagen))
              .Append("' alt='").Append(HttpUtility.HtmlAttributeEncode(pNombreLayout ?? "")).Append("' />");
        }
        sb.Append("<div class='mapa-grilla'>");
        foreach (string fila in FILAS)
        {
            foreach (string columna in COLUMNAS)
            {
                string ubicacion = fila + " - " + columna;
                sb.Append("<div class='mapa-celda' title='").Append(HttpUtility.HtmlAttributeEncode(TraducirUbicacion(ubicacion, pTraducir))).Append("'>");
                foreach (SectorMapa sector in pSectores.Where(s => s.Ubicacion == ubicacion))
                {
                    AgregarSector(sb, sector, CalcularNivel(sector.Valor, minimo, maximo), pTraducir);
                }
                sb.Append("</div>");
            }
        }
        sb.Append("</div></div>");
        // Pie: orientación del plano y referencia de colores
        sb.Append("<div class='mapa-pie'>");
        sb.Append("<span class='texto-secundario'>&#8593; ").Append(Html(pTraducir("mapa.fondo")))
          .Append(" &middot; &#8595; ").Append(Html(pTraducir("mapa.frente"))).Append("</span>");
        sb.Append("<span class='mapa-leyenda'><span class='texto-secundario'>").Append(Html(pTraducir("mapa.menor"))).Append("</span>");
        for (int nivel = 1; nivel <= NIVELES; nivel++) sb.Append("<span class='mapa-leyenda-caja mapa-nivel-").Append(nivel).Append("'></span>");
        sb.Append("<span class='texto-secundario'>").Append(Html(pTraducir("mapa.mayor"))).Append("</span></span>");
        sb.Append("</div>");

        List<string> sinUbicacion = pSectores.Where(s => !FILAS.Any(f => (s.Ubicacion ?? "").StartsWith(f + " - "))).Select(s => s.Nombre).ToList();
        if (sinUbicacion.Count > 0)
        {
            sb.Append("<p class='texto-secundario mapa-nota'>")
              .Append(Html(string.Format(pTraducir("mapa.sinUbicacion"), string.Join(", ", sinUbicacion)))).Append("</p>");
        }
        if (pUrlImagen == null)
        {
            sb.Append("<p class='texto-secundario mapa-nota'>").Append(Html(pTraducir("mapa.sinPlano"))).Append("</p>");
        }
        sb.Append("</div>");
        return sb.ToString();
    }
    private static void AgregarSector(StringBuilder pSb, SectorMapa pSector, int pNivel, Func<string, string> pTraducir)
    {
        string valor = pSector.Valor == null ? pTraducir("mapa.sinDato") : pSector.ValorTexto;
        pSb.Append("<span class='mapa-sector mapa-nivel-").Append(pNivel).Append("'>")
           .Append("<span class='mapa-sector-nombre'>").Append(Html(pSector.Nombre)).Append("</span>")
           .Append("<span class='mapa-sector-valor'>").Append(Html(valor)).Append("</span>")
           .Append("</span>");
    }
    public static int CalcularNivel(decimal? pValor, decimal pMinimo, decimal pMaximo)
    {
        if (pValor == null) return 0;
        if (pMaximo <= pMinimo) return 3;
        decimal proporcion = (pValor.Value - pMinimo) / (pMaximo - pMinimo);
        int nivel = 1 + (int)Math.Round(Math.Max(0, Math.Min(1, proporcion)) * (NIVELES - 1), MidpointRounding.AwayFromZero);
        return nivel;
    }
    private static string TraducirUbicacion(string pUbicacion, Func<string, string> pTraducir)
    {
        string clave = ValoresTraducibles.ObtenerClave(pUbicacion);
        return clave == null ? pUbicacion : pTraducir(clave);
    }
    private static string Html(string pTexto)
    {
        return HttpUtility.HtmlEncode(pTexto ?? "");
    }
    public static string FormatearMoneda(decimal? pValor)
    {
        return pValor == null ? "" : pValor.Value.ToString("$#,##0", CultureInfo.CurrentCulture);
    }
    public static string FormatearPorcentaje(decimal? pValor)
    {
        return pValor == null ? "" : pValor.Value.ToString("0.#", CultureInfo.CurrentCulture) + "%";
    }
}