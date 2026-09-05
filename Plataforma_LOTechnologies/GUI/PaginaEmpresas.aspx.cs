using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BE;
using BLL;
using SERVICIO.Logica;

public partial class PaginaEmpresas : System.Web.UI.Page
{
    private EmpresaBLL empresaBLL = new EmpresaBLL();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Sesion.Instancia.IsLogueado() || Sesion.Instancia.Usuario.Rol != 1)
        {
            Response.Redirect("PaginaLogin.aspx");
            return;
        }
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
            MostrarMensaje("La empresa ya no existe.", esExito: false);
            CargarEmpresas();
            return;
        }
        hfEsEdicion.Value = "true";
        hfCodigoEmpresa.Value = empresa.CodigoEmpresa.ToString();
        litTituloModal.Text = "Editar Empresa";
        btnGuardarEmpresa.Text = "Guardar Cambios";
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
        litTituloModal.Text = "Nueva Empresa";
        btnGuardarEmpresa.Text = "Crear Empresa";
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
        int telefono = int.Parse(tbTelefono.Text.Trim());
        bool esEdicion = hfEsEdicion.Value == "true";
        int codigoEmpresa = esEdicion ? int.Parse(hfCodigoEmpresa.Value) : 0;
        EmpresaBE empresa = new EmpresaBE(codigoEmpresa, tbRazonSocial.Text.Trim(), tbCUIT.Text.Trim(), tbCorreo.Text.Trim(), telefono, tbDireccion.Text.Trim(), chkActiva.Checked);
        ResultadoGuardarEmpresa resultado = esEdicion ? empresaBLL.ModificarEmpresa(empresa) : empresaBLL.CrearEmpresa(empresa);
        switch (resultado)
        {
            case ResultadoGuardarEmpresa.Exitoso:
                pnlModalEmpresa.Style["display"] = "none";
                MostrarMensaje(esEdicion ? "Empresa modificada correctamente." : "Empresa registrada correctamente.", esExito: true);
                CargarEmpresas();
                break;
            case ResultadoGuardarEmpresa.CuitYaRegistrado:
                MostrarMensaje("La empresa ya se encuentra registrada.", esExito: false);
                AbrirModal();
                break;
            case ResultadoGuardarEmpresa.DatosInvalidos:
                MostrarMensaje("Datos inválidos.", esExito: false);
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
                MostrarMensaje("Estado de la empresa actualizado.", esExito: true);
                CargarEmpresas();
                break;
        }
    }
}