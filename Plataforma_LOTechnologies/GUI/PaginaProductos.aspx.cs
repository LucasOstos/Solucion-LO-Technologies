using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BE;
using BLL;
using SERVICIO.Logica;

public partial class PaginaProductos : System.Web.UI.Page
{
    private LocalBLL localBLL = new LocalBLL();
    private SectorBLL sectorBLL = new SectorBLL();
    private CategoriaBLL categoriaBLL = new CategoriaBLL();
    private ProductoBLL productoBLL = new ProductoBLL();
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
            CargarDropdownSectores();
            CargarDropdownCategorias();
            CargarProductos();
            CargarCategorias();
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
    private void CargarDropdownSectores()
    {
        List<SectorListado> sectores = CodigoLocalActual > 0 ? sectorBLL.ObtenerSectoresPorLocal(CodigoLocalActual) : new List<SectorListado>();
        ddlFiltroSector.Items.Clear();
        ddlFiltroSector.Items.Add(new ListItem("Todos los sectores", ""));
        ddlSectorProducto.Items.Clear();
        ddlSectorProducto.Items.Add(new ListItem("— Sin sector asignado —", ""));
        foreach (SectorListado sector in sectores)
        {
            ddlFiltroSector.Items.Add(new ListItem(sector.Nombre, sector.CodigoSector.ToString()));
            ddlSectorProducto.Items.Add(new ListItem(sector.Nombre, sector.CodigoSector.ToString()));
        }
    }
    private void CargarDropdownCategorias()
    {
        List<CategoriaBE> categorias = categoriaBLL.ObtenerCategorias();
        ddlCategoriaProducto.Items.Clear();
        foreach (CategoriaBE categoria in categorias)
        {
            ddlCategoriaProducto.Items.Add(new ListItem(categoria.Nombre, categoria.CodigoCategoria.ToString()));
        }
    }
    private void CargarProductos()
    {
        string busqueda = tbBuscar.Text.Trim();
        int? codigoSector = string.IsNullOrEmpty(ddlFiltroSector.SelectedValue) ? (int?)null : int.Parse(ddlFiltroSector.SelectedValue);
        List<ProductoListado> productos = productoBLL.ObtenerProductosListado(busqueda, codigoSector);
        if (productos.Count == 0)
        {
            rptProductos.Visible = false;
            lblSinProductos.Visible = true;
        }
        else
        {
            rptProductos.Visible = true;
            lblSinProductos.Visible = false;
            rptProductos.DataSource = productos;
            rptProductos.DataBind();
        }
    }
    private void CargarProductoEnModal(int codigoProducto)
    {
        ProductoBE producto = productoBLL.ObtenerProductoPorCodigo(codigoProducto);
        if (producto == null)
        {
            MostrarMensaje("El producto ya no existe.", esExito: false);
            CargarProductos();
            return;
        }
        hfCodigoProducto.Value = producto.CodigoProducto.ToString();
        hfNumeroSerie.Value = producto.NumeroSerie.ToString();
        litTituloModalProducto.Text = "Editar Producto";
        btnGuardarProducto.Text = "Guardar Cambios";
        tbCodigoInterno.Text = $"PROD-{producto.NumeroSerie:D3}";
        tbNombreProducto.Text = producto.Nombre;
        tbDescripcionProducto.Text = producto.Descripcion;
        tbPrecio.Text = producto.Precio?.ToString() ?? "";
        ddlCategoriaProducto.SelectedValue = producto.CodigoCategoria.ToString();
        ddlSectorProducto.SelectedValue = producto.CodigoSector?.ToString() ?? "";
        chkActivoProducto.Checked = producto.Estado;
        pnlModalProducto.Style["display"] = "flex";
    }
    private void CargarCategorias()
    {
        List<CategoriaBE> categorias = categoriaBLL.ObtenerCategorias();
        if (categorias.Count == 0)
        {
            rptCategorias.Visible = false;
            lblSinCategorias.Visible = true;
        }
        else
        {
            rptCategorias.Visible = true;
            lblSinCategorias.Visible = false;
            rptCategorias.DataSource = categorias;
            rptCategorias.DataBind();
        }
    }
    private void MostrarMensaje(string texto, bool esExito)
    {
        lblMensaje.Text = texto;
        lblMensaje.CssClass = esExito ? "mensaje" : "mensaje mensaje-error";
        lblMensaje.Visible = true;
    }
    #endregion

    protected void btnNuevo_Click(object sender, EventArgs e)
    {
        hfCodigoProducto.Value = "";
        litTituloModalProducto.Text = "Nuevo Producto";
        btnGuardarProducto.Text = "Guardar";
        int proximoNumero = productoBLL.ObtenerProximoNumeroSerie();
        hfNumeroSerie.Value = proximoNumero.ToString();
        tbCodigoInterno.Text = $"PROD-{proximoNumero:D3}";
        tbNombreProducto.Text = "";
        tbDescripcionProducto.Text = "";
        tbPrecio.Text = "";
        if (ddlCategoriaProducto.Items.Count > 0) ddlCategoriaProducto.SelectedIndex = 0;
        ddlSectorProducto.SelectedIndex = 0;
        chkActivoProducto.Checked = true;
        pnlModalProducto.Style["display"] = "flex";
    }
    protected void btnBuscar_Click(object sender, EventArgs e) { CargarProductos(); }
    protected void Filtro_Changed(object sender, EventArgs e) { CargarProductos(); }
    protected void btnCancelarProducto_Click(object sender, EventArgs e) { pnlModalProducto.Style["display"] = "none"; }
    protected void btnGuardarProducto_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid)
        {
            pnlModalProducto.Style["display"] = "flex";
            return;
        }
        decimal? precio = null;
        decimal precioParsed;
        if (decimal.TryParse(tbPrecio.Text.Trim(), out precioParsed))
        {
            precio = precioParsed;
        }
        int? codigoSector = string.IsNullOrEmpty(ddlSectorProducto.SelectedValue) ? (int?)null : int.Parse(ddlSectorProducto.SelectedValue);
        bool esEdicion = !string.IsNullOrEmpty(hfCodigoProducto.Value);
        ProductoBE producto = new ProductoBE(esEdicion ? int.Parse(hfCodigoProducto.Value) : 0, int.Parse(ddlCategoriaProducto.SelectedValue), codigoSector, int.Parse(hfNumeroSerie.Value),
                                             tbNombreProducto.Text.Trim(), tbDescripcionProducto.Text.Trim(), precio, chkActivoProducto.Checked);        
        if (esEdicion)
        {
            productoBLL.ModificarProducto(producto);
        }
        else
        {
            productoBLL.CrearProducto(producto);
        }
        pnlModalProducto.Style["display"] = "none";
        MostrarMensaje(esEdicion ? "Producto modificado correctamente." : "Producto creado correctamente.", esExito: true);
        CargarProductos();
    }
    protected void rptProductos_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int codigoProducto = int.Parse(e.CommandArgument.ToString());
        switch (e.CommandName)
        {
            case "EditarProducto":
                CargarProductoEnModal(codigoProducto);
                break;
            case "CambiarEstado":
                ProductoBE producto = productoBLL.ObtenerProductoPorCodigo(codigoProducto);
                productoBLL.CambiarEstado(codigoProducto, !producto.Estado);
                MostrarMensaje("Estado del producto actualizado.", esExito: true);
                CargarProductos();
                break;
        }
    }
    protected void btnNuevaCategoria_Click(object sender, EventArgs e)
    {
        hfCodigoCategoria.Value = "";
        litTituloModalCategoria.Text = "Nueva Categoría";
        tbNombreCategoria.Text = "";
        tbDescripcionCategoria.Text = "";
        chkActivoCategoria.Checked = true;
        pnlModalCategoria.Style["display"] = "flex";
    }
    protected void btnCancelarCategoria_Click(object sender, EventArgs e) { pnlModalCategoria.Style["display"] = "none"; }
    protected void btnGuardarCategoria_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid)
        {
            pnlModalCategoria.Style["display"] = "flex";
            return;
        }
        bool esEdicion = !string.IsNullOrEmpty(hfCodigoCategoria.Value);
        CategoriaBE categoria = new CategoriaBE(esEdicion ? int.Parse(hfCodigoCategoria.Value) : 0, tbNombreCategoria.Text.Trim(), tbDescripcionCategoria.Text.Trim(), chkActivoCategoria.Checked);        
        if (esEdicion)
        {
            categoriaBLL.ModificarCategoria(categoria);
        }
        else
        {
            categoriaBLL.CrearCategoria(categoria);
        }
        pnlModalCategoria.Style["display"] = "none";
        MostrarMensaje(esEdicion ? "Categoría modificada correctamente." : "Categoría creada correctamente.", esExito: true);
        CargarCategorias();
        CargarDropdownCategorias();
    }
    protected void rptCategorias_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "EditarCategoria")
        {
            int codigoCategoria = int.Parse(e.CommandArgument.ToString());
            CategoriaBE categoria = categoriaBLL.ObtenerCategoriaPorCodigo(codigoCategoria);
            hfCodigoCategoria.Value = categoria.CodigoCategoria.ToString();
            litTituloModalCategoria.Text = "Editar Categoría";
            tbNombreCategoria.Text = categoria.Nombre;
            tbDescripcionCategoria.Text = categoria.Descripcion;
            chkActivoCategoria.Checked = categoria.Estado;
            pnlModalCategoria.Style["display"] = "flex";
        }
    }
    protected void ddlLocal_SelectedIndexChanged(object sender, EventArgs e)
    {
        CargarDropdownSectores();
        CargarProductos();
    }
}