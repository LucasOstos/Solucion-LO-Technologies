using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class PaginaAuditorias : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void btnFiltrar_Click(object sender, EventArgs e)
    {

    }

    protected void btnExportarLog_Click(object sender, EventArgs e) { }
    protected void btnAnterior_Click(object sender, EventArgs e) { }
    protected void btnSiguiente_Click(object sender, EventArgs e) { }

    public string ObtenerNombreCriticidad(string criticidad)
    {
        switch (criticidad)
        {
            case "1": return "info";
            case "2": return "warn";
            case "3": return "error";
            default: return criticidad;
        }
    }

    public string ObtenerClaseCriticidad(string criticidad)
    {
        switch (criticidad)
        {
            case "1": return "etiqueta-activo";
            case "2": return "aud-etiqueta-warn";
            case "3": return "etiqueta-inactivo";
            default: return "";
        }
    }
}