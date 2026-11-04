using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BE;
using BLL;
using SERVICIO.Logica;

public partial class PaginaStockVentas : PaginaSegura
{
    protected override string PermisoRequerido
    {
        get { return PermisoNombres.CargarStockVentas; }
    }
    protected override string ClaveTitulo
    {
        get { return "stock.titulo"; }
    }
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
        List<ProductoListado> productos = CodigoLocalActual > 0 ? productoBLL.ObtenerProductosListado(CodigoLocalActual, "", null) : new List<ProductoListado>();
        ddlProductoCarga.Items.Clear();
        foreach (ProductoListado producto in productos.Where(p => p.Estado == 1))
        {
            string texto = $"{producto.CodigoInterno} - {producto.Nombre}";
            if (producto.NombreSector == null) texto = Traducir("stock.productoSinUbicar", texto);
            ddlProductoCarga.Items.Add(new ListItem(texto, producto.CodigoProducto.ToString()));
        }
        bool hayProductos = ddlProductoCarga.Items.Count > 0;
        lblSinProductosCarga.Visible = !hayProductos;
        btnGuardarCarga.Enabled = hayProductos;
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
        decimal? margen = ventaBLL.ObtenerMargenTotal(CodigoLocalActual, periodoDesde);
        litMargenTotal.Text = margen == null ? "-" : margen.Value.ToString("0.#") + "%";
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

    protected void btnImportarCSV_Click(object sender, EventArgs e)
    {
        MostrarMensaje(Traducir("stock.errImportarCsv"), esExito: false);
    }
    protected void btnCargaManual_Click(object sender, EventArgs e)
    {
        hfCodigoStockEdicion.Value = "";
        ddlTipoCarga.SelectedIndex = 0;
        ddlTipoCarga.Enabled = true;
        ddlProductoCarga.Enabled = true;
        if (ddlProductoCarga.Items.Count > 0) ddlProductoCarga.SelectedIndex = 0;
        tbCantidadCarga.Text = "";
        tbImporteCarga.Text = "";
        tbMinimoCarga.Text = "";
        tbMaximoCarga.Text = "";
        tbFechaCarga.Text = "";
        pnlModalCarga.Style["display"] = "flex";
    }
    protected void btnAplicar_Click(object sender, EventArgs e) { ActualizarVistas(); }
    protected void ddlLocal_SelectedIndexChanged(object sender, EventArgs e)
    {
        CargarDropdownProductos();
        ActualizarVistas();
    }
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
            MostrarMensaje(Traducir("stock.errSeleccionarLocal"), esExito: false);
            pnlModalCarga.Style["display"] = "flex";
            return;
        }
        int cantidad;
        if (!int.TryParse(tbCantidadCarga.Text.Trim(), out cantidad))
        {
            MostrarMensaje(Traducir("comun.datosInvalidos"), esExito: false);
            pnlModalCarga.Style["display"] = "flex";
            return;
        }
        DateTime fecha;
        if (!DateTime.TryParse(tbFechaCarga.Text, out fecha))
        {
            MostrarMensaje(Traducir("comun.completeDatos"), esExito: false);
            pnlModalCarga.Style["display"] = "flex";
            return;
        }
        int codigoProducto = int.Parse(ddlProductoCarga.SelectedValue);
        string periodo = fecha.ToString("yyyy-MM");
        if (ddlTipoCarga.SelectedValue == "Venta")
        {
            decimal importe = LeerDecimal(tbImporteCarga.Text) ?? 0;
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
        MostrarMensaje(Traducir("stock.msgRegistrado"), esExito: true);
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
            // "Actualizar" corrige el stock de ESE producto, el producto y el tipo no se pueden cambiar
            ddlTipoCarga.Enabled = false;
            ddlProductoCarga.Enabled = false;
            // Si el producto se desactivó no está en la lista: se agrega para poder actualizar su stock
            if (ddlProductoCarga.Items.FindByValue(stock.CodigoProducto.ToString()) == null)
            {
                ProductoBE producto = productoBLL.ObtenerProductoPorCodigo(stock.CodigoProducto);
                ddlProductoCarga.Items.Add(new ListItem(producto == null ? stock.CodigoProducto.ToString() : $"PROD-{producto.NumeroSerie:D3} - {producto.Nombre}", stock.CodigoProducto.ToString()));
                btnGuardarCarga.Enabled = true;
            }
            ddlProductoCarga.SelectedValue = stock.CodigoProducto.ToString();
            tbCantidadCarga.Text = stock.CantidadDisponible.ToString();
            tbImporteCarga.Text = "";
            tbMinimoCarga.Text = stock.Minimo?.ToString() ?? "";
            tbMaximoCarga.Text = stock.Maximo?.ToString() ?? "";
            tbFechaCarga.Text = DateTime.Now.ToString("yyyy-MM-dd");
            pnlModalCarga.Style["display"] = "flex";
        }
    }
}