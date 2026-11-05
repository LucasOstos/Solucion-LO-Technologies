<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PaginaAnalisisProductos.aspx.cs" Inherits="PaginaAnalisisProductos" %>

<%@ Register TagPrefix="uc" TagName="BotonVolverMenu" Src="~/Controles/BotonVolverMenu.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Análisis de Productos - LO Technologies</title>
    <link rel="stylesheet" type="text/css" href="/Estilos/BotonVolverMenu.css" />
    <link rel="stylesheet" type="text/css" href="/Estilos/Comun.css" />
    <link rel="stylesheet" type="text/css" href="/Estilos/EstilosAnalisisProductos.css" />
</head>
<body>
    <form id="formAnalisisProductos" runat="server">
        <div class="pagina">
            <div class="contenedor">

                <uc:BotonVolverMenu ID="volverMenu" runat="server" />

                <div class="encabezado">
                    <div>
                        <h1 runat="server" Clave="principal.analisisProductos.titulo">Análisis de Productos</h1>
                        <p class="subtitulo" runat="server" Clave="analisis.subtitulo">CUN 011 — Detectar oportunidades de reubicación de productos</p>
                    </div>
                </div>

                <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" Visible="false" EnableViewState="false" />

                <div class="tarjeta">
                    <h2 runat="server" Clave="analisis.configurar">Configurar análisis</h2>
                    <div class="fila-formulario">
                        <div class="grupo-campo">
                            <asp:Label runat="server" Text="Local *" AssociatedControlID="ddlLocal" Clave="comun.localObligatorio" />
                            <asp:DropDownList ID="ddlLocal" runat="server" CssClass="modal-campo" />
                        </div>
                        <div class="grupo-campo">
                            <asp:Label runat="server" Text="Período desde *" AssociatedControlID="tbPeriodoDesde" Clave="comun.periodoDesde" />
                            <asp:TextBox ID="tbPeriodoDesde" runat="server" CssClass="modal-campo" TextMode="Date" />
                        </div>
                        <div class="grupo-campo">
                            <asp:Label runat="server" Text="Período hasta *" AssociatedControlID="tbPeriodoHasta" Clave="comun.periodoHasta" />
                            <asp:TextBox ID="tbPeriodoHasta" runat="server" CssClass="modal-campo" TextMode="Date" />
                        </div>
                    </div>
                    <p class="texto-secundario" runat="server" Clave="analisis.datosRequeridos">Datos requeridos: productos, sectores, ventas, stock y circulación del local seleccionado.</p>
                    <asp:Button ID="btnDetectar" runat="server" CssClass="boton-nuevo" Text="Analizar productos" Clave="analisis.btnAnalizar" OnClick="btnDetectar_Click" CausesValidation="false" />
                    <details class="ap-como-se-detecta">
                        <summary runat="server" Clave="analisis.comoSeDetecta">¿Cómo se detecta cada oportunidad?</summary>
                        <dl>
                            <dt runat="server" Clave="analisis.tipoEstrellaEscondida">Estrella escondida</dt>
                            <dd runat="server" Clave="analisis.ayudaEstrella">Está en el 25% de productos que más venden del local (por importe), rota (stock para 60 días o menos) y está en un sector con circulación 10 puntos o más por debajo del promedio. Se sugiere una exhibición adicional en un sector compatible con más tránsito; si no hay ninguno, acercar su sector al frente.</dd>
                            <dt runat="server" Clave="analisis.tipoLugarPrivilegiado">Ocupa lugar privilegiado</dt>
                            <dd runat="server" Clave="analisis.ayudaLugar">Está en el 25% que menos vende (o no vendió nada) y ocupa un sector con circulación 10 puntos o más por encima del promedio.</dd>
                            <dt runat="server" Clave="analisis.tipoSinMovimiento">Sin movimiento</dt>
                            <dd runat="server" Clave="analisis.ayudaSinMovimiento">Tiene stock en el local y ninguna venta en el período.</dd>
                            <dt runat="server" Clave="analisis.tipoRiesgoQuiebre">Riesgo de quiebre</dt>
                            <dd runat="server" Clave="analisis.ayudaQuiebre">Al ritmo de venta del período, el stock alcanza para menos de 7 días (o ya no hay stock).</dd>
                            <dt runat="server" Clave="analisis.tipoSinUbicar">Sin ubicar con ventas</dt>
                            <dd runat="server" Clave="analisis.ayudaSinUbicar">Se vende en el local pero no está ubicado en ningún sector: sus ventas no cuentan en los indicadores por sector.</dd>
                        </dl>
                    </details>
                </div>

                <asp:Panel ID="pnlResultados" runat="server" Visible="false">

                    <div class="ap-stats">
                        <div class="ap-stat-card">
                            <div class="ap-stat-valor">
                                <asp:Literal ID="litProductosAnalizados" runat="server" Text="0" /></div>
                            <div class="texto-secundario" runat="server" Clave="analisis.analizados">Productos analizados</div>
                        </div>
                        <div class="ap-stat-card">
                            <div class="ap-stat-valor ap-color-alta">
                                <asp:Literal ID="litPrioridadAlta" runat="server" Text="0" /></div>
                            <div class="texto-secundario" runat="server" Clave="analisis.prioridadAlta">Prioridad Alta</div>
                        </div>
                        <div class="ap-stat-card">
                            <div class="ap-stat-valor ap-color-media">
                                <asp:Literal ID="litPrioridadMedia" runat="server" Text="0" /></div>
                            <div class="texto-secundario" runat="server" Clave="analisis.prioridadMedia">Prioridad Media</div>
                        </div>
                        <div class="ap-stat-card">
                            <div class="ap-stat-valor ap-color-baja">
                                <asp:Literal ID="litPrioridadBaja" runat="server" Text="0" /></div>
                            <div class="texto-secundario" runat="server" Clave="analisis.prioridadBaja">Prioridad Baja</div>
                        </div>
                    </div>

                    <div class="filtros">
                        <asp:TextBox ID="tbBuscarProducto" runat="server" CssClass="campo-buscar" placeholder="Buscar producto o categoría..." ClavePlaceholder="analisis.buscarPlaceholder"
                            ClaveAria="analisis.buscarPlaceholder" AutoPostBack="true" OnTextChanged="Filtro_Changed" />
                        <asp:DropDownList ID="ddlFiltroTipo" runat="server" CssClass="selector" AutoPostBack="true" OnSelectedIndexChanged="Filtro_Changed" ClaveAria="analisis.todosTipos">
                            <asp:ListItem Text="Todos los tipos" Value="" Clave="analisis.todosTipos" />
                            <asp:ListItem Text="Estrella escondida" Value="EstrellaEscondida" Clave="analisis.tipoEstrellaEscondida" />
                            <asp:ListItem Text="Ocupa lugar privilegiado" Value="LugarPrivilegiado" Clave="analisis.tipoLugarPrivilegiado" />
                            <asp:ListItem Text="Sin movimiento" Value="SinMovimiento" Clave="analisis.tipoSinMovimiento" />
                            <asp:ListItem Text="Riesgo de quiebre" Value="RiesgoQuiebre" Clave="analisis.tipoRiesgoQuiebre" />
                            <asp:ListItem Text="Sin ubicar con ventas" Value="SinUbicar" Clave="analisis.tipoSinUbicar" />
                        </asp:DropDownList>
                        <asp:DropDownList ID="ddlFiltroPrioridad" runat="server" CssClass="selector" AutoPostBack="true" OnSelectedIndexChanged="Filtro_Changed" ClaveAria="analisis.todasPrioridades">
                            <asp:ListItem Text="Todas las prioridades" Value="" Clave="analisis.todasPrioridades" />
                            <asp:ListItem Text="Alta" Value="Alta" Clave="valor.alta" />
                            <asp:ListItem Text="Media" Value="Media" Clave="valor.media" />
                            <asp:ListItem Text="Baja" Value="Baja" Clave="valor.baja" />
                        </asp:DropDownList>
                    </div>

                    <div class="tarjeta">
                        <h2 runat="server" Clave="analisis.oportunidades">Oportunidades detectadas</h2>
                        <asp:Repeater ID="rptCandidatos" runat="server">
                            <ItemTemplate>
                                <div class='<%# "ap-candidato ap-borde-" + Eval("Prioridad").ToString().ToLower() %>'>
                                    <div class="ap-candidato-info">
                                        <div class="texto-principal">
                                            <%# HttpUtility.HtmlEncode(Eval("CodigoInterno") + " - " + Eval("NombreProducto")) %>
                                            <span class='<%# "etiqueta ap-etiqueta-" + Eval("Prioridad").ToString().ToLower() %>'><%# TraducirValor(Eval("Prioridad")) %></span>
                                            <span class="etiqueta ap-etiqueta-tipo"><%# HttpUtility.HtmlEncode(TraducirTipo(Eval("Tipo"))) %></span>
                                        </div>
                                        <div class="texto-secundario ap-candidato-ubicacion">
                                            <%# HttpUtility.HtmlEncode(Eval("NombreCategoria")) %> &middot;
                                            <span class="etiqueta etiqueta-info"><%# HttpUtility.HtmlEncode(MostrarSector(Eval("SectorActual"))) %></span>
                                            <asp:PlaceHolder runat="server" Visible='<%# Eval("SectorSugerido") != null %>'>
                                                <span aria-hidden="true">&rarr;</span>
                                                <span class="etiqueta etiqueta-activo"><%# HttpUtility.HtmlEncode(Eval("SectorSugerido")) %></span>
                                            </asp:PlaceHolder>
                                        </div>
                                        <div class="ap-motivo"><%# HttpUtility.HtmlEncode(MostrarMotivo(Container.DataItem)) %></div>
                                    </div>
                                    <div class="ap-candidato-metricas">
                                        <div><span class="texto-principal"><%# Eval("Ventas") %></span><br />
                                            <span class="texto-secundario"><%# Traducir("stock.colUnidades") %></span></div>
                                        <div><span class="texto-principal"><%# Eval("Importe", "{0:$#,##0}") %></span><br />
                                            <span class="texto-secundario"><%# Traducir("stock.colMonto") %></span></div>
                                        <div><span class="texto-principal"><%# MostrarMargen(Eval("Margen")) %></span><br />
                                            <span class="texto-secundario"><%# Traducir("stock.colMargen") %></span></div>
                                        <div><span class="texto-principal"><%# HttpUtility.HtmlEncode(MostrarCobertura(Eval("CoberturaDias"))) %></span><br />
                                            <span class="texto-secundario"><%# Traducir("indicadores.colCobertura") %></span></div>
                                        <div><span class="texto-principal"><%# Eval("SectorActual") == null ? "-" : Convert.ToDecimal(Eval("Circulacion")).ToString("0.#") + "%" %></span><br />
                                            <span class="texto-secundario"><%# Traducir("layout.colCirculacion") %></span></div>
                                    </div>
                                    <asp:HyperLink runat="server" CssClass="ap-crear-escenario" NavigateUrl='<%# UrlCrearEscenario(Container.DataItem) %>'
                                        Visible='<%# UrlCrearEscenario(Container.DataItem) != null %>' Text="Crear escenario" Clave="indicadores.crearEscenario"
                                        ClaveTitle="indicadores.crearEscenarioAyuda" />
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:Label ID="lblSinCandidatos" runat="server" Text="No se detectaron oportunidades para el período y los filtros elegidos." Clave="analisis.sinOportunidades" Visible="false" CssClass="sin-resultados" />
                    </div>

                    <div class="tarjeta">
                        <h2 runat="server" Clave="analisis.sinRecomendacion">Productos analizados sin recomendación</h2>
                        <table class="tabla">
                            <thead>
                                <tr>
                                    <th runat="server" Clave="productos.colProducto">Producto</th>
                                    <th runat="server" Clave="productos.colCategoria">Categoría</th>
                                    <th runat="server" Clave="analisis.colSectorActual">Sector Actual</th>
                                    <th runat="server" Clave="stock.colUnidades">Unidades</th>
                                    <th runat="server" Clave="stock.colMonto">Monto</th>
                                    <th runat="server" Clave="indicadores.colCobertura">Cobertura</th>
                                    <th runat="server" Clave="layout.colCirculacion">Circulación</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptSinRecomendacion" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td class="texto-principal"><%# HttpUtility.HtmlEncode(Eval("CodigoInterno") + " - " + Eval("NombreProducto")) %></td>
                                            <td><%# HttpUtility.HtmlEncode(Eval("NombreCategoria")) %></td>
                                            <td><%# HttpUtility.HtmlEncode(MostrarSector(Eval("SectorActual"))) %></td>
                                            <td><%# Eval("Ventas") %></td>
                                            <td><%# Eval("Importe", "{0:$#,##0}") %></td>
                                            <td><%# HttpUtility.HtmlEncode(MostrarCobertura(Eval("CoberturaDias"))) %></td>
                                            <td><%# Eval("SectorActual") == null ? "-" : Convert.ToDecimal(Eval("Circulacion")).ToString("0.#") + "%" %></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                        <asp:Label ID="lblSinAnalizados" runat="server" Text="Todos los productos analizados tienen alguna observación." Clave="analisis.todosConObservacion" Visible="false" CssClass="sin-resultados" />
                    </div>
                </asp:Panel>
            </div>
        </div>
    </form>
</body>
</html>
