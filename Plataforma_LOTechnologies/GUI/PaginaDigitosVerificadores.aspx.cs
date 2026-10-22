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
    protected override string ClaveTitulo
    {
        get { return "integridad.titulo"; }
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
        if (exito) { MostrarMensaje(Traducir("integridad.msgRecalculado"), esExito: true); CargarRegistrosCorruptos(); }
        else { MostrarMensaje(Traducir("integridad.errRecalcular"), esExito: false); }
    }

    protected void btnRestore_Click(object sender, EventArgs e)
    {
        var backups = gestorBackup.ObtenerBackups();
        var ultimoExitoso = backups.Find(b => b.Exitoso);
        if (ultimoExitoso == null)
        {
            MostrarMensaje(Traducir("integridad.errSinBackup"), esExito: false);
            return;
        }
        try
        {
            gestorBackup.RealizarRestoreDigito(ultimoExitoso.RutaArchivo);
            GestorIdioma.Instancia.RecargarIdiomas();
            MostrarMensaje(Traducir("integridad.msgRestaurada"), esExito: true);
        }
        catch (Exception)
        {
            MostrarMensaje(Traducir("comun.errRestaurar"), esExito: false);
        }
    }

    protected void btnCancelar_Click(object sender, EventArgs e)
    {
        Sesion.Instancia.Logout();
        Response.Redirect("PaginaLogin.aspx");
    }
}