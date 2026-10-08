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
        pDesplegable.Items.Add(new ListItem("— Seleccionar —", ""));
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
                pLista.Items.Add(new ListItem("Permiso: " + hijo.Nombre, PREFIJO_PERMISO + hijo.Codigo));
            }
            else if (hijo is Familia familia)
            {
                int cantidad = familia.ObtenerPermisosSimples().Count;
                pLista.Items.Add(new ListItem($"Familia: {familia.Nombre} ({cantidad} permisos)", PREFIJO_FAMILIA + familia.Codigo));
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
    private bool Intentar(Action pOperacion, string pMensajeExito)
    {
        try
        {
            pOperacion();
            MostrarMensaje(pMensajeExito, esExito: true);
            return true;
        }
        catch (InvalidOperationException ex)
        {
            MostrarMensaje(ex.Message, esExito: false);
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
        bool exito = Intentar(() => codigoNuevo = permisoBLL.CrearPerfil(tbNombrePerfil.Text, CodigosMarcados(cblPermisosDisponibles), CodigosMarcados(cblFamiliasDisponiblesEnPerfil)), "Perfil creado correctamente.");
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
            MostrarMensaje("Elegí un perfil en la lista 'Perfiles' para agregarle permisos.", esExito: false);
            return;
        }
        if (Intentar(() => permisoBLL.AgregarAPerfil(codPerfil.Value, CodigosMarcados(cblPermisosDisponibles), CodigosMarcados(cblFamiliasDisponiblesEnPerfil)), "Permisos agregados al perfil."))
        {
            CargarPantalla();
        }
    }
    protected void btnQuitarPermisosPerfil_Click(object sender, EventArgs e)
    {
        int? codPerfil = CodigoSeleccionado(ddlPerfiles);
        if (codPerfil == null)
        {
            MostrarMensaje("Elegí un perfil en la lista 'Perfiles' para quitarle permisos.", esExito: false);
            return;
        }
        List<int> permisos, familias;
        LeerSeleccionContenido(cblContenidoPerfil, out permisos, out familias);
        if (Intentar(() => permisoBLL.QuitarDePerfil(codPerfil.Value, permisos, familias), "Permisos quitados del perfil."))
        {
            CargarPantalla();
        }
    }
    protected void btnBorrarPerfil_Click(object sender, EventArgs e)
    {
        int? codPerfil = CodigoSeleccionado(ddlPerfiles);
        if (codPerfil == null)
        {
            MostrarMensaje("Elegí el perfil que querés borrar en la lista 'Perfiles'.", esExito: false);
            return;
        }
        if (Intentar(() => permisoBLL.EliminarPerfil(codPerfil.Value), "Perfil borrado correctamente."))
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
        bool exito = Intentar(() => codigoNuevo = permisoBLL.CrearFamilia(tbNombreFamilia.Text, CodigosMarcados(cblPermisosDisponiblesEnFamilia), CodigosMarcados(cblFamiliasDisponibles)), "Familia creada correctamente.");
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
            MostrarMensaje("Elegí una familia en la lista 'Familias' para agregarle permisos.", esExito: false);
            return;
        }
        if (Intentar(() => permisoBLL.AgregarAFamilia(codFamilia.Value, CodigosMarcados(cblPermisosDisponiblesEnFamilia), CodigosMarcados(cblFamiliasDisponibles)), "Permisos agregados a la familia."))
        {
            CargarPantalla();
        }
    }
    protected void btnQuitarPermisosFamilia_Click(object sender, EventArgs e)
    {
        int? codFamilia = CodigoSeleccionado(ddlFamilias);
        if (codFamilia == null)
        {
            MostrarMensaje("Elegí una familia en la lista 'Familias' para quitarle permisos.", esExito: false);
            return;
        }
        List<int> permisos, familias;
        LeerSeleccionContenido(cblContenidoFamilia, out permisos, out familias);
        if (Intentar(() => permisoBLL.QuitarDeFamilia(codFamilia.Value, permisos, familias), "Permisos quitados de la familia."))
        {
            CargarPantalla();
        }
    }
    protected void btnBorrarFamilia_Click(object sender, EventArgs e)
    {
        int? codFamilia = CodigoSeleccionado(ddlFamilias);
        if (codFamilia == null)
        {
            MostrarMensaje("Elegí la familia que querés borrar en la lista 'Familias'.", esExito: false);
            return;
        }
        if (Intentar(() => permisoBLL.EliminarFamilia(codFamilia.Value), "Familia borrada correctamente."))
        {
            CargarPantalla(pFamiliaASeleccionar: "");
        }
    }
}