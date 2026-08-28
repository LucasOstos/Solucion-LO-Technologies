using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BE;
using SERVICIO.Logica;

public partial class PaginaPrincipal : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnLogout_Click(object sender, EventArgs e)
    {
        Usuario usuario = Sesion.Instancia.Usuario;
        SucesoServicio.Instancia.RegistrarSuceso($"{usuario.Nombre} {usuario.Apellido}", "Cierre de Sesión", "Acceso", 1);
        Sesion.Instancia.Logout();
        Response.Redirect("PaginaLogin.aspx");
    }

    protected void btnUsuarios_Click(object sender, EventArgs e)
    {
        Response.Redirect("PaginaUsuarios.aspx");
    }

    protected void btnDigitos_Click(object sender, EventArgs e)
    {
        Response.Redirect("PaginaDigitosVerificadores.aspx");
    }
    protected void btnBackupRestore_Click(object sender, EventArgs e)
    {
        Response.Redirect("PaginaBackupRestore.aspx");
    }

    protected void btnAuditoria_Click(object sender, EventArgs e)
    {
        Response.Redirect("PaginaAuditorias.aspx");
    }

    protected void btnIdiomas_Click(object sender, EventArgs e)
    {
        Response.Redirect("PaginaIdiomas.aspx");
    }
    protected void btnEmpresas_Click(object sender, EventArgs e)
    {
        Response.Redirect("PaginaEmpresas.aspx");
    }
    protected void btnLocales_Click(object sender, EventArgs e)
    {
        Response.Redirect("PaginaLocales.aspx");
    }

    protected void btnLayoutSectores_Click(object sender, EventArgs e)
    {
        Response.Redirect("PaginaLayoutSectores.aspx");
    }
    protected void btnProductos_Click(object sender, EventArgs e)
    {
        Response.Redirect("PaginaProductos.aspx");
    }
    protected void btnStockVentas_Click(object sender, EventArgs e)
    {
        Response.Redirect("PaginaStockVentas.aspx");
    }
    protected void btnEscenarios_Click(object sender, EventArgs e)
    {
        Response.Redirect("PaginaEscenarios.aspx");
    }
    protected void btnComparacionEscenarios_Click(object sender, EventArgs e)
    {
        Response.Redirect("PaginaComparacionEscenarios.aspx");
    }
    protected void btnIndicadores_Click(object sender, EventArgs e)
    {
        Response.Redirect("PaginaIndicadores.aspx");
    }
    protected void btnAnalisisProductos_Click(object sender, EventArgs e)
    {
        Response.Redirect("PaginaAnalisisProductos.aspx");
    }
    protected void btnReportes_Click(object sender, EventArgs e)
    {
        Response.Redirect("PaginaReportes.aspx");
    }
}