using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class PaginaProductos : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void btnNuevo_Click(object sender, EventArgs e) { }
    protected void btnBuscar_Click(object sender, EventArgs e) { }
    protected void Filtro_Changed(object sender, EventArgs e) { }

    protected void btnCancelarProducto_Click(object sender, EventArgs e) { }
    protected void btnGuardarProducto_Click(object sender, EventArgs e) { }
    protected void rptProductos_ItemCommand(object source, RepeaterCommandEventArgs e) { }

    protected void btnNuevaCategoria_Click(object sender, EventArgs e) { }
    protected void btnCancelarCategoria_Click(object sender, EventArgs e) { }
    protected void btnGuardarCategoria_Click(object sender, EventArgs e) { }
    protected void rptCategorias_ItemCommand(object source, RepeaterCommandEventArgs e) { }
}