using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BE;
using BLL;
using DAL;
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
    private const int CANTIDAD_ULTIMAS_VENTAS = 15;
    private const int ERRORES_A_MOSTRAR = 15;
    private LocalBLL localBLL = new LocalBLL();
    private ProductoBLL productoBLL = new ProductoBLL();
    private VentaBLL ventaBLL = new VentaBLL();
    private StockBLL stockBLL = new StockBLL();
    private ImportacionBLL importacionBLL = new ImportacionBLL();
    private int CodigoLocalActual
    {
        get { return string.IsNullOrEmpty(ddlLocal.SelectedValue) ? 0 : int.Parse(ddlLocal.SelectedValue); }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        tbFechaCarga.Attributes["max"] = DateTime.Today.ToString("yyyy-MM-dd");
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
    private string Agrupacion
    {
        get { return ddlAgrupacion.SelectedValue; }
    }
    private void ActualizarVistas()
    {
        if (CodigoLocalActual == 0) return;
        int meses = MesesDelFiltro(ddlPeriodo.SelectedValue);
        string periodoDesde = PeriodoDesde(meses);
        litRangoPeriodo.Text = HttpUtility.HtmlEncode(Traducir("stock.rangoPeriodo", DescribirPeriodo(meses)));
        CargarEstadisticasVentas(periodoDesde);
        CargarGraficoSectores(periodoDesde);
        CargarDetalleVentas(periodoDesde);
        CargarUltimasVentas();
        CargarStock();
        CargarImportaciones();
    }
    private void CargarEstadisticasVentas(string pPeriodoDesde)
    {
        decimal totalVentas = ventaBLL.ObtenerTotalVentas(CodigoLocalActual, pPeriodoDesde);
        int unidadesVendidas = ventaBLL.ObtenerUnidadesVendidas(CodigoLocalActual, pPeriodoDesde);
        decimal precioPromedio = unidadesVendidas > 0 ? totalVentas / unidadesVendidas : 0;
        litTotalVentas.Text = totalVentas.ToString("$#,##0");
        litPrecioPromedio.Text = precioPromedio.ToString("$#,##0.00");
        litUnidadesVendidas.Text = unidadesVendidas.ToString("#,##0");
        decimal? margen = ventaBLL.ObtenerMargenTotal(CodigoLocalActual, pPeriodoDesde);
        litMargenTotal.Text = margen == null ? "-" : margen.Value.ToString("0.#") + "%";
    }
    private void CargarGraficoSectores(string pPeriodoDesde)
    {
        var porSector = ventaBLL.ObtenerVentasAgrupadas(CodigoLocalActual, pPeriodoDesde, VentaDAL.AGRUPAR_SECTOR)
            .GroupBy(v => v.Grupo)
            .Select(g => new { Sector = MostrarGrupo(g.Key), Monto = g.Sum(v => v.Monto) })
            .OrderByDescending(s => s.Monto).ToList();
        decimal total = porSector.Sum(s => s.Monto);
        if (porSector.Count == 0 || total <= 0)
        {
            litGraficoSectores.Text = "<p class='sin-resultados'>" + HttpUtility.HtmlEncode(Traducir("stock.sinVentas")) + "</p>";
            return;
        }
        decimal maximo = porSector.Max(s => s.Monto);
        var sb = new StringBuilder("<div class='sv-barras'>");
        foreach (var sector in porSector)
        {
            decimal ancho = maximo > 0 ? Math.Round(sector.Monto / maximo * 100, 1) : 0;
            decimal participacion = Math.Round(sector.Monto / total * 100, 1);
            sb.Append("<div class='sv-barra-fila'>")
              .Append("<span class='sv-barra-nombre'>").Append(HttpUtility.HtmlEncode(sector.Sector)).Append("</span>")
              .Append("<span class='sv-barra-pista'><span class='sv-barra' style='width:")
              .Append(ancho.ToString(System.Globalization.CultureInfo.InvariantCulture)).Append("%'></span></span>")
              .Append("<span class='sv-barra-valor'>").Append(sector.Monto.ToString("$#,##0"))
              .Append(" <span class='texto-secundario'>(").Append(participacion.ToString("0.#")).Append("%)</span></span>")
              .Append("</div>");
        }
        sb.Append("</div>");
        litGraficoSectores.Text = sb.ToString();
    }
    private void CargarDetalleVentas(string pPeriodoDesde)
    {
        switch (Agrupacion)
        {
            case VentaDAL.AGRUPAR_CATEGORIA: litColGrupo.Text = HttpUtility.HtmlEncode(Traducir("productos.colCategoria")); break;
            case VentaDAL.AGRUPAR_PRODUCTO: litColGrupo.Text = HttpUtility.HtmlEncode(Traducir("productos.colProducto")); break;
            default: litColGrupo.Text = HttpUtility.HtmlEncode(Traducir("layout.colSector")); break;
        }
        List<VentaListado> ventas = ventaBLL.ObtenerVentasAgrupadas(CodigoLocalActual, pPeriodoDesde, Agrupacion);
        rptVentas.Visible = ventas.Count > 0;
        lblSinVentas.Visible = ventas.Count == 0;
        rptVentas.DataSource = ventas;
        rptVentas.DataBind();
    }
    private void CargarUltimasVentas()
    {
        List<VentaDetalle> ventas = ventaBLL.ObtenerUltimasVentas(CodigoLocalActual, CANTIDAD_ULTIMAS_VENTAS);
        rptUltimasVentas.Visible = ventas.Count > 0;
        lblSinUltimasVentas.Visible = ventas.Count == 0;
        rptUltimasVentas.DataSource = ventas;
        rptUltimasVentas.DataBind();
    }
    private void CargarStock()
    {
        List<StockListado> stock = stockBLL.ObtenerStockPorLocal(CodigoLocalActual);
        rptStock.Visible = stock.Count > 0;
        lblSinStock.Visible = stock.Count == 0;
        rptStock.DataSource = stock;
        rptStock.DataBind();
    }
    private void CargarImportaciones()
    {
        List<ImportacionBE> importaciones = importacionBLL.ObtenerImportacionesPorLocal(CodigoLocalActual);
        rptImportaciones.Visible = importaciones.Count > 0;
        lblSinImportaciones.Visible = importaciones.Count == 0;
        rptImportaciones.DataSource = importaciones;
        rptImportaciones.DataBind();
    }
    protected string MostrarGrupo(object pGrupo)
    {
        string grupo = pGrupo as string;
        return string.IsNullOrEmpty(grupo) ? Traducir("productos.sinUbicar") : grupo;
    }
    protected string MostrarOrigen(object pCodigoImportacion)
    {
        return pCodigoImportacion == null ? Traducir("stock.origenManual") : Traducir("stock.origenImportacion", pCodigoImportacion);
    }
    private void MostrarMensaje(string texto, bool esExito)
    {
        lblMensaje.Text = texto;
        lblMensaje.CssClass = esExito ? "mensaje" : "mensaje mensaje-error";
        lblMensaje.Visible = true;
    }
    private void MostrarErrorCarga(string pTexto)
    {
        lblMensajeCarga.Text = pTexto;
        lblMensajeCarga.Visible = true;
        pnlModalCarga.Style["display"] = "flex";
    }
    private void MostrarErroresImportacion(List<ErrorImportacion> pErrores)
    {
        var sb = new StringBuilder();
        sb.Append("<strong>").Append(HttpUtility.HtmlEncode(Traducir("importar.tituloErrores"))).Append("</strong><ul class='sv-errores'>");
        foreach (ErrorImportacion error in pErrores.Take(ERRORES_A_MOSTRAR))
        {
            string texto = Traducir(error.Clave, error.Parametros);
            if (error.Fila > 0) texto = Traducir("importar.fila", error.Fila) + ": " + texto;
            sb.Append("<li>").Append(HttpUtility.HtmlEncode(texto)).Append("</li>");
        }
        sb.Append("</ul>");
        if (pErrores.Count > ERRORES_A_MOSTRAR)
        {
            sb.Append("<span>").Append(HttpUtility.HtmlEncode(Traducir("importar.masErrores", pErrores.Count - ERRORES_A_MOSTRAR))).Append("</span>");
        }
        litErroresImportacion.Text = sb.ToString();
        pnlErroresImportacion.Visible = true;
        pnlModalImportar.Style["display"] = "flex";
    }
    #endregion

    protected void btnImportar_Click(object sender, EventArgs e)
    {
        // Si se está mirando el stock, el modal se abre con "Stock" elegido
        ddlTipoImportacion.SelectedValue = hfPestania.Value == "stock" ? ImportacionBE.TIPO_STOCK : ImportacionBE.TIPO_VENTA;
        pnlErroresImportacion.Visible = false;
        pnlModalImportar.Style["display"] = "flex";
    }
    protected void btnConfirmarImportar_Click(object sender, EventArgs e)
    {
        if (CodigoLocalActual == 0)
        {
            MostrarErroresImportacion(new List<ErrorImportacion> { new ErrorImportacion(0, "stock.errSeleccionarLocal") });
            return;
        }
        if (!fuArchivo.HasFile)
        {
            MostrarErroresImportacion(new List<ErrorImportacion> { new ErrorImportacion(0, "importar.errSinArchivo") });
            return;
        }
        string tipo = ddlTipoImportacion.SelectedValue;
        ResultadoImportacion resultado;
        try
        {
            resultado = importacionBLL.Importar(CodigoLocalActual, tipo, fuArchivo.FileName, fuArchivo.PostedFile.InputStream);
        }
        catch (ExcepcionTraducible error)
        {
            MostrarErroresImportacion(new List<ErrorImportacion> { new ErrorImportacion(0, error.Clave, error.Parametros) });
            return;
        }
        catch (Exception error) when (!(error is UnauthorizedAccessException))
        {
            // Error de base de datos al guardar: la transacción se deshizo, no quedó nada a medias
            MostrarErroresImportacion(new List<ErrorImportacion> { new ErrorImportacion(0, "error.inesperado") });
            return;
        }
        if (!resultado.Exito)
        {
            MostrarErroresImportacion(resultado.Errores);
            return;
        }
        pnlModalImportar.Style["display"] = "none";
        bool esVenta = tipo == ImportacionBE.TIPO_VENTA;
        string mensaje = Traducir(esVenta ? "importar.msgVentas" : "importar.msgStock", resultado.FilasImportadas, resultado.CodigoImportacion);
        if (resultado.FilasSinUbicar > 0) mensaje += " " + Traducir("importar.msgSinUbicar", resultado.FilasSinUbicar);
        MostrarMensaje(mensaje, esExito: true);
        hfPestania.Value = esVenta ? "ventas" : "stock";
        ActualizarVistas();
    }
    protected void btnCargaManual_Click(object sender, EventArgs e)
    {
        hfCodigoStockEdicion.Value = "";
        ddlTipoCarga.SelectedValue = hfPestania.Value == "stock" ? "Stock" : "Venta";
        ddlTipoCarga.Enabled = true;
        ddlProductoCarga.Enabled = true;
        if (ddlProductoCarga.Items.Count > 0) ddlProductoCarga.SelectedIndex = 0;
        tbCantidadCarga.Text = "";
        tbImporteCarga.Text = "";
        tbMinimoCarga.Text = "";
        tbMaximoCarga.Text = "";
        tbFechaCarga.Text = DateTime.Today.ToString("yyyy-MM-dd");
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
            MostrarErrorCarga(Traducir("stock.errSeleccionarLocal"));
            return;
        }
        int cantidad;
        if (!int.TryParse(tbCantidadCarga.Text.Trim(), out cantidad))
        {
            MostrarErrorCarga(Traducir("comun.datosInvalidos"));
            return;
        }
        DateTime fecha;
        if (!DateTime.TryParse(tbFechaCarga.Text, out fecha))
        {
            MostrarErrorCarga(Traducir("comun.completeDatos"));
            return;
        }
        int codigoProducto = int.Parse(ddlProductoCarga.SelectedValue);
        try
        {
            if (ddlTipoCarga.SelectedValue == "Venta")
            {
                // Importe vacío: la BLL usa precio del producto x cantidad
                ventaBLL.CrearVenta(CodigoLocalActual, codigoProducto, fecha, cantidad, LeerDecimal(tbImporteCarga.Text));
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
                StockBE stock = new StockBE(esEdicion ? int.Parse(hfCodigoStockEdicion.Value) : 0, CodigoLocalActual, codigoProducto, cantidad, minimo, maximo, fecha.ToString("yyyy-MM"), DateTime.Now);
                stockBLL.GuardarStock(stock);
            }
        }
        catch (ExcepcionTraducible error)
        {
            MostrarErrorCarga(TraducirError(error));
            return;
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
            tbFechaCarga.Text = DateTime.Today.ToString("yyyy-MM-dd");
            pnlModalCarga.Style["display"] = "flex";
        }
    }
    protected void rptUltimasVentas_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName != "EliminarVenta") return;
        ventaBLL.EliminarVenta(int.Parse(e.CommandArgument.ToString()));
        MostrarMensaje(Traducir("stock.msgVentaEliminada"), esExito: true);
        hfPestania.Value = "ventas";
        ActualizarVistas();
    }
    protected void rptImportaciones_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName != "Deshacer") return;
        hfPestania.Value = "importaciones";
        try
        {
            int borradas = importacionBLL.DeshacerImportacion(int.Parse(e.CommandArgument.ToString()));
            MostrarMensaje(Traducir("importar.msgDeshecha", borradas), esExito: true);
        }
        catch (ExcepcionTraducible error)
        {
            MostrarMensaje(TraducirError(error), esExito: false);
        }
        ActualizarVistas();
    }
}