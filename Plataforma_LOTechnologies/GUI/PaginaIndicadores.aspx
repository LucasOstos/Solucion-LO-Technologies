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
    <link rel="stylesheet" type="text/css" href="/Estilos/MapaLocal.css" />
</head>
<body>
    <form id="formIndicadores" runat="server">
        <div class="pagina">
            <div class="contenedor">

                <uc:BotonVolverMenu ID="volverMenu" runat="server" />

                <div class="encabezado">
                    <div>
                        <h1 runat="server" Clave="principal.indicadores.titulo">Indicadores y Recomendaciones</h1>
                        <p class="subtitulo" runat="server" Clave="indicadores.subtitulo">CUN 009 — Indicadores clave y recomendaciones automáticas</p>
                    </div>
                </div>

                <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" Visible="false" EnableViewState="false" />

                <div class="filtros">
                    <asp:DropDownList ID="ddlLocal" runat="server" CssClass="selector" AutoPostBack="true" OnSelectedIndexChanged="Filtro_Changed" />
                    <asp:DropDownList ID="ddlPeriodo" runat="server" CssClass="selector">
                        <asp:ListItem Text="Último mes" Value="ultimo_mes" Clave="stock.ultimoMes" />
                        <asp:ListItem Text="Últimos 3 meses" Value="3_meses" Clave="stock.ultimos3" />
                        <asp:ListItem Text="Últimos 5 meses" Value="5_meses" Clave="stock.ultimos5" />
                    </asp:DropDownList>
                    <asp:Button ID="btnAplicar" runat="server" CssClass="boton-buscar" Text="Aplicar" Clave="comun.aplicar" OnClick="btnAplicar_Click" CausesValidation="false" />
                    <span class="texto-secundario ind-rango"><asp:Literal ID="litRangoPeriodo" runat="server" /></span>
                </div>

                <asp:Panel ID="pnlResumenLocal" runat="server" CssClass="ind-stats" Visible="false">
                    <div class="ind-stat-card">
                        <div class="texto-secundario" runat="server" Clave="indicadores.ventasPeriodo">Ventas del período</div>
                        <div class="ind-stat-valor"><asp:Literal ID="litVentasPeriodo" runat="server" /></div>
                        <span class="texto-secundario ind-stat-ayuda"><asp:Literal ID="litVariacionLocal" runat="server" /></span>
                    </div>
                    <div class="ind-stat-card">
                        <div class="texto-secundario" runat="server" Clave="indicadores.margenPeriodo">Margen del período</div>
                        <div class="ind-stat-valor"><asp:Literal ID="litMargenPeriodo" runat="server" /></div>
                        <asp:Label ID="lblSinCostos" runat="server" CssClass="texto-secundario ind-stat-ayuda" Visible="false"
                            Text="Cargá el costo unitario de los productos para calcularlo." Clave="indicadores.sinCostos" />
                    </div>
                    <div class="ind-stat-card">
                        <div class="texto-secundario" runat="server" Clave="indicadores.ventasM2Local">Ventas por m² del local</div>
                        <div class="ind-stat-valor"><asp:Literal ID="litVentasM2Local" runat="server" /></div>
                        <asp:Label ID="lblSinSuperficie" runat="server" CssClass="texto-secundario ind-stat-ayuda" Visible="false"
                            Text="Cargá la superficie del local para calcularlo." Clave="indicadores.sinSuperficieLocal" />
                    </div>
                </asp:Panel>

                <div class="tarjeta">
                    <h2>&#128200; <span runat="server" Clave="indicadores.recomendaciones">Recomendaciones Automáticas</span></h2>
                    <asp:Repeater ID="rptRecomendaciones" runat="server">
                        <ItemTemplate>
                            <div class='<%# "ind-recomendacion ind-prioridad-" + Eval("Prioridad").ToString().ToLower() %>'>
                                <div class="ind-recomendacion-texto">
                                    <div class="texto-principal"><%# TextoRecomendacion(Container.DataItem, false) %></div>
                                    <div class="texto-secundario"><%# TextoRecomendacion(Container.DataItem, true) %></div>
                                </div>
                                <div class="ind-recomendacion-acciones">
                                    <span class='<%# "etiqueta ind-etiqueta-" + Eval("Prioridad").ToString().ToLower() %>'><%# TraducirValor(Eval("Prioridad")) %></span>
                                    <asp:HyperLink runat="server" CssClass="ind-crear-escenario" NavigateUrl='<%# UrlCrearEscenario(Container.DataItem) %>'
                                        Visible='<%# UrlCrearEscenario(Container.DataItem) != null %>' Text="Crear escenario" Clave="indicadores.crearEscenario"
                                        ClaveTitle="indicadores.crearEscenarioAyuda" />
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Label ID="lblSinRecomendaciones" runat="server" Text="No hay recomendaciones generadas para este período." Clave="indicadores.sinRecomendaciones" Visible="false" CssClass="sin-resultados" />
                </div>

                <div class="tarjeta">
                    <h2 runat="server" Clave="indicadores.porSector">Indicadores por Sector</h2>
                    <table class="tabla">
                        <thead>
                            <tr>
                                <th runat="server" Clave="layout.colSector">Sector</th>
                                <th runat="server" Clave="indicadores.colVentas">Ventas</th>
                                <th runat="server" Clave="indicadores.colVariacion">vs. período anterior</th>
                                <th runat="server" Clave="stock.colMargen">Margen</th>
                                <th runat="server" Clave="indicadores.colVentasM2">Ventas/m²</th>
                                <th runat="server" Clave="layout.colCirculacion">Circulación</th>
                                <th runat="server" Clave="indicadores.colRotacionMes">Rotación mensual</th>
                                <th runat="server" Clave="indicadores.colCobertura">Cobertura</th>
                                <th runat="server" Clave="indicadores.colIndice">Índice Global</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptIndicadoresSector" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td class="texto-principal"><%# Eval("Sector") %></td>
                                        <td><%# Eval("Ventas", "{0:$#,##0}") %></td>
                                        <td><%# MostrarVariacion(Eval("VariacionVentas"), Eval("Ventas"), Eval("VentasAnterior")) %></td>
                                        <td><%# Eval("Margen") == null ? "-" : Eval("Margen") + "%" %></td>
                                        <td><%# Eval("VentasPorM2") == null ? "-" : Eval("VentasPorM2", "{0:$#,##0}") %></td>
                                        <td><%# Eval("Circulacion") %>%</td>
                                        <td><%# Eval("Rotacion") %>%</td>
                                        <td><%# MostrarCobertura(Eval("CoberturaDias"), Eval("StockDisponible")) %></td>
                                        <td><span class='<%# "etiqueta " + Eval("ClaseIndiceGlobal") %>'><%# Eval("IndiceGlobal") %></span></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                    <asp:Label ID="lblSinIndicadores" runat="server" Text="No existen datos suficientes para generar indicadores completos." Clave="indicadores.sinIndicadores" Visible="false" CssClass="sin-resultados" />

                    <details class="ind-como-se-calcula">
                        <summary runat="server" Clave="indicadores.comoSeCalcula">¿Cómo se calcula cada indicador?</summary>
                        <dl>
                            <dt runat="server" Clave="indicadores.colVariacion">vs. período anterior</dt>
                            <dd runat="server" Clave="indicadores.ayudaVariacion">Ventas del período contra el mismo tramo del período anterior (por ejemplo, del 1 al 4 de noviembre contra del 1 al 4 de octubre).</dd>
                            <dt runat="server" Clave="stock.colMargen">Margen</dt>
                            <dd runat="server" Clave="indicadores.ayudaMargen">(Ventas − unidades × costo) ÷ ventas, solo con los productos que tienen costo cargado.</dd>
                            <dt runat="server" Clave="indicadores.colVentasM2">Ventas/m²</dt>
                            <dd runat="server" Clave="indicadores.ayudaVentasM2">Ventas del sector ÷ superficie del sector. Mide cuánto rinde cada metro cuadrado.</dd>
                            <dt runat="server" Clave="layout.colCirculacion">Circulación</dt>
                            <dd runat="server" Clave="indicadores.ayudaCirculacion">% estimado de clientes que pasan por el sector. Se carga en Layout y sectores (observación o conteo en el local).</dd>
                            <dt runat="server" Clave="indicadores.colRotacionMes">Rotación mensual</dt>
                            <dd runat="server" Clave="indicadores.ayudaRotacion">Unidades vendidas por mes ÷ stock actual × 100. 200% significa que por mes se vende el doble del stock disponible.</dd>
                            <dt runat="server" Clave="indicadores.colCobertura">Cobertura</dt>
                            <dd runat="server" Clave="indicadores.ayudaCobertura">Stock actual ÷ unidades vendidas por día: cuántos días alcanza el stock al ritmo de venta del período.</dd>
                            <dt runat="server" Clave="indicadores.colIndice">Índice global</dt>
                            <dd runat="server" Clave="indicadores.ayudaIndice">De 0 a 100: 40% circulación + 30% rotación + 30% ventas/m². Rotación y ventas/m² se comparan con el mejor sector del local (que saca 100). Sin superficie cargada se usan solo circulación y rotación. Verde desde 70, amarillo desde 50.</dd>
                        </dl>
                    </details>
                </div>

                <asp:Panel ID="pnlEvolucion" runat="server" CssClass="tarjeta" Visible="false">
                    <h2 runat="server" Clave="indicadores.evolucion">Evolución de las ventas del local</h2>
                    <p class="texto-secundario" runat="server" Clave="indicadores.ayudaEvolucion">Últimos 6 meses. El mes actual todavía está en curso.</p>
                    <asp:Literal ID="litEvolucion" runat="server" />
                </asp:Panel>

                <asp:Panel ID="pnlMapa" runat="server" CssClass="tarjeta" Visible="false">
                    <div class="mapa-encabezado">
                        <h2 runat="server" Clave="mapa.titulo">Mapa del local</h2>
                        <div>
                            <asp:Label runat="server" Text="Indicador" AssociatedControlID="ddlIndicadorMapa" Clave="mapa.indicador" />
                            <asp:DropDownList ID="ddlIndicadorMapa" runat="server" CssClass="selector" AutoPostBack="true" OnSelectedIndexChanged="Filtro_Changed">
                                <asp:ListItem Text="Ventas" Value="ventas" Clave="indicadores.colVentas" />
                                <asp:ListItem Text="Ventas/m²" Value="ventasM2" Clave="indicadores.colVentasM2" />
                                <asp:ListItem Text="Margen" Value="margen" Clave="stock.colMargen" />
                                <asp:ListItem Text="Circulación" Value="circulacion" Clave="layout.colCirculacion" />
                                <asp:ListItem Text="Rotación" Value="rotacion" Clave="indicadores.colRotacion" />
                                <asp:ListItem Text="Índice global" Value="indice" Clave="indicadores.colIndice" />
                            </asp:DropDownList>
                        </div>
                    </div>
                    <p class="texto-secundario" runat="server" Clave="mapa.ayuda">Cada sector se ubica en una de las 9 zonas del plano según su ubicación. El frente (entrada) es la parte inferior de la imagen.</p>
                    <asp:Literal ID="litMapa" runat="server" />
                </asp:Panel>

            </div>
        </div>
    </form>
</body>
</html>
