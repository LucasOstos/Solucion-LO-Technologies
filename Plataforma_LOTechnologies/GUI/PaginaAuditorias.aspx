<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PaginaAuditorias.aspx.cs" Inherits="PaginaAuditorias" %>
<%@ Register TagPrefix="uc" TagName="BotonVolverMenu" Src="~/Controles/BotonVolverMenu.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Auditorías - LO Technologies</title>
    <link rel="stylesheet" type="text/css" href="/Estilos/BotonVolverMenu.css" />
    <link rel="stylesheet" type="text/css" href="/Estilos/Comun.css" />
    <link rel="stylesheet" type="text/css" href="/Estilos/EstilosAuditorias.css" />
</head>
<body>
    <form id="formAuditoria" runat="server">
        <div class="pagina">
            <div class="contenedor">

                <uc:BotonVolverMenu ID="volverMenu" runat="server" />

                <div class="encabezado">
                    <div>
                        <h1>Auditoría / Bitácora</h1>
                        <p class="subtitulo">CUS 015 — Registro de acciones para auditoría y seguridad</p>
                    </div>
                    <asp:Button ID="btnExportarLog" runat="server" CssClass="boton-secundario" Text="Exportar log" OnClick="btnExportarLog_Click" CausesValidation="false" />
                </div>

                <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" Visible="false" />

                <div class="filtros">
                    <asp:TextBox ID="tbBuscar" runat="server" CssClass="campo-buscar" placeholder="Buscar por usuario, acción, entidad..." />

                    <asp:TextBox ID="tbFechaDesde" runat="server" CssClass="selector" TextMode="Date" />
                    <asp:TextBox ID="tbFechaHasta" runat="server" CssClass="selector" TextMode="Date" />

                    <asp:DropDownList ID="ddlTipoSuceso" runat="server" CssClass="selector">
                        <asp:ListItem Text="Todas las acciones" Value="" />
                    </asp:DropDownList>

                    <asp:DropDownList ID="ddlCriticidad" runat="server" CssClass="selector">
                        <asp:ListItem Text="Todos los niveles" Value="" />
                        <asp:ListItem Text="Info (1)" Value="1" />
                        <asp:ListItem Text="Advertencia (2)" Value="2" />
                        <asp:ListItem Text="Error (3)" Value="3" />
                    </asp:DropDownList>

                    <asp:Button ID="btnFiltrar" runat="server" CssClass="boton-buscar" Text="Filtrar" OnClick="btnFiltrar_Click" CausesValidation="false" />
                </div>

                <div class="tarjeta">
                    <table class="tabla">
                        <thead>
                            <tr>
                                <th>Fecha/Hora</th>
                                <th>Usuario</th>
                                <th>Tipo de Suceso</th>
                                <th>Detalle</th>
                                <th>Nivel</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptBitacora" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("Fecha", "{0:dd/MM/yyyy HH:mm:ss}") %></td>
                                        <td><%# Eval("Usuario") %></td>
                                        <td><span class="etiqueta etiqueta-info"><%# Eval("TipoSuceso") %></span></td>
                                        <td><%# Eval("Descripcion") %></td>
                                        <td><span class='<%# "etiqueta " + ObtenerClaseCriticidad(Eval("Criticidad").ToString()) %>'><%# ObtenerNombreCriticidad(Eval("Criticidad").ToString()) %></span></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                    <asp:Label ID="lblSinSucesos" runat="server" Text="No hay sucesos que coincidan con los filtros aplicados." Visible="false" CssClass="sin-resultados" />

                    <div class="aud-paginacion">
                        <asp:Label ID="lblMostrando" runat="server" CssClass="texto-secundario" Text="Mostrando los últimos 3 días" />
                        <div>
                            <asp:Button ID="btnAnterior" runat="server" CssClass="boton-secundario" Text="Anterior" OnClick="btnAnterior_Click" CausesValidation="false" />
                            <asp:Button ID="btnSiguiente" runat="server" CssClass="boton-secundario" Text="Siguiente" OnClick="btnSiguiente_Click" CausesValidation="false" />
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </form>
</body>
</html>
