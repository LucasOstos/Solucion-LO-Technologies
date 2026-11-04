using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BE;
using BLL;
using SERVICIO.Logica;

public partial class PaginaProductos : PaginaSegura
{
    protected override string PermisoRequerido
    {
        get { return PermisoNombres.GestionarProductos; }
    }
    protected override string ClaveTitulo
    {
        get { return "productos.titulo"; }
    }
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
        ddlFiltroSector.Items.Add(new ListItem(Traducir("productos.todosSectores"), ""));
        foreach (SectorListado sector in sectores)
        {
            ddlFiltroSector.Items.Add(new ListItem(sector.Nombre, sector.CodigoSector.ToString()));
        }
        CargarSectoresProducto(sectores);
    }
    private void CargarSectoresProducto(List<SectorListado> pSectores)
    {
        ddlSectorProducto.Items.Clear();
        ddlSectorProducto.Items.Add(new ListItem(Traducir("productos.sinSector"), ""));
        foreach (SectorListado sector in pSectores)
        {
            ddlSectorProducto.Items.Add(new ListItem(sector.Nombre, sector.CodigoSector.ToString()));
        }
    }
    private void CargarDropdownCategorias()
    {
        List<CategoriaBE> categorias = categoriaBLL.ObtenerCategorias(CodigoLocalActual);
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
        List<ProductoListado> productos = productoBLL.ObtenerProductosListado(CodigoLocalActual, busqueda, codigoSector);
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
            MostrarMensaje(Traducir("productos.msgNoExiste"), esExito: false);
            CargarProductos();
            return;
        }
        // Se recargan los desplegables del modal para descartar opciones agregadas en una edición anterior
        CargarSectoresProducto(CodigoLocalActual > 0 ? sectorBLL.ObtenerSectoresPorLocal(CodigoLocalActual) : new List<SectorListado>());
        CargarDropdownCategorias();
        hfCodigoProducto.Value = producto.CodigoProducto.ToString();
        hfNumeroSerie.Value = producto.NumeroSerie.ToString();
        litTituloModalProducto.Text = Traducir("productos.modalEditar");
        btnGuardarProducto.Text = Traducir("comun.guardarCambios");
        tbCodigoInterno.Text = $"PROD-{producto.NumeroSerie:D3}";
        tbNombreProducto.Text = producto.Nombre;
        tbDescripcionProducto.Text = producto.Descripcion;
        tbPrecio.Text = MostrarDecimal(producto.Precio);
        tbCosto.Text = MostrarDecimal(producto.Costo);
        SeleccionarCategoria(producto.CodigoCategoria);
        // El sector depende del local: es la ubicación del producto en el local elegido
        int? sectorEnLocal = productoBLL.ObtenerSectorEnLocal(producto.CodigoProducto, CodigoLocalActual);
        ddlSectorProducto.SelectedValue = sectorEnLocal?.ToString() ?? "";
        MostrarEtiquetaSector();
        chkActivoProducto.Checked = producto.Estado;
        pnlModalProducto.Style["display"] = "flex";
    }
    private void MostrarEtiquetaSector()
    {
        string local = ddlLocal.SelectedItem == null ? "" : ddlLocal.SelectedItem.Text;
        lblSectorProducto.Text = Traducir("productos.sectorEnLocal", local);
    }
    // Una categoría inactiva no está en el desplegable: se agrega como opción para que no se pierda al guardar
    private void SeleccionarCategoria(int pCodigoCategoria)
    {
        string valor = pCodigoCategoria.ToString();
        if (ddlCategoriaProducto.Items.FindByValue(valor) == null)
        {
            CategoriaBE categoria = categoriaBLL.ObtenerCategoriaPorCodigo(pCodigoCategoria);
            string nombre = categoria == null ? valor : categoria.Nombre;
            ddlCategoriaProducto.Items.Add(new ListItem(Traducir("productos.categoriaInactiva", nombre), valor));
        }
        ddlCategoriaProducto.SelectedValue = valor;
    }
    private void CargarCategorias()
    {
        List<CategoriaBE> categorias = categoriaBLL.ObtenerCategorias(CodigoLocalActual);
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
        if(CodigoLocalActual == 0)
        {
            MostrarMensaje(Traducir("productos.errSinLocalProductos"), esExito: false);
            return;
        }
        hfCodigoProducto.Value = "";
        litTituloModalProducto.Text = Traducir("productos.btnNuevo");
        btnGuardarProducto.Text = Traducir("comun.guardar");
        int proximoNumero = productoBLL.ObtenerProximoNumeroSerie(CodigoLocalActual);
        hfNumeroSerie.Value = proximoNumero.ToString();
        tbCodigoInterno.Text = $"PROD-{proximoNumero:D3}";
        tbNombreProducto.Text = "";
        tbDescripcionProducto.Text = "";
        tbPrecio.Text = "";
        tbCosto.Text = "";
        CargarSectoresProducto(CodigoLocalActual > 0 ? sectorBLL.ObtenerSectoresPorLocal(CodigoLocalActual) : new List<SectorListado>());
        CargarDropdownCategorias();
        if (ddlCategoriaProducto.Items.Count > 0) ddlCategoriaProducto.SelectedIndex = 0;
        ddlSectorProducto.SelectedIndex = 0;
        MostrarEtiquetaSector();
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
        decimal? precio = LeerDecimal(tbPrecio.Text);
        decimal? costo = LeerDecimal(tbCosto.Text);
        int? codigoSector = string.IsNullOrEmpty(ddlSectorProducto.SelectedValue) ? (int?)null : int.Parse(ddlSectorProducto.SelectedValue);
        bool esEdicion = !string.IsNullOrEmpty(hfCodigoProducto.Value);
        ProductoBE producto = new ProductoBE(esEdicion ? int.Parse(hfCodigoProducto.Value) : 0, int.Parse(ddlCategoriaProducto.SelectedValue), int.Parse(hfNumeroSerie.Value),
                                             tbNombreProducto.Text.Trim(), tbDescripcionProducto.Text.Trim(), precio, chkActivoProducto.Checked, costo);
        if (esEdicion)
        {
            productoBLL.ModificarProducto(producto, CodigoLocalActual, codigoSector);
        }
        else
        {
            productoBLL.CrearProducto(producto, CodigoLocalActual, codigoSector);
        }
        pnlModalProducto.Style["display"] = "none";
        MostrarMensaje(Traducir(esEdicion ? "productos.msgModificado" : "productos.msgCreado"), esExito: true);
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
                MostrarMensaje(Traducir("productos.msgEstado"), esExito: true);
                CargarProductos();
                break;
        }
    }
    protected void btnNuevaCategoria_Click(object sender, EventArgs e)
    {
        if (CodigoLocalActual == 0)
        {
            MostrarMensaje(Traducir("productos.errSinLocalCategorias"), esExito: false);
            return;
        }
        hfCodigoCategoria.Value = "";
        litTituloModalCategoria.Text = Traducir("productos.btnNuevaCategoria");
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
            categoriaBLL.CrearCategoria(categoria, CodigoLocalActual);
        }
        pnlModalCategoria.Style["display"] = "none";
        MostrarMensaje(Traducir(esEdicion ? "productos.msgCategoriaModificada" : "productos.msgCategoriaCreada"), esExito: true);
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
            litTituloModalCategoria.Text = Traducir("productos.modalCategoriaEditar");
            tbNombreCategoria.Text = categoria.Nombre;
            tbDescripcionCategoria.Text = categoria.Descripcion;
            chkActivoCategoria.Checked = categoria.Estado;
            pnlModalCategoria.Style["display"] = "flex";
        }
    }
    protected void ddlLocal_SelectedIndexChanged(object sender, EventArgs e)
    {
        CargarDropdownSectores();
        CargarDropdownCategorias();
        CargarCategorias();
        CargarProductos();
    }
}