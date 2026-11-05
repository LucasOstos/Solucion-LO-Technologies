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
                        <h1 runat="server" clave="reportes.encabezado">Generación de Reportes</h1>
                        <p class="subtitulo" runat="server" clave="reportes.subtitulo">CUN 010 — Generar reportes de análisis, indicadores y recomendaciones</p>
                    </div>
                </div>

                <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" Visible="false" EnableViewState="false" />

                <div class="rep-columnas">
                    <div class="tarjeta">
                        <h2 runat="server" clave="reportes.configurar">Configurar Reporte</h2>

                        <div class="fila-formulario">
                            <div class="grupo-campo">
                                <asp:Label runat="server" Text="Local *" AssociatedControlID="ddlLocal" Clave="comun.localObligatorio" />
                                <asp:DropDownList ID="ddlLocal" runat="server" CssClass="modal-campo" AutoPostBack="true" OnSelectedIndexChanged="ddlLocal_SelectedIndexChanged" />
                            </div>
                            <div class="grupo-campo">
                                <asp:Label runat="server" Text="Tipo de Reporte *" AssociatedControlID="ddlTipoReporte" Clave="reportes.tipo" />
                                <asp:DropDownList ID="ddlTipoReporte" runat="server" CssClass="modal-campo" onchange="aplicarTipoReporte(this.value)">
                                    <asp:ListItem Text="Completo" Value="Completo" Clave="valor.completo" />
                                    <asp:ListItem Text="Comparación" Value="Comparación" Clave="valor.comparacion" />
                                    <asp:ListItem Text="Indicadores" Value="Indicadores" Clave="valor.indicadores" />
                                    <asp:ListItem Text="Stock" Value="Stock" Clave="valor.stock" />
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="fila-formulario">
                            <div class="grupo-campo">
                                <asp:Label runat="server" Text="Desde (mes) *" AssociatedControlID="tbPeriodoDesde" Clave="reportes.mesDesde" />
                                <asp:TextBox ID="tbPeriodoDesde" runat="server" CssClass="modal-campo" TextMode="Month" ClavePlaceholder="reportes.formatoMes" />
                            </div>
                            <div class="grupo-campo">
                                <asp:Label runat="server" Text="Hasta (mes) *" AssociatedControlID="tbPeriodoHasta" Clave="reportes.mesHasta" />
                                <asp:TextBox ID="tbPeriodoHasta" runat="server" CssClass="modal-campo" TextMode="Month" ClavePlaceholder="reportes.formatoMes" />
                            </div>
                        </div>
                        <p class="campo-ayuda" runat="server" clave="reportes.ayudaPeriodo">Todas las secciones usan los meses elegidos. Si el período llega al mes en curso, los datos van hasta hoy.</p>

                        <div class="fila-formulario">
                            <div class="grupo-campo">
                                <asp:Label runat="server" Text="Escenario Base" AssociatedControlID="ddlEscenarioBase" Clave="reportes.escenarioBase" />
                                <asp:DropDownList ID="ddlEscenarioBase" runat="server" CssClass="modal-campo" />
                            </div>
                            <div class="grupo-campo">
                                <asp:Label runat="server" Text="Escenario Propuesto" AssociatedControlID="ddlEscenarioPropuesto" Clave="reportes.escenarioPropuesto" />
                                <asp:DropDownList ID="ddlEscenarioPropuesto" runat="server" CssClass="modal-campo" />
                            </div>
                        </div>

                        <div class="fila-formulario">
                            <div class="grupo-campo">
                                <asp:Label runat="server" Text="Formato" AssociatedControlID="ddlFormato" Clave="reportes.formato" />
                                <asp:DropDownList ID="ddlFormato" runat="server" CssClass="modal-campo">
                                    <asp:ListItem Text="PDF" Value="PDF" />
                                </asp:DropDownList>
                            </div>
                            <div class="grupo-campo">
                                <asp:Label runat="server" Text="Idioma del reporte" AssociatedControlID="ddlIdiomaReporte" Clave="reportes.idioma" />
                                <asp:DropDownList ID="ddlIdiomaReporte" runat="server" CssClass="modal-campo" />
                            </div>
                        </div>

                        <div class="grupo-campo">
                            <label runat="server" clave="reportes.secciones">Secciones a incluir</label>
                            <div class="rep-checks">
                                <asp:CheckBox ID="chkResumenEjecutivo" runat="server" CssClass="rep-check" Text="Resumen ejecutivo" Clave="reportes.resumen" Checked="true" />
                                <asp:CheckBox ID="chkEvolucionVentas" runat="server" CssClass="rep-check" Text="Evolución de ventas" Clave="reportes.evolucionVentas" Checked="true" />
                                <asp:CheckBox ID="chkIndicadoresPrincipales" runat="server" CssClass="rep-check" Text="Indicadores por sector" Clave="indicadores.porSector" Checked="true" />
                                <asp:CheckBox ID="chkRecomendaciones" runat="server" CssClass="rep-check" Text="Recomendaciones" Clave="reportes.recomendaciones" Checked="true" />
                                <asp:CheckBox ID="chkComparacionEscenarios" runat="server" CssClass="rep-check" Text="Comparación de escenarios" Clave="principal.comparacion.titulo" Checked="true" />
                                <asp:CheckBox ID="chkOportunidades" runat="server" CssClass="rep-check" Text="Oportunidades de productos" Clave="pdf.oportunidades" Checked="true" />
                                <asp:CheckBox ID="chkEstadoStock" runat="server" CssClass="rep-check" Text="Estado de stock" Clave="reportes.estadoStock" Checked="true" />
                                <asp:CheckBox ID="chkHistorialVentas" runat="server" CssClass="rep-check" Text="Historial de ventas" Clave="reportes.historialVentas" Checked="true" />
                                <asp:CheckBox ID="chkDetalleProductos" runat="server" CssClass="rep-check" Text="Detalle de productos" Clave="reportes.detalleProductos" Checked="true" />
                            </div>
                        </div>

                        <asp:Button ID="btnGenerarReporte" runat="server" CssClass="boton-nuevo" Text="Generar Reporte" Clave="reportes.btnGenerar" OnClick="btnGenerarReporte_Click" CausesValidation="false" />
                    </div>

                    <div class="tarjeta">
                        <h2 runat="server" clave="reportes.recientes">Reportes Recientes</h2>
                        <asp:Repeater ID="rptReportesRecientes" runat="server" OnItemCommand="rptReportesRecientes_ItemCommand">
                            <ItemTemplate>
                                <div class="rep-item">
                                    <span class="avatar avatar-cuadrado">&#128196;</span>
                                    <div class="rep-item-info">
                                        <div class="texto-principal"><%# Eval("Titulo") %></div>
                                        <div class="texto-secundario"><%# Eval("FechaGeneracion", "{0:dd/MM/yyyy HH:mm}") %><%# string.IsNullOrEmpty(Eval("Descripcion") as string) ? "" : " · " + HttpUtility.HtmlEncode(Eval("Descripcion")) %></div>
                                        <span class="etiqueta etiqueta-info"><%# TraducirValor(Eval("TipoReporte")) %></span>
                                    </div>
                                    <div class="rep-item-acciones">
                                        <asp:LinkButton runat="server" CssClass="boton-icono" ToolTip="Ver" ClaveTitle="reportes.ver"
                                            CommandName="VerReporte" CommandArgument='<%# Eval("CodigoReporte") %>' Text="Ver" Clave="reportes.ver" />
                                        <asp:LinkButton runat="server" CssClass="boton-icono" ToolTip="Bajar" ClaveTitle="reportes.bajar"
                                            CommandName="DescargarReporte" CommandArgument='<%# Eval("CodigoReporte") %>'>&#8595; <%# Traducir("reportes.bajar") %></asp:LinkButton>
                                        <asp:LinkButton runat="server" CssClass="boton-icono rep-eliminar" ToolTip="Eliminar" ClaveTitle="reportes.eliminar" ClaveConfirmar="reportes.confirmarEliminar"
                                            CommandName="EliminarReporte" CommandArgument='<%# Eval("CodigoReporte") %>' Text="Eliminar" Clave="comun.eliminar" />
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:Label ID="lblSinReportes" runat="server" Text="Todavía no generaste ningún reporte." Clave="reportes.sinReportes" Visible="false" CssClass="sin-resultados" />
                    </div>
                </div>
            </div>
        </div>
    </form>
    <script src="/JS/ReportesJS.js"></script>
</body>
</html>
