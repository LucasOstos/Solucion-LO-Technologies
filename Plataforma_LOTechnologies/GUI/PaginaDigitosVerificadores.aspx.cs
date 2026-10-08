using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BE;
using SERVICIO.Logica;

public partial class PaginaDigitosVerificadores : PaginaSegura
{
    protected override string PermisoRequerido
    {
        get { return PermisoNombres.RecalcularDigitos; }
    }
    protected override bool PermitidaConIntegridadComprometida
    {
        get { return true; }
    }
    private DigitoVerificador digitos = new DigitoVerificador();
    private BackupRestore gestorBackup = new BackupRestore();
    protected void Page_Load(object sender, EventArgs e)
    {        
        if (!IsPostBack)
        {
            CargarRegistrosCorruptos();
        }
    }
    private void CargarRegistrosCorruptos()
    {
        List<RegistroCorrupto> corruptos = digitos.ObtenerRegistrosCorruptos();
        bool hayInconsistencias = corruptos.Count > 0;
        Sesion.Instancia.IntegridadComprometida = hayInconsistencias;
        volverMenu.Visible = !hayInconsistencias;
        if (!hayInconsistencias)
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
        var backups = gestorBackup.ObtenerBackups();
        var ultimoExitoso = backups.Find(b => b.Exitoso);
        if (ultimoExitoso == null)
        {
            MostrarMensaje("No hay ningún backup disponible para restaurar.", esExito: false);
            return;
        }
        try
        {
            gestorBackup.RealizarRestoreDigito(ultimoExitoso.RutaArchivo);
            MostrarMensaje("Base de datos restaurada. Volvé a intentar iniciar sesión.", esExito: true);
        }
        catch (Exception)
        {
            MostrarMensaje("No fue posible restaurar la base de datos.", esExito: false);
        }
    }

    protected void btnCancelar_Click(object sender, EventArgs e)
    {
        Sesion.Instancia.Logout();
        Response.Redirect("PaginaLogin.aspx");
    }
}