using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BE;
using ClosedXML.Excel;
using SERVICIO.Logica;

public partial class PaginaAuditorias : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Sesion.Instancia.IsLogueado() || Sesion.Instancia.Usuario.Rol != 1)
        {
            Response.Redirect("PaginaLogin.aspx");
            return;
        }

        if (!IsPostBack)
        {
            CargarDropdownTipos();
            CargarBitacora(SucesoServicio.Instancia.ObtenerSucesosUltimosTresDias());
            lblMostrando.Text = "Mostrando los últimos 3 días";
        }
    }

    #region Funciones
    private void CargarDropdownTipos()
    {
        List<string> tipos = SucesoServicio.Instancia.ObtenerTiposSuceso();
        ddlTipoSuceso.Items.Clear();
        ddlTipoSuceso.Items.Add(new System.Web.UI.WebControls.ListItem("Todas las acciones", ""));
        foreach (string tipo in tipos)
        {
            ddlTipoSuceso.Items.Add(new System.Web.UI.WebControls.ListItem(tipo, tipo));
        }
    }
    private void CargarBitacora(List<Suceso> sucesos)
    {
        if (sucesos.Count == 0)
        {
            rptBitacora.Visible = false;
            lblSinSucesos.Visible = true;
        }
        else
        {
            rptBitacora.Visible = true;
            lblSinSucesos.Visible = false;
            rptBitacora.DataSource = sucesos;
            rptBitacora.DataBind();
        }
    }    
    public string ObtenerClaseCriticidad(string criticidad)
    {
        switch (criticidad)
        {
            case "1": return "etiqueta-activo";
            case "2": return "aud-etiqueta-warn";
            case "3": return "etiqueta-inactivo";
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

    protected void btnFiltrar_Click(object sender, EventArgs e)
    {
        string busqueda = tbBuscar.Text.Trim();
        DateTime? desde = null, hasta = null;
        DateTime fechaDesdeParsed, fechaHastaParsed;
        bool tieneDesde = DateTime.TryParse(tbFechaDesde.Text, out fechaDesdeParsed);
        bool tieneHasta = DateTime.TryParse(tbFechaHasta.Text, out fechaHastaParsed);
        if (tieneDesde) desde = fechaDesdeParsed;
        if (tieneHasta) hasta = fechaHastaParsed;
        string tipoSuceso = ddlTipoSuceso.SelectedValue;
        int? criticidad = string.IsNullOrEmpty(ddlCriticidad.SelectedValue) ? (int?)null : int.Parse(ddlCriticidad.SelectedValue);
        bool hayAlgunFiltro = !string.IsNullOrEmpty(busqueda) || desde.HasValue || hasta.HasValue || !string.IsNullOrEmpty(tipoSuceso) || criticidad.HasValue;
        if (!hayAlgunFiltro)
        {
            MostrarMensaje("Debe elegir mínimo un filtro.", esExito: false);
            return;
        }
        if (desde.HasValue && hasta.HasValue && desde.Value > hasta.Value)
        {
            MostrarMensaje("La fecha DESDE no puede ser mayor a la fecha HASTA.", esExito: false);
            return;
        }
        List<Suceso> resultado = SucesoServicio.Instancia.FiltrarSucesos(desde, hasta, busqueda, tipoSuceso, criticidad);
        if (resultado.Count == 0)
        {
            MostrarMensaje("No hay sucesos.", esExito: true);
        }
        else
        {
            lblMensaje.Visible = false;
        }
        lblMostrando.Text = $"Mostrando {resultado.Count} resultado(s) filtrado(s)";
        CargarBitacora(resultado);
    }

    protected void btnExportarLog_Click(object sender, EventArgs e)
    {
        List<Suceso> sucesos = SucesoServicio.Instancia.ObtenerSucesos();
        using (var workbook = new XLWorkbook())
        {
            var hoja = workbook.Worksheets.Add("Auditoría");
            // Encabezados
            hoja.Cell(1, 1).Value = "Fecha";
            hoja.Cell(1, 2).Value = "Usuario";
            hoja.Cell(1, 3).Value = "Tipo de Suceso";
            hoja.Cell(1, 4).Value = "Descripción";
            hoja.Cell(1, 5).Value = "Criticidad";
            hoja.Range(1, 1, 1, 5).Style.Font.Bold = true;
            // Filas
            int fila = 2;
            foreach (var s in sucesos)
            {
                hoja.Cell(fila, 1).Value = s.Fecha;
                hoja.Cell(fila, 1).Style.DateFormat.Format = "dd/MM/yyyy HH:mm:ss";
                hoja.Cell(fila, 2).Value = s.Usuario;
                hoja.Cell(fila, 3).Value = s.TipoSuceso;
                hoja.Cell(fila, 4).Value = s.Descripcion;
                hoja.Cell(fila, 5).Value = s.Criticidad;
                fila++;
            }
            hoja.Columns().AdjustToContents();
            using (var stream = new System.IO.MemoryStream())
            {
                workbook.SaveAs(stream);
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AppendHeader("Content-Disposition", $"attachment; filename=auditoria_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
                Response.BinaryWrite(stream.ToArray());
                Response.End();
            }
        }
    }
    protected void btnAnterior_Click(object sender, EventArgs e) { }
    protected void btnSiguiente_Click(object sender, EventArgs e) { }    
}