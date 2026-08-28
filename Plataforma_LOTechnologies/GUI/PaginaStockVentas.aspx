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
                        <h1>Stock y Ventas</h1>
                        <p class="subtitulo">CUN 006 — Carga y gestión de datos de ventas y stock</p>
                    </div>
                    <div>
                        <asp:Button ID="btnImportarCSV" runat="server" CssClass="boton-secundario" Text="Importar CSV" OnClick="btnImportarCSV_Click" CausesValidation="false" />
                        <asp:Button ID="btnCargaManual" runat="server" CssClass="boton-nuevo" Text="Carga Manual" OnClick="btnCargaManual_Click" CausesValidation="false" />
                    </div>
                </div>

                <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" Visible="false" />

                <div class="filtros">
                    <asp:DropDownList ID="ddlLocal" runat="server" CssClass="selector" />
                    <asp:DropDownList ID="ddlPeriodo" runat="server" CssClass="selector">
                        <asp:ListItem Text="Último mes" Value="ultimo_mes" />
                        <asp:ListItem Text="Últimos 3 meses" Value="3_meses" />
                        <asp:ListItem Text="Últimos 5 meses" Value="5_meses" />
                    </asp:DropDownList>
                    <asp:DropDownList ID="ddlAgrupacion" runat="server" CssClass="selector">
                        <asp:ListItem Text="Por Sector" Value="sector" />
                        <asp:ListItem Text="Por Categoría" Value="categoria" />
                        <asp:ListItem Text="Por Producto" Value="producto" />
                    </asp:DropDownList>
                    <asp:Button ID="btnAplicar" runat="server" CssClass="boton-buscar" Text="Aplicar" OnClick="btnAplicar_Click" CausesValidation="false" />
                </div>

                <div class="pestanias">
                    <span id="tabVentas" class="pestania pestania-activa" onclick="mostrarPestania('ventas')">Datos de Ventas</span>
                    <span id="tabStock" class="pestania" onclick="mostrarPestania('stock')">Datos de Stock</span>
                </div>

                <div id="panelVentas" class="panel-pestania">

                    <div class="sv-stats">
                        <div class="sv-stat-card">
                            <div class="texto-secundario">Total Ventas (mes)</div>
                            <div class="sv-stat-valor">
                                <asp:Literal ID="litTotalVentas" runat="server" Text="$0" /></div>
                        </div>
                        <div class="sv-stat-card">
                            <div class="texto-secundario">Ticket Promedio</div>
                            <div class="sv-stat-valor">
                                <asp:Literal ID="litTicketPromedio" runat="server" Text="$0" /></div>
                        </div>
                        <div class="sv-stat-card">
                            <div class="texto-secundario">Unidades Vendidas</div>
                            <div class="sv-stat-valor">
                                <asp:Literal ID="litUnidadesVendidas" runat="server" Text="0" /></div>
                        </div>
                        <div class="sv-stat-card">
                            <div class="texto-secundario">Margen Total</div>
                            <div class="sv-stat-valor">
                                <asp:Literal ID="litMargenTotal" runat="server" Text="0%" /></div>
                        </div>
                    </div>

                    <div class="tarjeta">
                        <h2>Ventas por Sector</h2>
                        <div class="sv-grafico-placeholder">
                            <%-- TODO: gráfico de barras (Chart.js o similar) - se arma cuando esta
                                 pantalla pase a ser funcional. Placeholder por ahora. --%>
                            Gráfico de ventas por sector (pendiente)
                        </div>
                    </div>

                    <div class="tarjeta">
                        <h2>Detalle de Ventas</h2>
                        <table class="tabla">
                            <thead>
                                <tr>
                                    <th>Período</th>
                                    <th>Sector</th>
                                    <th>Categoría</th>
                                    <th>Unidades</th>
                                    <th>Monto</th>
                                    <th>Margen</th>
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
                        <asp:Label ID="lblSinVentas" runat="server" Text="No hay ventas cargadas para el período seleccionado." Visible="false" CssClass="sin-resultados" />
                    </div>
                </div>

                <div id="panelStock" class="panel-pestania" style="display: none;">
                    <div class="tarjeta">
                        <h2>Inventario por Producto</h2>
                        <table class="tabla">
                            <thead>
                                <tr>
                                    <th>Producto</th>
                                    <th>Categoría</th>
                                    <th>Stock Actual</th>
                                    <th>Mínimo</th>
                                    <th>Máximo</th>
                                    <th>Estado</th>
                                    <th>Acción</th>
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
                                            <td><span class='<%# "etiqueta " + Eval("ClaseEstadoStock") %>'><%# Eval("EstadoStock") %></span></td>
                                            <td>
                                                <asp:LinkButton runat="server" CssClass="boton-icono" ToolTip="Actualizar"
                                                    CommandName="ActualizarStock" CommandArgument='<%# Eval("CodigoStock") %>'>Actualizar</asp:LinkButton>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                        <asp:Label ID="lblSinStock" runat="server" Text="No hay datos de stock cargados." Visible="false" CssClass="sin-resultados" />
                    </div>
                </div>

            </div>
        </div>

        <asp:Panel ID="pnlModalCarga" runat="server" ClientIDMode="Static" CssClass="modal-fondo" Style="display: none;">
            <div class="modal-caja">
                <div class="modal-encabezado">
                    <h3>Carga Manual</h3>
                    <span class="modal-cerrar" onclick="cerrarModal('pnlModalCarga')">&times;</span>
                </div>

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Tipo de dato *" AssociatedControlID="ddlTipoCarga" />
                    <asp:DropDownList ID="ddlTipoCarga" runat="server" CssClass="modal-campo">
                        <asp:ListItem Text="Venta" Value="Venta" />
                        <asp:ListItem Text="Stock" Value="Stock" />
                    </asp:DropDownList>
                </div>

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Producto / Categoría *" AssociatedControlID="ddlProductoCarga" />
                    <asp:DropDownList ID="ddlProductoCarga" runat="server" CssClass="modal-campo" />
                </div>

                <div class="fila-formulario">
                    <div class="grupo-campo">
                        <asp:Label runat="server" Text="Cantidad *" AssociatedControlID="tbCantidadCarga" />
                        <asp:TextBox ID="tbCantidadCarga" runat="server" CssClass="modal-campo" TextMode="Number" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="tbCantidadCarga" CssClass="campo-error"
                            ErrorMessage="Ingrese la cantidad" Display="Dynamic" ValidationGroup="vgCarga" />
                    </div>
                    <div class="grupo-campo">
                        <asp:Label runat="server" Text="Importe total (si es venta)" AssociatedControlID="tbImporteCarga" />
                        <asp:TextBox ID="tbImporteCarga" runat="server" CssClass="modal-campo" TextMode="Number" />
                    </div>
                </div>

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Período / Fecha *" AssociatedControlID="tbFechaCarga" />
                    <asp:TextBox ID="tbFechaCarga" runat="server" CssClass="modal-campo" TextMode="Date" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="tbFechaCarga" CssClass="campo-error"
                        ErrorMessage="Ingrese la fecha" Display="Dynamic" ValidationGroup="vgCarga" />
                </div>

                <div class="modal-botones">
                    <asp:Button ID="btnCancelarCarga" runat="server" CssClass="modal-boton-cancelar"
                        Text="Cancelar" OnClick="btnCancelarCarga_Click" CausesValidation="false" />
                    <asp:Button ID="btnGuardarCarga" runat="server" CssClass="modal-boton-guardar"
                        Text="Guardar" OnClick="btnGuardarCarga_Click" ValidationGroup="vgCarga" />
                </div>
            </div>
        </asp:Panel>

    </form>
</body>
</html>
