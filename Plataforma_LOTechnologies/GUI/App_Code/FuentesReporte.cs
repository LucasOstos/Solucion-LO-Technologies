using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Web;
using iTextSharp.text.pdf;
using SERVICIO.Logica;

/// <summary>
/// Descripción breve de FuentesReporte
/// </summary>
public class FuentesReporte
{
    public BaseFont Normal { get; private set; }
    public BaseFont Negrita { get; private set; }
    public string FamiliaGrafico { get; private set; }
    private class Familia
    {
        public string Nombre;
        public string ArchivoNormal;
        public string ArchivoNegrita;
        public Familia(string pNombre, string pNormal, string pNegrita) { Nombre = pNombre; ArchivoNormal = pNormal; ArchivoNegrita = pNegrita; }
    }
    private static readonly Familia[] FAMILIAS_LATINAS =
    {
        new Familia("Aptos", "Aptos.ttf", "Aptos-Bold.ttf"),
        new Familia("Arial", "arial.ttf", "arialbd.ttf"),
        new Familia("Segoe UI", "segoeui.ttf", "segoeuib.ttf"),
        new Familia("Tahoma", "tahoma.ttf", "tahomabd.ttf")
    };
    private static readonly Familia[] FAMILIAS_JAPONES =
    {
        new Familia("MS Gothic", "msgothic.ttc,0", "msgothic.ttc,0"),
        new Familia("Meiryo", "meiryo.ttc,0", "meiryob.ttc,0"),
        new Familia("Yu Gothic", "YuGothM.ttc,0", "YuGothB.ttc,0")
    };
    private static readonly Familia[] FAMILIAS_CHINO =
    {
        new Familia("Microsoft YaHei", "msyh.ttc,0", "msyhbd.ttc,0"),
        new Familia("SimSun", "simsun.ttc,0", "simsun.ttc,0")
    };
    private static readonly Familia[] FAMILIAS_COREANO =
    {
        new Familia("Malgun Gothic", "malgun.ttf", "malgunbd.ttf")
    };
    // Armenio y georgiano no están en Arial
    private static readonly Familia[] FAMILIAS_CAUCASO =
    {
        new Familia("Segoe UI", "segoeui.ttf", "segoeuib.ttf"),
        new Familia("Sylfaen", "sylfaen.ttf", "sylfaen.ttf")
    };
    private static readonly Familia[] FAMILIAS_AMARICO =
    {
        new Familia("Ebrima", "ebrima.ttf", "ebrimabd.ttf")
    };
    // Escrituras que iTextSharp no soporta para estos idiomas no se ofrece el PDF.
    private static readonly string[] IDIOMAS_ESCRITURA_COMPLEJA = { "hi", "bn", "ta", "te", "mr", "gu", "pa", "ne", "th", "km", "si", "my", "lo", "kn", "ml", "or" };

    // El PDF se puede generar en este idioma
    public static bool IdiomaSoportado(string pCodigoIdioma)
    {
        return !CatalogoTraducciones.EsDerechaAIzquierda(pCodigoIdioma) && !IDIOMAS_ESCRITURA_COMPLEJA.Contains(pCodigoIdioma);
    }
    private static Familia[] FamiliasDelIdioma(string pCodigoIdioma)
    {
        switch (pCodigoIdioma)
        {
            case "ja": return FAMILIAS_JAPONES;
            case "zh": return FAMILIAS_CHINO;
            case "ko": return FAMILIAS_COREANO;
            case "hy": case "ka": return FAMILIAS_CAUCASO;
            case "am": return FAMILIAS_AMARICO;
            default: return FAMILIAS_LATINAS;
        }
    }

    // pCarpetaAplicacion: carpeta física del sitio (se busca en su subcarpeta Fuentes)
    public static FuentesReporte Preparar(string pCodigoIdioma, string pCarpetaAplicacion)
    {
        var carpetas = new List<string>();
        if (!string.IsNullOrEmpty(pCarpetaAplicacion)) carpetas.Add(Path.Combine(pCarpetaAplicacion, "Fuentes"));
        carpetas.Add(Environment.GetFolderPath(Environment.SpecialFolder.Fonts));
        // Fuentes instaladas "solo para este usuario"
        carpetas.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft", "Windows", "Fonts"));

        var fuentes = new FuentesReporte { FamiliaGrafico = "Arial" };
        foreach (Familia familia in FamiliasDelIdioma(pCodigoIdioma))
        {
            BaseFont normal = CrearFuenteBase(carpetas, familia.ArchivoNormal);
            if (normal == null) continue;
            fuentes.Normal = normal;
            fuentes.Negrita = CrearFuenteBase(carpetas, familia.ArchivoNegrita) ?? normal;
            if (FamiliaInstalada(familia.Nombre)) fuentes.FamiliaGrafico = familia.Nombre;
            break;
        }
        return fuentes;
    }
    private static BaseFont CrearFuenteBase(List<string> pCarpetas, string pArchivo)
    {
        string nombre = pArchivo.Split(',')[0];
        string indice = pArchivo.Contains(",") ? pArchivo.Substring(pArchivo.IndexOf(',')) : "";
        foreach (string carpeta in pCarpetas.Where(c => !string.IsNullOrEmpty(c) && Directory.Exists(c)))
        {
            string ruta = Path.Combine(carpeta, nombre);
            if (!File.Exists(ruta)) continue;
            try
            {
                return BaseFont.CreateFont(ruta + indice, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            }
            catch (Exception) { }
        }
        return null;
    }
    private static bool FamiliaInstalada(string pFamilia)
    {
        try
        {
            using (var instaladas = new InstalledFontCollection())
            {
                return instaladas.Families.Any(f => string.Equals(f.Name, pFamilia, StringComparison.OrdinalIgnoreCase));
            }
        }
        catch (Exception) { return false; }
    }
}