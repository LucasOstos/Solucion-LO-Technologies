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
                        <asp:Button ID="btnImportar" runat="server" CssClass="boton-secundario" Text="Importar archivo" Clave="stock.btnImportarArchivo" OnClick="btnImportar_Click" CausesValidation="false" />
                        <asp:Button ID="btnCargaManual" runat="server" CssClass="boton-nuevo" Text="Carga Manual" Clave="stock.btnCargaManual" OnClick="btnCargaManual_Click" CausesValidation="false" />
                    </div>
                </div>

                <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" Visible="false" EnableViewState="false" />

                <div class="filtros">
                    <asp:DropDownList ID="ddlLocal" runat="server" CssClass="selector" ClaveAria="comun.local" AutoPostBack="true" OnSelectedIndexChanged="ddlLocal_SelectedIndexChanged" />
                    <asp:DropDownList ID="ddlPeriodo" runat="server" CssClass="selector" ClaveAria="stock.colPeriodo">
                        <asp:ListItem Text="Último mes" Value="ultimo_mes" Clave="stock.ultimoMes" />
                        <asp:ListItem Text="Últimos 3 meses" Value="3_meses" Clave="stock.ultimos3" />
                        <asp:ListItem Text="Últimos 5 meses" Value="5_meses" Clave="stock.ultimos5" />
                    </asp:DropDownList>
                    <asp:DropDownList ID="ddlAgrupacion" runat="server" CssClass="selector" ClaveAria="stock.agrupacion">
                        <asp:ListItem Text="Por Sector" Value="sector" Clave="stock.porSector" />
                        <asp:ListItem Text="Por Categoría" Value="categoria" Clave="stock.porCategoria" />
                        <asp:ListItem Text="Por Producto" Value="producto" Clave="stock.porProducto" />
                    </asp:DropDownList>
                    <asp:Button ID="btnAplicar" runat="server" CssClass="boton-buscar" Text="Aplicar" Clave="comun.aplicar" OnClick="btnAplicar_Click" CausesValidation="false" />
                    <span class="texto-secundario sv-rango">
                        <asp:Literal ID="litRangoPeriodo" runat="server" /></span>
                </div>

                <asp:HiddenField ID="hfPestania" runat="server" ClientIDMode="Static" />
                <div class="pestanias" role="tablist">
                    <span id="tabVentas" runat="server" clientidmode="Static" class="pestania pestania-activa" role="tab" tabindex="0" onclick="mostrarPestania('ventas')" onkeydown="teclaPestania(event, 'ventas')" clave="stock.tabVentas">Datos de Ventas</span>
                    <span id="tabStock" runat="server" clientidmode="Static" class="pestania" role="tab" tabindex="0" onclick="mostrarPestania('stock')" onkeydown="teclaPestania(event, 'stock')" clave="stock.tabStock">Datos de Stock</span>
                    <span id="tabImportaciones" runat="server" clientidmode="Static" class="pestania" role="tab" tabindex="0" onclick="mostrarPestania('importaciones')" onkeydown="teclaPestania(event, 'importaciones')" clave="importar.tabImportaciones">Importaciones</span>
                </div>

                <%-- ===================== VENTAS ===================== --%>
                <div id="panelVentas" class="panel-pestania" role="tabpanel">

                    <div class="sv-stats">
                        <div class="sv-stat-card">
                            <div class="texto-secundario" runat="server" clave="stock.totalVentasPeriodo">Total ventas del período</div>
                            <div class="sv-stat-valor">
                                <asp:Literal ID="litTotalVentas" runat="server" Text="$0" />
                            </div>
                        </div>
                        <div class="sv-stat-card">
                            <div class="texto-secundario" runat="server" clave="stock.precioPromedioUnidad">Precio promedio por unidad</div>
                            <div class="sv-stat-valor">
                                <asp:Literal ID="litPrecioPromedio" runat="server" Text="$0" />
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
                        <asp:Literal ID="litGraficoSectores" runat="server" />
                    </div>

                    <div class="tarjeta">
                        <h2 runat="server" clave="stock.detalleVentas">Detalle de Ventas</h2>
                        <table class="tabla">
                            <thead>
                                <tr>
                                    <th runat="server" clave="stock.colPeriodo">Período</th>
                                    <th>
                                        <asp:Literal ID="litColGrupo" runat="server" Text="Sector" /></th>
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
                                            <td><%# MostrarGrupo(Eval("Grupo")) %></td>
                                            <td><%# Eval("Unidades") %></td>
                                            <td><%# Eval("Monto", "{0:$#,##0.00}") %></td>
                                            <td><%# Eval("Margen") == null ? "-" : Eval("Margen") + "%" %></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                        <asp:Label ID="lblSinVentas" runat="server" Text="No hay ventas cargadas para el período seleccionado." Clave="stock.sinVentas" Visible="false" CssClass="sin-resultados" />
                    </div>

                    <div class="tarjeta">
                        <h2 runat="server" clave="stock.ultimasVentas">Últimas ventas cargadas</h2>
                        <p class="texto-secundario" runat="server" clave="stock.ayudaUltimasVentas">Si cargaste una venta por error, eliminala desde acá. Para borrar un archivo completo usá la pestaña Importaciones.</p>
                        <table class="tabla">
                            <thead>
                                <tr>
                                    <th runat="server" clave="stock.colFecha">Fecha</th>
                                    <th runat="server" clave="productos.colProducto">Producto</th>
                                    <th runat="server" clave="stock.colUnidades">Unidades</th>
                                    <th runat="server" clave="stock.colMonto">Monto</th>
                                    <th runat="server" clave="stock.colOrigen">Origen</th>
                                    <th runat="server" clave="stock.colAccion">Acción</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptUltimasVentas" runat="server" OnItemCommand="rptUltimasVentas_ItemCommand">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%# Eval("Fecha", "{0:dd/MM/yyyy}") %></td>
                                            <td class="texto-principal"><%# HttpUtility.HtmlEncode(Eval("Producto")) %></td>
                                            <td><%# Eval("Cantidad") %></td>
                                            <td><%# Eval("ImporteTotal", "{0:$#,##0.00}") %></td>
                                            <td><%# MostrarOrigen(Eval("CodigoImportacion")) %></td>
                                            <td>
                                                <asp:LinkButton runat="server" CssClass="boton-icono" CommandName="EliminarVenta" CommandArgument='<%# Eval("CodigoVenta") %>'
                                                    Text="Eliminar" Clave="comun.eliminar" ClaveConfirmar="stock.confirmarEliminarVenta" CausesValidation="false" />
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                        <asp:Label ID="lblSinUltimasVentas" runat="server" Text="Todavía no se cargaron ventas en este local." Clave="stock.sinUltimasVentas" Visible="false" CssClass="sin-resultados" />
                    </div>
                </div>

                <%-- ===================== STOCK ===================== --%>
                <div id="panelStock" class="panel-pestania" role="tabpanel" style="display: none;">
                    <div class="tarjeta">
                        <h2 runat="server" clave="stock.inventario">Inventario por Producto</h2>
                        <p class="texto-secundario" runat="server" clave="stock.ayudaEstados">Crítico: sin stock. Bajo: en el mínimo o por debajo. Sobrestock: por encima del máximo.</p>
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

                <%-- ===================== IMPORTACIONES ===================== --%>
                <div id="panelImportaciones" class="panel-pestania" role="tabpanel" style="display: none;">
                    <div class="tarjeta">
                        <h2 runat="server" clave="importar.historial">Historial de importaciones</h2>
                        <p class="texto-secundario" runat="server" clave="importar.ayudaHistorial">"Deshacer" borra todas las ventas que entraron con ese archivo (por ejemplo, si se importó dos veces). Las importaciones de stock no se deshacen: reemplazan el stock que había.</p>
                        <table class="tabla">
                            <thead>
                                <tr>
                                    <th runat="server" clave="stock.colFecha">Fecha</th>
                                    <th runat="server" clave="stock.tipoDato">Tipo de dato</th>
                                    <th runat="server" clave="importar.colArchivo">Archivo</th>
                                    <th runat="server" clave="importar.colFilas">Filas</th>
                                    <th runat="server" clave="usuarios.colUsuario">Usuario</th>
                                    <th runat="server" clave="comun.estado">Estado</th>
                                    <th runat="server" clave="stock.colAccion">Acción</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptImportaciones" runat="server" OnItemCommand="rptImportaciones_ItemCommand">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%# Eval("Fecha", "{0:dd/MM/yyyy HH:mm}") %></td>
                                            <td><%# TraducirValor(Eval("Tipo")) %></td>
                                            <td class="texto-principal"><%# "#" + Eval("CodigoImportacion") + " " + HttpUtility.HtmlEncode(Eval("NombreArchivo")) %></td>
                                            <td><%# Eval("CantidadFilas") %></td>
                                            <td><%# HttpUtility.HtmlEncode(Eval("Usuario")) %></td>
                                            <td><span class='<%# "etiqueta " + ((string)Eval("Estado") == "Aplicada" ? "etiqueta-activo" : "etiqueta-inactivo") %>'><%# TraducirValor(Eval("Estado")) %></span></td>
                                            <td>
                                                <asp:LinkButton runat="server" CssClass="boton-icono" CommandName="Deshacer" CommandArgument='<%# Eval("CodigoImportacion") %>'
                                                    Visible='<%# (bool)Eval("PuedeDeshacer") %>' Text="Deshacer" Clave="importar.deshacer"
                                                    ClaveConfirmar="importar.confirmarDeshacer" CausesValidation="false" />
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                        <asp:Label ID="lblSinImportaciones" runat="server" Text="Todavía no se importaron archivos en este local." Clave="importar.sinImportaciones" Visible="false" CssClass="sin-resultados" />
                    </div>
                </div>

            </div>
        </div>

        <%-- ===================== MODAL CARGA MANUAL ===================== --%>
        <asp:Panel ID="pnlModalCarga" runat="server" ClientIDMode="Static" CssClass="modal-fondo" Style="display: none;">
            <div class="modal-caja">
                <div class="modal-encabezado">
                    <h3 runat="server" clave="stock.btnCargaManual">Carga Manual</h3>
                    <span class="modal-cerrar" onclick="cerrarModal('pnlModalCarga')">&times;</span>
                </div>
                <asp:Label ID="lblMensajeCarga" runat="server" CssClass="mensaje mensaje-error" Visible="false" EnableViewState="false" />
                <asp:HiddenField ID="hfCodigoStockEdicion" runat="server" />
                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Tipo de dato *" AssociatedControlID="ddlTipoCarga" Clave="stock.tipoDato" />
                    <asp:DropDownList ID="ddlTipoCarga" runat="server" CssClass="modal-campo" ClientIDMode="Static" onchange="actualizarCamposCarga()">
                        <asp:ListItem Text="Venta" Value="Venta" Clave="valor.venta" />
                        <asp:ListItem Text="Stock" Value="Stock" Clave="valor.stock" />
                    </asp:DropDownList>
                </div>

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Producto / Categoría *" AssociatedControlID="ddlProductoCarga" Clave="stock.productoCategoria" />
                    <asp:DropDownList ID="ddlProductoCarga" runat="server" CssClass="modal-campo" />
                    <asp:Label ID="lblSinProductosCarga" runat="server" CssClass="texto-secundario" Visible="false"
                        Text="Este local no tiene productos cargados. Cargalos desde Productos y categorías." Clave="stock.sinProductosCarga" />
                </div>

                <div class="fila-formulario">
                    <div class="grupo-campo">
                        <asp:Label runat="server" Text="Cantidad *" AssociatedControlID="tbCantidadCarga" Clave="stock.cantidad" />
                        <asp:TextBox ID="tbCantidadCarga" runat="server" CssClass="modal-campo" TextMode="Number" min="0" step="1" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="tbCantidadCarga" CssClass="campo-error"
                            ErrorMessage="Ingrese la cantidad" Clave="stock.errCantidad" Display="Dynamic" ValidationGroup="vgCarga" />
                    </div>
                    <div class="grupo-campo" id="grpImporteCarga">
                        <asp:Label runat="server" Text="Importe total (vacío = precio × cantidad)" AssociatedControlID="tbImporteCarga" Clave="stock.importeOpcional" />
                        <asp:TextBox ID="tbImporteCarga" runat="server" CssClass="modal-campo" TextMode="Number" step="any" min="0" />
                    </div>
                </div>
                <div class="fila-formulario" id="filaMinMaxCarga">
                    <div class="grupo-campo">
                        <asp:Label runat="server" Text="Stock mínimo (si es stock)" AssociatedControlID="tbMinimoCarga" Clave="stock.minimo" />
                        <asp:TextBox ID="tbMinimoCarga" runat="server" CssClass="modal-campo" TextMode="Number" min="0" step="1" />
                    </div>
                    <div class="grupo-campo">
                        <asp:Label runat="server" Text="Stock máximo (si es stock)" AssociatedControlID="tbMaximoCarga" Clave="stock.maximo" />
                        <asp:TextBox ID="tbMaximoCarga" runat="server" CssClass="modal-campo" TextMode="Number" min="0" step="1" />
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

        <%-- ===================== MODAL IMPORTAR ARCHIVO ===================== --%>
        <asp:Panel ID="pnlModalImportar" runat="server" ClientIDMode="Static" CssClass="modal-fondo" Style="display: none;">
            <div class="modal-caja modal-ancho">
                <div class="modal-encabezado">
                    <h3 runat="server" clave="importar.titulo">Importar archivo</h3>
                    <span class="modal-cerrar" onclick="cerrarModal('pnlModalImportar')">&times;</span>
                </div>
                <p class="texto-secundario" runat="server" clave="importar.descripcion">Importá las ventas o el stock exportados del sistema de punto de venta del local, en un archivo CSV o Excel (.xlsx).</p>

                <asp:Panel ID="pnlErroresImportacion" runat="server" CssClass="mensaje mensaje-error" Visible="false">
                    <asp:Literal ID="litErroresImportacion" runat="server" />
                </asp:Panel>

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Tipo de dato *" AssociatedControlID="ddlTipoImportacion" Clave="stock.tipoDato" />
                    <asp:DropDownList ID="ddlTipoImportacion" runat="server" CssClass="modal-campo" ClientIDMode="Static" onchange="actualizarAyudaImportacion()">
                        <asp:ListItem Text="Venta" Value="Venta" Clave="valor.venta" />
                        <asp:ListItem Text="Stock" Value="Stock" Clave="valor.stock" />
                    </asp:DropDownList>
                </div>

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Archivo (.csv o .xlsx, hasta 2 MB) *" AssociatedControlID="fuArchivo" Clave="importar.archivo" />
                    <asp:FileUpload ID="fuArchivo" runat="server" CssClass="modal-campo" accept=".csv,.xlsx" />
                </div>

                <div class="sv-formato" id="ayudaImportVentas">
                    <strong runat="server" clave="importar.formato">Formato</strong>
                    <code>codigo ; fecha ; cantidad ; importe</code>
                    <span class="texto-secundario" runat="server" clave="importar.ayudaVentas">codigo: PROD-001 · fecha: 2026-10-15 o 15/10/2026 · cantidad: entero mayor que 0 · importe: opcional (vacío = precio × cantidad), sin separador de miles.</span>
                    <span class="sv-plantillas">
                        <a href="/Plantillas/Plantilla_Ventas.xlsx" download="Plantilla_Ventas.xlsx" runat="server" clave="importar.plantillaExcel">Descargar plantilla Excel</a>
                        <a href="/Plantillas/Plantilla_Ventas.csv" download="Plantilla_Ventas.csv" runat="server" clave="importar.plantillaCsv">Descargar plantilla CSV</a>
                    </span>
                </div>
                <div class="sv-formato" id="ayudaImportStock" style="display: none;">
                    <strong runat="server" clave="importar.formato">Formato</strong>
                    <code>codigo ; cantidad ; minimo ; maximo</code>
                    <span class="texto-secundario" runat="server" clave="importar.ayudaStock">codigo: PROD-001 · cantidad: entero mayor o igual a 0 · minimo y maximo: opcionales. Si el producto ya tiene stock en el local, se reemplaza.</span>
                    <span class="sv-plantillas">
                        <a href="/Plantillas/Plantilla_Stock.xlsx" download="Plantilla_Stock.xlsx" runat="server" clave="importar.plantillaExcel">Descargar plantilla Excel</a>
                        <a href="/Plantillas/Plantilla_Stock.csv" download="Plantilla_Stock.csv" runat="server" clave="importar.plantillaCsv">Descargar plantilla CSV</a>
                    </span>
                </div>
                <p class="texto-secundario" runat="server" clave="importar.todoONada">El archivo se valida completo: si alguna fila tiene un error no se importa ninguna y se indica la fila y el motivo.</p>

                <div class="modal-botones">
                    <asp:Button ID="btnCancelarImportar" runat="server" CssClass="modal-boton-cancelar"
                        Text="Cancelar" Clave="comun.cancelar" OnClientClick="cerrarModal('pnlModalImportar'); return false;" CausesValidation="false" />
                    <asp:Button ID="btnConfirmarImportar" runat="server" CssClass="modal-boton-guardar"
                        Text="Importar" Clave="importar.btnImportar" OnClick="btnConfirmarImportar_Click" CausesValidation="false" />
                </div>
            </div>
        </asp:Panel>
    </form>
</body>
</html>
