using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using BE;

namespace SERVICIO.Logica
{
    public class CatalogoTraducciones
    {
        private static readonly CatalogoTraducciones instancia = new CatalogoTraducciones();
        public static CatalogoTraducciones Instancia { get { return instancia; } }
        private CatalogoTraducciones() { }
        private const string NOMBRE_RECURSO = "SERVICIO.Recursos.Traducciones";
        public const string CODIGO_BASE = "es";
        private static readonly Dictionary<string, string> NOMBRES_EN_ESPANIOL = new Dictionary<string, string>
        {
            { "es", "Español" }, { "en", "Inglés" }, { "pt", "Portugués" }, { "fr", "Francés" },
            { "de", "Alemán" }, { "it", "Italiano" }, { "ja", "Japonés" }, { "ru", "Ruso" },
            { "ar", "Árabe" }, { "tr", "Turco" }, { "zh", "Chino" }, { "ko", "Coreano" },
            { "nl", "Neerlandés" }, { "pl", "Polaco" }, { "he", "Hebreo" }, { "hi", "Hindi" },
            { "sv", "Sueco" }, { "el", "Griego" }
        };
        private readonly ResourceManager recursos = new ResourceManager(NOMBRE_RECURSO, typeof(CatalogoTraducciones).Assembly);
        private readonly object bloqueo = new object();
        private List<Idioma> idiomasCatalogo;
        public List<Idioma> ObtenerIdiomas()
        {
            lock (bloqueo)
            {
                if (idiomasCatalogo == null)
                {
                    idiomasCatalogo = new List<Idioma>();
                    foreach (CultureInfo cultura in CultureInfo.GetCultures(CultureTypes.NeutralCultures))
                    {
                        if (string.IsNullOrEmpty(cultura.Name)) continue; // cultura invariante
                        if (ObtenerRecursos(cultura) == null) continue;
                        idiomasCatalogo.Add(new Idioma
                        {
                            Codigo = cultura.Name,
                            Nombre = NOMBRES_EN_ESPANIOL.ContainsKey(cultura.Name) ? NOMBRES_EN_ESPANIOL[cultura.Name] : cultura.EnglishName,
                            NombreNativo = Capitalizar(cultura.NativeName, cultura)
                        });
                    }
                    idiomasCatalogo = idiomasCatalogo.OrderBy(i => i.Nombre).ToList();
                }
                return idiomasCatalogo;
            }
        }
        public Dictionary<string, string> ObtenerTextos(string pCodigo)
        {
            Dictionary<string, string> textos = new Dictionary<string, string>();
            CultureInfo cultura;
            try { cultura = CultureInfo.GetCultureInfo(pCodigo); }
            catch (CultureNotFoundException) { return textos; }

            ResourceSet recursosIdioma = ObtenerRecursos(cultura);
            if (recursosIdioma == null) return textos;
            foreach (DictionaryEntry entrada in recursosIdioma)
            {
                string texto = entrada.Value as string;
                if (!string.IsNullOrWhiteSpace(texto)) textos[entrada.Key.ToString()] = texto;
            }
            return textos;
        }
        private ResourceSet ObtenerRecursos(CultureInfo pCultura)
        {
            try { return recursos.GetResourceSet(pCultura, true, false); }
            catch (MissingManifestResourceException) { return null; }
        }
        private static string Capitalizar(string pTexto, CultureInfo pCultura)
        {
            if (string.IsNullOrEmpty(pTexto)) return pTexto;
            return pCultura.TextInfo.ToUpper(pTexto[0]) + pTexto.Substring(1);
        }
    }
}
