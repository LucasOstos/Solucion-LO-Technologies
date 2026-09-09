<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PaginaReportes.aspx.cs" Inherits="PaginaReportes" %>

<%@ Register TagPrefix="uc" TagName="BotonVolverMenu" Src="~/Controles/BotonVolverMenu.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Generación de Reportes - LO Technologies</title>
    <link rel="stylesheet" type="text/css" href="/Estilos/BotonVolverMenu.css" />
    <link rel="stylesheet" type="text/css" href="/Estilos/Comun.css" />
    <link rel="stylesheet" type="text/css" href="/Estilos/EstilosReportes.css" />
</head>
<body>
    <form id="formReportes" runat="server">
        <div class="pagina">
            <div class="contenedor">

                <uc:BotonVolverMenu ID="volverMenu" runat="server" />

                <div class="encabezado">
                    <div>
                        <h1>Generación de Reportes</h1>
                        <p class="subtitulo">CUN 010 — Generar reportes de análisis, indicadores y recomendaciones</p>
                    </div>
                </div>

                <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" Visible="false" />

                <div class="rep-columnas">
                    <div class="tarjeta">
                        <h2>Configurar Reporte</h2>

                        <div class="fila-formulario">
                            <div class="grupo-campo">
                                <asp:Label runat="server" Text="Local *" AssociatedControlID="ddlLocal" />
                                <asp:DropDownList ID="ddlLocal" runat="server" CssClass="modal-campo" />
                            </div>
                            <div class="grupo-campo">
                                <asp:Label runat="server" Text="Tipo de Reporte *" AssociatedControlID="ddlTipoReporte" />
                                <asp:DropDownList ID="ddlTipoReporte" runat="server" CssClass="modal-campo">
                                    <asp:ListItem Text="Completo" Value="Completo" />
                                    <asp:ListItem Text="Comparación" Value="Comparación" />
                                    <asp:ListItem Text="Indicadores" Value="Indicadores" />
                                    <asp:ListItem Text="Stock" Value="Stock" />
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="fila-formulario">
                            <div class="grupo-campo">
                                <asp:Label runat="server" Text="Período Desde *" AssociatedControlID="tbPeriodoDesde" />
                                <asp:TextBox ID="tbPeriodoDesde" runat="server" CssClass="modal-campo" TextMode="Date" />
                            </div>
                            <div class="grupo-campo">
                                <asp:Label runat="server" Text="Período Hasta *" AssociatedControlID="tbPeriodoHasta" />
                                <asp:TextBox ID="tbPeriodoHasta" runat="server" CssClass="modal-campo" TextMode="Date" />
                            </div>
                        </div>

                        <div class="fila-formulario">
                            <div class="grupo-campo">
                                <asp:Label runat="server" Text="Escenario Base" AssociatedControlID="ddlEscenarioBase" />
                                <asp:DropDownList ID="ddlEscenarioBase" runat="server" CssClass="modal-campo" />
                            </div>
                            <div class="grupo-campo">
                                <asp:Label runat="server" Text="Escenario Propuesto" AssociatedControlID="ddlEscenarioPropuesto" />
                                <asp:DropDownList ID="ddlEscenarioPropuesto" runat="server" CssClass="modal-campo" />
                            </div>
                        </div>

                        <div class="fila-formulario">
                            <div class="grupo-campo">
                                <asp:Label runat="server" Text="Formato" AssociatedControlID="ddlFormato" />
                                <asp:DropDownList ID="ddlFormato" runat="server" CssClass="modal-campo">
                                    <asp:ListItem Text="PDF" Value="PDF" />
                                </asp:DropDownList>
                            </div>
                            <div class="grupo-campo">
                                <asp:Label runat="server" Text="Idioma" AssociatedControlID="ddlIdiomaReporte" />
                                <asp:DropDownList ID="ddlIdiomaReporte" runat="server" CssClass="modal-campo">
                                    <asp:ListItem Text="Español" Value="Español" />
                                    <asp:ListItem Text="Inglés" Value="Inglés" />
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="grupo-campo">
                            <label>Secciones a incluir</label>
                            <div class="rep-checks">
                                <asp:CheckBox ID="chkResumenEjecutivo" runat="server" Text="Resumen Ejecutivo" Checked="true" />
                                <asp:CheckBox ID="chkComparacionEscenarios" runat="server" Text="Comparación de Escenarios" Checked="true" />
                                <asp:CheckBox ID="chkDetallePorSector" runat="server" Text="Detalle por Sector" Checked="true" />
                                <asp:CheckBox ID="chkEstadoStock" runat="server" Text="Estado de Stock" Checked="true" />
                                <asp:CheckBox ID="chkIndicadoresPrincipales" runat="server" Text="Indicadores Principales" Checked="true" />
                                <asp:CheckBox ID="chkRecomendaciones" runat="server" Text="Recomendaciones" Checked="true" />
                                <asp:CheckBox ID="chkDetalleProductos" runat="server" Text="Detalle de Productos" Checked="true" />
                                <asp:CheckBox ID="chkHistorialVentas" runat="server" Text="Historial de Ventas" Checked="true" />
                            </div>
                        </div>

                        <asp:Button ID="btnGenerarReporte" runat="server" CssClass="boton-nuevo" Text="Generar Reporte" OnClick="btnGenerarReporte_Click" CausesValidation="false" />
                    </div>

                    <div class="tarjeta">
                        <h2>Reportes Recientes</h2>
                        <asp:Repeater ID="rptReportesRecientes" runat="server" OnItemCommand="rptReportesRecientes_ItemCommand">
                            <ItemTemplate>
                                <div class="rep-item">
                                    <span class="avatar avatar-cuadrado">&#128196;</span>
                                    <div class="rep-item-info">
                                        <div class="texto-principal"><%# Eval("Titulo") %></div>
                                        <div class="texto-secundario"><%# Eval("FechaGeneracion", "{0:dd/MM/yyyy HH:mm}") %></div>
                                        <span class="etiqueta etiqueta-info"><%# Eval("TipoReporte") %></span>
                                    </div>
                                    <div class="rep-item-acciones">
                                        <asp:LinkButton runat="server" CssClass="boton-icono" ToolTip="Ver"
                                            CommandName="VerReporte" CommandArgument='<%# Eval("CodigoReporte") %>'>Ver</asp:LinkButton>
                                        <asp:LinkButton runat="server" CssClass="boton-icono" ToolTip="Bajar"
                                            CommandName="DescargarReporte" CommandArgument='<%# Eval("CodigoReporte") %>'>&#8595; Bajar</asp:LinkButton>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:Label ID="lblSinReportes" runat="server" Text="Todavía no generaste ningún reporte." Visible="false" CssClass="sin-resultados" />
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
