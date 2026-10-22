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
                <h1 runat="server" clave="integridad.encabezado">Integridad de datos</h1>
                <asp:Label ID="lbMensaje" runat="server" CssClass="dv-mensaje" Visible="false" EnableViewState="false"></asp:Label>
                <div class="dv-tarjeta">
                    <h2 runat="server" clave="integridad.inconsistencias">Inconsistencias detectadas</h2>
                    <asp:Repeater ID="rptCorruptos" runat="server">
                        <HeaderTemplate>
                            <table class="dv-tabla">
                                <thead>
                                    <tr>
                                        <th><%# Traducir("integridad.colTabla") %></th>
                                        <th><%# Traducir("integridad.colRegistro") %></th>
                                        <th><%# Traducir("integridad.colProblema") %></th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("NombreTabla") %></td>
                                <td class="dv-registro"><%# (bool)Eval("EsTablaCompleta") ? Traducir("integridad.todaLaTabla") : Eval("CodigoRegistro") %></td>
                                <td><%# Traducir((string)Eval("ClaveProblema"), (object[])Eval("Parametros")) %></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                    <asp:Label ID="lbIntegridadOK" runat="server" Text="No se detectaron inconsistencias." Clave="integridad.sinInconsistencias" Visible="false"></asp:Label>
                </div>
                <div class="dv-acciones">
                    <asp:Button ID="btnRecalcular" runat="server" CssClass="dv-btn dv-btn-primary" Text="Recalcular dígitos verificadores" Clave="integridad.btnRecalcular" OnClick="btnRecalcular_Click"
                        ClaveConfirmar="integridad.confirmarRecalcular" />

                    <asp:Button ID="btnRestore" runat="server" CssClass="dv-btn dv-btn-secundario" Text="Restaurar base de datos" Clave="integridad.btnRestaurar" OnClick="btnRestore_Click"
                        ClaveConfirmar="integridad.confirmarRestaurar" />

                    <asp:Button ID="btnCancelar" runat="server" CssClass="dv-btn dv-btn-cancelar" Text="Salir" Clave="integridad.btnSalir" OnClick="btnCancelar_Click" CausesValidation="false" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>
