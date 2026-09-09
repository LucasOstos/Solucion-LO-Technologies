using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SERVICIO.Logica;

public partial class PaginaBackupRestore : System.Web.UI.Page
{
    private BackupRestore gestorBackup = new BackupRestore();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Sesion.Instancia.IsLogueado() || Sesion.Instancia.Usuario.Rol != 1)
        {
            Response.Redirect("PaginaLogin.aspx");
            return;
        }

        if (!IsPostBack)
        {
            CargarBackups();
        }
    }

    #region Funciones
    private void CargarBackups()
    {
        List<Backup> backups = gestorBackup.ObtenerBackups();

        if (backups.Count == 0)
        {
            rptBackups.Visible = false;
            lblSinBackups.Visible = true;
        }
        else
        {
            rptBackups.Visible = true;
            lblSinBackups.Visible = false;
            rptBackups.DataSource = backups;
            rptBackups.DataBind();
            Backup ultimo = backups[0];
            litUltimoBackup.Text = ultimo.Fecha.ToString("dd/MM/yyyy HH:mm");
        }
    }
    private void AbrirModalRestaurar(string nombreArchivo)
    {
        Backup backup = gestorBackup.ObtenerBackupPorNombreArchivo(nombreArchivo);
        hfNombreArchivo.Value = backup.NombreArchivo;
        litFechaRestaurar.Text = backup.Fecha.ToString("dd/MM/yyyy HH:mm");
        litTamanioRestaurar.Text = backup.Tamanio;
        tbConfirmarRestauracion.Text = "";
        pnlModalRestaurar.Style["display"] = "flex";
    }
    private void MostrarMensaje(string texto, bool esExito)
    {
        lblMensaje.Text = texto;
        lblMensaje.CssClass = esExito ? "mensaje" : "mensaje mensaje-error";
        lblMensaje.Visible = true;
    }
    #endregion

    protected void btnCrearBackup_Click(object sender, EventArgs e)
    {
        try
        {
            gestorBackup.RealizarBackup();
            MostrarMensaje("Backup realizado correctamente.", esExito: true);
        }
        catch (Exception)
        {
            MostrarMensaje("No fue posible generar el backup.", esExito: false);
        }
        CargarBackups();
    }

    protected void rptBackups_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        string nombreArchivo = e.CommandArgument.ToString();
        switch (e.CommandName)
        {
            case "Descargar":
                Backup backup = gestorBackup.ObtenerBackupPorNombreArchivo(nombreArchivo);
                Response.ContentType = "application/octet-stream";
                Response.AppendHeader("Content-Disposition", $"attachment; filename={backup.NombreArchivo}");
                Response.TransmitFile(backup.RutaArchivo);
                Response.End();
                break;
            case "Restaurar":
                AbrirModalRestaurar(nombreArchivo);
                break;
        }
    }

    protected void btnCancelarRestaurar_Click(object sender, EventArgs e) { pnlModalRestaurar.Style["display"] = "none"; }

    protected void btnConfirmarRestaurar_Click(object sender, EventArgs e)
    {
        if (tbConfirmarRestauracion.Text.Trim() != "CONFIRMAR")
        {
            MostrarMensaje("Tenés que escribir CONFIRMAR para proceder.", esExito: false);
            pnlModalRestaurar.Style["display"] = "flex";
            return;
        }
        try
        {
            gestorBackup.RealizarRestore(hfNombreArchivo.Value);
            pnlModalRestaurar.Style["display"] = "none";
            MostrarMensaje("Base de datos restaurada correctamente.", esExito: true);
        }
        catch (Exception)
        {
            pnlModalRestaurar.Style["display"] = "none";
            MostrarMensaje("No fue posible restaurar la base de datos.", esExito: false);
        }
        CargarBackups();
    }
}