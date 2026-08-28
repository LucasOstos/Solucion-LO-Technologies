<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PaginaComparacionEscenarios.aspx.cs" Inherits="PaginaComparacionEscenarios" %>

<%@ Register TagPrefix="uc" TagName="BotonVolverMenu" Src="~/Controles/BotonVolverMenu.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Comparación de Escenarios - LO Technologies</title>
    <link rel="stylesheet" type="text/css" href="/Estilos/BotonVolverMenu.css" />
    <link rel="stylesheet" type="text/css" href="/Estilos/Comun.css" />
    <link rel="stylesheet" type="text/css" href="/Estilos/EstilosComparacion.css" />
</head>
<body>
    <form id="formComparacion" runat="server">
        <div class="pagina">
            <div class="contenedor">

                <uc:BotonVolverMenu ID="volverMenu" runat="server" />

                <div class="encabezado">
                    <div>
                        <h1>Comparación de Escenarios</h1>
                        <p class="subtitulo">CUN 008 — Comparar escenario actual vs. escenario propuesto</p>
                    </div>
                </div>

                <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" Visible="false" />

                <div class="tarjeta">
                    <div class="fila-formulario">
                        <div class="grupo-campo">
                            <asp:Label runat="server" Text="Local" AssociatedControlID="ddlLocal" />
                            <asp:DropDownList ID="ddlLocal" runat="server" CssClass="modal-campo" AutoPostBack="true" OnSelectedIndexChanged="ddlLocal_SelectedIndexChanged" />
                        </div>
                        <div class="grupo-campo">
                            <asp:Label runat="server" Text="Período de análisis" AssociatedControlID="ddlPeriodo" />
                            <asp:DropDownList ID="ddlPeriodo" runat="server" CssClass="modal-campo">
                                <asp:ListItem Text="Último mes" Value="ultimo_mes" />
                                <asp:ListItem Text="Últimos 3 meses" Value="3_meses" />
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="fila-formulario">
                        <div class="grupo-campo">
                            <asp:Label runat="server" Text="Escenario actual *" AssociatedControlID="ddlEscenarioA" />
                            <asp:DropDownList ID="ddlEscenarioA" runat="server" CssClass="modal-campo" />
                        </div>
                        <div class="grupo-campo">
                            <asp:Label runat="server" Text="Escenario propuesto *" AssociatedControlID="ddlEscenarioB" />
                            <asp:DropDownList ID="ddlEscenarioB" runat="server" CssClass="modal-campo" />
                        </div>
                    </div>
                    <asp:Button ID="btnComparar" runat="server" CssClass="boton-nuevo" Text="Comparar Escenarios" OnClick="btnComparar_Click" CausesValidation="false" />
                </div>

                <asp:Panel ID="pnlResultados" runat="server" Visible="false">
                    <div class="ce-comparacion">
                        <div class="tarjeta">
                            <h2>
                                <asp:Literal ID="litNombreEscenarioA" runat="server" />
                                (actual)</h2>
                            <asp:Repeater ID="rptIndicadoresA" runat="server">
                                <ItemTemplate>
                                    <div class="ce-indicador">
                                        <span class="texto-secundario"><%# Eval("Nombre") %></span>
                                        <span class="texto-principal"><%# Eval("Valor") %></span>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                        <div class="tarjeta">
                            <h2>
                                <asp:Literal ID="litNombreEscenarioB" runat="server" />
                                (propuesto)</h2>
                            <asp:Repeater ID="rptIndicadoresB" runat="server">
                                <ItemTemplate>
                                    <div class="ce-indicador">
                                        <span class="texto-secundario"><%# Eval("Nombre") %></span>
                                        <span class="texto-principal"><%# Eval("Valor") %></span>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>

                    <div class="tarjeta">
                        <h2>Diferencias principales</h2>
                        <table class="tabla">
                            <thead>
                                <tr>
                                    <th>Indicador</th>
                                    <th>Actual</th>
                                    <th>Propuesto</th>
                                    <th>Diferencia</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptDiferencias" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td class="texto-principal"><%# Eval("Indicador") %></td>
                                            <td><%# Eval("ValorActual") %></td>
                                            <td><%# Eval("ValorPropuesto") %></td>
                                            <td><span class='<%# "etiqueta " + Eval("ClaseDiferencia") %>'><%# Eval("Diferencia") %></span></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>

                    <asp:Button ID="btnGuardarComparacion" runat="server" CssClass="boton-nuevo" Text="Guardar Comparación" OnClick="btnGuardarComparacion_Click" CausesValidation="false" />
                </asp:Panel>
            </div>
        </div>
    </form>
</body>
</html>
