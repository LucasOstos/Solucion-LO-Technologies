<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RecuperarContrasenia.aspx.cs" Inherits="RecuperarContrasenia" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Recuperar Contraseña - LO Technologies</title>
    <link rel="stylesheet" type="text/css" href="/Estilos/EstilosRecuperarContrasenia.css" />
</head>
<body>
    <form id="formRecuperarContrasenia" runat="server">
        <div class="rc-pagina">
            <div class="rc-wrapper">
                <div class="rc-logo">LT</div>
                <h1 class="rc-marca-titulo">LO <span>TECHNOLOGIES</span></h1>
                <div class="rc-tarjeta">
                    <h2>Recuperar contraseña</h2>
                    <asp:Label ID="lbError" runat="server" CssClass="rc-error" Visible="false"></asp:Label>
                    <asp:Panel ID="panelNuevaContrasenia" runat="server" Visible="false">
                        <p class="rc-subtexto">Ingresá la nueva contraseña</p>
                        <div class="form-group">
                            <asp:Label ID="lbNuevaContrasenia" runat="server" Text="Nueva contraseña" AssociatedControlID="tbNuevaContrasenia"></asp:Label>
                            <asp:TextBox ID="tbNuevaContrasenia" runat="server" TextMode="Password"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvNueva" runat="server" ControlToValidate="tbNuevaContrasenia" CssClass="campo-error"
                                ErrorMessage="Ingrese la nueva contraseña" Display="Dynamic" ValidationGroup="vgRecuperar"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="revNueva" runat="server" ControlToValidate="tbNuevaContrasenia" CssClass="campo-error"
                                ValidationExpression="^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&amp;*(),.?&quot;:{}|&lt;&gt;_\-]).{8,}$"
                                ErrorMessage="Mínimo 8 caracteres, con mayúscula, número y carácter especial" Display="Dynamic" ValidationGroup="vgRecuperar"></asp:RegularExpressionValidator>
                        </div>
                        <div class="form-group">
                            <asp:Label ID="lbConfirmarContrasenia" runat="server" Text="Confirmar contraseña" AssociatedControlID="tbConfirmarContrasenia"></asp:Label>
                            <asp:TextBox ID="tbConfirmarContrasenia" runat="server" TextMode="Password"></asp:TextBox>
                            <asp:CompareValidator ID="cvConfirmar" runat="server" ControlToValidate="tbConfirmarContrasenia" ControlToCompare="tbNuevaContrasenia"
                                CssClass="campo-error" ErrorMessage="Las contraseñas no coinciden" Display="Dynamic" ValidationGroup="vgRecuperar"></asp:CompareValidator>
                        </div>
                        <asp:Button ID="btnConfirmar" runat="server" CssClass="rc-btn" Text="Confirmar" OnClick="btnConfirmar_Click" ValidationGroup="vgRecuperar"/>
                    </asp:Panel>
                    <asp:Label ID="lbExito" runat="server" CssClass="rc-exito" Visible="false"></asp:Label>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
