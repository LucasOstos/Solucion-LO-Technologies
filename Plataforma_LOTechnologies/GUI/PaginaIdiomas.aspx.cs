using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class PaginaIdiomas : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void btnAgregarIdioma_Click(object sender, EventArgs e) {  }
    protected void btnCancelarIdioma_Click(object sender, EventArgs e) {  }

    protected void btnGuardarIdioma_Click(object sender, EventArgs e)
    {

    }

    protected void rptIdiomas_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        //Response.Redirect($"PaginaEditarTraducciones.aspx?idioma={e.CommandArgument}")
    }

    public string ObtenerClaseCompletitud(object completitud)
    {
        int valor = Convert.ToInt32(completitud);
        if (valor >= 90) return "idi-completitud-alta";
        if (valor >= 60) return "idi-completitud-media";
        return "idi-completitud-baja";
    }
}