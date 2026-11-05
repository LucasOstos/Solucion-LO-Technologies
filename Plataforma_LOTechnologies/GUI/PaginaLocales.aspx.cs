using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BE;
using BLL;
using SERVICIO.Logica;

public partial class PaginaLocales : PaginaSegura
{
    protected override string PermisoRequerido
    {
        get { return PermisoNombres.GestionarLocales; }
    }
    protected override string ClaveTitulo
    {
        get { return "locales.titulo"; }
    }
    private LocalBLL localBLL = new LocalBLL();
    private EmpresaBLL empresaBLL = new EmpresaBLL();
    protected void Page_Load(object sender, EventArgs e)
    {        
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
        ddlFiltroEmpresa.Items.Add(new ListItem(Traducir("comun.todasEmpresas"), ""));
        foreach (EmpresaBE emp in empresas)
        {
            ddlFiltroEmpresa.Items.Add(new ListItem(emp.RazonSocial, emp.CodigoEmpresa.ToString()));
        }
        ddlEmpresa.Items.Clear();
        foreach (EmpresaBE emp in empresas)
        {
            ddlEmpresa.Items.Add(new ListItem(emp.RazonSocial, emp.CodigoEmpresa.ToString()));
        }
        bool esSuperUsuario = Sesion.Instancia.EsSuperUsuario;
        ddlFiltroEmpresa.Visible = esSuperUsuario;
        ddlEmpresa.Enabled = esSuperUsuario;
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
        litTituloModal.Text = Traducir("locales.btnNuevo");
        btnGuardarLocal.Text = Traducir("locales.btnCrear");
        AbrirModal();
    }
    private void CargarLocalEnModal(int codigoLocal)
    {
        LocalBE local = localBLL.ObtenerLocalPorCodigo(codigoLocal);
        if (local == null)
        {
            MostrarMensaje(Traducir("locales.msgNoExiste"), esExito: false);
            CargarLocales();
            return;
        }
        hfEsEdicion.Value = "true";
        hfCodigoLocal.Value = local.CodigoLocal.ToString();
        litTituloModal.Text = Traducir("locales.modalEditar");
        btnGuardarLocal.Text = Traducir("comun.guardarCambios");
        tbNombre.Text = local.Nombre;
        ddlEmpresa.SelectedValue = local.CodigoEmpresa.ToString();
        tbDireccion.Text = local.Direccion;
        ddlTipoLocal.SelectedValue = string.IsNullOrEmpty(local.TipoLocal) ? "Local a la calle" : local.TipoLocal;
        tbSuperficie.Text = MostrarDecimal(local.SuperficieAprox);
        tbTelefono.Text = local.Telefono;
        tbObservaciones.Text = local.Observaciones;
        chkActivo.Checked = local.Estado == "Activo";
        AbrirModal();
    }    
    private void MostrarErrorModal(string pTexto)
    {
        lblMensajeModal.Text = pTexto;
        lblMensajeModal.Visible = true;
        AbrirModal();
    }
    protected string MostrarSuperficie(object pSuperficie)
    {
        return pSuperficie == null ? HttpUtility.HtmlEncode(Traducir("locales.sinSuperficie")) : Convert.ToDecimal(pSuperficie).ToString("#,##0.##") + " m²";
    }
    protected string ArmarDatosAnalisis(object pDato)
    {
        LocalListado local = pDato as LocalListado;
        if (local == null) return "";
        var sb = new System.Text.StringBuilder("<ul class='loc-datos' aria-label='");
        sb.Append(HttpUtility.HtmlAttributeEncode(Traducir("locales.datosAnalisis"))).Append("'>");
        sb.Append("<li class='loc-datos-titulo'>").Append(HttpUtility.HtmlEncode(Traducir("locales.datosAnalisis"))).Append("</li>");
        AgregarDato(sb, local.TieneLayoutVigente, Traducir(local.TieneLayoutVigente ? "locales.datoLayoutSi" : "locales.datoLayoutNo"));
        AgregarDato(sb, local.CantidadSectores > 0 && local.SectoresCompletos == local.CantidadSectores,
            Traducir("locales.datoSectores", local.SectoresCompletos, local.CantidadSectores));
        AgregarDato(sb, local.ProductosUbicados > 0, Traducir("locales.datoProductos", local.ProductosUbicados));
        AgregarDato(sb, local.ProductosConStock > 0, Traducir("locales.datoStock", local.ProductosConStock));
        AgregarDato(sb, local.VentasDelMes > 0, Traducir("locales.datoVentas", local.VentasDelMes));
        if (local.SuperficieAprox != null && local.SuperficieSectores > local.SuperficieAprox)
        {
            AgregarDato(sb, false, Traducir("locales.datoSuperficieExcedida", local.SuperficieSectores.ToString("#,##0.##"), local.SuperficieAprox.Value.ToString("#,##0.##")));
        }
        sb.Append("</ul>");
        return sb.ToString();
    }
    private static void AgregarDato(System.Text.StringBuilder pSb, bool pCompleto, string pTexto)
    {
        pSb.Append("<li class='").Append(pCompleto ? "loc-dato-ok" : "loc-dato-falta").Append("'>")
           .Append("<span class='loc-dato-icono' aria-hidden='true'>").Append(pCompleto ? "&#10003;" : "!").Append("</span> ")
           .Append(HttpUtility.HtmlEncode(pTexto)).Append("</li>");
    }
    private void MostrarMensaje(string texto, bool esExito)
    {
        lblMensaje.Text = texto;
        lblMensaje.CssClass = esExito ? "mensaje" : "mensaje mensaje-error";
        lblMensaje.Visible = true;
    }
    #endregion
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
        decimal? superficie = LeerDecimal(tbSuperficie.Text);
        bool esEdicion = hfEsEdicion.Value == "true";
        LocalBE local = new LocalBE(esEdicion ? int.Parse(hfCodigoLocal.Value) : 0, int.Parse(ddlEmpresa.SelectedValue), tbNombre.Text.Trim(), tbDireccion.Text.Trim(), ddlTipoLocal.SelectedValue,
                                    superficie, tbTelefono.Text.Trim(), tbObservaciones.Text.Trim(), chkActivo.Checked ? "Activo" : "Inactivo");
        ResultadoGuardarLocal resultado = esEdicion ? localBLL.ModificarLocal(local) : localBLL.CrearLocal(local);

        switch (resultado)
        {
            case ResultadoGuardarLocal.Exitoso:
                pnlModalLocal.Style["display"] = "none";
                MostrarMensaje(Traducir(esEdicion ? "locales.msgModificado" : "locales.msgRegistrado"), esExito: true);
                CargarLocales();
                break;
            case ResultadoGuardarLocal.DatosInvalidos:
                MostrarErrorModal(Traducir("comun.datosInvalidos"));
                break;
            case ResultadoGuardarLocal.EmpresaInactiva:
                MostrarErrorModal(Traducir("locales.errEmpresaInactiva"));
                break;
            case ResultadoGuardarLocal.NombreRepetido:
                MostrarErrorModal(Traducir("locales.errNombreRepetido"));
                break;
            case ResultadoGuardarLocal.SuperficieInvalida:
                MostrarErrorModal(Traducir("locales.errSuperficie"));
                break;
            case ResultadoGuardarLocal.TelefonoInvalido:
                MostrarErrorModal(Traducir("comun.errTelefonoInvalido"));
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
                MostrarMensaje(Traducir("locales.msgEstado"), esExito: true);
                CargarLocales();
                break;
            case "VerLayout":
                Response.Redirect($"PaginaLayoutSectores.aspx?local={codigoLocal}");
                break;
        }
    }
}