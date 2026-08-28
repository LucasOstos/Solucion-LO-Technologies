using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class PaginaEscenarios : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void ddlLocal_SelectedIndexChanged(object sender, EventArgs e) { }

    protected void btnNuevoEscenario_Click(object sender, EventArgs e)
    {
        
    }
    protected void btnCancelarEscenario_Click(object sender, EventArgs e) { /* TODO */ }

    protected void btnGuardarEscenario_Click(object sender, EventArgs e)
    {
        
    }

    protected void rptEscenarios_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
    {
        
    }

    public string ObtenerClaseEstado(string estado)
    {
        switch (estado)
        {
            case "Base": return "esc-etiqueta-base";
            case "Activo": return "etiqueta-activo";
            case "En revisión": return "esc-etiqueta-en-revision";
            case "Aprobado": return "esc-etiqueta-aprobado";
            case "Borrador": return "esc-etiqueta-borrador";
            default: return "";
        }
    }
}