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
    <link rel="stylesheet" type="text/css" href="/Estilos/MapaLocal.css" />
</head>
<body>
    <form id="formComparacion" runat="server">
        <div class="pagina">
            <div class="contenedor">

                <uc:BotonVolverMenu ID="volverMenu" runat="server" />

                <div class="encabezado">
                    <div>
                        <h1 runat="server" Clave="principal.comparacion.titulo">Comparación de Escenarios</h1>
                        <p class="subtitulo" runat="server" Clave="comparacion.subtitulo">CUN 008 — Comparar escenario actual vs. escenario propuesto</p>
                    </div>
                </div>

                <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" Visible="false" EnableViewState="false" />

                <div class="tarjeta">
                    <div class="fila-formulario">
                        <div class="grupo-campo">
                            <asp:Label runat="server" Text="Local" AssociatedControlID="ddlLocal" Clave="comun.local" />
                            <asp:DropDownList ID="ddlLocal" runat="server" CssClass="modal-campo" AutoPostBack="true" OnSelectedIndexChanged="ddlLocal_SelectedIndexChanged" />
                        </div>
                        <div class="grupo-campo">
                            <asp:Label runat="server" Text="Período de análisis" AssociatedControlID="ddlPeriodo" Clave="comparacion.periodo" />
                            <asp:DropDownList ID="ddlPeriodo" runat="server" CssClass="modal-campo">
                                <asp:ListItem Text="Último mes" Value="ultimo_mes" Clave="stock.ultimoMes" />
                                <asp:ListItem Text="Últimos 3 meses" Value="3_meses" Clave="stock.ultimos3" />
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="fila-formulario">
                        <div class="grupo-campo">
                            <asp:Label runat="server" Text="Escenario actual *" AssociatedControlID="ddlEscenarioA" Clave="comparacion.escenarioActual" />
                            <asp:DropDownList ID="ddlEscenarioA" runat="server" CssClass="modal-campo" />
                        </div>
                        <div class="grupo-campo">
                            <asp:Label runat="server" Text="Escenario propuesto *" AssociatedControlID="ddlEscenarioB" Clave="comparacion.escenarioPropuesto" />
                            <asp:DropDownList ID="ddlEscenarioB" runat="server" CssClass="modal-campo" />
                        </div>
                    </div>
                    <asp:Button ID="btnComparar" runat="server" CssClass="boton-nuevo" Text="Comparar Escenarios" Clave="comparacion.btnComparar" OnClick="btnComparar_Click" CausesValidation="false" />
                </div>

                <asp:Panel ID="pnlResultados" runat="server" Visible="false">
                    <%-- Conclusión automática: qué cambia del escenario propuesto frente al actual --%>
                    <div class="tarjeta ce-conclusion" role="status">
                        <h2 runat="server" Clave="comparacion.conclusion">Conclusión</h2>
                        <p><asp:Literal ID="litConclusion" runat="server" /></p>
                        <p class="texto-secundario"><asp:Literal ID="litConclusionDetalle" runat="server" /></p>
                    </div>
                    <div class="ce-comparacion">
                        <div class="tarjeta">
                            <h2>
                                <asp:Literal ID="litNombreEscenarioA" runat="server" />
                                <span runat="server" Clave="comparacion.actual">(actual)</span></h2>
                            <p class="texto-secundario">
                                <span runat="server" Clave="escenarios.layoutBase">Layout base:</span>
                                <asp:Literal ID="litLayoutA" runat="server" />
                            </p>
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
                                <span runat="server" Clave="comparacion.propuesto">(propuesto)</span></h2>
                            <p class="texto-secundario">
                                <span runat="server" Clave="escenarios.layoutBase">Layout base:</span>
                                <asp:Literal ID="litLayoutB" runat="server" />
                            </p>
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
                        <h2 runat="server" Clave="mapa.tituloComparacion">Mapa del local: ventas estimadas</h2>
                        <p class="texto-secundario" runat="server" Clave="mapa.ayudaComparacion">Los dos mapas usan la misma escala de colores, así se puede ver en qué zonas mejora o empeora el escenario propuesto.</p>
                        <div class="mapa-comparacion">
                            <div><strong><asp:Literal ID="litTituloMapaA" runat="server" /></strong><asp:Literal ID="litMapaA" runat="server" /></div>
                            <div><strong><asp:Literal ID="litTituloMapaB" runat="server" /></strong><asp:Literal ID="litMapaB" runat="server" /></div>
                        </div>
                    </div>

                    <div class="tarjeta">
                        <h2 runat="server" Clave="comparacion.diferencias">Diferencias principales</h2>
                        <table class="tabla">
                            <thead>
                                <tr>
                                    <th runat="server" Clave="layout.colSector">Indicador</th>
                                    <th runat="server" Clave="comparacion.colActual">Actual</th>
                                    <th runat="server" Clave="comparacion.colPropuesto">Propuesto</th>
                                    <th runat="server" Clave="comparacion.colDiferencia">Diferencia</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptDiferencias" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td class="texto-principal"><%# HttpUtility.HtmlEncode(Eval("Indicador")) %></td>
                                            <td><%# Eval("ValorActual") %></td>
                                            <td><%# Eval("ValorPropuesto") %></td>
                                            <td><span class='<%# "etiqueta " + Eval("ClaseDiferencia") %>'><%# Eval("Diferencia") %></span></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>

                    <details class="tarjeta ce-supuestos">
                        <summary runat="server" Clave="escenarios.supuestos">Supuestos de la simulación</summary>
                        <p class="texto-secundario" runat="server" Clave="escenarios.ayudaSupuestos">Cada acción cambia los valores reales del último mes del sector elegido en estos porcentajes. Son estimaciones del modelo (no una predicción exacta) y no tienen en cuenta el efecto sobre los otros sectores (por ejemplo, el sector que deja el frente).</p>
                        <table class="tabla">
                            <thead>
                                <tr>
                                    <th runat="server" Clave="escenarios.colAccion">Acción</th>
                                    <th runat="server" Clave="layout.colCirculacion">Circulación</th>
                                    <th runat="server" Clave="indicadores.colVentas">Ventas</th>
                                    <th runat="server" Clave="layout.colSuperficie">Superficie</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptSupuestos" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td class="texto-principal"><%# TraducirValor(Eval("Accion")) %></td>
                                            <td><%# MostrarDelta(Eval("DeltaCirculacion")) %></td>
                                            <td><%# MostrarDelta(Eval("DeltaVentas")) %></td>
                                            <td><%# MostrarDelta(Eval("DeltaSuperficie")) %></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </details>

                    <asp:Button ID="btnGuardarComparacion" runat="server" CssClass="boton-nuevo" Text="Generar reporte de esta comparación" Clave="comparacion.btnGenerarReporte" OnClick="btnGuardarComparacion_Click" CausesValidation="false" />
                </asp:Panel>
            </div>
        </div>
    </form>
</body>
</html>
