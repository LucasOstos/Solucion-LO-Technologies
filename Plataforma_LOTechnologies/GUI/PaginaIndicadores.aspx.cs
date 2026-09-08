using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BE;
using BLL;
using SERVICIO.Logica;

public partial class PaginaIndicadores : System.Web.UI.Page
{
    private LocalBLL localBLL = new LocalBLL();
    private StockBLL stockBLL = new StockBLL();
    private IndicadorBLL indicadorBLL = new IndicadorBLL();
    private RecomendacionBLL recomendacionBLL = new RecomendacionBLL();
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
            AplicarFiltros();
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
    private string ObtenerPeriodoDesde()
    {
        int mesesAtras = ddlPeriodo.SelectedValue == "3_meses" ? 3 : 1;
        return DateTime.Now.AddMonths(-mesesAtras).ToString("yyyy-MM");
    }
    private void AplicarFiltros()
    {
        if (CodigoLocalActual == 0) return;
        string periodoDesde = ObtenerPeriodoDesde();
        string periodoActual = DateTime.Now.ToString("yyyy-MM");
        List<IndicadorSectorListado> indicadores = indicadorBLL.CalcularIndicadoresPorSector(CodigoLocalActual, periodoDesde);
        if (indicadores.Count == 0)
        {
            rptIndicadoresSector.Visible = false;
            lblSinIndicadores.Visible = true;
            rptRecomendaciones.Visible = false;
            lblSinRecomendaciones.Visible = true;
            return;
        }
        rptIndicadoresSector.Visible = true;
        lblSinIndicadores.Visible = false;
        rptIndicadoresSector.DataSource = indicadores;
        rptIndicadoresSector.DataBind();
        indicadorBLL.GuardarIndicadores(CodigoLocalActual, periodoActual, indicadores);
        List<StockListado> stock = stockBLL.ObtenerStockPorLocal(CodigoLocalActual);
        List<RecomendacionBE> recomendaciones = recomendacionBLL.GenerarRecomendaciones(CodigoLocalActual, indicadores, stock);
        if (recomendaciones.Count == 0)
        {
            rptRecomendaciones.Visible = false;
            lblSinRecomendaciones.Visible = true;
        }
        else
        {
            rptRecomendaciones.Visible = true;
            lblSinRecomendaciones.Visible = false;
            rptRecomendaciones.DataSource = recomendaciones;
            rptRecomendaciones.DataBind();
            recomendacionBLL.GuardarRecomendaciones(recomendaciones);
        }
    }
    #endregion

    protected void Filtro_Changed(object sender, EventArgs e)
    {
        AplicarFiltros();
    }
    protected void btnAplicar_Click(object sender, EventArgs e)
    {
        AplicarFiltros();
    }
}