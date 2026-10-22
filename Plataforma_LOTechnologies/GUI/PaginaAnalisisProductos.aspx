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
                    <asp:Button ID="btnDetectar" runat="server" CssClass="boton-nuevo" Text="Detectar oportunidades de reubicación" Clave="analisis.btnDetectar" OnClick="btnDetectar_Click" CausesValidation="false" />
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
                        <asp:TextBox ID="tbBuscarProducto" runat="server" CssClass="campo-buscar" placeholder="Buscar producto o categoría..." ClavePlaceholder="analisis.buscarPlaceholder" />
                        <asp:DropDownList ID="ddlFiltroPrioridad" runat="server" CssClass="selector" AutoPostBack="true" OnSelectedIndexChanged="Filtro_Changed">
                            <asp:ListItem Text="Todas las prioridades" Value="" Clave="analisis.todasPrioridades" />
                            <asp:ListItem Text="Alta" Value="Alta" Clave="valor.alta" />
                            <asp:ListItem Text="Media" Value="Media" Clave="valor.media" />
                            <asp:ListItem Text="Baja" Value="Baja" Clave="valor.baja" />
                        </asp:DropDownList>
                    </div>

                    <div class="tarjeta">
                        <h2 runat="server" Clave="analisis.candidatos">Candidatos a reubicación</h2>
                        <asp:Repeater ID="rptCandidatos" runat="server">
                            <ItemTemplate>
                                <div class='<%# "ap-candidato ap-borde-" + Eval("Prioridad").ToString().ToLower() %>'>
                                    <div class="ap-candidato-info">
                                        <div class="texto-principal">
                                            <%# Eval("NombreProducto") %>
                                            <span class='<%# "etiqueta ap-etiqueta-" + Eval("Prioridad").ToString().ToLower() %>'><%# TraducirValor(Eval("Prioridad")) %></span>
                                        </div>
                                        <div class="texto-secundario"><%# Eval("NombreCategoria") %></div>
                                    </div>
                                    <div class="ap-candidato-movimiento">
                                        <span class="etiqueta etiqueta-info"><%# Eval("SectorActual") %></span>
                                        <span>&rarr;</span>
                                        <span class="etiqueta etiqueta-activo"><%# Eval("SectorSugerido") %></span>
                                    </div>
                                    <div class="ap-candidato-metricas">
                                        <div><span class="texto-principal"><%# Eval("Ventas") %></span><br />
                                            <span class="texto-secundario"><%# Traducir("indicadores.colVentas") %></span></div>
                                        <div><span class="texto-principal"><%# Eval("Rotacion") %>%</span><br />
                                            <span class="texto-secundario"><%# Traducir("indicadores.colRotacion") %></span></div>
                                        <div><span class="texto-principal"><%# Eval("Circulacion") %>%</span><br />
                                            <span class="texto-secundario"><%# Traducir("layout.colCirculacion") %></span></div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:Label ID="lblSinCandidatos" runat="server" Text="No se detectaron productos con oportunidad de reubicación para el período seleccionado." Clave="analisis.sinCandidatos" Visible="false" CssClass="sin-resultados" />
                    </div>

                    <div class="tarjeta">
                        <h2><span runat="server" Clave="analisis.sinRecomendacion">Productos analizados sin recomendación</span> <span class="etiqueta etiqueta-activo" runat="server" Clave="analisis.bienPosicionados">Bien posicionados</span></h2>
                        <table class="tabla">
                            <thead>
                                <tr>
                                    <th runat="server" Clave="productos.colProducto">Producto</th>
                                    <th runat="server" Clave="productos.colCategoria">Categoría</th>
                                    <th runat="server" Clave="analisis.colSectorActual">Sector Actual</th>
                                    <th runat="server" Clave="indicadores.colVentas">Ventas</th>
                                    <th runat="server" Clave="indicadores.colRotacion">Rotación</th>
                                    <th runat="server" Clave="layout.colCirculacion">Circulación</th>
                                    <th runat="server" Clave="comun.estado">Estado</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptSinRecomendacion" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td class="texto-principal"><%# Eval("NombreProducto") %></td>
                                            <td><%# Eval("NombreCategoria") %></td>
                                            <td><%# Eval("SectorActual") %></td>
                                            <td><%# Eval("Ventas") %></td>
                                            <td><%# Eval("Rotacion") %>%</td>
                                            <td><%# Eval("Circulacion") %>%</td>
                                            <td><span class="etiqueta etiqueta-activo">&#10003; <%# Traducir("analisis.optimo") %></span></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                </asp:Panel>
            </div>
        </div>
    </form>
</body>
</html>
