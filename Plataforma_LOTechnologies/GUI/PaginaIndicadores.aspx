<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PaginaIndicadores.aspx.cs" Inherits="PaginaIndicadores" %>
<%@ Register TagPrefix="uc" TagName="BotonVolverMenu" Src="~/Controles/BotonVolverMenu.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Indicadores y Recomendaciones - LO Technologies</title>
    <link rel="stylesheet" type="text/css" href="/Estilos/BotonVolverMenu.css" />
    <link rel="stylesheet" type="text/css" href="/Estilos/Comun.css" />
    <link rel="stylesheet" type="text/css" href="/Estilos/EstilosIndicadores.css" />
</head>
<body>
    <form id="formIndicadores" runat="server">
        <div class="pagina">
            <div class="contenedor">

                <uc:BotonVolverMenu ID="volverMenu" runat="server" />

                <div class="encabezado">
                    <div>
                        <h1>Indicadores y Recomendaciones</h1>
                        <p class="subtitulo">CUN 009 — Indicadores clave y recomendaciones automáticas</p>
                    </div>
                </div>

                <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" Visible="false" />

                <div class="filtros">
                    <asp:DropDownList ID="ddlLocal" runat="server" CssClass="selector" AutoPostBack="true" OnSelectedIndexChanged="Filtro_Changed" />
                    <asp:DropDownList ID="ddlPeriodo" runat="server" CssClass="selector">
                        <asp:ListItem Text="Último mes" Value="ultimo_mes" />
                        <asp:ListItem Text="Últimos 3 meses" Value="3_meses" />
                    </asp:DropDownList>
                    <asp:Button ID="btnAplicar" runat="server" CssClass="boton-buscar" Text="Aplicar" OnClick="btnAplicar_Click" CausesValidation="false" />
                </div>

                <div class="tarjeta">
                    <h2>&#128200; Recomendaciones Automáticas</h2>
                    <asp:Repeater ID="rptRecomendaciones" runat="server">
                        <ItemTemplate>
                            <div class='<%# "ind-recomendacion ind-prioridad-" + Eval("Prioridad").ToString().ToLower() %>'>
                                <div class="ind-recomendacion-texto">
                                    <div class="texto-principal"><%# Eval("Descripcion") %></div>
                                    <div class="texto-secundario"><%# Eval("Motivo") %></div>
                                </div>
                                <span class='<%# "etiqueta ind-etiqueta-" + Eval("Prioridad").ToString().ToLower() %>'><%# Eval("Prioridad") %></span>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Label ID="lblSinRecomendaciones" runat="server" Text="No hay recomendaciones generadas para este período." Visible="false" CssClass="sin-resultados" />
                </div>

                <div class="tarjeta">
                    <h2>Indicadores por Sector</h2>
                    <table class="tabla">
                        <thead>
                            <tr>
                                <th>Sector</th>
                                <th>Ventas</th>
                                <th>Margen</th>
                                <th>Circulación</th>
                                <th>Rotación</th>
                                <th>Índice Global</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptIndicadoresSector" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td class="texto-principal"><%# Eval("Sector") %></td>
                                        <td><%# Eval("Ventas", "{0:$#,##0}") %></td>
                                        <td><%# Eval("Margen") %>%</td>
                                        <td><%# Eval("Circulacion") %>%</td>
                                        <td><%# Eval("Rotacion") %>%</td>
                                        <td><span class='<%# "etiqueta " + Eval("ClaseIndiceGlobal") %>'><%# Eval("IndiceGlobal") %></span></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                    <asp:Label ID="lblSinIndicadores" runat="server" Text="No existen datos suficientes para generar indicadores completos." Visible="false" CssClass="sin-resultados" />
                </div>

            </div>
        </div>
    </form>
</body>
</html>
