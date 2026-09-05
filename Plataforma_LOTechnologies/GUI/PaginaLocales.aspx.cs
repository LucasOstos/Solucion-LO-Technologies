using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BE;
using BLL;
using SERVICIO.Logica;

public partial class PaginaLocales : System.Web.UI.Page
{
    private LocalBLL localBLL = new LocalBLL();
    private EmpresaBLL empresaBLL = new EmpresaBLL();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Sesion.Instancia.IsLogueado())
        {
            Response.Redirect("PaginaLogin.aspx");
            return;
        }

        if (!IsPostBack)
        {
            CargarFiltroEmpresas();
            CargarLocales();
        }
    }

    #region Funciones
    private void CargarFiltroEmpresas()
    {
        List<EmpresaBE> empresas = empresaBLL.ObtenerEmpresasListado("", true);
        ddlFiltroEmpresa.Items.Clear();
        ddlFiltroEmpresa.Items.Add(new ListItem("Todas las empresas", ""));
        foreach (EmpresaBE emp in empresas)
        {
            ddlFiltroEmpresa.Items.Add(new ListItem(emp.RazonSocial, emp.CodigoEmpresa.ToString()));
        }
        ddlEmpresa.Items.Clear();
        foreach (EmpresaBE emp in empresas)
        {
            ddlEmpresa.Items.Add(new ListItem(emp.RazonSocial, emp.CodigoEmpresa.ToString()));
        }
    }
    private void CargarLocales()
    {
        string busqueda = tbBuscar.Text.Trim();
        int? codigoEmpresa = string.IsNullOrEmpty(ddlFiltroEmpresa.SelectedValue) ? (int?)null : int.Parse(ddlFiltroEmpresa.SelectedValue);
        string estado = ddlFiltroEstado.SelectedValue;
        List<LocalListado> locales = localBLL.ObtenerLocalesListado(busqueda, codigoEmpresa, estado);
        if (locales.Count == 0)
        {
            rptLocales.Visible = false;
            lblSinLocales.Visible = true;
        }
        else
        {
            rptLocales.Visible = true;
            lblSinLocales.Visible = false;
            rptLocales.DataSource = locales;
            rptLocales.DataBind();
        }
    }
    private void LimpiarModal()
    {
        hfCodigoLocal.Value = "";
        tbNombre.Text = "";
        tbDireccion.Text = "";
        tbSuperficie.Text = "";
        tbTelefono.Text = "";
        tbObservaciones.Text = "";
        if (ddlEmpresa.Items.Count > 0) ddlEmpresa.SelectedIndex = 0;
        ddlTipoLocal.SelectedIndex = 0;
        chkActivo.Checked = true;
    }
    private void AbrirModal()
    {
        pnlModalLocal.Style["display"] = "flex";
    }
    protected void btnNuevoLocal_Click(object sender, EventArgs e)
    {
        LimpiarModal();
        hfEsEdicion.Value = "false";
        litTituloModal.Text = "Nuevo Local";
        btnGuardarLocal.Text = "Crear Local";
        AbrirModal();
    }
    private void CargarLocalEnModal(int codigoLocal)
    {
        LocalBE local = localBLL.ObtenerLocalPorCodigo(codigoLocal);
        if (local == null)
        {
            MostrarMensaje("El local ya no existe.", esExito: false);
            CargarLocales();
            return;
        }
        hfEsEdicion.Value = "true";
        hfCodigoLocal.Value = local.CodigoLocal.ToString();
        litTituloModal.Text = "Editar Local";
        btnGuardarLocal.Text = "Guardar Cambios";
        tbNombre.Text = local.Nombre;
        ddlEmpresa.SelectedValue = local.CodigoEmpresa.ToString();
        tbDireccion.Text = local.Direccion;
        ddlTipoLocal.SelectedValue = string.IsNullOrEmpty(local.TipoLocal) ? "Local a la calle" : local.TipoLocal;
        tbSuperficie.Text = local.SuperficieAprox?.ToString() ?? "";
        tbTelefono.Text = local.Telefono;
        tbObservaciones.Text = local.Observaciones;
        chkActivo.Checked = local.Estado == "Activo";
        AbrirModal();
    }
    #endregion

    private void MostrarMensaje(string texto, bool esExito)
    {
        lblMensaje.Text = texto;
        lblMensaje.CssClass = esExito ? "mensaje" : "mensaje mensaje-error";
        lblMensaje.Visible = true;
    }
    protected void btnCancelarModal_Click(object sender, EventArgs e)
    {
        pnlModalLocal.Style["display"] = "none";
    }
    protected void btnGuardarLocal_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid)
        {
            AbrirModal();
            return;
        }
        decimal? superficie = null;
        decimal superficieParsed;
        if (decimal.TryParse(tbSuperficie.Text.Trim(), out superficieParsed)) { superficie = superficieParsed; }
        bool esEdicion = hfEsEdicion.Value == "true";
        LocalBE local = new LocalBE(esEdicion ? int.Parse(hfCodigoLocal.Value) : 0, int.Parse(ddlEmpresa.SelectedValue), tbNombre.Text.Trim(), tbDireccion.Text.Trim(), ddlTipoLocal.SelectedValue,
                                    superficie, tbTelefono.Text.Trim(), tbObservaciones.Text.Trim(), chkActivo.Checked ? "Activo" : "Inactivo");
        ResultadoGuardarLocal resultado = esEdicion ? localBLL.ModificarLocal(local) : localBLL.CrearLocal(local);

        switch (resultado)
        {
            case ResultadoGuardarLocal.Exitoso:
                pnlModalLocal.Style["display"] = "none";
                MostrarMensaje(esEdicion ? "Local modificado correctamente." : "Local registrado correctamente.", esExito: true);
                CargarLocales();
                break;
            case ResultadoGuardarLocal.DatosInvalidos:
                MostrarMensaje("Datos inválidos.", esExito: false);
                AbrirModal();
                break;
        }
    }
    protected void btnBuscar_Click(object sender, EventArgs e)
    {
        CargarLocales();
    }
    protected void Filtro_Changed(object sender, EventArgs e)
    {
        CargarLocales();
    }
    protected void rptLocales_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int codigoLocal = int.Parse(e.CommandArgument.ToString());
        switch (e.CommandName)
        {
            case "Editar":
                CargarLocalEnModal(codigoLocal);
                break;
            case "CambiarEstado":
                LocalBE local = localBLL.ObtenerLocalPorCodigo(codigoLocal);
                string nuevoEstado = local.Estado == "Activo" ? "Inactivo" : "Activo";
                localBLL.CambiarEstado(codigoLocal, nuevoEstado);
                MostrarMensaje("Estado del local actualizado.", esExito: true);
                CargarLocales();
                break;
            case "VerLayout":
                Response.Redirect($"PaginaLayoutSectores.aspx?local={codigoLocal}");
                break;
        }
    }
}