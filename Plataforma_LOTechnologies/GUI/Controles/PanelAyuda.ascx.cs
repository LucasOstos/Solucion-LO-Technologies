using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SERVICIO.Logica;

public partial class Controles_PanelAyuda : System.Web.UI.UserControl
{
    public string Pantalla { get; set; }

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        RegistrarRecursos();
        // Se arma al final para usar el idioma ya elegido por el usuario
        litAyuda.Text = ArmarHtml();
    }

    private void RegistrarRecursos()
    {
        if (Page.Header != null && Page.Header.FindControl("estilosAyuda") == null)
        {
            var estilos = new LiteralControl("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + ResolveUrl("~/Estilos/Ayuda.css") + "\" />");
            estilos.ID = "estilosAyuda";
            Page.Header.Controls.Add(estilos);
        }
        Page.ClientScript.RegisterClientScriptInclude(typeof(Controles_PanelAyuda), "AyudaJS", ResolveUrl("~/JS/AyudaJS.js"));
    }
    #region HTML
    private string ArmarHtml()
    {
        PantallaAyuda pantalla = CatalogoAyuda.ObtenerPantalla(Pantalla);
        List<TemaAyuda> temas = CatalogoAyuda.TemasDelPanel(pantalla);
        var html = new StringBuilder();

        // Botón (va en el encabezado de la página)
        html.Append("<button type=\"button\" id=\"btnAyuda\" class=\"boton-ayuda\" aria-controls=\"panelAyuda\" aria-expanded=\"false\" title=\"")
            .Append(A(T("ayuda.botonTitulo"))).Append("\"><span class=\"icono\" aria-hidden=\"true\">?</span><span>")
            .Append(H(T("ayuda.boton"))).Append("</span></button>");

        // Panel
        html.Append("<aside id=\"panelAyuda\" class=\"ayuda-panel\" role=\"complementary\" aria-labelledby=\"ayudaTitulo\" hidden=\"hidden\" data-pantalla=\"")
            .Append(A(pantalla == null ? "" : pantalla.Codigo)).Append("\">");
        html.Append("<div class=\"ayuda-cabecera\"><h2 id=\"ayudaTitulo\" tabindex=\"-1\">").Append(H(T("ayuda.titulo")));
        if (pantalla != null) html.Append("<span class=\"contexto\">").Append(H(T(pantalla.ClaveTitulo))).Append("</span>");
        html.Append("</h2><button type=\"button\" class=\"ayuda-cerrar\" data-ayuda-cerrar=\"1\" aria-label=\"").Append(A(T("ayuda.cerrar")))
            .Append("\" title=\"").Append(A(T("ayuda.cerrar"))).Append("\">&times;</button></div>");

        html.Append("<div class=\"ayuda-busqueda\"><button type=\"button\" class=\"ayuda-volver\" data-ayuda-volver=\"1\" hidden=\"hidden\" aria-label=\"")
            .Append(A(T("ayuda.volver"))).Append("\" title=\"").Append(A(T("ayuda.volver"))).Append("\">&#8592;</button>")
            .Append("<label class=\"campo\"><span aria-hidden=\"true\">&#128269;</span><input type=\"search\" id=\"ayudaBuscar\" autocomplete=\"off\" placeholder=\"")
            .Append(A(T("ayuda.buscar"))).Append("\" aria-label=\"").Append(A(T("ayuda.buscar"))).Append("\" /></label></div>");

        html.Append("<div class=\"ayuda-cuerpo\">");
        ArmarInicio(html, pantalla, temas);
        foreach (TemaAyuda tema in temas) ArmarArticulo(html, tema, temas);
        html.Append("</div>");

        html.Append("<div class=\"ayuda-pie\"><span>").Append(FormatoAtajo()).Append("</span></div>");
        html.Append("<div class=\"ayuda-anuncio\" aria-live=\"polite\" data-texto-marcado=\"").Append(A(T("ayuda.marcado")))
            .Append("\" data-texto-no-visible=\"").Append(A(T("ayuda.noVisible"))).Append("\"></div>");
        html.Append("</aside>");
        return html.ToString();
    }

    private void ArmarInicio(StringBuilder pHtml, PantallaAyuda pPantalla, List<TemaAyuda> pTemas)
    {
        pHtml.Append("<div class=\"ayuda-inicio\">");
        if (pPantalla != null && pPantalla.Temas.Count > 0)
        {
            pHtml.Append("<div class=\"ayuda-grupo\"><h3>").Append(H(T("ayuda.enEstaPantalla"))).Append("</h3><ul>");
            foreach (TemaAyuda tema in pPantalla.Temas)
            {
                pHtml.Append("<li><button type=\"button\" class=\"ayuda-tema\" data-tema=\"").Append(A(tema.Clave)).Append("\" data-buscar=\"")
                    .Append(A(TextoBuscable(tema))).Append("\"><span class=\"ic\" aria-hidden=\"true\">").Append(tema.Icono)
                    .Append("</span><span class=\"texto\">").Append(H(T(tema.ClaveTitulo))).Append("</span><span class=\"flecha\" aria-hidden=\"true\">&#8250;</span></button></li>");
            }
            pHtml.Append("</ul></div>");
        }
        // Pantallas relacionadas a las que el usuario tiene acceso: abren esa pantalla con la ayuda abierta
        List<PantallaAyuda> relacionadas = (pPantalla == null ? new string[0] : pPantalla.Relacionadas)
            .Select(CatalogoAyuda.ObtenerPantalla)
            .Where(p => p != null && (string.IsNullOrEmpty(p.Permiso) || Sesion.Instancia.TienePermiso(p.Permiso)))
            .ToList();
        if (relacionadas.Count > 0)
        {
            pHtml.Append("<div class=\"ayuda-grupo ayuda-grupo-relacionadas\"><h3>").Append(H(T("ayuda.relacionadas"))).Append("</h3><ul>");
            foreach (PantallaAyuda relacionada in relacionadas)
            {
                pHtml.Append("<li><a class=\"ayuda-tema\" href=\"").Append(A(ResolveUrl("~/" + relacionada.Url) + "?ayuda=1")).Append("\">")
                    .Append("<span class=\"ic\" aria-hidden=\"true\">&#8599;</span><span class=\"texto\">").Append(H(T(relacionada.ClaveTitulo)))
                    .Append("</span><span class=\"flecha\" aria-hidden=\"true\">&#8250;</span></a></li>");
            }
            pHtml.Append("</ul></div>");
        }
        TemaAyuda primeros = CatalogoAyuda.PRIMEROS_PASOS;
        pHtml.Append("<div class=\"ayuda-destacado\" data-buscar=\"").Append(A(TextoBuscable(primeros))).Append("\"><h3>")
            .Append(H(T(primeros.ClaveTitulo))).Append("</h3><p>").Append(Formatear(T("ayuda.general.primerosPasos.resumen")))
            .Append("</p><button type=\"button\" class=\"enlace\" data-tema=\"").Append(A(primeros.Clave)).Append("\">")
            .Append(H(T("ayuda.verGuia"))).Append(" &#8250;</button></div>");
        pHtml.Append("<p class=\"ayuda-sin-resultados\" hidden=\"hidden\" data-texto=\"").Append(A(T("ayuda.sinResultados"))).Append("\"></p>");
        pHtml.Append("</div>");
    }

    private void ArmarArticulo(StringBuilder pHtml, TemaAyuda pTema, List<TemaAyuda> pTemas)
    {
        pHtml.Append("<article class=\"ayuda-articulo\" data-articulo=\"").Append(A(pTema.Clave)).Append("\" hidden=\"hidden\" aria-labelledby=\"ayudaArt_")
            .Append(IdHtml(pTema.Clave)).Append("\">");
        pHtml.Append("<h3 id=\"ayudaArt_").Append(IdHtml(pTema.Clave)).Append("\" tabindex=\"-1\">").Append(H(T(pTema.ClaveTitulo))).Append("</h3>");
        pHtml.Append("<p>").Append(Formatear(T(pTema.ClaveCuerpo))).Append("</p>");

        string pasos = TOpcional(pTema.ClavePasos);
        if (pasos != null)
        {
            pHtml.Append("<ol>");
            foreach (string paso in pasos.Split('|').Select(p => p.Trim()).Where(p => p.Length > 0))
            {
                pHtml.Append("<li>").Append(Formatear(paso)).Append("</li>");
            }
            pHtml.Append("</ol>");
        }
        if (!string.IsNullOrEmpty(pTema.Resaltar))
        {
            pHtml.Append("<button type=\"button\" class=\"ayuda-mostrar\" data-resaltar=\"").Append(A(pTema.Resaltar)).Append("\"><span aria-hidden=\"true\">&#9673;</span> ")
                .Append(H(T("ayuda.mostrar"))).Append("</button>");
        }
        string consejo = pTema.TieneConsejo ? TOpcional(pTema.ClaveConsejo) : null;
        if (consejo != null)
        {
            pHtml.Append("<div class=\"ayuda-consejo\"><b>").Append(H(T("ayuda.consejo"))).Append("</b> ").Append(Formatear(consejo)).Append("</div>");
        }
        List<TemaAyuda> relacionados = pTema.Relacionados.Select(c => pTemas.FirstOrDefault(t => t.Clave == c)).Where(t => t != null).ToList();
        if (relacionados.Count > 0)
        {
            pHtml.Append("<div class=\"ayuda-relacionados\"><h4>").Append(H(T("ayuda.verTambien"))).Append("</h4><ul>");
            foreach (TemaAyuda relacionado in relacionados)
            {
                pHtml.Append("<li><button type=\"button\" class=\"enlace\" data-tema=\"").Append(A(relacionado.Clave)).Append("\">")
                    .Append(H(T(relacionado.ClaveTitulo))).Append("</button></li>");
            }
            pHtml.Append("</ul></div>");
        }
        pHtml.Append("</article>");
    }
    #endregion
    #region Textos
    private static string T(string pClave)
    {
        return GestorIdioma.Instancia.Traducir(pClave);
    }
    // null si la clave no tiene traducción (el traductor devuelve la clave misma)
    private static string TOpcional(string pClave)
    {
        string texto = T(pClave);
        return string.IsNullOrWhiteSpace(texto) || texto == pClave ? null : texto;
    }
    private static string H(string pTexto)
    {
        return HttpUtility.HtmlEncode(pTexto ?? "");
    }
    private static string A(string pTexto)
    {
        return HttpUtility.HtmlAttributeEncode(pTexto ?? "");
    }
    // Los textos de ayuda admiten **negrita**; el resto se codifica para que nunca entre HTML desde una traducción
    private static string Formatear(string pTexto)
    {
        return Regex.Replace(H(pTexto), @"\*\*(.+?)\*\*", "<b>$1</b>");
    }
    private static string FormatoAtajo()
    {
        string texto = H(T("ayuda.atajo"));
        return texto.Replace("{0}", "<kbd class=\"tecla\">F1</kbd>");
    }
    private static string IdHtml(string pClave)
    {
        return Regex.Replace(pClave, "[^A-Za-z0-9]", "_");
    }
    // Título, cuerpo y pasos, en minúsculas y sin tildes, para el buscador
    private static string TextoBuscable(TemaAyuda pTema)
    {
        string texto = string.Join(" ", new[] { T(pTema.ClaveTitulo), TOpcional(pTema.ClaveCuerpo), TOpcional(pTema.ClavePasos) }.Where(t => t != null));
        texto = texto.Replace("**", "").Replace("|", " ").ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var limpio = new StringBuilder();
        foreach (char c in texto)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark) limpio.Append(c);
        }
        return limpio.ToString().Normalize(NormalizationForm.FormC);
    }
    #endregion
}