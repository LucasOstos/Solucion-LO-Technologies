using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BE;
using BLL;
using SERVICIO.Logica;

public partial class PaginaAnalisisProductos : System.Web.UI.Page
{
    private LocalBLL localBLL = new LocalBLL();
    private AnalisisProductoBLL analisisBLL = new AnalisisProductoBLL();
    private List<ProductoAnalisisListado> ultimoResultado
    {
        get { return (List<ProductoAnalisisListado>)(ViewState["UltimoResultado"] ?? new List<ProductoAnalisisListado>()); }
        set { ViewState["UltimoResultado"] = value; }
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
    private void MostrarResultados(List<ProductoAnalisisListado> resultado)
    {
        litProductosAnalizados.Text = resultado.Count.ToString();
        litPrioridadAlta.Text = resultado.Count(r => r.Prioridad == "Alta").ToString();
        litPrioridadMedia.Text = resultado.Count(r => r.Prioridad == "Media").ToString();
        litPrioridadBaja.Text = resultado.Count(r => r.Prioridad == "Baja").ToString();
        AplicarFiltroYBindear();
        pnlResultados.Visible = true;
    }
    private void AplicarFiltroYBindear()
    {
        string busqueda = tbBuscarProducto.Text.Trim().ToLower();
        string prioridad = ddlFiltroPrioridad.SelectedValue;
        var filtrados = ultimoResultado.Where(r => (string.IsNullOrEmpty(busqueda) || r.NombreProducto.ToLower().Contains(busqueda) || r.NombreCategoria.ToLower().Contains(busqueda))).ToList();
        var candidatos = filtrados.Where(r => !string.IsNullOrEmpty(r.Prioridad));
        if (!string.IsNullOrEmpty(prioridad))
        {
            candidatos = candidatos.Where(r => r.Prioridad == prioridad);
        }
        var listaCandidatos = candidatos.ToList();
        if (listaCandidatos.Count == 0)
        {
            rptCandidatos.Visible = false;
            lblSinCandidatos.Visible = true;
        }
        else
        {
            rptCandidatos.Visible = true;
            lblSinCandidatos.Visible = false;
            rptCandidatos.DataSource = listaCandidatos;
            rptCandidatos.DataBind();
        }
        var sinRecomendacion = filtrados.Where(r => string.IsNullOrEmpty(r.Prioridad)).ToList();
        rptSinRecomendacion.DataSource = sinRecomendacion;
        rptSinRecomendacion.DataBind();
    }

    private void MostrarMensaje(string texto, bool esExito)
    {
        lblMensaje.Text = texto;
        lblMensaje.CssClass = esExito ? "mensaje" : "mensaje mensaje-error";
        lblMensaje.Visible = true;
    }
    #endregion

    protected void Filtro_Changed(object sender, EventArgs e)
    {
        if (ultimoResultado.Count > 0)
        {
            AplicarFiltroYBindear();
            pnlResultados.Visible = true;
        }
    }

    protected void btnDetectar_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(ddlLocal.SelectedValue))
        {
            MostrarMensaje("Seleccioná un local.", esExito: false);
            return;
        }
        DateTime periodoDesde, periodoHasta;
        if (!DateTime.TryParse(tbPeriodoDesde.Text, out periodoDesde) || !DateTime.TryParse(tbPeriodoHasta.Text, out periodoHasta))
        {
            MostrarMensaje("Período inválido.", esExito: false);
            pnlResultados.Visible = false;
            return;
        }
        if (periodoDesde > periodoHasta)
        {
            MostrarMensaje("Período inválido.", esExito: false);
            pnlResultados.Visible = false;
            return;
        }
        int codigoLocal = int.Parse(ddlLocal.SelectedValue);
        string periodoDesdeStr = periodoDesde.ToString("yyyy-MM");
        List<ProductoAnalisisListado> resultado = analisisBLL.DetectarOportunidades(codigoLocal, periodoDesdeStr);
        if (resultado.Count == 0)
        {
            MostrarMensaje("No existen datos suficientes para detectar oportunidades de reubicación.", esExito: false);
            pnlResultados.Visible = false;
            return;
        }
        ultimoResultado = resultado;
        MostrarResultados(resultado);
    }
}