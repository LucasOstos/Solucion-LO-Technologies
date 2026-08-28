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
                        <h1>Productos y Categorías</h1>
                        <p class="subtitulo">CUN 005 — Gestión de productos y categorías del local</p>
                    </div>
                    <asp:Button ID="btnNuevo" runat="server" CssClass="boton-nuevo" Text="Nuevo Producto" OnClick="btnNuevo_Click" CausesValidation="false" />
                </div>

                <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" Visible="false" />

                <div class="pestanias">
                    <span id="tabCategorias" class="pestania" onclick="mostrarPestania('categorias')">Categorías</span>
                    <span id="tabProductos" class="pestania pestania-activa" onclick="mostrarPestania('productos')">Productos</span>
                </div>

                <div class="filtros">
                    <asp:TextBox ID="tbBuscar" runat="server" CssClass="campo-buscar" placeholder="Buscar productos..." />
                    <asp:DropDownList ID="ddlFiltroSector" runat="server" CssClass="selector" AutoPostBack="true" OnSelectedIndexChanged="Filtro_Changed">
                        <asp:ListItem Text="Todos los sectores" Value="" />
                    </asp:DropDownList>
                    <asp:Button ID="btnBuscar" runat="server" CssClass="boton-buscar" Text="Buscar" OnClick="btnBuscar_Click" CausesValidation="false" />
                </div>

                <div id="panelProductos" class="panel-pestania">
                    <div class="tarjeta">
                        <table class="tabla">
                            <thead>
                                <tr>
                                    <th>C&oacute;digo</th>
                                    <th>Producto</th>
                                    <th>Categor&iacute;a</th>
                                    <th>Sector</th>
                                    <th>Precio</th>
                                    <th>Stock</th>
                                    <th>Acciones</th>
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
                                            <td><%# Eval("Stock") %> u.</td>
                                            <td>
                                                <asp:LinkButton runat="server" CssClass="boton-icono" ToolTip="Editar"
                                                    CommandName="EditarProducto" CommandArgument='<%# Eval("CodigoProducto") %>'>&#9998;</asp:LinkButton>
                                                <asp:LinkButton runat="server" CssClass="boton-icono" ToolTip="Eliminar"
                                                    CommandName="EliminarProducto" CommandArgument='<%# Eval("CodigoProducto") %>'
                                                    OnClientClick="return confirm('¿Confirmás eliminar este producto?');">&#128465;</asp:LinkButton>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                        <asp:Label ID="lblSinProductos" runat="server" Text="No hay productos que coincidan con la búsqueda." Visible="false" CssClass="sin-resultados" />
                    </div>
                </div>

                <div id="panelCategorias" class="panel-pestania" style="display: none;">
                    <div class="acciones-tab">
                        <asp:Button ID="btnNuevaCategoria" runat="server" CssClass="boton-nuevo" Text="Nueva Categoría" OnClick="btnNuevaCategoria_Click" CausesValidation="false" />
                    </div>
                    <div class="tarjeta">
                        <table class="tabla">
                            <thead>
                                <tr>
                                    <th>Categoría</th>
                                    <th>Categoría Padre</th>
                                    <th>Sector</th>
                                    <th>Margen Estimado</th>
                                    <th>Acciones</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptCategorias" runat="server" OnItemCommand="rptCategorias_ItemCommand">
                                    <ItemTemplate>
                                        <tr>
                                            <td class="texto-principal"><%# Eval("Nombre") %></td>
                                            <td><%# Eval("NombreCategoriaPadre") %></td>
                                            <td><%# Eval("NombreSector") %></td>
                                            <td><%# Eval("MargenEstimado") %>%</td>
                                            <td>
                                                <asp:LinkButton runat="server" CssClass="boton-icono" ToolTip="Editar"
                                                    CommandName="EditarCategoria" CommandArgument='<%# Eval("CodigoCategoria") %>'>&#9998;</asp:LinkButton>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                        <asp:Label ID="lblSinCategorias" runat="server" Text="No hay categorías cargadas." Visible="false" CssClass="sin-resultados" />
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

                <div class="fila-formulario">
                    <div class="grupo-campo">
                        <asp:Label runat="server" Text="Código interno" AssociatedControlID="tbCodigoInterno" />
                        <asp:TextBox ID="tbCodigoInterno" runat="server" CssClass="modal-campo" placeholder="PROD-XXX" />
                    </div>
                    <div class="grupo-campo">
                        <asp:Label runat="server" Text="Precio" AssociatedControlID="tbPrecio" />
                        <asp:TextBox ID="tbPrecio" runat="server" CssClass="modal-campo" TextMode="Number" />
                    </div>
                </div>

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Nombre del Producto *" AssociatedControlID="tbNombreProducto" />
                    <asp:TextBox ID="tbNombreProducto" runat="server" CssClass="modal-campo" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="tbNombreProducto" CssClass="campo-error"
                        ErrorMessage="Ingrese el nombre" Display="Dynamic" ValidationGroup="vgProducto" />
                </div>

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Categoría" AssociatedControlID="ddlCategoriaProducto" />
                    <asp:DropDownList ID="ddlCategoriaProducto" runat="server" CssClass="modal-campo" />
                </div>

                <div class="grupo-campo grupo-casilla">
                    <asp:CheckBox ID="chkActivoProducto" runat="server" Checked="true" />
                    <asp:Label runat="server" Text="Producto activo" AssociatedControlID="chkActivoProducto" />
                </div>

                <div class="modal-botones">
                    <asp:Button ID="btnCancelarProducto" runat="server" CssClass="modal-boton-cancelar"
                        Text="Cancelar" OnClick="btnCancelarProducto_Click" CausesValidation="false" />
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
                    <asp:Label runat="server" Text="Nombre *" AssociatedControlID="tbNombreCategoria" />
                    <asp:TextBox ID="tbNombreCategoria" runat="server" CssClass="modal-campo" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="tbNombreCategoria" CssClass="campo-error"
                        ErrorMessage="Ingrese el nombre" Display="Dynamic" ValidationGroup="vgCategoria" />
                </div>

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Categoría Padre" AssociatedControlID="ddlCategoriaPadre" />
                    <asp:DropDownList ID="ddlCategoriaPadre" runat="server" CssClass="modal-campo">
                        <asp:ListItem Text="— Sin categoría padre —" Value="" />
                    </asp:DropDownList>
                </div>

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Sector asociado" AssociatedControlID="ddlSectorCategoria" />
                    <asp:DropDownList ID="ddlSectorCategoria" runat="server" CssClass="modal-campo" />
                </div>

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Margen estimado (%)" AssociatedControlID="tbMargenEstimado" />
                    <asp:TextBox ID="tbMargenEstimado" runat="server" CssClass="modal-campo" TextMode="Number" />
                </div>

                <div class="modal-botones">
                    <asp:Button ID="btnCancelarCategoria" runat="server" CssClass="modal-boton-cancelar"
                        Text="Cancelar" OnClick="btnCancelarCategoria_Click" CausesValidation="false" />
                    <asp:Button ID="btnGuardarCategoria" runat="server" CssClass="modal-boton-guardar"
                        Text="Guardar" OnClick="btnGuardarCategoria_Click" ValidationGroup="vgCategoria" />
                </div>
            </div>
        </asp:Panel>

    </form>
</body>
</html>
