using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BE;
using BLL;
using SERVICIO.Logica;

public partial class PaginaComparacionEscenarios : System.Web.UI.Page
{
    private LocalBLL localBLL = new LocalBLL();
    private EscenarioBLL escenarioBLL = new EscenarioBLL();
    private ComparacionBLL comparacionBLL = new ComparacionBLL();
    private IndicadorBLL indicadorBLL = new IndicadorBLL();
    private int CodigoLocalActual
    {
        get { return string.IsNullOrEmpty(ddlLocal.SelectedValue) ? 0 : int.Parse(ddlLocal.SelectedValue); }
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
    private void CargarDropdownEscenarios()
    {
        if (CodigoLocalActual == 0) return;
        List<EscenarioListado> escenarios = escenarioBLL.ObtenerEscenariosPorLocal(CodigoLocalActual);
        ddlEscenarioA.Items.Clear();
        ddlEscenarioB.Items.Clear();
        foreach (EscenarioListado esc in escenarios)
        {
            ddlEscenarioA.Items.Add(new ListItem(esc.Nombre, esc.CodigoEscenario.ToString()));
            ddlEscenarioB.Items.Add(new ListItem(esc.Nombre, esc.CodigoEscenario.ToString()));
        }
    }
    #endregion

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
            int codigoLocalQuery, codigoEscenarioAQuery;
            if (int.TryParse(Request.QueryString["local"], out codigoLocalQuery))
            {
                ddlLocal.SelectedValue = codigoLocalQuery.ToString();
            }
            CargarDropdownEscenarios();
            if (int.TryParse(Request.QueryString["escenarioA"], out codigoEscenarioAQuery))
            {
                ddlEscenarioA.SelectedValue = codigoEscenarioAQuery.ToString();
            }
        }
    }    
    protected void ddlLocal_SelectedIndexChanged(object sender, EventArgs e)
    {
        CargarDropdownEscenarios();
        pnlResultados.Visible = false;
    }

    protected void btnComparar_Click(object sender, EventArgs e)
    {
        if (ddlEscenarioA.Items.Count == 0 || ddlEscenarioB.Items.Count == 0)
        {
            MostrarMensaje("Complete los datos.", esExito: false);
            return;
        }
        int codigoEscenarioA = int.Parse(ddlEscenarioA.SelectedValue);
        int codigoEscenarioB = int.Parse(ddlEscenarioB.SelectedValue);
        if (codigoEscenarioA == codigoEscenarioB)
        {
            MostrarMensaje("Seleccioná dos escenarios distintos para comparar.", esExito: false);
            return;
        }
        EscenarioBE escenarioA = escenarioBLL.ObtenerEscenarioPorCodigo(codigoEscenarioA);
        EscenarioBE escenarioB = escenarioBLL.ObtenerEscenarioPorCodigo(codigoEscenarioB);
        if (escenarioA.CodigoLocal != escenarioB.CodigoLocal)
        {
            MostrarMensaje("Los escenarios deben pertenecer al mismo local.", esExito: false);
            return;
        }
        List<EscenarioCambio> cambiosA = escenarioBLL.ObtenerCambiosPorEscenario(codigoEscenarioA);
        List<EscenarioCambio> cambiosB = escenarioBLL.ObtenerCambiosPorEscenario(codigoEscenarioB);
        litNombreEscenarioA.Text = escenarioA.Nombre;
        litNombreEscenarioB.Text = escenarioB.Nombre;
        string periodoDesde = ddlPeriodo.SelectedValue == "3_meses" ? DateTime.Now.AddMonths(-3).ToString("yyyy-MM") : DateTime.Now.AddMonths(-1).ToString("yyyy-MM");
        List<IndicadorSectorListado> indicadoresReales = indicadorBLL.CalcularIndicadoresPorSector(CodigoLocalActual, periodoDesde);
        if (indicadoresReales.Count == 0)
        {
            MostrarMensaje("Faltan datos suficientes para comparar.", esExito: false);
            return;
        }
        List<IndicadorSectorListado> proyectadosA = indicadorBLL.ProyectarIndicadores(indicadoresReales, cambiosA);
        List<IndicadorSectorListado> proyectadosB = indicadorBLL.ProyectarIndicadores(indicadoresReales, cambiosB);
        rptIndicadoresA.DataSource = ArmarResumenProyectado(proyectadosA);
        rptIndicadoresA.DataBind();
        rptIndicadoresB.DataSource = ArmarResumenProyectado(proyectadosB);
        rptIndicadoresB.DataBind();
        rptDiferencias.DataSource = ArmarDiferenciasPorSector(proyectadosA, proyectadosB);
        rptDiferencias.DataBind();
        ViewState["CodigoEscenarioA"] = codigoEscenarioA;
        ViewState["CodigoEscenarioB"] = codigoEscenarioB;
        pnlResultados.Visible = true;
    }
    private List<IndicadorComparado> ArmarResumenProyectado(List<IndicadorSectorListado> proyectados)
    {
        decimal ventasTotales = proyectados.Sum(p => p.Ventas);
        decimal circulacionPromedio = proyectados.Count > 0 ? proyectados.Average(p => p.Circulacion) : 0;
        decimal rotacionPromedio = proyectados.Count > 0 ? proyectados.Average(p => p.Rotacion) : 0;
        decimal indiceGlobalPromedio = proyectados.Count > 0 ? (decimal)proyectados.Average(p => p.IndiceGlobal) : 0;
        return new List<IndicadorComparado>
        {
            new IndicadorComparado { Nombre = "Ventas estimadas", Valor = ventasTotales.ToString("$#,##0") },
            new IndicadorComparado { Nombre = "Circulación promedio", Valor = $"{circulacionPromedio:0.0}%" },
            new IndicadorComparado { Nombre = "Rotación promedio", Valor = $"{rotacionPromedio:0.0}%" },
            new IndicadorComparado { Nombre = "Índice Global promedio", Valor = indiceGlobalPromedio.ToString("0") }
        };
    }
    private List<DiferenciaComparacion> ArmarDiferenciasPorSector(List<IndicadorSectorListado> proyectadosA, List<IndicadorSectorListado> proyectadosB)
    {
        var filas = new List<DiferenciaComparacion>();
        foreach (var indB in proyectadosB)
        {
            var indA = proyectadosA.FirstOrDefault(p => p.CodigoSector == indB.CodigoSector);
            if (indA == null) continue;
            decimal diferenciaPorc = indA.Ventas != 0 ? ((indB.Ventas - indA.Ventas) / indA.Ventas) * 100 : 0;
            bool mejora = indB.Ventas > indA.Ventas;
            filas.Add(new DiferenciaComparacion
            {
                Indicador = indB.Sector,
                ValorActual = indA.Ventas.ToString("$#,##0"),
                ValorPropuesto = indB.Ventas.ToString("$#,##0"),
                Diferencia = (diferenciaPorc >= 0 ? "+" : "") + diferenciaPorc.ToString("0.0") + "%",
                ClaseDiferencia = mejora ? "etiqueta-activo" : (diferenciaPorc == 0 ? "etiqueta-info" : "etiqueta-inactivo")
            });
        }
        return filas;
    }
    protected void btnGuardarComparacion_Click(object sender, EventArgs e)
    {
        if (ViewState["CodigoEscenarioA"] == null || ViewState["CodigoEscenarioB"] == null)
        {
            MostrarMensaje("Primero generá una comparación.", esExito: false);
            return;
        }
        ComparacionBE comparacion = new ComparacionBE((int)ViewState["CodigoEscenarioA"], (int)ViewState["CodigoEscenarioB"], DateTime.Now, "Comparación basada en cambios propuestos por sector.", "");        
        comparacionBLL.GuardarComparacion(comparacion);
        MostrarMensaje("Comparación de escenarios guardada correctamente.", esExito: true);
    }

    private void MostrarMensaje(string texto, bool esExito)
    {
        lblMensaje.Text = texto;
        lblMensaje.CssClass = esExito ? "mensaje" : "mensaje mensaje-error";
        lblMensaje.Visible = true;
    }
}