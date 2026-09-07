<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PaginaLayoutSectores.aspx.cs" Inherits="PaginaLayoutSectores" %>

<%@ Register TagPrefix="uc" TagName="BotonVolverMenu" Src="~/Controles/BotonVolverMenu.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Layout y Sectores - LO Technologies</title>
    <link rel="stylesheet" type="text/css" href="/Estilos/BotonVolverMenu.css" />
    <link rel="stylesheet" type="text/css" href="/Estilos/Comun.css" />
    <link rel="stylesheet" type="text/css" href="/Estilos/EstilosLayoutSectores.css" />
    <script type="text/javascript" src="/JS/LayoutSectoresJS.js"></script>
</head>
<body>
    <form id="formLayoutSectores" runat="server">
        <div class="pagina">
            <div class="contenedor">
                <uc:BotonVolverMenu ID="volverMenu" runat="server" />
                <div class="encabezado">
                    <div>
                        <h1>Layout y Sectores</h1>
                    </div>
                    <asp:DropDownList ID="ddlLocal" runat="server" CssClass="selector" AutoPostBack="true" OnSelectedIndexChanged="ddlLocal_SelectedIndexChanged" />
                </div>
                <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" Visible="false" />
                <div class="pestanias">
                    <span id="tabLayout" class="pestania pestania-activa" onclick="mostrarPestania('layout')">Layout del Local</span>
                    <span id="tabSectores" class="pestania" onclick="mostrarPestania('sectores')">Gestión de Sectores</span>
                </div>
                <div id="panelLayout" class="panel-pestania">
                    <div class="acciones-tab">
                        <asp:Button ID="btnCargarLayout" runat="server" CssClass="boton-nuevo"
                            Text="Cargar layout" OnClick="btnCargarLayout_Click" CausesValidation="false" />
                    </div>
                    <asp:Repeater ID="rptLayouts" runat="server" OnItemCommand="rptLayouts_ItemCommand">
                        <ItemTemplate>
                            <div class="ls-item">
                                <span class="avatar avatar-cuadrado">&#128196;</span>
                                <div class="ls-item-info">
                                    <div class="texto-principal">
                                        <%# Eval("Nombre") %>
                                        <span class='<%# "etiqueta " + ObtenerClaseEstadoLayout(Eval("Estado").ToString()) %>'><%# Eval("Estado") %></span>
                                    </div>
                                    <div class="texto-secundario"><%# Eval("Descripcion") %></div>
                                    <div class="texto-secundario"><%# Eval("Fecha", "{0:yyyy-MM-dd}") %> &middot; <%# Eval("NombreArchivo") %> &middot; <%# Eval("TamanioArchivo") %></div>
                                </div>
                                <div class="ls-item-acciones">
                                    <asp:LinkButton runat="server" CssClass="boton-icono" ToolTip="Ver detalle"
                                        CommandName="VerDetalle" CommandArgument='<%# Eval("CodigoLayout") %>'>Ver detalle</asp:LinkButton>
                                    <asp:LinkButton runat="server" CssClass="boton-icono" ToolTip="Eliminar"
                                        CommandName="EliminarLayout" CommandArgument='<%# Eval("CodigoLayout") %>'
                                        OnClientClick="return confirm('¿Confirmás eliminar este layout?');">&#128465;</asp:LinkButton>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Label ID="lblSinLayouts" runat="server" Text="Este local todavía no tiene layouts cargados." Visible="false" CssClass="sin-resultados" />
                </div>
                <div id="panelSectores" class="panel-pestania" style="display: none;">
                    <div class="acciones-tab">
                        <asp:Button ID="btnNuevoSector" runat="server" CssClass="boton-nuevo"
                            Text="Nuevo Sector" OnClick="btnNuevoSector_Click" CausesValidation="false" />
                    </div>
                    <div class="tarjeta">
                        <table class="tabla">
                            <thead>
                                <tr>
                                    <th>Sector</th>
                                    <th>Ubicación</th>
                                    <th>Circulación</th>
                                    <th>N° Productos</th>
                                    <th>Acciones</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptSectores" runat="server" OnItemCommand="rptSectores_ItemCommand">
                                    <ItemTemplate>
                                        <tr>
                                            <td class="texto-principal"><%# Eval("Nombre") %></td>
                                            <td><%# Eval("Ubicacion") %></td>
                                            <td><%# Eval("Circulacion") %>%</td>
                                            <td><%# Eval("CantidadProductos") %></td>
                                            <td>
                                                <asp:LinkButton runat="server" CssClass="boton-icono" ToolTip="Editar"
                                                    CommandName="EditarSector" CommandArgument='<%# Eval("CodigoSector") %>'>Editar</asp:LinkButton>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>

                        <asp:Label ID="lblSinSectores" runat="server" Text="Este local todavía no tiene sectores cargados." Visible="false" CssClass="sin-resultados" />
                    </div>
                </div>

            </div>
        </div>

        <asp:Panel ID="pnlModalLayout" runat="server" ClientIDMode="Static" CssClass="modal-fondo" Style="display: none;">
            <div class="modal-caja">
                <div class="modal-encabezado">
                    <h3>Cargar Layout del Local</h3>
                    <span class="modal-cerrar" onclick="cerrarModal('pnlModalLayout')">&times;</span>
                </div>
                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Nombre del layout *" AssociatedControlID="tbNombreLayout" />
                    <asp:TextBox ID="tbNombreLayout" runat="server" CssClass="modal-campo" placeholder="Ej: Layout Principal 2026" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="tbNombreLayout" CssClass="campo-error"
                        ErrorMessage="Ingrese el nombre del layout" Display="Dynamic" ValidationGroup="vgLayout" />
                </div>
                <div class="fila-formulario">
                    <div class="grupo-campo">
                        <asp:Label runat="server" Text="Fecha *" AssociatedControlID="tbFechaLayout" />
                        <asp:TextBox ID="tbFechaLayout" runat="server" CssClass="modal-campo" TextMode="Date" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="tbFechaLayout" CssClass="campo-error"
                            ErrorMessage="Ingrese la fecha" Display="Dynamic" ValidationGroup="vgLayout" />
                    </div>
                    <div class="grupo-campo">
                        <asp:Label runat="server" Text="Estado *" AssociatedControlID="ddlEstadoLayout" />
                        <asp:DropDownList ID="ddlEstadoLayout" runat="server" CssClass="modal-campo">
                            <asp:ListItem Text="Activo" Value="Activo" />
                            <asp:ListItem Text="Inactivo" Value="Inactivo" />
                            <asp:ListItem Text="Borrador" Value="Borrador" />
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Descripción *" AssociatedControlID="tbDescripcionLayout" />
                    <asp:TextBox ID="tbDescripcionLayout" runat="server" CssClass="modal-campo" TextMode="MultiLine" Rows="3"
                        placeholder="Descripción básica de la distribución del local..." />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="tbDescripcionLayout" CssClass="campo-error"
                        ErrorMessage="Ingrese la descripción" Display="Dynamic" ValidationGroup="vgLayout" />
                </div>
                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Imagen o plano del local * (PNG, JPG, PDF — max. 5 MB)" AssociatedControlID="fuArchivoLayout" />
                    <asp:FileUpload ID="fuArchivoLayout" runat="server" CssClass="modal-campo" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="fuArchivoLayout" CssClass="campo-error"
                        ErrorMessage="Adjunte un archivo" Display="Dynamic" ValidationGroup="vgLayout" />
                </div>
                <div class="modal-botones">
                    <asp:Button ID="btnCancelarLayout" runat="server" CssClass="modal-boton-cancelar"
                        Text="Cancelar" OnClick="btnCancelarLayout_Click" CausesValidation="false" />
                    <asp:Button ID="btnConfirmarLayout" runat="server" CssClass="modal-boton-guardar"
                        Text="Confirmar carga" OnClick="btnConfirmarLayout_Click" ValidationGroup="vgLayout" />
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlModalSector" runat="server" ClientIDMode="Static" CssClass="modal-fondo" Style="display: none;">
            <div class="modal-caja">
                <div class="modal-encabezado">
                    <h3>
                        <asp:Literal ID="litTituloModalSector" runat="server" Text="Nuevo Sector" /></h3>
                    <span class="modal-cerrar" onclick="cerrarModal('pnlModalSector')">&times;</span>
                </div>

                <asp:HiddenField ID="hfCodigoSector" runat="server" />
                <asp:HiddenField ID="hfEsEdicionSector" runat="server" Value="false" />

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Nombre del Sector *" AssociatedControlID="tbNombreSector" />
                    <asp:TextBox ID="tbNombreSector" runat="server" CssClass="modal-campo" placeholder="Ej: Perfumería" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="tbNombreSector" CssClass="campo-error"
                        ErrorMessage="Ingrese el nombre del sector" Display="Dynamic" ValidationGroup="vgSector" />
                </div>

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Tipo de sector" AssociatedControlID="ddlTipoSector" />
                    <asp:DropDownList ID="ddlTipoSector" runat="server" CssClass="modal-campo">
                        <asp:ListItem Text="Frutería" Value="Frutería" />
                        <asp:ListItem Text="Carnicería" Value="Carnicería" />
                        <asp:ListItem Text="Lácteos" Value="Lácteos" />
                        <asp:ListItem Text="Panadería" Value="Panadería" />
                        <asp:ListItem Text="Bebidas" Value="Bebidas" />
                        <asp:ListItem Text="Almacén" Value="Almacén" />
                        <asp:ListItem Text="Limpieza" Value="Limpieza" />
                        <asp:ListItem Text="Perfumería" Value="Perfumería" />
                        <asp:ListItem Text="Electrónica" Value="Electrónica" />
                        <asp:ListItem Text="Indumentaria" Value="Indumentaria" />
                        <asp:ListItem Text="Otro" Value="Otro" />
                    </asp:DropDownList>
                </div>

                <div class="fila-formulario">
                    <div class="grupo-campo">
                        <asp:Label runat="server" Text="Circulación estimada (%)" AssociatedControlID="tbCirculacionSector" />
                        <asp:TextBox ID="tbCirculacionSector" runat="server" CssClass="modal-campo" TextMode="Number" />
                    </div>
                    <div class="grupo-campo">
                        <asp:Label runat="server" Text="Ubicación" AssociatedControlID="ddlUbicacionSector" />
                        <asp:DropDownList ID="ddlUbicacionSector" runat="server" CssClass="modal-campo">
                            <asp:ListItem Text="Frente - Izquierda" Value="Frente - Izquierda" />
                            <asp:ListItem Text="Frente - Centro" Value="Frente - Centro" />
                            <asp:ListItem Text="Frente - Derecha" Value="Frente - Derecha" />
                            <asp:ListItem Text="Centro - Izquierda" Value="Centro - Izquierda" />
                            <asp:ListItem Text="Centro - Centro" Value="Centro - Centro" />
                            <asp:ListItem Text="Centro - Derecha" Value="Centro - Derecha" />
                            <asp:ListItem Text="Fondo - Izquierda" Value="Fondo - Izquierda" />
                            <asp:ListItem Text="Fondo - Centro" Value="Fondo - Centro" />
                            <asp:ListItem Text="Fondo - Derecha" Value="Fondo - Derecha" />
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Descripción" AssociatedControlID="tbDescripcionSector" />
                    <asp:TextBox ID="tbDescripcionSector" runat="server" CssClass="modal-campo" TextMode="MultiLine" Rows="3" />
                </div>

                <div class="modal-botones">
                    <asp:Button ID="btnCancelarSector" runat="server" CssClass="modal-boton-cancelar"
                        Text="Cancelar" OnClick="btnCancelarSector_Click" CausesValidation="false" />
                    <asp:Button ID="btnGuardarSector" runat="server" CssClass="modal-boton-guardar"
                        Text="Crear Sector" OnClick="btnGuardarSector_Click" ValidationGroup="vgSector" />
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlDetalleLayout" runat="server" ClientIDMode="Static" CssClass="modal-fondo" Style="display: none;">
            <div class="modal-caja">
                <div class="modal-encabezado">
                    <h3>
                        <asp:Literal ID="litNombreDetalle" runat="server" /></h3>
                    <span class="modal-cerrar" onclick="cerrarModal('pnlDetalleLayout')">&times;</span>
                </div>
                <p>
                    <strong>Fecha:</strong>
                    <asp:Literal ID="litFechaDetalle" runat="server" />
                </p>
                <p>
                    <strong>Estado:</strong>
                    <asp:Literal ID="litEstadoDetalle" runat="server" />
                </p>
                <p>
                    <strong>Descripción:</strong>
                    <asp:Literal ID="litDescripcionDetalle" runat="server" />
                </p>
                <asp:PlaceHolder ID="phPreview" runat="server" />
                <div class="modal-botones">
                    <asp:Button runat="server" CssClass="modal-boton-cancelar" Text="Cerrar"
                        OnClientClick="cerrarModal('pnlDetalleLayout'); return false;" />
                </div>
            </div>
        </asp:Panel>
    </form>
</body>
</html>
