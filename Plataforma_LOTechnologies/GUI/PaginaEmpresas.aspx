<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PaginaEmpresas.aspx.cs" Inherits="PaginaEmpresas" %>

<%@ Register TagPrefix="uc" TagName="BotonVolverMenu" Src="~/Controles/BotonVolverMenu.ascx" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Maestro Empresas - LO Technologies</title>
    <link rel="stylesheet" type="text/css" href="/Estilos/Comun.css" />
    <link rel="stylesheet" type="text/css" href="/Estilos/EstilosEmpresas.css" />
    <link rel="stylesheet" type="text/css" href="/Estilos/BotonVolverMenu.css" />
    <script type="text/javascript" src="/JS/Empresas.js"></script>
</head>
<body>
    <form id="formEmpresas" runat="server">
        <div class="pagina">
            <div class="contenedor">

                <uc:BotonVolverMenu ID="volverMenu" runat="server" />

                <div class="encabezado">
                    <div>
                        <h1>Maestro Empresas</h1>
                    </div>
                    <asp:Button ID="btnNuevaEmpresa" runat="server" CssClass="boton-nuevo"
                        Text="Nueva Empresa" OnClick="btnNuevaEmpresa_Click" CausesValidation="false" />
                </div>

                <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" Visible="false" />

                <div class="filtros">
                    <asp:TextBox ID="tbBuscar" runat="server" CssClass="campo-buscar" placeholder="Buscar por nombre o CUIT..." />

                    <asp:DropDownList ID="ddlFiltroEstado" runat="server" CssClass="selector" AutoPostBack="true" OnSelectedIndexChanged="Filtro_Changed">
                        <asp:ListItem Text="Todos los estados" Value="" />
                        <asp:ListItem Text="Activo" Value="1" />
                        <asp:ListItem Text="Inactivo" Value="0" />
                    </asp:DropDownList>

                    <asp:Button ID="btnBuscar" runat="server" CssClass="boton-buscar" Text="Buscar" OnClick="btnBuscar_Click" CausesValidation="false" />
                </div>

                <div class="tarjeta">
                    <table class="tabla">
                        <thead>
                            <tr>
                                <th>Empresa</th>
                                <th>CUIT</th>
                                <th>Correo</th>
                                <th>Teléfono</th>
                                <th>Dirección</th>
                                <th>Estado</th>
                                <th>Acciones</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptEmpresas" runat="server" OnItemCommand="rptEmpresas_ItemCommand">
                                <ItemTemplate>
                                    <tr>
                                        <td class="texto-principal">&#127970; <%# Eval("RazonSocial") %></td>
                                        <td><%# Eval("CUIT") %></td>
                                        <td><%# Eval("Correo") %></td>
                                        <td><%# Eval("Telefono") %></td>
                                        <td><%# Eval("Direccion") %></td>
                                        <td>
                                            <span class='<%# (bool)Eval("Estado") ? "etiqueta etiqueta-activo" : "etiqueta etiqueta-inactivo" %>'>
                                                <%# (bool)Eval("Estado") ? "Activo" : "Inactivo" %>
                                            </span>
                                        </td>
                                        <td>
                                            <asp:LinkButton runat="server" CssClass="boton-icono" ToolTip="Editar"
                                                CommandName="Editar" CommandArgument='<%# Eval("CodigoEmpresa") %>'>&#9998;</asp:LinkButton>
                                            <asp:LinkButton runat="server" CssClass="boton-icono" ToolTip="Activar/Desactivar"
                                                CommandName="CambiarEstado" CommandArgument='<%# Eval("CodigoEmpresa") %>'
                                                OnClientClick="return confirm('¿Confirmás cambiar el estado de esta empresa?');">&#8635;</asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                    <asp:Label ID="lblSinEmpresas" runat="server" Text="No hay empresas que coincidan con la búsqueda." Visible="false" CssClass="sin-resultados" />
                </div>
            </div>
        </div>

        <asp:Panel ID="pnlModalEmpresa" runat="server" ClientIDMode="Static" CssClass="modal-fondo" Style="display:none;">
            <div class="modal-caja">
                <div class="modal-encabezado">
                    <h3><asp:Literal ID="litTituloModal" runat="server" Text="Nueva Empresa" /></h3>
                    <span class="modal-cerrar" onclick="document.getElementById('pnlModalEmpresa').style.display='none';">&times;</span>
                </div>

                <asp:HiddenField ID="hfCodigoEmpresa" runat="server" />
                <asp:HiddenField ID="hfEsEdicion" runat="server" Value="false" />

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Razón Social *" AssociatedControlID="tbRazonSocial" />
                    <asp:TextBox ID="tbRazonSocial" runat="server" CssClass="modal-campo" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="tbRazonSocial" CssClass="campo-error"
                        ErrorMessage="Ingrese la razón social" Display="Dynamic" ValidationGroup="vgEmpresa" />
                </div>

                <div class="fila-formulario">
                    <div class="grupo-campo">
                        <asp:Label runat="server" Text="CUIT *" AssociatedControlID="tbCUIT" />
                        <asp:TextBox ID="tbCUIT" runat="server" CssClass="modal-campo" placeholder="XX-XXXXXXXX-X" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="tbCUIT" CssClass="campo-error"
                            ErrorMessage="Ingrese el CUIT" Display="Dynamic" ValidationGroup="vgEmpresa" />
                    </div>
                    <div class="grupo-campo">
                        <asp:Label runat="server" Text="Correo *" AssociatedControlID="tbCorreo" />
                        <asp:TextBox ID="tbCorreo" runat="server" CssClass="modal-campo" TextMode="Email" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="tbCorreo" CssClass="campo-error"
                            ErrorMessage="Ingrese el correo" Display="Dynamic" ValidationGroup="vgEmpresa" />
                    </div>
                </div>

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Teléfono" AssociatedControlID="tbTelefono" />
                    <asp:TextBox ID="tbTelefono" runat="server" CssClass="modal-campo" TextMode="Number" />
                </div>

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Dirección" AssociatedControlID="tbDireccion" />
                    <asp:TextBox ID="tbDireccion" runat="server" CssClass="modal-campo" />
                </div>

                <div class="grupo-campo grupo-casilla">
                    <asp:CheckBox ID="chkActiva" runat="server" Checked="true" />
                    <asp:Label runat="server" Text="Empresa activa" AssociatedControlID="chkActiva" />
                </div>

                <div class="modal-botones">
                    <asp:Button ID="btnCancelarModal" runat="server" CssClass="modal-boton-cancelar"
                        Text="Cancelar" OnClick="btnCancelarModal_Click" CausesValidation="false" />
                    <asp:Button ID="btnGuardarEmpresa" runat="server" CssClass="modal-boton-guardar"
                        Text="Crear Empresa" OnClick="btnGuardarEmpresa_Click" ValidationGroup="vgEmpresa" />
                </div>
            </div>
        </asp:Panel>
    </form>
</body>
</html>
