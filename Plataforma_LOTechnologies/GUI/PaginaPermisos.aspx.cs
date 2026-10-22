using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BE;
using BLL;

public partial class PaginaPermisos : PaginaSegura
{
    protected override string PermisoRequerido
    {
        get { return PermisoNombres.GestionarPermisos; }
    }
    protected override string ClaveTitulo
    {
        get { return "permisos.titulo"; }
    }
    private PermisoBLL permisoBLL = new PermisoBLL();
    private const string PREFIJO_PERMISO = "P:";
    private const string PREFIJO_FAMILIA = "F:";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CargarPantalla();
        }
    }
    #region Funciones
    private void CargarPantalla(string pPerfilASeleccionar = null, string pFamiliaASeleccionar = null)
    {
        PermisosDatos estructura = permisoBLL.ObtenerEstructura();
        CargarDesplegable(ddlPerfiles, estructura.Perfiles, pPerfilASeleccionar ?? ddlPerfiles.SelectedValue);
        CargarDesplegable(ddlFamilias, estructura.Familias, pFamiliaASeleccionar ?? ddlFamilias.SelectedValue);
        int? codPerfil = CodigoSeleccionado(ddlPerfiles);
        int? codFamilia = CodigoSeleccionado(ddlFamilias);
        CargarChecks(cblPermisosDisponibles, estructura.Permisos);
        CargarChecks(cblFamiliasDisponiblesEnPerfil, estructura.Familias);
        CargarChecks(cblPermisosDisponiblesEnFamilia, estructura.Permisos);
        CargarChecks(cblFamiliasDisponibles, estructura.Familias.Where(f => f.Codigo != codFamilia));
        CargarContenido(cblContenidoPerfil, estructura.Perfiles.FirstOrDefault(p => p.Codigo == codPerfil));
        CargarContenido(cblContenidoFamilia, estructura.Familias.FirstOrDefault(f => f.Codigo == codFamilia));
    }
    private void CargarDesplegable(DropDownList pDesplegable, IEnumerable<Permiso> pItems, string pValorASeleccionar)
    {
        pDesplegable.Items.Clear();
        pDesplegable.Items.Add(new ListItem(Traducir("comun.seleccionar"), ""));
        foreach (Permiso item in pItems)
        {
            pDesplegable.Items.Add(new ListItem(item.Nombre, item.Codigo.ToString()));
        }
        if (!string.IsNullOrEmpty(pValorASeleccionar) && pDesplegable.Items.FindByValue(pValorASeleccionar) != null)
        {
            pDesplegable.SelectedValue = pValorASeleccionar;
        }
    }
    private void CargarChecks(CheckBoxList pLista, IEnumerable<Permiso> pItems)
    {
        pLista.Items.Clear();
        foreach (Permiso item in pItems)
        {
            pLista.Items.Add(new ListItem(item.Nombre, item.Codigo.ToString()));
        }
    }
    private void CargarContenido(ListControl pLista, PermisoCompuesto pCompuesto)
    {
        pLista.Items.Clear();
        if (pCompuesto == null) return;
        foreach (Permiso hijo in pCompuesto.ObtenerHijos())
        {
            if (hijo is PermisoSimple)
            {
                pLista.Items.Add(new ListItem(Traducir("permisos.itemPermiso", hijo.Nombre), PREFIJO_PERMISO + hijo.Codigo));
            }
            else if (hijo is Familia familia)
            {
                int cantidad = familia.ObtenerPermisosSimples().Count;
                pLista.Items.Add(new ListItem(Traducir("permisos.itemFamilia", familia.Nombre, cantidad), PREFIJO_FAMILIA + familia.Codigo));
            }
        }
    }
    private int? CodigoSeleccionado(DropDownList pDesplegable)
    {
        int codigo;
        return int.TryParse(pDesplegable.SelectedValue, out codigo) ? codigo : (int?)null;
    }
    private List<int> CodigosMarcados(CheckBoxList pLista)
    {
        return pLista.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => int.Parse(i.Value)).ToList();
    }
    private void LeerSeleccionContenido(ListControl pLista, out List<int> pCodPermisos, out List<int> pCodFamilias)
    {
        pCodPermisos = new List<int>();
        pCodFamilias = new List<int>();
        foreach (ListItem item in pLista.Items)
        {
            if (!item.Selected) continue;
            if (item.Value.StartsWith(PREFIJO_PERMISO))
            {
                pCodPermisos.Add(int.Parse(item.Value.Substring(PREFIJO_PERMISO.Length)));
            }                
            else if (item.Value.StartsWith(PREFIJO_FAMILIA))
            {
                pCodFamilias.Add(int.Parse(item.Value.Substring(PREFIJO_FAMILIA.Length)));
            }                
        }
    }
    private bool Intentar(Action pOperacion, string pClaveExito)
    {
        try
        {
            pOperacion();
            MostrarMensaje(Traducir(pClaveExito), esExito: true);
            return true;
        }
        catch (InvalidOperationException ex)
        {
            MostrarMensaje(TraducirError(ex), esExito: false);
            return false;
        }
    }
    private void MostrarMensaje(string pTexto, bool esExito)
    {
        lblMensaje.Text = pTexto;
        lblMensaje.CssClass = esExito ? "mensaje" : "mensaje mensaje-error";
        lblMensaje.Visible = true;
    }
    #endregion
    protected void ddlPerfiles_SelectedIndexChanged(object sender, EventArgs e)
    {
        CargarPantalla();
    }
    protected void btnCrearPerfil_Click(object sender, EventArgs e)
    {
        int codigoNuevo = 0;
        bool exito = Intentar(() => codigoNuevo = permisoBLL.CrearPerfil(tbNombrePerfil.Text, CodigosMarcados(cblPermisosDisponibles), CodigosMarcados(cblFamiliasDisponiblesEnPerfil)), "permisos.msgPerfilCreado");
        if (exito)
        {
            tbNombrePerfil.Text = "";
            CargarPantalla(pPerfilASeleccionar: codigoNuevo.ToString());
        }
    }
    protected void btnAgregarPermisosPerfil_Click(object sender, EventArgs e)
    {
        int? codPerfil = CodigoSeleccionado(ddlPerfiles);
        if (codPerfil == null)
        {
            MostrarMensaje(Traducir("permisos.errElegirPerfilAgregar"), esExito: false);
            return;
        }
        if (Intentar(() => permisoBLL.AgregarAPerfil(codPerfil.Value, CodigosMarcados(cblPermisosDisponibles), CodigosMarcados(cblFamiliasDisponiblesEnPerfil)), "permisos.msgPermisosAgregadosPerfil"))
        {
            CargarPantalla();
        }
    }
    protected void btnQuitarPermisosPerfil_Click(object sender, EventArgs e)
    {
        int? codPerfil = CodigoSeleccionado(ddlPerfiles);
        if (codPerfil == null)
        {
            MostrarMensaje(Traducir("permisos.errElegirPerfilQuitar"), esExito: false);
            return;
        }
        List<int> permisos, familias;
        LeerSeleccionContenido(cblContenidoPerfil, out permisos, out familias);
        if (Intentar(() => permisoBLL.QuitarDePerfil(codPerfil.Value, permisos, familias), "permisos.msgPermisosQuitadosPerfil"))
        {
            CargarPantalla();
        }
    }
    protected void btnBorrarPerfil_Click(object sender, EventArgs e)
    {
        int? codPerfil = CodigoSeleccionado(ddlPerfiles);
        if (codPerfil == null)
        {
            MostrarMensaje(Traducir("permisos.errElegirPerfilBorrar"), esExito: false);
            return;
        }
        if (Intentar(() => permisoBLL.EliminarPerfil(codPerfil.Value), "permisos.msgPerfilBorrado"))
        {
            CargarPantalla(pPerfilASeleccionar: "");
        }
    }

    protected void ddlFamilias_SelectedIndexChanged(object sender, EventArgs e)
    {
        CargarPantalla();
    }
    protected void btnCrearFamilia_Click(object sender, EventArgs e)
    {
        int codigoNuevo = 0;
        bool exito = Intentar(() => codigoNuevo = permisoBLL.CrearFamilia(tbNombreFamilia.Text, CodigosMarcados(cblPermisosDisponiblesEnFamilia), CodigosMarcados(cblFamiliasDisponibles)), "permisos.msgFamiliaCreada");
        if (exito)
        {
            tbNombreFamilia.Text = "";
            CargarPantalla(pFamiliaASeleccionar: codigoNuevo.ToString());
        }
    }
    protected void btnAgregarPermisosFamilia_Click(object sender, EventArgs e)
    {
        int? codFamilia = CodigoSeleccionado(ddlFamilias);
        if (codFamilia == null)
        {
            MostrarMensaje(Traducir("permisos.errElegirFamiliaAgregar"), esExito: false);
            return;
        }
        if (Intentar(() => permisoBLL.AgregarAFamilia(codFamilia.Value, CodigosMarcados(cblPermisosDisponiblesEnFamilia), CodigosMarcados(cblFamiliasDisponibles)), "permisos.msgPermisosAgregadosFamilia"))
        {
            CargarPantalla();
        }
    }
    protected void btnQuitarPermisosFamilia_Click(object sender, EventArgs e)
    {
        int? codFamilia = CodigoSeleccionado(ddlFamilias);
        if (codFamilia == null)
        {
            MostrarMensaje(Traducir("permisos.errElegirFamiliaQuitar"), esExito: false);
            return;
        }
        List<int> permisos, familias;
        LeerSeleccionContenido(cblContenidoFamilia, out permisos, out familias);
        if (Intentar(() => permisoBLL.QuitarDeFamilia(codFamilia.Value, permisos, familias), "permisos.msgPermisosQuitadosFamilia"))
        {
            CargarPantalla();
        }
    }
    protected void btnBorrarFamilia_Click(object sender, EventArgs e)
    {
        int? codFamilia = CodigoSeleccionado(ddlFamilias);
        if (codFamilia == null)
        {
            MostrarMensaje(Traducir("permisos.errElegirFamiliaBorrar"), esExito: false);
            return;
        }
        if (Intentar(() => permisoBLL.EliminarFamilia(codFamilia.Value), "permisos.msgFamiliaBorrada"))
        {
            CargarPantalla(pFamiliaASeleccionar: "");
        }
    }
}