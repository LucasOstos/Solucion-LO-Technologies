using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class PaginaLanding : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnEnviarResena_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid)
        {
            return;
        }
        lblGraciasResena.Visible = true;
        tbComentario.Text = "";
        hfPuntuacion.Value = "0";
    }

    protected void cvResena_ServerValidate(object source, ServerValidateEventArgs args)
    {
        int puntuacion;
        int.TryParse(hfPuntuacion.Value, out puntuacion);
        bool tieneComentario = !string.IsNullOrWhiteSpace(tbComentario.Text);
        bool tienePuntuacion = puntuacion > 0;
        args.IsValid = tieneComentario || tienePuntuacion;
    }
}