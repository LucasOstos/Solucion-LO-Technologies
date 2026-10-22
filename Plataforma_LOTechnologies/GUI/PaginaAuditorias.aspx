<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PaginaAuditorias.aspx.cs" Inherits="PaginaAuditorias" %>

<%@ Register TagPrefix="uc" TagName="BotonVolverMenu" Src="~/Controles/BotonVolverMenu.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
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
                        <h1 runat="server" clave="principal.bitacora.titulo">Auditoría / Bitácora</h1>
                        <p class="subtitulo" runat="server" clave="auditoria.subtitulo">CUS 015 — Registro de acciones para auditoría y seguridad</p>
                    </div>
                    <asp:Button ID="btnExportarLog" runat="server" CssClass="boton-secundario" Text="Exportar log" Clave="auditoria.btnExportar" OnClick="btnExportarLog_Click" CausesValidation="false" />
                </div>

                <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" Visible="false" EnableViewState="false" />

                <div class="filtros">
                    <asp:TextBox ID="tbBuscar" runat="server" CssClass="campo-buscar" placeholder="Buscar por usuario, acción, entidad..." ClavePlaceholder="auditoria.buscarPlaceholder" />

                    <asp:TextBox ID="tbFechaDesde" runat="server" CssClass="selector" TextMode="Date" ToolTip="Fecha desde" ClaveTitle="auditoria.fechaDesde" ClaveAria="auditoria.fechaDesde" />
                    <asp:TextBox ID="tbFechaHasta" runat="server" CssClass="selector" TextMode="Date" ToolTip="Fecha hasta" ClaveTitle="auditoria.fechaHasta" ClaveAria="auditoria.fechaHasta" />

                    <asp:DropDownList ID="ddlTipoSuceso" runat="server" CssClass="selector">
                        <asp:ListItem Text="Todas las acciones" Value="" />
                    </asp:DropDownList>

                    <asp:DropDownList ID="ddlCriticidad" runat="server" CssClass="selector">
                        <asp:ListItem Text="Todos los niveles" Value="" Clave="auditoria.todosNiveles" />
                        <asp:ListItem Text="Info (1)" Value="1" Clave="auditoria.nivelInfo" />
                        <asp:ListItem Text="Advertencia (2)" Value="2" Clave="auditoria.nivelAdvertencia" />
                        <asp:ListItem Text="Error (3)" Value="3" Clave="auditoria.nivelError" />
                    </asp:DropDownList>

                    <asp:Button ID="btnFiltrar" runat="server" CssClass="boton-buscar" Text="Filtrar" Clave="comun.filtrar" OnClick="btnFiltrar_Click" CausesValidation="false" />
                </div>

                <div class="tarjeta">
                    <table class="tabla">
                        <thead>
                            <tr>
                                <th runat="server" clave="auditoria.colFecha">Fecha/Hora</th>
                                <th runat="server" clave="usuarios.colUsuario">Usuario</th>
                                <th runat="server" clave="auditoria.colTipo">Tipo de Suceso</th>
                                <th runat="server" clave="auditoria.colDetalle">Detalle</th>
                                <th runat="server" clave="auditoria.colNivel">Nivel</th>
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
                                        <td><span class='<%# "etiqueta " + ObtenerClaseCriticidad(Eval("Criticidad").ToString()) %>'><%# Eval("Criticidad") %></span></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                    <asp:Label ID="lblSinSucesos" runat="server" Text="No hay sucesos que coincidan con los filtros aplicados." Clave="auditoria.sinResultados" Visible="false" CssClass="sin-resultados" />

                    <div class="aud-paginacion">
                        <asp:Label ID="lblMostrando" runat="server" CssClass="texto-secundario" Text="Mostrando los últimos 3 días" />
                        <div>
                            <asp:Button ID="btnAnterior" runat="server" CssClass="boton-secundario" Text="Anterior" Clave="comun.anterior" OnClick="btnAnterior_Click" CausesValidation="false" />
                            <asp:Button ID="btnSiguiente" runat="server" CssClass="boton-secundario" Text="Siguiente" Clave="comun.siguiente" OnClick="btnSiguiente_Click" CausesValidation="false" />
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </form>
</body>
</html>
