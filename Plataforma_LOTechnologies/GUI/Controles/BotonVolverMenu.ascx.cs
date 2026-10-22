using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Controles_BotonVolverMenu : System.Web.UI.UserControl
{
    public string UrlDestino { get; set; } = "PaginaPrincipal.aspx";
    public string ClaveTexto { get; set; } = "comun.volverMenu";
    public string Texto
    {
        get { return litTexto.Text; }
        set { litTexto.Text = value; ClaveTexto = null; }
    }
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        if (!string.IsNullOrEmpty(ClaveTexto)) litTexto.Text = SERVICIO.Logica.GestorIdioma.Instancia.Traducir(ClaveTexto);
    }
}