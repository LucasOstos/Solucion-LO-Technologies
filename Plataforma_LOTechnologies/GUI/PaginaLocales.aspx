<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PaginaLocales.aspx.cs" Inherits="PaginaLocales" %>

<%@ Register TagPrefix="uc" TagName="BotonVolverMenu" Src="~/Controles/BotonVolverMenu.ascx" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Locales - LO Technologies</title>
    <link rel="stylesheet" type="text/css" href="/Estilos/BotonVolverMenu.css" />
    <link rel="stylesheet" type="text/css" href="/Estilos/Comun.css" />
    <link rel="stylesheet" type="text/css" href="/Estilos/EstilosLocales.css" />
    <script type="text/javascript" src="/JS/LocalesJS.js"></script>
</head>
<body>
    <form id="formLocales" runat="server">
        <div class="pagina">
            <div class="contenedor">

                <uc:BotonVolverMenu ID="volverMenu" runat="server" />

                <div class="encabezado">
                    <div>
                        <h1>Locales Comerciales</h1>
                    </div>
                    <asp:Button ID="btnNuevoLocal" runat="server" CssClass="boton-nuevo"
                        Text="Nuevo Local" OnClick="btnNuevoLocal_Click" CausesValidation="false" />
                </div>

                <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" Visible="false" />

                <div class="filtros">
                    <asp:TextBox ID="tbBuscar" runat="server" CssClass="campo-buscar" placeholder="Buscar local..." />
                    <asp:DropDownList ID="ddlFiltroEmpresa" runat="server" CssClass="selector" AutoPostBack="true" OnSelectedIndexChanged="Filtro_Changed">
                        <asp:ListItem Text="Todas las empresas" Value="" />
                    </asp:DropDownList>
                    <asp:DropDownList ID="ddlFiltroEstado" runat="server" CssClass="selector" AutoPostBack="true" OnSelectedIndexChanged="Filtro_Changed">
                        <asp:ListItem Text="Todos los estados" Value="" />
                        <asp:ListItem Text="Activo" Value="Activo" />
                        <asp:ListItem Text="Inactivo" Value="Inactivo" />
                    </asp:DropDownList>
                    <asp:Button ID="btnBuscar" runat="server" CssClass="boton-buscar" Text="Buscar" OnClick="btnBuscar_Click" CausesValidation="false" />
                </div>

                <asp:Repeater ID="rptLocales" runat="server" OnItemCommand="rptLocales_ItemCommand">
                    <HeaderTemplate>
                        <div class="loc-grilla">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="loc-tarjeta">
                            <div class="loc-tarjeta-header">
                                <span class="avatar avatar-cuadrado">&#127970;</span>
                                <span class='<%# (int)Eval("Estado") == 1 ? "loc-estado-punto loc-activo" : "loc-estado-punto loc-inactivo" %>'
                                    title='<%# (int)Eval("Estado") == 1 ? "Activo" : "Inactivo" %>'></span>
                            </div>
                            <div class="texto-principal"><%# Eval("Nombre") %></div>
                            <div class="texto-secundario"><%# Eval("NombreEmpresa") %></div>
                            <div class="loc-direccion">&#128205; <%# Eval("Direccion") %></div>
                            <div class="loc-stats">
                                <span><%# Eval("SuperficieAprox") %> m&sup2;</span>
                                <span><%# Eval("TipoLocal") %></span>
                            </div>

                            <asp:LinkButton runat="server" CssClass="loc-boton-layout" ToolTip="Ver Layout"
                                CommandName="VerLayout" CommandArgument='<%# Eval("CodigoLocal") %>'>Ver Layout</asp:LinkButton>

                            <div class="loc-acciones">
                                <asp:LinkButton runat="server" CssClass="boton-icono" ToolTip="Editar"
                                    CommandName="Editar" CommandArgument='<%# Eval("CodigoLocal") %>'>&#9998;</asp:LinkButton>
                                <asp:LinkButton runat="server" CssClass="boton-icono" ToolTip="Activar/Desactivar"
                                    CommandName="CambiarEstado" CommandArgument='<%# Eval("CodigoLocal") %>'
                                    OnClientClick="return confirm('¿Confirmás cambiar el estado de este local?');">&#8635;</asp:LinkButton>
                            </div>
                        </div>
                    </ItemTemplate>
                    <FooterTemplate></div></FooterTemplate>
                </asp:Repeater>

                <asp:Label ID="lblSinLocales" runat="server" Text="No hay locales que coincidan con la búsqueda." Visible="false" CssClass="sin-resultados" />

            </div>
        </div>

        <asp:Panel ID="pnlModalLocal" runat="server" ClientIDMode="Static" CssClass="modal-fondo" Style="display: none;">
            <div class="modal-caja">
                <div class="modal-encabezado">
                    <h3>
                        <asp:Literal ID="litTituloModal" runat="server" Text="Nuevo Local" /></h3>
                    <span class="modal-cerrar" onclick="cerrarModalLocal()">&times;</span>
                </div>

                <asp:HiddenField ID="hfCodigoLocal" runat="server" />
                <asp:HiddenField ID="hfEsEdicion" runat="server" Value="false" />

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Nombre *" AssociatedControlID="tbNombre" />
                    <asp:TextBox ID="tbNombre" runat="server" CssClass="modal-campo" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="tbNombre" CssClass="campo-error"
                        ErrorMessage="Ingrese el nombre" Display="Dynamic" ValidationGroup="vgLocal" />
                </div>

                <div class="fila-formulario">
                    <div class="grupo-campo">
                        <asp:Label runat="server" Text="Empresa Cliente *" AssociatedControlID="ddlEmpresa" />
                        <asp:DropDownList ID="ddlEmpresa" runat="server" CssClass="modal-campo" />
                    </div>
                </div>

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Dirección *" AssociatedControlID="tbDireccion" />
                    <asp:TextBox ID="tbDireccion" runat="server" CssClass="modal-campo" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="tbDireccion" CssClass="campo-error"
                        ErrorMessage="Ingrese la dirección" Display="Dynamic" ValidationGroup="vgLocal" />
                </div>

                <div class="fila-formulario">
                    <div class="grupo-campo">
                        <asp:Label runat="server" Text="Tipo de local" AssociatedControlID="ddlTipoLocal" />
                        <asp:DropDownList ID="ddlTipoLocal" runat="server" CssClass="modal-campo">
                            <asp:ListItem Text="Local a la calle" Value="Local a la calle" />
                            <asp:ListItem Text="Shopping" Value="Shopping" />
                            <asp:ListItem Text="Strip Center" Value="Strip Center" />
                            <asp:ListItem Text="Otro" Value="Otro" />
                        </asp:DropDownList>
                    </div>
                    <div class="grupo-campo">
                        <asp:Label runat="server" Text="Superficie aprox. (m&sup2;)" AssociatedControlID="tbSuperficie" />
                        <asp:TextBox ID="tbSuperficie" runat="server" CssClass="modal-campo" TextMode="Number" />
                    </div>
                </div>

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Teléfono" AssociatedControlID="tbTelefono" />
                    <asp:TextBox ID="tbTelefono" runat="server" CssClass="modal-campo" placeholder="+54 11 0000-0000" />
                </div>

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Observaciones" AssociatedControlID="tbObservaciones" />
                    <asp:TextBox ID="tbObservaciones" runat="server" CssClass="modal-campo" TextMode="MultiLine" Rows="3" />
                </div>

                <div class="grupo-campo grupo-casilla">
                    <asp:CheckBox ID="chkActivo" runat="server" Checked="true" />
                    <asp:Label runat="server" Text="Local activo" AssociatedControlID="chkActivo" />
                </div>

                <div class="modal-botones">
                    <asp:Button ID="btnCancelarModal" runat="server" CssClass="modal-boton-cancelar"
                        Text="Cancelar" OnClick="btnCancelarModal_Click" CausesValidation="false" />
                    <asp:Button ID="btnGuardarLocal" runat="server" CssClass="modal-boton-guardar"
                        Text="Crear Local" OnClick="btnGuardarLocal_Click" ValidationGroup="vgLocal" />
                </div>
            </div>
        </asp:Panel>

    </form>
</body>
</html>
