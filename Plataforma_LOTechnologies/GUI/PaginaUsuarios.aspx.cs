using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BE;
using BLL;
using SERVICIO.Logica;

public partial class PaginaUsuarios : System.Web.UI.Page
{
    private UsuarioBLL usuarioBLL = new UsuarioBLL();
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
            CargarFiltroEmpresas();
            CargarUsuarios();
        }
    }
    #region Funciones
    private void CargarFiltroEmpresas()
    {
        List<EmpresaBE> empresas = empresaBLL.ObtenerEmpresas();

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
    private void CargarUsuarios()
    {
        string busqueda = tbBuscar.Text.Trim();
        int? rol = string.IsNullOrEmpty(ddlFiltroRol.SelectedValue) ? (int?)null : int.Parse(ddlFiltroRol.SelectedValue);
        int? empresa = string.IsNullOrEmpty(ddlFiltroEmpresa.SelectedValue) ? (int?)null : int.Parse(ddlFiltroEmpresa.SelectedValue);

        List<UsuarioListado> usuarios = usuarioBLL.ObtenerUsuariosListado(busqueda, rol, empresa);

        if (usuarios.Count == 0)
        {
            rptUsuarios.Visible = false;
            lbNoResultado.Visible = true;
        }
        else
        {
            rptUsuarios.Visible = true;
            lbNoResultado.Visible = false;
            rptUsuarios.DataSource = usuarios;
            rptUsuarios.DataBind();
        }
    }
    public string ObtenerIniciales(string nombre, string apellido)
    {
        string inicialNombre = string.IsNullOrEmpty(nombre) ? "" : nombre.Substring(0, 1);
        string inicialApellido = string.IsNullOrEmpty(apellido) ? "" : apellido.Substring(0, 1);
        return (inicialNombre + inicialApellido).ToUpper();
    }

    public string ObtenerNombreRol(object rol)
    {
        switch (Convert.ToInt32(rol))
        {
            case 1: return "Administrador";
            case 2: return "Analista";
            case 3: return "Gerente";
            default: return "Desconocido";
        }
    }
    private void LimpiarModal()
    {
        hfDNI.Value = "";
        tbDNI.Text = "";
        tbNombre.Text = "";
        tbApellido.Text = "";
        tbEmail.Text = "";
        tbContrasenia.Text = "";
        ddlRol.SelectedIndex = 0;
        if (ddlEmpresa.Items.Count > 0) ddlEmpresa.SelectedIndex = 0;
        chkActivo.Checked = true;
    }
    private void AbrirModal()
    {
        pnlModalUsuario.Style["display"] = "flex";
    }
    private void MostrarMensaje(string texto, bool esExito)
    {
        lbMensaje.Text = texto;
        lbMensaje.CssClass = esExito ? "mensaje" : "mensaje mensaje-error";
        lbMensaje.Visible = true;
    }
    private void CargarUsuarioEnModal(int dni)
    {
        Usuario usuario = usuarioBLL.ObtenerUsuarioPorDNI(dni);
        if (usuario == null)
        {
            MostrarMensaje("El usuario ya no existe.", esExito: false);
            CargarUsuarios();
            return;
        }
        hfEsEdicion.Value = "true";
        hfDNI.Value = usuario.DNI.ToString();
        litTituloModal.Text = "Editar Usuario";
        btnGuardarUsuario.Text = "Guardar Cambios";

        tbDNI.Text = usuario.DNI.ToString();
        tbDNI.Enabled = false;
        tbNombre.Text = usuario.Nombre;
        tbApellido.Text = usuario.Apellido;
        tbEmail.Text = usuario.Email;
        ddlRol.SelectedValue = usuario.Rol.ToString();
        if (usuario.CodigoEmpresa > 0)
        {
            ddlEmpresa.SelectedValue = usuario.CodigoEmpresa.ToString();
        }
        chkActivo.Checked = usuario.estadoActivo;
        grupoContrasenia.Visible = false;
        rfvContrasenia.Enabled = false;
        revContrasenia.Enabled = false;
        tbContrasenia.Text = "";
        AbrirModal();
    }
    #endregion
    protected void btnAgregarUsuario_Click(object sender, EventArgs e)
    {
        LimpiarModal();
        hfEsEdicion.Value = "false";
        litTituloModal.Text = "Nuevo Usuario";
        btnGuardarUsuario.Text = "Crear Usuario";
        tbDNI.Enabled = true;
        grupoContrasenia.Visible = true;
        rfvContrasenia.Enabled = true;
        revContrasenia.Enabled = true;
        AbrirModal();
    }

    protected void btnBuscar_Click(object sender, EventArgs e)
    {
        CargarUsuarios();
    }

    protected void rptUsuarios_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int dni = int.Parse(e.CommandArgument.ToString());

        switch (e.CommandName)
        {
            case "Editar":
                CargarUsuarioEnModal(dni);
                break;

            case "CambiarEstado":
                Usuario usuario = usuarioBLL.ObtenerUsuarioPorDNI(dni);
                usuarioBLL.CambiarEstadoActivo(dni, !usuario.estadoActivo);
                MostrarMensaje("Estado del usuario actualizado.", esExito: true);
                CargarUsuarios();
                break;
            case "Desbloquear":
                Usuario usuarioADesbloquear = usuarioBLL.ObtenerUsuarioPorDNI(dni);
                if (!usuarioADesbloquear.estadoBloqueado)
                {
                    MostrarMensaje("El usuario no está bloqueado.", esExito: true);
                }
                else
                {
                    usuarioBLL.DesbloquearUsuario(dni);
                    MostrarMensaje("Usuario desbloqueado. Se le envió un mail para que defina su nueva contraseña.", esExito: true);
                    CargarUsuarios();
                }
                break;
        }
    }

    protected void Filtro_Changed(object sender, EventArgs e)
    {
        CargarUsuarios();
    }

    protected void btnCancelarModal_Click(object sender, EventArgs e)
    {
        pnlModalUsuario.Style["display"] = "none";
    }

    protected void btnGuardarUsuario_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid)
        {
            AbrirModal();
            return;
        }

        int dni;
        if (!int.TryParse(tbDNI.Text.Trim(), out dni))
        {
            MostrarMensaje("DNI inválido.", esExito: false);
            AbrirModal();
            return;
        }
        var usuario = new Usuario
        {
            DNI = dni,
            Nombre = tbNombre.Text.Trim(),
            Apellido = tbApellido.Text.Trim(),
            Email = tbEmail.Text.Trim(),
            Rol = int.Parse(ddlRol.SelectedValue),
            estadoActivo = chkActivo.Checked,
            CodigoEmpresa = ddlEmpresa.Items.Count > 0 ? int.Parse(ddlEmpresa.SelectedValue) : 0
        };
        bool esEdicion = hfEsEdicion.Value == "true";
        ResultadoGuardarUsuario resultado;

        if (esEdicion)
        {
            resultado = usuarioBLL.ModificarUsuario(usuario);
        }
        else
        {
            resultado = usuarioBLL.CrearUsuario(usuario, tbContrasenia.Text);
        }
        switch (resultado)
        {
            case ResultadoGuardarUsuario.Exitoso:
                pnlModalUsuario.Style["display"] = "none";
                MostrarMensaje(esEdicion ? "Usuario modificado correctamente." : "Usuario creado correctamente.", esExito: true);
                CargarUsuarios();
                break;

            case ResultadoGuardarUsuario.DniYaRegistrado:
                MostrarMensaje("Usuario ya registrado.", esExito: false);
                AbrirModal();
                break;

            case ResultadoGuardarUsuario.DatosInvalidos:
                MostrarMensaje("Datos inválidos.", esExito: false);
                AbrirModal();
                break;
        }
    }
}