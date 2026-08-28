using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class PaginaStockVentas : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void btnImportarCSV_Click(object sender, EventArgs e) { }
    protected void btnCargaManual_Click(object sender, EventArgs e) { }
    protected void btnAplicar_Click(object sender, EventArgs e) { }

    protected void btnCancelarCarga_Click(object sender, EventArgs e) { }
    protected void btnGuardarCarga_Click(object sender, EventArgs e) { }

    protected void rptStock_ItemCommand(object source, RepeaterCommandEventArgs e) { }
}