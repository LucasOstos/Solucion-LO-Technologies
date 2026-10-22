using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BE;
using SERVICIO.Logica;

/// <summary>
/// Descripción breve de PaginaBase
/// </summary>
public class PaginaBase : Page, IObservadorIdioma
{
    private const string ATR_CLAVE = "Clave";
    private const string ATR_PLACEHOLDER = "ClavePlaceholder";
    private const string ATR_TITLE = "ClaveTitle";
    private const string ATR_ARIA = "ClaveAria";
    private const string ATR_CONFIRMAR = "ClaveConfirmar";
    private static readonly string[] ATRIBUTOS_CLAVE = { ATR_CLAVE, ATR_PLACEHOLDER, ATR_TITLE, ATR_ARIA, ATR_CONFIRMAR };
    private static readonly string[] IDIOMAS_DERECHA_A_IZQUIERDA = { "ar", "he", "fa", "ur" };
    protected virtual string ClaveTitulo
    {
        get { return null; }
    }
    protected Idioma IdiomaActual
    {
        get { return GestorIdioma.Instancia.IdiomaActual; }
    }
    protected override void OnInit(EventArgs e)
    {
        GestorIdioma.Instancia.Suscribir(this);
        base.OnInit(e);
    }
    protected override void OnUnload(EventArgs e)
    {
        GestorIdioma.Instancia.Desuscribir(this);
        base.OnUnload(e);
    }
    public virtual void ActualizarIdioma(Idioma pIdioma)
    {
        AplicarIdioma(pIdioma);
    }
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        AplicarIdioma(IdiomaActual);
    }
    protected override void Render(HtmlTextWriter writer)
    {
        QuitarAtributosClave(this);
        base.Render(writer);
    }
    protected string Traducir(string pClave)
    {
        return GestorIdioma.Instancia.Traducir(pClave);
    }
    protected string Traducir(string pClave, params object[] pValores)
    {
        string texto = Traducir(pClave);
        try { return string.Format(texto, pValores); }
        catch (FormatException) { return texto; }
    }
    protected string NombreIdioma(Idioma pIdioma)
    {
        if (pIdioma == null) return "";
        string clave = "idioma." + pIdioma.Codigo;
        string nombre = Traducir(clave);
        return nombre == clave ? pIdioma.Nombre : nombre;
    }
    protected string TraducirError(Exception pError)
    {
        ExcepcionTraducible traducible = pError as ExcepcionTraducible;
        if (traducible == null && pError != null) traducible = pError.GetBaseException() as ExcepcionTraducible;
        if (traducible != null) return Traducir(traducible.Clave, traducible.Parametros);
        return Traducir("error.inesperado");
    }
    private void AplicarIdioma(Idioma pIdioma)
    {
        if (pIdioma == null) return;
        if (!string.IsNullOrEmpty(ClaveTitulo) && Header != null) Title = Traducir(ClaveTitulo);
        TraducirControles(this);
        AplicarIdiomaHtml(pIdioma);
    }
    private void TraducirControles(Control pRaiz)
    {
        foreach (Control control in pRaiz.Controls)
        {
            AttributeCollection atributos = ObtenerAtributos(control);
            if (atributos != null)
            {
                string clave = atributos[ATR_CLAVE];
                if (!string.IsNullOrEmpty(clave)) AsignarTexto(control, Traducir(clave));
                TraducirAtributo(atributos, ATR_PLACEHOLDER, "placeholder");
                TraducirAtributo(atributos, ATR_ARIA, "aria-label");
                string claveTitulo = atributos[ATR_TITLE];
                if (!string.IsNullOrEmpty(claveTitulo))
                {
                    if (control is WebControl) ((WebControl)control).ToolTip = Traducir(claveTitulo);
                    else atributos["title"] = Traducir(claveTitulo);
                }
                string claveConfirmar = atributos[ATR_CONFIRMAR];
                if (!string.IsNullOrEmpty(claveConfirmar))
                {
                    string script = "return confirm(" + HttpUtility.JavaScriptStringEncode(Traducir(claveConfirmar), true) + ");";
                    if (control is Button) ((Button)control).OnClientClick = script;
                    else if (control is LinkButton) ((LinkButton)control).OnClientClick = script;
                }
            }
            ListControl lista = control as ListControl;
            if (lista != null)
            {
                foreach (ListItem item in lista.Items)
                {
                    string claveItem = item.Attributes[ATR_CLAVE];
                    if (!string.IsNullOrEmpty(claveItem)) item.Text = Traducir(claveItem);
                }
            }
            if (control.HasControls()) TraducirControles(control);
        }
    }
    private void AsignarTexto(Control pControl, string pTexto)
    {
        if (pControl is BaseValidator)
        {
            ((BaseValidator)pControl).ErrorMessage = pTexto;
        }
        else if (pControl is HyperLink)
        {
            // Si el link tiene otros controles adentro, Text los borraría
            if (!pControl.HasControls()) ((HyperLink)pControl).Text = pTexto;
        }
        else if (pControl is IButtonControl)
        {
            ((IButtonControl)pControl).Text = pTexto;
        }
        else if (pControl is CheckBox)
        {
            ((CheckBox)pControl).Text = pTexto;
        }
        else if (pControl is ListControl)
        {
            // En un DropDownList, Text cambia el valor seleccionado: se traducen sus ítems
        }
        else if (pControl is ITextControl)
        {
            ((ITextControl)pControl).Text = pTexto;
        }
        else if (pControl is HtmlContainerControl && TieneSoloTexto(pControl))
        {
            ((HtmlContainerControl)pControl).InnerText = pTexto;
        }
    }
    private static bool TieneSoloTexto(Control pControl)
    {
        return !pControl.HasControls() || (pControl.Controls.Count == 1 && pControl.Controls[0] is LiteralControl);
    }
    private void TraducirAtributo(AttributeCollection pAtributos, string pAtributoClave, string pAtributoDestino)
    {
        string clave = pAtributos[pAtributoClave];
        if (!string.IsNullOrEmpty(clave)) pAtributos[pAtributoDestino] = Traducir(clave);
    }
    private void AplicarIdiomaHtml(Idioma pIdioma)
    {
        string codigo = pIdioma.Codigo;
        string direccion = IDIOMAS_DERECHA_A_IZQUIERDA.Contains(codigo) ? "rtl" : "ltr";

        HtmlControl html = Controls.OfType<HtmlControl>().FirstOrDefault(c => string.Equals(c.TagName, "html", StringComparison.OrdinalIgnoreCase));
        if (html != null)
        {
            html.Attributes["lang"] = codigo;
            html.Attributes["dir"] = direccion;
        }
        else
        {
            string script = "document.documentElement.lang='" + HttpUtility.JavaScriptStringEncode(codigo) + "';document.documentElement.dir='" + direccion + "';";
            ClientScript.RegisterStartupScript(typeof(PaginaBase), "IdiomaHtml", script, true);
        }
    }
    private static AttributeCollection ObtenerAtributos(Control pControl)
    {
        if (pControl is WebControl) return ((WebControl)pControl).Attributes;
        if (pControl is HtmlControl) return ((HtmlControl)pControl).Attributes;
        if (pControl is UserControl) return ((UserControl)pControl).Attributes;
        return null;
    }
    private static void QuitarAtributosClave(Control pRaiz)
    {
        foreach (Control control in pRaiz.Controls)
        {
            AttributeCollection atributos = ObtenerAtributos(control);
            if (atributos != null)
            {
                foreach (string atributo in ATRIBUTOS_CLAVE) atributos.Remove(atributo);
            }
            ListControl lista = control as ListControl;
            if (lista != null)
            {
                foreach (ListItem item in lista.Items) item.Attributes.Remove(ATR_CLAVE);
            }
            if (control.HasControls()) QuitarAtributosClave(control);
        }
    }
}