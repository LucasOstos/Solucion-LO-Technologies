using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BE;
using BLL;
using SERVICIO.Logica;

public partial class PaginaStockVentas : System.Web.UI.Page
{
    private LocalBLL localBLL = new LocalBLL();
    private ProductoBLL productoBLL = new ProductoBLL();
    private VentaBLL ventaBLL = new VentaBLL();
    private StockBLL stockBLL = new StockBLL();
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
            CargarDropdownProductos();
            ActualizarVistas();
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
    private void CargarDropdownProductos()
    {
        List<ProductoListado> productos = productoBLL.ObtenerProductosListado("", null);
        ddlProductoCarga.Items.Clear();
        foreach (ProductoListado producto in productos)
        {
            ddlProductoCarga.Items.Add(new ListItem($"{producto.CodigoInterno} - {producto.Nombre}", producto.CodigoProducto.ToString()));
        }
    }
    private string ObtenerPeriodoDesde()
    {
        int mesesAtras;
        switch (ddlPeriodo.SelectedValue)
        {
            case "3_meses": mesesAtras = 3; break;
            case "5_meses": mesesAtras = 5; break;
            default: mesesAtras = 1; break;
        }
        DateTime fechaCorte = DateTime.Now.AddMonths(-mesesAtras);
        return fechaCorte.ToString("yyyy-MM");
    }
    private void ActualizarVistas()
    {
        if (CodigoLocalActual == 0) return;

        CargarEstadisticasVentas();
        CargarDetalleVentas();
        CargarStock();
    }
    private void CargarEstadisticasVentas()
    {
        string periodoDesde = ObtenerPeriodoDesde();
        decimal totalVentas = ventaBLL.ObtenerTotalVentas(CodigoLocalActual, periodoDesde);
        decimal ticketPromedio = ventaBLL.ObtenerTicketPromedio(CodigoLocalActual, periodoDesde);
        int unidadesVendidas = ventaBLL.ObtenerUnidadesVendidas(CodigoLocalActual, periodoDesde);
        litTotalVentas.Text = totalVentas.ToString("$#,##0");
        litTicketPromedio.Text = ticketPromedio.ToString("$#,##0");
        litUnidadesVendidas.Text = unidadesVendidas.ToString();
        litMargenTotal.Text = "-"; // TODO: sin dato de costo modelado todavía
    }
    private void CargarDetalleVentas()
    {
        string periodoDesde = ObtenerPeriodoDesde();
        List<VentaListado> ventas = ventaBLL.ObtenerVentasAgrupadas(CodigoLocalActual, periodoDesde);
        if (ventas.Count == 0)
        {
            rptVentas.Visible = false;
            lblSinVentas.Visible = true;
        }
        else
        {
            rptVentas.Visible = true;
            lblSinVentas.Visible = false;
            rptVentas.DataSource = ventas;
            rptVentas.DataBind();
        }
    }
    private void CargarStock()
    {
        List<StockListado> stock = stockBLL.ObtenerStockPorLocal(CodigoLocalActual);
        if (stock.Count == 0)
        {
            rptStock.Visible = false;
            lblSinStock.Visible = true;
        }
        else
        {
            rptStock.Visible = true;
            lblSinStock.Visible = false;
            rptStock.DataSource = stock;
            rptStock.DataBind();
        }
    }
    private void MostrarMensaje(string texto, bool esExito)
    {
        lblMensaje.Text = texto;
        lblMensaje.CssClass = esExito ? "mensaje" : "mensaje mensaje-error";
        lblMensaje.Visible = true;
    }
    #endregion

    protected void btnImportarCSV_Click(object sender, EventArgs e) { MostrarMensaje("La importación por CSV todavía no está disponible.", esExito: false); }
    protected void btnCargaManual_Click(object sender, EventArgs e)
    {
        hfCodigoStockEdicion.Value = "";
        ddlTipoCarga.SelectedIndex = 0;
        if (ddlProductoCarga.Items.Count > 0) ddlProductoCarga.SelectedIndex = 0;
        tbCantidadCarga.Text = "";
        tbImporteCarga.Text = "";
        tbFechaCarga.Text = "";
        pnlModalCarga.Style["display"] = "flex";
    }
    protected void btnAplicar_Click(object sender, EventArgs e) { ActualizarVistas(); }

    protected void btnCancelarCarga_Click(object sender, EventArgs e) { pnlModalCarga.Style["display"] = "none"; }
    protected void btnGuardarCarga_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid)
        {
            pnlModalCarga.Style["display"] = "flex";
            return;
        }
        if (CodigoLocalActual == 0)
        {
            MostrarMensaje("Seleccioná un local.", esExito: false);
            pnlModalCarga.Style["display"] = "flex";
            return;
        }
        int cantidad;
        if (!int.TryParse(tbCantidadCarga.Text.Trim(), out cantidad))
        {
            MostrarMensaje("Datos inválidos.", esExito: false);
            pnlModalCarga.Style["display"] = "flex";
            return;
        }
        DateTime fecha;
        if (!DateTime.TryParse(tbFechaCarga.Text, out fecha))
        {
            MostrarMensaje("Complete los datos.", esExito: false);
            pnlModalCarga.Style["display"] = "flex";
            return;
        }
        int codigoProducto = int.Parse(ddlProductoCarga.SelectedValue);
        string periodo = fecha.ToString("yyyy-MM");
        if (ddlTipoCarga.SelectedValue == "Venta")
        {
            decimal importe;
            decimal.TryParse(tbImporteCarga.Text.Trim(), out importe);
            decimal precioUnitario = cantidad > 0 ? importe / cantidad : 0;
            VentaBE venta = new VentaBE(CodigoLocalActual, codigoProducto, fecha, cantidad, precioUnitario, importe, periodo);            
            ventaBLL.CrearVenta(venta);
        }
        else //Stock
        {
            bool esEdicion = !string.IsNullOrEmpty(hfCodigoStockEdicion.Value);
            int? minimo = null;
            int minimoParsed;
            if (int.TryParse(tbMinimoCarga.Text.Trim(), out minimoParsed)) minimo = minimoParsed;
            int? maximo = null;
            int maximoParsed;
            if (int.TryParse(tbMaximoCarga.Text.Trim(), out maximoParsed)) maximo = maximoParsed;
            StockBE stock = new StockBE(esEdicion ? int.Parse(hfCodigoStockEdicion.Value) : 0, CodigoLocalActual, codigoProducto, cantidad, minimo, maximo, periodo, DateTime.Now);
            stockBLL.GuardarStock(stock);
        }
        pnlModalCarga.Style["display"] = "none";
        MostrarMensaje("Datos registrados correctamente.", esExito: true);
        ActualizarVistas();
    }

    protected void rptStock_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "ActualizarStock")
        {
            int codigoStock = int.Parse(e.CommandArgument.ToString());
            StockBE stock = stockBLL.ObtenerStockPorCodigo(codigoStock);
            hfCodigoStockEdicion.Value = stock.CodigoStock.ToString();
            ddlTipoCarga.SelectedValue = "Stock";
            ddlProductoCarga.SelectedValue = stock.CodigoProducto.ToString();
            tbCantidadCarga.Text = stock.CantidadDisponible.ToString();
            tbImporteCarga.Text = "";
            tbFechaCarga.Text = DateTime.Now.ToString("yyyy-MM-dd");
            pnlModalCarga.Style["display"] = "flex";
        }
    }
}