using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BE;
using BLL;
using SERVICIO.Logica;

public partial class PaginaEmpresas : PaginaSegura
{
    protected override string PermisoRequerido
    {
        get { return PermisoNombres.GestionarEmpresas; }
    }
    protected override string ClaveTitulo
    {
        get { return "empresas.titulo"; }
    }
    private EmpresaBLL empresaBLL = new EmpresaBLL();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CargarEmpresas();
        }
    }
    private void CargarEmpresas()
    {
        string busqueda = tbBuscar.Text.Trim();
        bool? estado = string.IsNullOrEmpty(ddlFiltroEstado.SelectedValue) ? (bool?)null : ddlFiltroEstado.SelectedValue == "1";
        List<EmpresaBE> empresas = empresaBLL.ObtenerEmpresasListado(busqueda, estado);
        if (empresas.Count == 0)
        {
            rptEmpresas.Visible = false;
            lblSinEmpresas.Visible = true;
        }
        else
        {
            rptEmpresas.Visible = true;
            lblSinEmpresas.Visible = false;
            rptEmpresas.DataSource = empresas;
            rptEmpresas.DataBind();
        }
    }
    private void LimpiarModal()
    {
        hfCodigoEmpresa.Value = "";
        tbRazonSocial.Text = "";
        tbCUIT.Text = "";
        tbCorreo.Text = "";
        tbTelefono.Text = "";
        tbDireccion.Text = "";
        chkActiva.Checked = true;
    }

    private void AbrirModal()
    {
        pnlModalEmpresa.Style["display"] = "flex";
    }
    private void CargarEmpresaEnModal(int codigoEmpresa)
    {
        EmpresaBE empresa = empresaBLL.ObtenerEmpresaPorCodigo(codigoEmpresa);
        if (empresa == null)
        {
            MostrarMensaje(Traducir("empresas.msgNoExiste"), esExito: false);
            CargarEmpresas();
            return;
        }
        hfEsEdicion.Value = "true";
        hfCodigoEmpresa.Value = empresa.CodigoEmpresa.ToString();
        litTituloModal.Text = Traducir("empresas.modalEditar");
        btnGuardarEmpresa.Text = Traducir("comun.guardarCambios");
        tbRazonSocial.Text = empresa.RazonSocial;
        tbCUIT.Text = empresa.CUIT;
        tbCorreo.Text = empresa.Correo;
        tbTelefono.Text = empresa.Telefono.ToString();
        tbDireccion.Text = empresa.Direccion;
        chkActiva.Checked = empresa.Estado;
        AbrirModal();
    }

    private void MostrarMensaje(string texto, bool esExito)
    {
        lblMensaje.Text = texto;
        lblMensaje.CssClass = esExito ? "mensaje" : "mensaje mensaje-error";
        lblMensaje.Visible = true;
    }
    protected void btnNuevaEmpresa_Click(object sender, EventArgs e)
    {
        LimpiarModal();
        hfEsEdicion.Value = "false";
        litTituloModal.Text = Traducir("empresas.btnNueva");
        btnGuardarEmpresa.Text = Traducir("empresas.btnCrear");
        AbrirModal();
    }

    protected void btnCancelarModal_Click(object sender, EventArgs e)
    {
        pnlModalEmpresa.Style["display"] = "none";
    }

    protected void btnGuardarEmpresa_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid)
        {
            AbrirModal();
            return;
        }
        int telefono;
        int.TryParse(tbTelefono.Text.Trim(), out telefono);
        bool esEdicion = hfEsEdicion.Value == "true";
        int codigoEmpresa = esEdicion ? int.Parse(hfCodigoEmpresa.Value) : 0;
        EmpresaBE empresa = new EmpresaBE(codigoEmpresa, tbRazonSocial.Text.Trim(), tbCUIT.Text.Trim(), tbCorreo.Text.Trim(), telefono, tbDireccion.Text.Trim(), chkActiva.Checked);
        ResultadoGuardarEmpresa resultado = esEdicion ? empresaBLL.ModificarEmpresa(empresa) : empresaBLL.CrearEmpresa(empresa);
        switch (resultado)
        {
            case ResultadoGuardarEmpresa.Exitoso:
                pnlModalEmpresa.Style["display"] = "none";
                MostrarMensaje(Traducir(esEdicion ? "empresas.msgModificada" : "empresas.msgRegistrada"), esExito: true);
                CargarEmpresas();
                break;
            case ResultadoGuardarEmpresa.CuitYaRegistrado:
                MostrarMensaje(Traducir("empresas.msgCuitExiste"), esExito: false);
                AbrirModal();
                break;
            case ResultadoGuardarEmpresa.DatosInvalidos:
                MostrarMensaje(Traducir("comun.datosInvalidos"), esExito: false);
                AbrirModal();
                break;
        }
    }
    protected void btnBuscar_Click(object sender, EventArgs e)
    {
        CargarEmpresas();
    }
    protected void Filtro_Changed(object sender, EventArgs e)
    {
        CargarEmpresas();
    }
    protected void rptEmpresas_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int codigoEmpresa = int.Parse(e.CommandArgument.ToString());
        switch (e.CommandName)
        {
            case "Editar":
                CargarEmpresaEnModal(codigoEmpresa);
                break;
            case "CambiarEstado":
                EmpresaBE empresa = empresaBLL.ObtenerEmpresaPorCodigo(codigoEmpresa);
                empresaBLL.CambiarEstado(codigoEmpresa, !empresa.Estado);
                MostrarMensaje(Traducir("empresas.msgEstado"), esExito: true);
                CargarEmpresas();
                break;
        }
    }
}