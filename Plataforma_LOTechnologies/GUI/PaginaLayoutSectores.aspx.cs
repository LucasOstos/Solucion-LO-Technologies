using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class PaginaLayoutSectores : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void ddlLocal_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
    protected void btnCargarLayout_Click(object sender, EventArgs e)
    {

    }
    protected void btnCancelarLayout_Click(object sender, EventArgs e)
    {

    }
    protected void btnConfirmarLayout_Click(object sender, EventArgs e)
    {
        
    }

    protected void btnNuevoSector_Click(object sender, EventArgs e)
    {

    }
    protected void btnCancelarSector_Click(object sender, EventArgs e)
    {

    }
    protected void btnGuardarSector_Click(object sender, EventArgs e)
    {

    }

    protected void rptLayouts_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        
    }

    protected void rptSectores_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        
    }

    public string ObtenerClaseEstadoLayout(string estado)
    {
        switch (estado)
        {
            case "Activo": return "etiqueta-activo";
            case "Inactivo": return "etiqueta-inactivo";
            case "Borrador": return "etiqueta-info";
            default: return "";
        }
    }
}