<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PaginaProductos.aspx.cs" Inherits="PaginaProductos" %>

<%@ Register TagPrefix="uc" TagName="BotonVolverMenu" Src="~/Controles/BotonVolverMenu.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Productos y Categorías - LO Technologies</title>
    <link rel="stylesheet" type="text/css" href="/Estilos/BotonVolverMenu.css" />
    <link rel="stylesheet" type="text/css" href="/Estilos/Comun.css" />
    <link rel="stylesheet" type="text/css" href="/Estilos/EstilosProductos.css" />
    <script type="text/javascript" src="/JS/ProductosJS.js"></script>
</head>
<body>
    <form id="formProductos" runat="server">
        <div class="pagina">
            <div class="contenedor">

                <uc:BotonVolverMenu ID="volverMenu" runat="server" />

                <div class="encabezado">
                    <div>
                        <h1 runat="server" clave="principal.productos.titulo">Productos y Categorías</h1>
                    </div>
                    <div>
                        <asp:DropDownList ID="ddlLocal" runat="server" CssClass="selector" ClaveAria="comun.local" AutoPostBack="true" OnSelectedIndexChanged="ddlLocal_SelectedIndexChanged" />
                        <asp:Button ID="btnNuevo" runat="server" CssClass="boton-nuevo" Text="Nuevo Producto" Clave="productos.btnNuevo" OnClick="btnNuevo_Click" CausesValidation="false" />
                    </div>
                </div>

                <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" Visible="false" EnableViewState="false" />

                <div class="pestanias">
                    <span id="tabCategorias" runat="server" clientidmode="Static" class="pestania" onclick="mostrarPestania('categorias')" clave="productos.tabCategorias">Categorías</span>
                    <span id="tabProductos" runat="server" clientidmode="Static" class="pestania pestania-activa" onclick="mostrarPestania('productos')" clave="productos.tabProductos">Productos</span>
                </div>

                <div class="filtros">
                    <asp:TextBox ID="tbBuscar" runat="server" CssClass="campo-buscar" placeholder="Buscar productos..." ClavePlaceholder="productos.buscarPlaceholder" />
                    <asp:DropDownList ID="ddlFiltroSector" runat="server" CssClass="selector" AutoPostBack="true" OnSelectedIndexChanged="Filtro_Changed">
                        <asp:ListItem Text="Todos los sectores" Value="" />
                    </asp:DropDownList>
                    <asp:Button ID="btnBuscar" runat="server" CssClass="boton-buscar" Text="Buscar" Clave="comun.buscar" OnClick="btnBuscar_Click" CausesValidation="false" />
                </div>

                <div id="panelProductos" class="panel-pestania">
                    <div class="tarjeta">
                        <table class="tabla">
                            <thead>
                                <tr>
                                    <th runat="server" clave="productos.colCodigo">C&oacute;digo</th>
                                    <th runat="server" clave="productos.colProducto">Producto</th>
                                    <th runat="server" clave="productos.colCategoria">Categor&iacute;a</th>
                                    <th runat="server" clave="layout.colSector">Sector</th>
                                    <th runat="server" clave="productos.colPrecio">Precio</th>
                                    <th runat="server" clave="productos.colStock">Stock</th>
                                    <th runat="server" clave="comun.acciones">Acciones</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptProductos" runat="server" OnItemCommand="rptProductos_ItemCommand">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%# Eval("CodigoInterno") %></td>
                                            <td class="texto-principal">&#128230; <%# Eval("Nombre") %></td>
                                            <td><%# Eval("NombreCategoria") %></td>
                                            <td><%# Eval("NombreSector") %></td>
                                            <td><%# Eval("Precio", "{0:$#,##0.00}") %></td>
                                            <td><%# Eval("Stock") %> <%# Traducir("comun.unidadesAbrev") %></td>
                                            <td>
                                                <asp:LinkButton runat="server" CssClass="boton-icono" ToolTip="Editar" ClaveTitle="comun.editar"
                                                    CommandName="EditarProducto" CommandArgument='<%# Eval("CodigoProducto") %>'>&#9998;</asp:LinkButton>
                                                <asp:LinkButton runat="server" CssClass="boton-icono" ToolTip="Activar/Desactivar" ClaveTitle="comun.activarDesactivar"
                                                    CommandName="CambiarEstado" CommandArgument='<%# Eval("CodigoProducto") %>'
                                                    ClaveConfirmar="productos.confirmarEstado">&#8635;</asp:LinkButton>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                        <asp:Label ID="lblSinProductos" runat="server" Text="No hay productos que coincidan con la búsqueda." Clave="productos.sinProductos" Visible="false" CssClass="sin-resultados" />
                    </div>
                </div>

                <div id="panelCategorias" class="panel-pestania" style="display: none;">
                    <div class="acciones-tab">
                        <asp:Button ID="btnNuevaCategoria" runat="server" CssClass="boton-nuevo" Text="Nueva Categoría" Clave="productos.btnNuevaCategoria" OnClick="btnNuevaCategoria_Click" CausesValidation="false" />
                    </div>
                    <div class="tarjeta">
                        <table class="tabla">
                            <thead>
                                <tr>
                                    <th runat="server" clave="productos.colCategoria">Categoría</th>
                                    <th runat="server" clave="comun.descripcion">Descripción</th>
                                    <th runat="server" clave="comun.estado">Estado</th>
                                    <th runat="server" clave="comun.acciones">Acciones</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptCategorias" runat="server" OnItemCommand="rptCategorias_ItemCommand">
                                    <ItemTemplate>
                                        <tr>
                                            <td class="texto-principal"><%# Eval("Nombre") %></td>
                                            <td><%# Eval("Descripcion") %></td>
                                            <td>
                                                <span class='<%# (bool)Eval("Estado") ? "etiqueta etiqueta-activo" : "etiqueta etiqueta-inactivo" %>'>
                                                    <%# Traducir((bool)Eval("Estado") ? "comun.activo" : "comun.inactivo") %>
                                                </span>
                                            </td>
                                            <td>
                                                <asp:LinkButton runat="server" CssClass="boton-icono" ToolTip="Editar" ClaveTitle="comun.editar"
                                                    CommandName="EditarCategoria" CommandArgument='<%# Eval("CodigoCategoria") %>'>&#9998;</asp:LinkButton>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                        <asp:Label ID="lblSinCategorias" runat="server" Text="No hay categorías cargadas." Clave="productos.sinCategorias" Visible="false" CssClass="sin-resultados" />
                    </div>
                </div>

            </div>
        </div>

        <asp:Panel ID="pnlModalProducto" runat="server" ClientIDMode="Static" CssClass="modal-fondo" Style="display: none;">
            <div class="modal-caja">
                <div class="modal-encabezado">
                    <h3>
                        <asp:Literal ID="litTituloModalProducto" runat="server" Text="Nuevo Producto" /></h3>
                    <span class="modal-cerrar" onclick="cerrarModal('pnlModalProducto')">&times;</span>
                </div>
                <asp:HiddenField ID="hfCodigoProducto" runat="server" />
                <asp:HiddenField ID="hfNumeroSerie" runat="server" />
                <div class="fila-formulario">
                    <div class="grupo-campo">
                        <asp:Label runat="server" Text="Código interno" AssociatedControlID="tbCodigoInterno" Clave="productos.codigoInterno" />
                        <asp:TextBox ID="tbCodigoInterno" runat="server" CssClass="modal-campo" Enabled="false" />
                    </div>
                    <div class="grupo-campo">
                        <asp:Label runat="server" Text="Precio" AssociatedControlID="tbPrecio" Clave="productos.colPrecio" />
                        <asp:TextBox ID="tbPrecio" runat="server" CssClass="modal-campo" TextMode="Number" />
                    </div>
                </div>

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Nombre del Producto *" AssociatedControlID="tbNombreProducto" Clave="productos.nombre" />
                    <asp:TextBox ID="tbNombreProducto" runat="server" CssClass="modal-campo" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="tbNombreProducto" CssClass="campo-error"
                        ErrorMessage="Ingrese el nombre" Clave="comun.errNombre" Display="Dynamic" ValidationGroup="vgProducto" />
                </div>

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Descripción" AssociatedControlID="tbDescripcionProducto" Clave="comun.descripcion" />
                    <asp:TextBox ID="tbDescripcionProducto" runat="server" CssClass="modal-campo" TextMode="MultiLine" Rows="2" />
                </div>

                <div class="fila-formulario">
                    <div class="grupo-campo">
                        <asp:Label runat="server" Text="Categoría *" AssociatedControlID="ddlCategoriaProducto" Clave="productos.categoriaObligatoria" />
                        <asp:DropDownList ID="ddlCategoriaProducto" runat="server" CssClass="modal-campo" />
                    </div>
                    <div class="grupo-campo">
                        <asp:Label runat="server" Text="Sector" AssociatedControlID="ddlSectorProducto" Clave="layout.colSector" />
                        <asp:DropDownList ID="ddlSectorProducto" runat="server" CssClass="modal-campo">
                            <asp:ListItem Text="— Sin sector asignado —" Value="" />
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="grupo-campo grupo-casilla">
                    <asp:CheckBox ID="chkActivoProducto" runat="server" Checked="true" />
                    <asp:Label runat="server" Text="Producto activo" AssociatedControlID="chkActivoProducto" Clave="productos.activo" />
                </div>

                <div class="modal-botones">
                    <asp:Button ID="btnCancelarProducto" runat="server" CssClass="modal-boton-cancelar"
                        Text="Cancelar" Clave="comun.cancelar" OnClick="btnCancelarProducto_Click" CausesValidation="false" />
                    <asp:Button ID="btnGuardarProducto" runat="server" CssClass="modal-boton-guardar"
                        Text="Guardar" OnClick="btnGuardarProducto_Click" ValidationGroup="vgProducto" />
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlModalCategoria" runat="server" ClientIDMode="Static" CssClass="modal-fondo" Style="display: none;">
            <div class="modal-caja">
                <div class="modal-encabezado">
                    <h3>
                        <asp:Literal ID="litTituloModalCategoria" runat="server" Text="Nueva Categoría" /></h3>
                    <span class="modal-cerrar" onclick="cerrarModal('pnlModalCategoria')">&times;</span>
                </div>
                <asp:HiddenField ID="hfCodigoCategoria" runat="server" />
                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Nombre *" AssociatedControlID="tbNombreCategoria" Clave="comun.nombreObligatorio" />
                    <asp:TextBox ID="tbNombreCategoria" runat="server" CssClass="modal-campo" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="tbNombreCategoria" CssClass="campo-error"
                        ErrorMessage="Ingrese el nombre" Clave="comun.errNombre" Display="Dynamic" ValidationGroup="vgCategoria" />
                </div>

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Descripción" AssociatedControlID="tbDescripcionCategoria" Clave="comun.descripcion" />
                    <asp:TextBox ID="tbDescripcionCategoria" runat="server" CssClass="modal-campo" TextMode="MultiLine" Rows="3" />
                </div>

                <div class="grupo-campo grupo-casilla">
                    <asp:CheckBox ID="chkActivoCategoria" runat="server" Checked="true" />
                    <asp:Label runat="server" Text="Categoría activa" AssociatedControlID="chkActivoCategoria" Clave="productos.categoriaActiva" />
                </div>

                <div class="modal-botones">
                    <asp:Button ID="btnCancelarCategoria" runat="server" CssClass="modal-boton-cancelar"
                        Text="Cancelar" Clave="comun.cancelar" OnClick="btnCancelarCategoria_Click" CausesValidation="false" />
                    <asp:Button ID="btnGuardarCategoria" runat="server" CssClass="modal-boton-guardar"
                        Text="Guardar" Clave="comun.guardar" OnClick="btnGuardarCategoria_Click" ValidationGroup="vgCategoria" />
                </div>
            </div>
        </asp:Panel>
    </form>
</body>
</html>
