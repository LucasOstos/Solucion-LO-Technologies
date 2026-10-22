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
                        <h1 runat="server" Clave="principal.layout.titulo">Layout y Sectores</h1>
                    </div>
                    <asp:DropDownList ID="ddlLocal" runat="server" CssClass="selector" ClaveAria="comun.local" AutoPostBack="true" OnSelectedIndexChanged="ddlLocal_SelectedIndexChanged" />
                </div>
                <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" Visible="false" EnableViewState="false" />
                <div class="pestanias">
                    <span id="tabLayout" runat="server" ClientIDMode="Static" class="pestania pestania-activa" onclick="mostrarPestania('layout')" Clave="layout.tabLayout">Layout del Local</span>
                    <span id="tabSectores" runat="server" ClientIDMode="Static" class="pestania" onclick="mostrarPestania('sectores')" Clave="layout.tabSectores">Gestión de Sectores</span>
                </div>
                <div id="panelLayout" class="panel-pestania">
                    <div class="acciones-tab">
                        <asp:Button ID="btnCargarLayout" runat="server" CssClass="boton-nuevo"
                            Text="Cargar layout" Clave="layout.btnCargar" OnClick="btnCargarLayout_Click" CausesValidation="false" />
                    </div>
                    <asp:Repeater ID="rptLayouts" runat="server" OnItemCommand="rptLayouts_ItemCommand">
                        <ItemTemplate>
                            <div class="ls-item">
                                <span class="avatar avatar-cuadrado">&#128196;</span>
                                <div class="ls-item-info">
                                    <div class="texto-principal">
                                        <%# Eval("Nombre") %>
                                        <span class='<%# "etiqueta " + ObtenerClaseEstadoLayout(Eval("Estado").ToString()) %>'><%# TraducirValor(Eval("Estado")) %></span>
                                    </div>
                                    <div class="texto-secundario"><%# Eval("Descripcion") %></div>
                                    <div class="texto-secundario"><%# Eval("Fecha", "{0:yyyy-MM-dd}") %> &middot; <%# Eval("NombreArchivo") %> &middot; <%# Eval("TamanioArchivo") %></div>
                                </div>
                                <div class="ls-item-acciones">
                                    <asp:LinkButton runat="server" CssClass="boton-icono" ToolTip="Ver detalle" ClaveTitle="layout.verDetalle"
                                        CommandName="VerDetalle" CommandArgument='<%# Eval("CodigoLayout") %>' Text="Ver detalle" Clave="layout.verDetalle" />
                                    <asp:LinkButton runat="server" CssClass="boton-icono" ToolTip="Eliminar" ClaveTitle="comun.eliminar"
                                        CommandName="EliminarLayout" CommandArgument='<%# Eval("CodigoLayout") %>'
                                        ClaveConfirmar="layout.confirmarEliminar">&#128465;</asp:LinkButton>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Label ID="lblSinLayouts" runat="server" Text="Este local todavía no tiene layouts cargados." Clave="layout.sinLayouts" Visible="false" CssClass="sin-resultados" />
                </div>
                <div id="panelSectores" class="panel-pestania" style="display: none;">
                    <div class="acciones-tab">
                        <asp:Button ID="btnNuevoSector" runat="server" CssClass="boton-nuevo"
                            Text="Nuevo Sector" Clave="layout.btnNuevoSector" OnClick="btnNuevoSector_Click" CausesValidation="false" />
                    </div>
                    <div class="tarjeta">
                        <table class="tabla">
                            <thead>
                                <tr>
                                    <th runat="server" Clave="layout.colSector">Sector</th>
                                    <th runat="server" Clave="layout.colUbicacion">Ubicación</th>
                                    <th runat="server" Clave="layout.colCirculacion">Circulación</th>
                                    <th runat="server" Clave="layout.colProductos">N° Productos</th>
                                    <th runat="server" Clave="comun.acciones">Acciones</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptSectores" runat="server" OnItemCommand="rptSectores_ItemCommand">
                                    <ItemTemplate>
                                        <tr>
                                            <td class="texto-principal"><%# Eval("Nombre") %></td>
                                            <td><%# TraducirValor(Eval("Ubicacion")) %></td>
                                            <td><%# Eval("Circulacion") %>%</td>
                                            <td><%# Eval("CantidadProductos") %></td>
                                            <td>
                                                <asp:LinkButton runat="server" CssClass="boton-icono" ToolTip="Editar" ClaveTitle="comun.editar"
                                                    CommandName="EditarSector" CommandArgument='<%# Eval("CodigoSector") %>' Text="Editar" Clave="comun.editar" />
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>

                        <asp:Label ID="lblSinSectores" runat="server" Text="Este local todavía no tiene sectores cargados." Clave="layout.sinSectores" Visible="false" CssClass="sin-resultados" />
                    </div>
                </div>

            </div>
        </div>

        <asp:Panel ID="pnlModalLayout" runat="server" ClientIDMode="Static" CssClass="modal-fondo" Style="display: none;">
            <div class="modal-caja">
                <div class="modal-encabezado">
                    <h3 runat="server" Clave="layout.modalTitulo">Cargar Layout del Local</h3>
                    <span class="modal-cerrar" onclick="cerrarModal('pnlModalLayout')">&times;</span>
                </div>
                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Nombre del layout *" AssociatedControlID="tbNombreLayout" Clave="layout.nombre" />
                    <asp:TextBox ID="tbNombreLayout" runat="server" CssClass="modal-campo" placeholder="Ej: Layout Principal 2026" ClavePlaceholder="layout.ejNombre" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="tbNombreLayout" CssClass="campo-error"
                        ErrorMessage="Ingrese el nombre del layout" Clave="layout.errNombre" Display="Dynamic" ValidationGroup="vgLayout" />
                </div>
                <div class="fila-formulario">
                    <div class="grupo-campo">
                        <asp:Label runat="server" Text="Fecha *" AssociatedControlID="tbFechaLayout" Clave="comun.fechaObligatoria" />
                        <asp:TextBox ID="tbFechaLayout" runat="server" CssClass="modal-campo" TextMode="Date" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="tbFechaLayout" CssClass="campo-error"
                            ErrorMessage="Ingrese la fecha" Clave="comun.errFecha" Display="Dynamic" ValidationGroup="vgLayout" />
                    </div>
                    <div class="grupo-campo">
                        <asp:Label runat="server" Text="Estado *" AssociatedControlID="ddlEstadoLayout" Clave="comun.estadoObligatorio" />
                        <asp:DropDownList ID="ddlEstadoLayout" runat="server" CssClass="modal-campo">
                            <asp:ListItem Text="Activo" Value="Activo" Clave="comun.activo" />
                            <asp:ListItem Text="Inactivo" Value="Inactivo" Clave="comun.inactivo" />
                            <asp:ListItem Text="Borrador" Value="Borrador" Clave="valor.borrador" />
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Descripción *" AssociatedControlID="tbDescripcionLayout" Clave="comun.descripcionObligatoria" />
                    <asp:TextBox ID="tbDescripcionLayout" runat="server" CssClass="modal-campo" TextMode="MultiLine" Rows="3"
                        placeholder="Descripción básica de la distribución del local..." ClavePlaceholder="layout.ejDescripcion" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="tbDescripcionLayout" CssClass="campo-error"
                        ErrorMessage="Ingrese la descripción" Clave="comun.errDescripcion" Display="Dynamic" ValidationGroup="vgLayout" />
                </div>
                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Imagen o plano del local * (PNG, JPG, PDF — max. 5 MB)" AssociatedControlID="fuArchivoLayout" Clave="layout.archivo" />
                    <asp:FileUpload ID="fuArchivoLayout" runat="server" CssClass="modal-campo" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="fuArchivoLayout" CssClass="campo-error"
                        ErrorMessage="Adjunte un archivo" Clave="layout.errArchivo" Display="Dynamic" ValidationGroup="vgLayout" />
                </div>
                <div class="modal-botones">
                    <asp:Button ID="btnCancelarLayout" runat="server" CssClass="modal-boton-cancelar"
                        Text="Cancelar" Clave="comun.cancelar" OnClick="btnCancelarLayout_Click" CausesValidation="false" />
                    <asp:Button ID="btnConfirmarLayout" runat="server" CssClass="modal-boton-guardar"
                        Text="Confirmar carga" Clave="layout.btnConfirmar" OnClick="btnConfirmarLayout_Click" ValidationGroup="vgLayout" />
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
                    <asp:Label runat="server" Text="Nombre del Sector *" AssociatedControlID="tbNombreSector" Clave="layout.nombreSector" />
                    <asp:TextBox ID="tbNombreSector" runat="server" CssClass="modal-campo" placeholder="Ej: Perfumería" ClavePlaceholder="layout.ejSector" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="tbNombreSector" CssClass="campo-error"
                        ErrorMessage="Ingrese el nombre del sector" Clave="layout.errNombreSector" Display="Dynamic" ValidationGroup="vgSector" />
                </div>

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Tipo de sector" AssociatedControlID="ddlTipoSector" Clave="layout.tipoSector" />
                    <asp:DropDownList ID="ddlTipoSector" runat="server" CssClass="modal-campo">
                        <asp:ListItem Text="Frutería" Value="Frutería" Clave="valor.fruteria" />
                        <asp:ListItem Text="Carnicería" Value="Carnicería" Clave="valor.carniceria" />
                        <asp:ListItem Text="Lácteos" Value="Lácteos" Clave="valor.lacteos" />
                        <asp:ListItem Text="Panadería" Value="Panadería" Clave="valor.panaderia" />
                        <asp:ListItem Text="Bebidas" Value="Bebidas" Clave="valor.bebidas" />
                        <asp:ListItem Text="Almacén" Value="Almacén" Clave="valor.almacen" />
                        <asp:ListItem Text="Limpieza" Value="Limpieza" Clave="valor.limpieza" />
                        <asp:ListItem Text="Perfumería" Value="Perfumería" Clave="valor.perfumeria" />
                        <asp:ListItem Text="Electrónica" Value="Electrónica" Clave="valor.electronica" />
                        <asp:ListItem Text="Indumentaria" Value="Indumentaria" Clave="valor.indumentaria" />
                        <asp:ListItem Text="Otro" Value="Otro" Clave="valor.otro" />
                    </asp:DropDownList>
                </div>

                <div class="fila-formulario">
                    <div class="grupo-campo">
                        <asp:Label runat="server" Text="Circulación estimada (%)" AssociatedControlID="tbCirculacionSector" Clave="layout.circulacion" />
                        <asp:TextBox ID="tbCirculacionSector" runat="server" CssClass="modal-campo" TextMode="Number" />
                    </div>
                    <div class="grupo-campo">
                        <asp:Label runat="server" Text="Ubicación" AssociatedControlID="ddlUbicacionSector" Clave="layout.colUbicacion" />
                        <asp:DropDownList ID="ddlUbicacionSector" runat="server" CssClass="modal-campo">
                            <asp:ListItem Text="Frente - Izquierda" Value="Frente - Izquierda" Clave="valor.frenteIzquierda" />
                            <asp:ListItem Text="Frente - Centro" Value="Frente - Centro" Clave="valor.frenteCentro" />
                            <asp:ListItem Text="Frente - Derecha" Value="Frente - Derecha" Clave="valor.frenteDerecha" />
                            <asp:ListItem Text="Centro - Izquierda" Value="Centro - Izquierda" Clave="valor.centroIzquierda" />
                            <asp:ListItem Text="Centro - Centro" Value="Centro - Centro" Clave="valor.centroCentro" />
                            <asp:ListItem Text="Centro - Derecha" Value="Centro - Derecha" Clave="valor.centroDerecha" />
                            <asp:ListItem Text="Fondo - Izquierda" Value="Fondo - Izquierda" Clave="valor.fondoIzquierda" />
                            <asp:ListItem Text="Fondo - Centro" Value="Fondo - Centro" Clave="valor.fondoCentro" />
                            <asp:ListItem Text="Fondo - Derecha" Value="Fondo - Derecha" Clave="valor.fondoDerecha" />
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Descripción" AssociatedControlID="tbDescripcionSector" Clave="comun.descripcion" />
                    <asp:TextBox ID="tbDescripcionSector" runat="server" CssClass="modal-campo" TextMode="MultiLine" Rows="3" />
                </div>

                <div class="modal-botones">
                    <asp:Button ID="btnCancelarSector" runat="server" CssClass="modal-boton-cancelar"
                        Text="Cancelar" Clave="comun.cancelar" OnClick="btnCancelarSector_Click" CausesValidation="false" />
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
                    <strong runat="server" Clave="layout.fecha">Fecha:</strong>
                    <asp:Literal ID="litFechaDetalle" runat="server" />
                </p>
                <p>
                    <strong runat="server" Clave="layout.estado">Estado:</strong>
                    <asp:Literal ID="litEstadoDetalle" runat="server" />
                </p>
                <p>
                    <strong runat="server" Clave="layout.descripcion">Descripción:</strong>
                    <asp:Literal ID="litDescripcionDetalle" runat="server" />
                </p>
                <asp:PlaceHolder ID="phPreview" runat="server" />
                <div class="modal-botones">
                    <asp:Button runat="server" CssClass="modal-boton-cancelar" Text="Cerrar" Clave="comun.cerrar"
                        OnClientClick="cerrarModal('pnlDetalleLayout'); return false;" />
                </div>
            </div>
        </asp:Panel>
    </form>
</body>
</html>
