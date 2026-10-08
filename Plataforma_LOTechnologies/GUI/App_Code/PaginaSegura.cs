using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SERVICIO.Logica;

/// <summary>
/// Descripción breve de PaginaSegura
/// </summary>
public class PaginaSegura : Page
{
    protected virtual string PermisoRequerido
    {
        get { return null; }
    }
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (!Sesion.Instancia.IsLogueado())
        {
            Response.Redirect("~/PaginaLogin.aspx", true);
            return;
        }

        if (PermisoRequerido != null && !Sesion.Instancia.TienePermiso(PermisoRequerido))
        {
            Response.Redirect("~/PaginaPrincipal.aspx", true);
        }
    }
    protected override void OnPreRender(EventArgs e)
    {
        AplicarPermisosAControles(this);
        ClientScript.RegisterClientScriptInclude(typeof(PaginaSegura), "Mensajes", ResolveUrl("~/JS/Mensajes.js"));
        base.OnPreRender(e);
    }
    private void AplicarPermisosAControles(Control pRaiz)
    {
        foreach (Control control in pRaiz.Controls)
        {
            if (control is WebControl webControl)
            {
                string permiso = webControl.Attributes["Permiso"];
                if (!string.IsNullOrEmpty(permiso))
                {
                    webControl.Visible = Sesion.Instancia.TienePermiso(permiso);
                    webControl.Attributes.Remove("Permiso");
                }
            }
            if (control.HasControls()) AplicarPermisosAControles(control);
        }
    }
}