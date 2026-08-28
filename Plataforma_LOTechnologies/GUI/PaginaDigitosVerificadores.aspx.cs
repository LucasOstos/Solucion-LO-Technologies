using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SERVICIO.Logica;

public partial class PaginaDigitosVerificadores : System.Web.UI.Page
{
    private const int ROL_ADMIN = 1;
    private DigitoVerificador digitos = new DigitoVerificador();
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!Sesion.Instancia.IsLogueado() || Sesion.Instancia.Usuario.Rol != ROL_ADMIN)
        {
            Response.Redirect("PaginaLogin.aspx");
            return;
        }
        if (!IsPostBack)
        {
            CargarRegistrosCorruptos();
        }
    }
    private void CargarRegistrosCorruptos()
    {
        List<RegistroCorrupto> corruptos = digitos.ObtenerRegistrosCorruptos();
        if(corruptos.Count == 0)
        {
            rptCorruptos.Visible = false; lbIntegridadOK.Visible = true;
        }
        else { rptCorruptos.Visible = true; lbIntegridadOK.Visible = false; rptCorruptos.DataSource = corruptos; rptCorruptos.DataBind(); }
    }
    private void MostrarMensaje(string texto, bool esExito)
    {
        lbMensaje.Text = texto;
        lbMensaje.CssClass = "dv-mensaje " + (esExito ? "exito" : "error");
        lbMensaje.Visible = true;
    }
    protected void btnRecalcular_Click(object sender, EventArgs e)
    {
        bool exito = digitos.CalcularTablas();
        if (exito) { MostrarMensaje("Digitos calculados correctamente", esExito: true); CargarRegistrosCorruptos(); }
        else { MostrarMensaje("No fue posible recalcular los digitos verificadores", esExito: false); }
    }

    protected void btnRestore_Click(object sender, EventArgs e)
    {

    }

    protected void btnCancelar_Click(object sender, EventArgs e)
    {
        Sesion.Instancia.Logout();
        Response.Redirect("PaginaLogin.aspx");
    }
}