<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PaginaStockVentas.aspx.cs" Inherits="PaginaStockVentas" %>

<%@ Register TagPrefix="uc" TagName="BotonVolverMenu" Src="~/Controles/BotonVolverMenu.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Stock y Ventas - LO Technologies</title>
    <link rel="stylesheet" type="text/css" href="/Estilos/BotonVolverMenu.css" />
    <link rel="stylesheet" type="text/css" href="/Estilos/Comun.css" />
    <link rel="stylesheet" type="text/css" href="/Estilos/EstilosStockVentas.css" />
    <script type="text/javascript" src="/JS/StockVentasJS.js"></script>
</head>
<body>
    <form id="formStockVentas" runat="server">
        <div class="pagina">
            <div class="contenedor">

                <uc:BotonVolverMenu ID="volverMenu" runat="server" />

                <div class="encabezado">
                    <div>
                        <h1 runat="server" clave="principal.stockVentas.titulo">Stock y Ventas</h1>
                        <p class="subtitulo" runat="server" clave="stock.subtitulo">CUN 006 — Carga y gestión de datos de ventas y stock</p>
                    </div>
                    <div>
                        <asp:Button ID="btnImportarCSV" runat="server" CssClass="boton-secundario" Text="Importar CSV" Clave="stock.btnImportar" OnClick="btnImportarCSV_Click" CausesValidation="false" />
                        <asp:Button ID="btnCargaManual" runat="server" CssClass="boton-nuevo" Text="Carga Manual" Clave="stock.btnCargaManual" OnClick="btnCargaManual_Click" CausesValidation="false" />
                    </div>
                </div>

                <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" Visible="false" EnableViewState="false" />

                <div class="filtros">
                    <asp:DropDownList ID="ddlLocal" runat="server" CssClass="selector" ClaveAria="comun.local" />
                    <asp:DropDownList ID="ddlPeriodo" runat="server" CssClass="selector">
                        <asp:ListItem Text="Último mes" Value="ultimo_mes" Clave="stock.ultimoMes" />
                        <asp:ListItem Text="Últimos 3 meses" Value="3_meses" Clave="stock.ultimos3" />
                        <asp:ListItem Text="Últimos 5 meses" Value="5_meses" Clave="stock.ultimos5" />
                    </asp:DropDownList>
                    <asp:DropDownList ID="ddlAgrupacion" runat="server" CssClass="selector">
                        <asp:ListItem Text="Por Sector" Value="sector" Clave="stock.porSector" />
                        <asp:ListItem Text="Por Categoría" Value="categoria" Clave="stock.porCategoria" />
                        <asp:ListItem Text="Por Producto" Value="producto" Clave="stock.porProducto" />
                    </asp:DropDownList>
                    <asp:Button ID="btnAplicar" runat="server" CssClass="boton-buscar" Text="Aplicar" Clave="comun.aplicar" OnClick="btnAplicar_Click" CausesValidation="false" />
                </div>

                <div class="pestanias">
                    <span id="tabVentas" runat="server" clientidmode="Static" class="pestania pestania-activa" onclick="mostrarPestania('ventas')" clave="stock.tabVentas">Datos de Ventas</span>
                    <span id="tabStock" runat="server" clientidmode="Static" class="pestania" onclick="mostrarPestania('stock')" clave="stock.tabStock">Datos de Stock</span>
                </div>

                <div id="panelVentas" class="panel-pestania">

                    <div class="sv-stats">
                        <div class="sv-stat-card">
                            <div class="texto-secundario" runat="server" clave="stock.totalVentas">Total Ventas (mes)</div>
                            <div class="sv-stat-valor">
                                <asp:Literal ID="litTotalVentas" runat="server" Text="$0" />
                            </div>
                        </div>
                        <div class="sv-stat-card">
                            <div class="texto-secundario" runat="server" clave="stock.ticketPromedio">Ticket Promedio</div>
                            <div class="sv-stat-valor">
                                <asp:Literal ID="litTicketPromedio" runat="server" Text="$0" />
                            </div>
                        </div>
                        <div class="sv-stat-card">
                            <div class="texto-secundario" runat="server" clave="stock.unidadesVendidas">Unidades Vendidas</div>
                            <div class="sv-stat-valor">
                                <asp:Literal ID="litUnidadesVendidas" runat="server" Text="0" />
                            </div>
                        </div>
                        <div class="sv-stat-card">
                            <div class="texto-secundario" runat="server" clave="stock.margenTotal">Margen Total</div>
                            <div class="sv-stat-valor">
                                <asp:Literal ID="litMargenTotal" runat="server" Text="0%" />
                            </div>
                        </div>
                    </div>

                    <div class="tarjeta">
                        <h2 runat="server" clave="stock.ventasPorSector">Ventas por Sector</h2>
                        <div class="sv-grafico-placeholder" runat="server" clave="stock.graficoPendiente">
                            <%-- TODO: gráfico de barras (Chart.js o similar) - se arma cuando esta
                                 pantalla pase a ser funcional. Placeholder por ahora. --%>
                            Gráfico de ventas por sector (pendiente)
                        </div>
                    </div>

                    <div class="tarjeta">
                        <h2 runat="server" clave="stock.detalleVentas">Detalle de Ventas</h2>
                        <table class="tabla">
                            <thead>
                                <tr>
                                    <th runat="server" clave="stock.colPeriodo">Período</th>
                                    <th runat="server" clave="layout.colSector">Sector</th>
                                    <th runat="server" clave="productos.colCategoria">Categoría</th>
                                    <th runat="server" clave="stock.colUnidades">Unidades</th>
                                    <th runat="server" clave="stock.colMonto">Monto</th>
                                    <th runat="server" clave="stock.colMargen">Margen</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptVentas" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%# Eval("Periodo") %></td>
                                            <td><%# Eval("Sector") %></td>
                                            <td><%# Eval("Categoria") %></td>
                                            <td><%# Eval("Unidades") %></td>
                                            <td><%# Eval("Monto", "{0:$#,##0.00}") %></td>
                                            <td><%# Eval("Margen") %>%</td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                        <asp:Label ID="lblSinVentas" runat="server" Text="No hay ventas cargadas para el período seleccionado." Clave="stock.sinVentas" Visible="false" CssClass="sin-resultados" />
                    </div>
                </div>

                <div id="panelStock" class="panel-pestania" style="display: none;">
                    <div class="tarjeta">
                        <h2 runat="server" clave="stock.inventario">Inventario por Producto</h2>
                        <table class="tabla">
                            <thead>
                                <tr>
                                    <th runat="server" clave="productos.colProducto">Producto</th>
                                    <th runat="server" clave="productos.colCategoria">Categoría</th>
                                    <th runat="server" clave="stock.colStockActual">Stock Actual</th>
                                    <th runat="server" clave="stock.colMinimo">Mínimo</th>
                                    <th runat="server" clave="stock.colMaximo">Máximo</th>
                                    <th runat="server" clave="comun.estado">Estado</th>
                                    <th runat="server" clave="stock.colAccion">Acción</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptStock" runat="server" OnItemCommand="rptStock_ItemCommand">
                                    <ItemTemplate>
                                        <tr>
                                            <td class="texto-principal"><%# Eval("Producto") %></td>
                                            <td><%# Eval("Categoria") %></td>
                                            <td>
                                                <div class="sv-barra-stock">
                                                    <div class="sv-barra-relleno" style='<%# "width:" + Eval("PorcentajeStock") + "%" %>'></div>
                                                </div>
                                                <%# Eval("CantidadDisponible") %>
                                            </td>
                                            <td><%# Eval("Minimo") %></td>
                                            <td><%# Eval("Maximo") %></td>
                                            <td><span class='<%# "etiqueta " + Eval("ClaseEstadoStock") %>'><%# TraducirValor(Eval("EstadoStock")) %></span></td>
                                            <td>
                                                <asp:LinkButton runat="server" CssClass="boton-icono" ToolTip="Actualizar" ClaveTitle="stock.actualizar"
                                                    CommandName="ActualizarStock" CommandArgument='<%# Eval("CodigoStock") %>' Text="Actualizar" Clave="stock.actualizar" />
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                        <asp:Label ID="lblSinStock" runat="server" Text="No hay datos de stock cargados." Clave="stock.sinStock" Visible="false" CssClass="sin-resultados" />
                    </div>
                </div>

            </div>
        </div>

        <asp:Panel ID="pnlModalCarga" runat="server" ClientIDMode="Static" CssClass="modal-fondo" Style="display: none;">
            <div class="modal-caja">
                <div class="modal-encabezado">
                    <h3 runat="server" clave="stock.btnCargaManual">Carga Manual</h3>
                    <span class="modal-cerrar" onclick="cerrarModal('pnlModalCarga')">&times;</span>
                </div>
                <asp:HiddenField ID="hfCodigoStockEdicion" runat="server" />
                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Tipo de dato *" AssociatedControlID="ddlTipoCarga" Clave="stock.tipoDato" />
                    <asp:DropDownList ID="ddlTipoCarga" runat="server" CssClass="modal-campo">
                        <asp:ListItem Text="Venta" Value="Venta" Clave="valor.venta" />
                        <asp:ListItem Text="Stock" Value="Stock" Clave="valor.stock" />
                    </asp:DropDownList>
                </div>

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Producto / Categoría *" AssociatedControlID="ddlProductoCarga" Clave="stock.productoCategoria" />
                    <asp:DropDownList ID="ddlProductoCarga" runat="server" CssClass="modal-campo" />
                </div>

                <div class="fila-formulario">
                    <div class="grupo-campo">
                        <asp:Label runat="server" Text="Cantidad *" AssociatedControlID="tbCantidadCarga" Clave="stock.cantidad" />
                        <asp:TextBox ID="tbCantidadCarga" runat="server" CssClass="modal-campo" TextMode="Number" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="tbCantidadCarga" CssClass="campo-error"
                            ErrorMessage="Ingrese la cantidad" Clave="stock.errCantidad" Display="Dynamic" ValidationGroup="vgCarga" />
                    </div>
                    <div class="grupo-campo">
                        <asp:Label runat="server" Text="Importe total (si es venta)" AssociatedControlID="tbImporteCarga" Clave="stock.importe" />
                        <asp:TextBox ID="tbImporteCarga" runat="server" CssClass="modal-campo" TextMode="Number" />
                    </div>
                    <div class="fila-formulario">
                        <div class="grupo-campo">
                            <asp:Label runat="server" Text="Stock mínimo (si es stock)" AssociatedControlID="tbMinimoCarga" Clave="stock.minimo" />
                            <asp:TextBox ID="tbMinimoCarga" runat="server" CssClass="modal-campo" TextMode="Number" />
                        </div>
                        <div class="grupo-campo">
                            <asp:Label runat="server" Text="Stock máximo (si es stock)" AssociatedControlID="tbMaximoCarga" Clave="stock.maximo" />
                            <asp:TextBox ID="tbMaximoCarga" runat="server" CssClass="modal-campo" TextMode="Number" />
                        </div>
                    </div>
                </div>

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Período / Fecha *" AssociatedControlID="tbFechaCarga" Clave="stock.fecha" />
                    <asp:TextBox ID="tbFechaCarga" runat="server" CssClass="modal-campo" TextMode="Date" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="tbFechaCarga" CssClass="campo-error"
                        ErrorMessage="Ingrese la fecha" Clave="comun.errFecha" Display="Dynamic" ValidationGroup="vgCarga" />
                </div>

                <div class="modal-botones">
                    <asp:Button ID="btnCancelarCarga" runat="server" CssClass="modal-boton-cancelar"
                        Text="Cancelar" Clave="comun.cancelar" OnClick="btnCancelarCarga_Click" CausesValidation="false" />
                    <asp:Button ID="btnGuardarCarga" runat="server" CssClass="modal-boton-guardar"
                        Text="Guardar" Clave="comun.guardar" OnClick="btnGuardarCarga_Click" ValidationGroup="vgCarga" />
                </div>
            </div>
        </asp:Panel>
    </form>
</body>
</html>
