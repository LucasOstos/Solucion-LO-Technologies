using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;

public partial class RecuperarContrasenia : System.Web.UI.Page
{
    private UsuarioBLL usuarioBLL = new UsuarioBLL();
    private Guid tokenActual;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ValidarTokenURL();
        }
    }
    private void ValidarTokenURL()
    {
        string tokenURL = Request.QueryString["token"];
        if (string.IsNullOrEmpty(tokenURL) || !Guid.TryParse(tokenURL, out tokenActual))
        {
            MostrarError("El link de recuperación no es válido.");
            return;
        }
        ResultadoCambio resultado = usuarioBLL.ValidarToken(tokenActual);
        switch (resultado)
        {
            case ResultadoCambio.Exitoso:
                ViewState["Token"] = tokenActual;
                panelNuevaContrasenia.Visible = true;
                break;
            case ResultadoCambio.TokenInvalido:
                MostrarError("El link de recuperación no es válido.");
                break;
            case ResultadoCambio.TokenExpirado:
                MostrarError("El link de recuperación expiró. Solicitá uno nuevo.");
                break;
            case ResultadoCambio.TokenYaUsado:
                MostrarError("Este link ya fue utilizado.");
                break;
        }
    }
    private void MostrarError(string mensaje)
    {
        lbError.Text = mensaje;
        lbError.Visible = true;
        panelNuevaContrasenia.Visible = false;
    }
    protected void btnConfirmar_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) return;
        if (ViewState["Token"] == null)
        {
            MostrarError("El link de recuperación no es válido");
            return;
        }
        Guid token = (Guid)ViewState["Token"];
        string nuevaContrasenia = tbNuevaContrasenia.Text;
        ResultadoCambio resultado = usuarioBLL.CambiarContraseniaConToken(token, nuevaContrasenia);
        switch (resultado)
        {
            case ResultadoCambio.Exitoso:
                panelNuevaContrasenia.Visible = false;
                lbExito.Text = "Tu contraseña fue actualizada correctamente. Ya podés iniciar sesión.";
                lbExito.Visible = true;
                break;
            case ResultadoCambio.TokenExpirado:
                panelNuevaContrasenia.Visible = false;
                MostrarError("El link de recuperación expiró. Solicitá uno nuevo.");
                break;
            case ResultadoCambio.TokenYaUsado:
                panelNuevaContrasenia.Visible = false;
                MostrarError("Este link ya fue utilizado.");
                break;
            case ResultadoCambio.TokenInvalido:
                panelNuevaContrasenia.Visible = false;
                MostrarError("El link de recuperación no es válido.");
                break;
        }
    }
}