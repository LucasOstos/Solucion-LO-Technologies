<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PaginaDigitosVerificadores.aspx.cs" Inherits="PaginaDigitosVerificadores" %>
<%@ Register TagPrefix="uc" TagName="BotonVolverMenu" Src="~/Controles/BotonVolverMenu.ascx" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Integridad del sistema - LO Technologies</title>
    <link rel="stylesheet" type="text/css" href="/Estilos/EstilosDigitos.css" />
    <link rel="stylesheet" type="text/css" href="/Estilos/BotonVolverMenu.css" />
    <script type="text/javascript" src="/JS/DigitosJS.js"></script>
</head>
<body>
    <form id="formDigitosVerificadores" runat="server">
        <div class="dv-pagina">
            <div class="dv-wrapper">
                <uc:BotonVolverMenu ID="volverMenu" runat="server" />
                <h1>Integridad de datos</h1>
                <p class="dv-subtitulo">CUS 014</p>
                <asp:Label ID="lbMensaje" runat="server" CssClass="dv-mensaje" Visible="false"></asp:Label>
                <div class="dv-tarjeta">
                    <h2>Registros con inconsistencias</h2>
                    <asp:Repeater ID="rptCorruptos" runat="server">
                        <HeaderTemplate>
                            <table class="dv-tabla">
                                <thead>
                                    <tr>
                                        <th>Tabla</th>
                                        <th>Registro</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("NombreTabla")%></td>
                                <td><%# Eval("CodigoRegistro")%></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                    <asp:Label ID="lbIntegridadOK" runat="server" Text="No hay registros con inconsistencias." Visible="false"></asp:Label>
                </div>
                <div class="dv-acciones">
                    <asp:Button ID="btnRecalcular" runat="server" CssClass="dv-btn dv-btn-primary" Text="Recalcular dígitos verificadores" OnClick="btnRecalcular_Click"
                        OnClientClick="return confirmarAccion('¿Confirmás que querés recalcular los dígitos verificadores de todas las tablas?');"/>

                    <asp:Button ID="btnRestore" runat="server" CssClass="dv-btn dv-btn-secundario" Text="Restaurar base de datos" OnClick="btnRestore_Click"
                        OnClientClick="return confirmarAccion('¿Confirmás que querés restaurar la base de datos desde un backup? Esta acción reemplaza los datos actuales.');"/>

                    <asp:Button ID="btnCancelar" runat="server" CssClass="dv-btn dv-btn-cancelar" Text="Salir" OnClick="btnCancelar_Click" CausesValidation="false"/>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
