using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BE;
using BLL;
using SERVICIO.Logica;

public partial class PaginaEscenarios : PaginaSegura
{
    protected override string PermisoRequerido
    {
        get { return PermisoNombres.GestionarEscenarios; }
    }
    protected override string ClaveTitulo
    {
        get { return "escenarios.titulo"; }
    }
    private LocalBLL localBLL = new LocalBLL();
    private SectorBLL sectorBLL = new SectorBLL();
    private EscenarioBLL escenarioBLL = new EscenarioBLL();
    private IndicadorBLL indicadorBLL = new IndicadorBLL();
    private LayoutBLL layoutBLL = new LayoutBLL();
    private int CodigoLocalActual
    {
        get { return string.IsNullOrEmpty(ddlLocal.SelectedValue) ? 0 : int.Parse(ddlLocal.SelectedValue); }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CargarDropdownLocales();
            int codigoLocalQuery;
            if (int.TryParse(Request.QueryString["local"], out codigoLocalQuery) && ddlLocal.Items.FindByValue(codigoLocalQuery.ToString()) != null)
            {
                ddlLocal.SelectedValue = codigoLocalQuery.ToString();
            }
            CargarEscenarios();
            AbrirDesdeRecomendacion();
        }
    }
    #region Funciones
    private void AbrirDesdeRecomendacion()
    {
        int codigoSector;
        string accion = Request.QueryString["accion"];
        if (CodigoLocalActual == 0 || !int.TryParse(Request.QueryString["sector"], out codigoSector) || string.IsNullOrEmpty(accion)) return;
        SectorListado sector = sectorBLL.ObtenerSectoresPorLocal(CodigoLocalActual).FirstOrDefault(s => s.CodigoSector == codigoSector);
        if (sector == null) return;
        AbrirNuevoEscenario();
        bool aplicado = false;
        foreach (RepeaterItem item in rptCambiosPorSector.Items)
        {
            var hfSector = (HiddenField)item.FindControl("hfCodigoSectorCambio");
            var ddlCambio = (DropDownList)item.FindControl("ddlCambioSector");
            if (hfSector != null && ddlCambio != null && hfSector.Value == codigoSector.ToString() && ddlCambio.Items.FindByValue(accion) != null)
            {
                ddlCambio.SelectedValue = accion;
                aplicado = true;
            }
        }
        if (!aplicado) return;
        tbNombreEscenario.Text = $"{TraducirValor(accion)} - {sector.Nombre}";
        tbDescripcionEscenario.Text = Traducir("escenarios.desdeRecomendacion");
    }
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
        rptSupuestos.DataSource = IndicadorBLL.ObtenerSupuestos();
        rptSupuestos.DataBind();
        pnlSituacionActual.Visible = false;
        if (CodigoLocalActual == 0)
        {
            rptEscenarios.Visible = false;
            lblSinEscenarios.Visible = true;
            return;
        }
        MostrarSituacionActual();
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
            CalcularEstimaciones(escenarios);
            rptEscenarios.DataSource = escenarios;
            rptEscenarios.DataBind();
        }
    }
    private List<IndicadorSectorListado> indicadoresReales;
    private List<IndicadorSectorListado> IndicadoresReales
    {
        get
        {
            if (indicadoresReales == null) indicadoresReales = indicadorBLL.CalcularIndicadoresPorSector(CodigoLocalActual, PeriodoDesde(1));
            return indicadoresReales;
        }
    }
    // Tarjeta de referencia: los valores reales del local (último mes), sin ningún cambio
    private void MostrarSituacionActual()
    {
        List<IndicadorSectorListado> reales = IndicadoresReales;
        if (reales.Count == 0) return;
        litPeriodoActual.Text = HttpUtility.HtmlEncode(Traducir("stock.rangoPeriodo", DescribirPeriodo(1)));
        litVentasActual.Text = reales.Sum(r => r.Ventas).ToString("$#,##0");
        litCirculacionActual.Text = reales.Average(r => r.Circulacion).ToString("0.#") + "%";
        litIndiceActual.Text = ((int)Math.Round(reales.Average(r => r.IndiceGlobal), 0)) + "/100";
        pnlSituacionActual.Visible = true;
    }
    // Ventas, circulación e índice global que tendría el local con los cambios de cada escenario, y cuánto cambian
    // contra la situación actual (los dos se calculan sobre el mismo período: la diferencia es solo el efecto de los cambios)
    private void CalcularEstimaciones(List<EscenarioListado> pEscenarios)
    {
        List<IndicadorSectorListado> reales = IndicadoresReales;
        if (reales.Count == 0) return;
        decimal ventasActuales = reales.Sum(r => r.Ventas);
        int indiceActual = (int)Math.Round(reales.Average(r => r.IndiceGlobal), 0);
        foreach (EscenarioListado escenario in pEscenarios)
        {
            List<IndicadorSectorListado> proyectados = indicadorBLL.ProyectarIndicadores(reales, escenarioBLL.ObtenerCambiosPorEscenario(escenario.CodigoEscenario));
            escenario.VentasEstimadas = proyectados.Sum(p => p.Ventas);
            escenario.CirculacionEstimada = Math.Round(proyectados.Average(p => p.Circulacion), 1);
            escenario.IndiceGlobalEstimado = (int)Math.Round(proyectados.Average(p => p.IndiceGlobal), 0);
            escenario.VariacionVentas = IndicadorBLL.Variacion(escenario.VentasEstimadas, ventasActuales);
            escenario.VariacionIndice = escenario.IndiceGlobalEstimado - indiceActual;
        }
    }
    protected string MostrarVariacionPorcentaje(object pVariacion)
    {
        if (pVariacion == null) return "";
        decimal variacion = Convert.ToDecimal(pVariacion);
        return SpanVariacion(variacion, (variacion > 0 ? "+" : "") + variacion.ToString("0.#") + "%");
    }
    protected string MostrarVariacionPuntos(object pPuntos)
    {
        int puntos = pPuntos == null ? 0 : Convert.ToInt32(pPuntos);
        return SpanVariacion(puntos, Traducir("escenarios.puntos", (puntos > 0 ? "+" : "") + puntos));
    }
    // Verde si sube, rojo si baja; la flecha y el signo hacen que no dependa solo del color
    private string SpanVariacion(decimal pValor, string pTexto)
    {
        string clase = pValor > 0 ? "esc-variacion-sube" : pValor < 0 ? "esc-variacion-baja" : "esc-variacion-igual";
        string flecha = pValor > 0 ? "&#9650; " : pValor < 0 ? "&#9660; " : "= ";
        return "<span class='esc-variacion " + clase + "' title='" + HttpUtility.HtmlAttributeEncode(Traducir("escenarios.vsActual")) + "'>"
            + flecha + HttpUtility.HtmlEncode(pTexto) + "</span>";
    }
    protected string MostrarDelta(object pDelta)
    {
        decimal delta = pDelta == null ? 0 : Convert.ToDecimal(pDelta);
        return delta == 0 ? "-" : (delta > 0 ? "+" : "") + delta.ToString("0.#") + "%";
    }
    private void MostrarDetalleEscenario(int codigoEscenario)
    {
        EscenarioBE escenario = escenarioBLL.ObtenerEscenarioPorCodigo(codigoEscenario);
        List<EscenarioCambio> cambios = escenarioBLL.ObtenerCambiosPorEscenario(codigoEscenario);
        hfCodigoEscenarioDetalle.Value = escenario.CodigoEscenario.ToString();
        litNombreDetalleEscenario.Text = HttpUtility.HtmlEncode(escenario.Nombre);
        litEstadoDetalleEscenario.Text = TraducirValor(escenario.Estado);
        if (ddlEstadoDetalle.Items.FindByValue(escenario.Estado) != null) ddlEstadoDetalle.SelectedValue = escenario.Estado;
        litDescripcionDetalleEscenario.Text = string.IsNullOrEmpty(escenario.Descripcion) ? "-" : HttpUtility.HtmlEncode(escenario.Descripcion);
        litObjetivoDetalleEscenario.Text = string.IsNullOrEmpty(escenario.Objetivo) ? "-" : HttpUtility.HtmlEncode(escenario.Objetivo);
        LayoutBE layoutBase = escenario.CodigoLayout == null ? null : layoutBLL.ObtenerLayoutPorCodigo(escenario.CodigoLayout.Value);
        litLayoutDetalleEscenario.Text = HttpUtility.HtmlEncode(layoutBase?.Nombre ?? Traducir("escenarios.sinLayout"));
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
            case "Descartado": return "esc-etiqueta-descartado";
            default: return "";
        }
    }
    protected string TraducirCambio(object descripcion)
    {
        string texto = descripcion == null ? "" : descripcion.ToString();
        int separador = texto.LastIndexOf(": ");
        if (separador < 0) return TraducirValor(texto);
        return texto.Substring(0, separador + 2) + TraducirValor(texto.Substring(separador + 2));
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
            MostrarMensaje(Traducir("stock.errSeleccionarLocal"), esExito: false);
            return;
        }
        AbrirNuevoEscenario();
    }
    private void AbrirNuevoEscenario()
    {
        tbNombreEscenario.Text = "";
        tbDescripcionEscenario.Text = "";
        tbObjetivoEscenario.Text = "";
        List<SectorListado> sectores = sectorBLL.ObtenerSectoresPorLocal(CodigoLocalActual);
        rptCambiosPorSector.DataSource = sectores;
        rptCambiosPorSector.DataBind();
        LayoutBE vigente = layoutBLL.ObtenerLayoutVigente(CodigoLocalActual);
        litLayoutBaseNuevo.Text = HttpUtility.HtmlEncode(vigente == null ? Traducir("escenarios.sinLayoutVigente") : Traducir("escenarios.layoutBaseNuevo", vigente.Nombre));
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
        Dictionary<string, string> nombresSector = sectorBLL.ObtenerSectoresPorLocal(CodigoLocalActual).ToDictionary(s => s.CodigoSector.ToString(), s => s.Nombre);
        var cambios = new List<EscenarioCambio>();
        foreach (RepeaterItem item in rptCambiosPorSector.Items)
        {
            var ddlCambio = (DropDownList)item.FindControl("ddlCambioSector");
            var hfSector = (HiddenField)item.FindControl("hfCodigoSectorCambio");
            string nombreSector;
            if (ddlCambio != null && !string.IsNullOrEmpty(ddlCambio.SelectedValue)
                && hfSector != null && nombresSector.TryGetValue(hfSector.Value, out nombreSector))
            {
                cambios.Add(new EscenarioCambio
                {
                    TipoCambio = "Modificación de sector",
                    Descripcion = $"{nombreSector}: {ddlCambio.SelectedValue}",
                    CodigoSector = int.Parse(hfSector.Value),
                    Accion = ddlCambio.SelectedValue
                });
            }
        }
        try
        {
            escenarioBLL.CrearEscenario(escenario, cambios);
        }
        catch (ExcepcionTraducible error)
        {
            // Sin cambios o nombre repetido: el error se ve dentro del modal, que queda abierto con lo cargado
            lblMensajeModal.Text = TraducirError(error);
            lblMensajeModal.Visible = true;
            pnlModalEscenario.Style["display"] = "flex";
            return;
        }
        pnlModalEscenario.Style["display"] = "none";
        MostrarMensaje(Traducir("escenarios.msgCreado"), esExito: true);
        CargarEscenarios();
    }
    protected void btnCambiarEstado_Click(object sender, EventArgs e)
    {
        int codigoEscenario;
        if (!int.TryParse(hfCodigoEscenarioDetalle.Value, out codigoEscenario)) return;
        escenarioBLL.CambiarEstado(codigoEscenario, ddlEstadoDetalle.SelectedValue);
        MostrarMensaje(Traducir("escenarios.msgEstado", TraducirValor(ddlEstadoDetalle.SelectedValue)), esExito: true);
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
            case "Eliminar":
                int comparaciones = escenarioBLL.EliminarEscenario(codigoEscenario);
                MostrarMensaje(comparaciones > 0 ? Traducir("escenarios.msgEliminadoComparaciones", comparaciones) : Traducir("escenarios.msgEliminado"), esExito: true);
                CargarEscenarios();
                break;
        }
    }    
}