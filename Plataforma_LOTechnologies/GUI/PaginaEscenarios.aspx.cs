using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BE;
using BLL;
using SERVICIO.Logica;

public partial class PaginaEscenarios : System.Web.UI.Page
{
    private LocalBLL localBLL = new LocalBLL();
    private SectorBLL sectorBLL = new SectorBLL();
    private EscenarioBLL escenarioBLL = new EscenarioBLL();
    private int CodigoLocalActual
    {
        get { return string.IsNullOrEmpty(ddlLocal.SelectedValue) ? 0 : int.Parse(ddlLocal.SelectedValue); }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Sesion.Instancia.IsLogueado())
        {
            Response.Redirect("PaginaLogin.aspx");
            return;
        }

        if (!IsPostBack)
        {
            CargarDropdownLocales();
            CargarEscenarios();
        }
    }

    #region Funciones
    private void CargarDropdownLocales()
    {
        List<LocalListado> locales = localBLL.ObtenerLocalesListado("", null, "Activo");
        ddlLocal.Items.Clear();
        foreach (LocalListado local in locales)
        {
            ddlLocal.Items.Add(new ListItem(local.Nombre, local.CodigoLocal.ToString()));
        }
    }
    private void CargarEscenarios()
    {
        if (CodigoLocalActual == 0)
        {
            rptEscenarios.Visible = false;
            lblSinEscenarios.Visible = true;
            return;
        }
        List<EscenarioListado> escenarios = escenarioBLL.ObtenerEscenariosPorLocal(CodigoLocalActual);
        if (escenarios.Count == 0)
        {
            rptEscenarios.Visible = false;
            lblSinEscenarios.Visible = true;
        }
        else
        {
            rptEscenarios.Visible = true;
            lblSinEscenarios.Visible = false;
            rptEscenarios.DataSource = escenarios;
            rptEscenarios.DataBind();
        }
    }
    private void MostrarDetalleEscenario(int codigoEscenario)
    {
        EscenarioBE escenario = escenarioBLL.ObtenerEscenarioPorCodigo(codigoEscenario);
        List<EscenarioCambio> cambios = escenarioBLL.ObtenerCambiosPorEscenario(codigoEscenario);
        litNombreDetalleEscenario.Text = escenario.Nombre;
        litEstadoDetalleEscenario.Text = escenario.Estado;
        litDescripcionDetalleEscenario.Text = string.IsNullOrEmpty(escenario.Descripcion) ? "-" : escenario.Descripcion;
        litObjetivoDetalleEscenario.Text = string.IsNullOrEmpty(escenario.Objetivo) ? "-" : escenario.Objetivo;
        if (cambios.Count == 0)
        {
            rptDetalleCambios.Visible = false;
            lblSinCambiosDetalle.Visible = true;
        }
        else
        {
            rptDetalleCambios.Visible = true;
            lblSinCambiosDetalle.Visible = false;
            rptDetalleCambios.DataSource = cambios;
            rptDetalleCambios.DataBind();
        }
        pnlDetalleEscenario.Style["display"] = "flex";
    }
    public string ObtenerClaseEstado(string estado)
    {
        switch (estado)
        {
            case "Base": return "esc-etiqueta-base";
            case "Activo": return "etiqueta-activo";
            case "En revisión": return "esc-etiqueta-en-revision";
            case "Aprobado": return "esc-etiqueta-aprobado";
            case "Borrador": return "esc-etiqueta-borrador";
            default: return "";
        }
    }
    private void MostrarMensaje(string texto, bool esExito)
    {
        lblMensaje.Text = texto;
        lblMensaje.CssClass = esExito ? "mensaje" : "mensaje mensaje-error";
        lblMensaje.Visible = true;
    }
    #endregion

    protected void ddlLocal_SelectedIndexChanged(object sender, EventArgs e) { CargarEscenarios(); }
    protected void btnNuevoEscenario_Click(object sender, EventArgs e)
    {
        if (CodigoLocalActual == 0)
        {
            MostrarMensaje("Seleccioná un local.", esExito: false);
            return;
        }
        tbNombreEscenario.Text = "";
        tbDescripcionEscenario.Text = "";
        tbObjetivoEscenario.Text = "";
        List<SectorListado> sectores = sectorBLL.ObtenerSectoresPorLocal(CodigoLocalActual);
        rptCambiosPorSector.DataSource = sectores;
        rptCambiosPorSector.DataBind();
        pnlModalEscenario.Style["display"] = "flex";
    }
    protected void btnCancelarEscenario_Click(object sender, EventArgs e) { pnlModalEscenario.Style["display"] = "none"; }
    protected void btnGuardarEscenario_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid)
        {
            pnlModalEscenario.Style["display"] = "flex";
            return;
        }
        EscenarioBE escenario = new EscenarioBE(CodigoLocalActual, tbNombreEscenario.Text.Trim(), tbDescripcionEscenario.Text.Trim(), tbObjetivoEscenario.Text.Trim(), DateTime.Now, "Borrador");        
        // Recorre cada fila del Repeater y arma un escenario por cada sector donde se haya elegido algo distinto de "Sin cambios".
        var cambios = new List<EscenarioCambio>();
        foreach (RepeaterItem item in rptCambiosPorSector.Items)
        {
            var ddlCambio = (DropDownList)item.FindControl("ddlCambioSector");
            if (ddlCambio != null && !string.IsNullOrEmpty(ddlCambio.SelectedValue))
            {
                var sector = item.DataItem as SectorListado;
                string nombreSector = sector?.Nombre ?? "";
                cambios.Add(new EscenarioCambio
                {
                    TipoCambio = "Modificación de sector",
                    Descripcion = $"{nombreSector}: {ddlCambio.SelectedValue}"
                });
            }
        }
        escenarioBLL.CrearEscenario(escenario, cambios);
        pnlModalEscenario.Style["display"] = "none";
        MostrarMensaje("Escenario creado correctamente.", esExito: true);
        CargarEscenarios();
    }
    protected void rptEscenarios_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int codigoEscenario = int.Parse(e.CommandArgument.ToString());
        switch (e.CommandName)
        {
            case "VerDetalle":
                MostrarDetalleEscenario(codigoEscenario);
                break;
            case "Comparar":
                Response.Redirect($"PaginaComparacionEscenarios.aspx?local={CodigoLocalActual}&escenarioA={codigoEscenario}");
                break;
        }
    }    
}