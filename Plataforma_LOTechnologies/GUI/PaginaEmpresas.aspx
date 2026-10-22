<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PaginaEmpresas.aspx.cs" Inherits="PaginaEmpresas" %>

<%@ Register TagPrefix="uc" TagName="BotonVolverMenu" Src="~/Controles/BotonVolverMenu.ascx" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
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
                        <h1 runat="server" clave="empresas.encabezado">Maestro Empresas</h1>
                    </div>
                    <asp:Button ID="btnNuevaEmpresa" runat="server" CssClass="boton-nuevo"
                        Text="Nueva Empresa" Clave="empresas.btnNueva" OnClick="btnNuevaEmpresa_Click" CausesValidation="false" />
                </div>

                <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" Visible="false" EnableViewState="false" />

                <div class="filtros">
                    <asp:TextBox ID="tbBuscar" runat="server" CssClass="campo-buscar" placeholder="Buscar por nombre o CUIT..." ClavePlaceholder="empresas.buscarPlaceholder" />

                    <asp:DropDownList ID="ddlFiltroEstado" runat="server" CssClass="selector" AutoPostBack="true" OnSelectedIndexChanged="Filtro_Changed">
                        <asp:ListItem Text="Todos los estados" Value="" Clave="comun.todosEstados" />
                        <asp:ListItem Text="Activo" Value="1" Clave="comun.activo" />
                        <asp:ListItem Text="Inactivo" Value="0" Clave="comun.inactivo" />
                    </asp:DropDownList>

                    <asp:Button ID="btnBuscar" runat="server" CssClass="boton-buscar" Text="Buscar" Clave="comun.buscar" OnClick="btnBuscar_Click" CausesValidation="false" />
                </div>

                <div class="tarjeta">
                    <table class="tabla">
                        <thead>
                            <tr>
                                <th runat="server" clave="comun.empresa">Empresa</th>
                                <th>CUIT</th>
                                <th runat="server" clave="empresas.colCorreo">Correo</th>
                                <th runat="server" clave="comun.telefono">Teléfono</th>
                                <th runat="server" clave="comun.direccion">Dirección</th>
                                <th runat="server" clave="comun.estado">Estado</th>
                                <th runat="server" clave="comun.acciones">Acciones</th>
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
                                                <%# Traducir((bool)Eval("Estado") ? "comun.activo" : "comun.inactivo") %>
                                            </span>
                                        </td>
                                        <td>
                                            <asp:LinkButton runat="server" CssClass="boton-icono" ToolTip="Editar" ClaveTitle="comun.editar"
                                                CommandName="Editar" CommandArgument='<%# Eval("CodigoEmpresa") %>'>&#9998;</asp:LinkButton>
                                            <asp:LinkButton runat="server" CssClass="boton-icono" ToolTip="Activar/Desactivar" ClaveTitle="comun.activarDesactivar"
                                                CommandName="CambiarEstado" CommandArgument='<%# Eval("CodigoEmpresa") %>'
                                                ClaveConfirmar="empresas.confirmarEstado">&#8635;</asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                    <asp:Label ID="lblSinEmpresas" runat="server" Text="No hay empresas que coincidan con la búsqueda." Clave="empresas.sinResultados" Visible="false" CssClass="sin-resultados" />
                </div>
            </div>
        </div>

        <asp:Panel ID="pnlModalEmpresa" runat="server" ClientIDMode="Static" CssClass="modal-fondo" Style="display: none;">
            <div class="modal-caja">
                <div class="modal-encabezado">
                    <h3>
                        <asp:Literal ID="litTituloModal" runat="server" Text="Nueva Empresa" /></h3>
                    <span class="modal-cerrar" onclick="document.getElementById('pnlModalEmpresa').style.display='none';">&times;</span>
                </div>

                <asp:HiddenField ID="hfCodigoEmpresa" runat="server" />
                <asp:HiddenField ID="hfEsEdicion" runat="server" Value="false" />

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Razón Social *" AssociatedControlID="tbRazonSocial" Clave="empresas.razonSocial" />
                    <asp:TextBox ID="tbRazonSocial" runat="server" CssClass="modal-campo" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="tbRazonSocial" CssClass="campo-error"
                        ErrorMessage="Ingrese la razón social" Clave="empresas.errRazonSocial" Display="Dynamic" ValidationGroup="vgEmpresa" />
                </div>

                <div class="fila-formulario">
                    <div class="grupo-campo">
                        <asp:Label runat="server" Text="CUIT *" AssociatedControlID="tbCUIT" />
                        <asp:TextBox ID="tbCUIT" runat="server" CssClass="modal-campo" placeholder="XX-XXXXXXXX-X" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="tbCUIT" CssClass="campo-error"
                            ErrorMessage="Ingrese el CUIT" Clave="empresas.errCuit" Display="Dynamic" ValidationGroup="vgEmpresa" />
                    </div>
                    <div class="grupo-campo">
                        <asp:Label runat="server" Text="Correo *" AssociatedControlID="tbCorreo" Clave="empresas.correo" />
                        <asp:TextBox ID="tbCorreo" runat="server" CssClass="modal-campo" TextMode="Email" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="tbCorreo" CssClass="campo-error"
                            ErrorMessage="Ingrese el correo" Clave="empresas.errCorreo" Display="Dynamic" ValidationGroup="vgEmpresa" />
                    </div>
                </div>

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Teléfono" AssociatedControlID="tbTelefono" Clave="comun.telefono" />
                    <asp:TextBox ID="tbTelefono" runat="server" CssClass="modal-campo" TextMode="Number" />
                </div>

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Dirección" AssociatedControlID="tbDireccion" Clave="comun.direccion" />
                    <asp:TextBox ID="tbDireccion" runat="server" CssClass="modal-campo" />
                </div>

                <div class="grupo-campo grupo-casilla">
                    <asp:CheckBox ID="chkActiva" runat="server" Checked="true" />
                    <asp:Label runat="server" Text="Empresa activa" AssociatedControlID="chkActiva" Clave="empresas.activa" />
                </div>

                <div class="modal-botones">
                    <asp:Button ID="btnCancelarModal" runat="server" CssClass="modal-boton-cancelar"
                        Text="Cancelar" Clave="comun.cancelar" OnClick="btnCancelarModal_Click" CausesValidation="false" />
                    <asp:Button ID="btnGuardarEmpresa" runat="server" CssClass="modal-boton-guardar"
                        Text="Crear Empresa" OnClick="btnGuardarEmpresa_Click" ValidationGroup="vgEmpresa" />
                </div>
            </div>
        </asp:Panel>
    </form>
</body>
</html>
