<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PaginaUsuarios.aspx.cs" Inherits="PaginaUsuarios" %>
<%@ Register TagPrefix="uc" TagName="BotonVolverMenu" Src="~/Controles/BotonVolverMenu.ascx" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Maestro Usuarios - LO Technologies</title>
    <link rel="stylesheet" type="text/css" href="/Estilos/Comun.css" />
    <link rel="stylesheet" type="text/css" href="/Estilos/EstilosUsuarios.css" />
    <link rel="stylesheet" type="text/css" href="/Estilos/BotonVolverMenu.css" />
    <script type="text/javascript" src="/JS/UsuariosJS.js"></script>
</head>
<body>
    <form id="formUsuarios" runat="server">
        <div class="pagina">
            <div class="contenedor">
                <uc:BotonVolverMenu ID="volverMenu" runat="server" />
                <div class="encabezado">
                    <div>
                        <h1>Maestro Usuarios</h1>
                        <%-- <p class="subtitulo">CUS 012 — Crear, modificar, activar o desactivar usuarios</p> --%>
                    </div>
                    <asp:Button ID="btnAgregarUsuario" runat="server" CssClass="boton-nuevo"
                        Text="Agregar Usuario" CausesValidation="false" OnClick="btnAgregarUsuario_Click" />
                </div>
                <asp:Label ID="lbMensaje" runat="server" CssClass="mensaje" Visible="false"></asp:Label>

                <div class="filtros">
                    <asp:TextBox ID="tbBuscar" runat="server" CssClass="campo-buscar" placeholder="Buscar usuario..." />
                    <asp:DropDownList ID="ddlFiltroRol" runat="server" CssClass="selector" AutoPostBack="true" OnSelectedIndexChanged="Filtro_Changed">
                        <asp:ListItem Text="Todos los roles" Value="" />
                        <asp:ListItem Text="Administrador" Value="1" />
                        <asp:ListItem Text="Analista" Value="2" />
                        <asp:ListItem Text="Gerente" Value="3" />
                    </asp:DropDownList>
                    <asp:DropDownList ID="ddlFiltroEmpresa" runat="server" CssClass="selector" AutoPostBack="true" OnSelectedIndexChanged="Filtro_Changed">
                        <asp:ListItem Text="Todas las empresas" Value="" />
                    </asp:DropDownList>
                    <asp:Button ID="btnBuscar" runat="server" CssClass="boton-buscar" Text="Buscar" OnClick="btnBuscar_Click" CausesValidation="false" />
                </div>

                <div class="tarjeta">
                    <table class="tabla">
                        <thead>
                            <tr>
                                <th>Usuario</th>
                                <th>Rol</th>
                                <th>Empresa</th>
                                <th>Estado</th>
                                <th>Último acceso</th>
                                <th>Acciones</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptUsuarios" runat="server" OnItemCommand="rptUsuarios_ItemCommand">
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <div class="celda-principal">
                                                <span class="avatar"><%# ObtenerIniciales(Eval("Nombre").ToString(), Eval("Apellido").ToString()) %></span>
                                                <div>
                                                    <div class="texto-principal"><%# Eval("Nombre") %> <%# Eval("Apellido") %></div>
                                                    <div class="texto-secundario"><%# Eval("Email") %></div>
                                                </div>
                                            </div>
                                        </td>
                                        <td><span class="etiqueta etiqueta-info"><%# ObtenerNombreRol(Eval("Rol")) %></span></td>
                                        <td><%# Eval("NombreEmpresa") %></td>
                                        <td>
                                            <span class='<%# (bool)Eval("estadoActivo") ? "etiqueta etiqueta-activo" : "etiqueta etiqueta-inactivo" %>'>
                                                <%# (bool)Eval("estadoActivo") ? "Activo" : "Inactivo" %>
                                            </span>
                                            <asp:Label runat="server" CssClass="etiqueta etiqueta-bloqueado"
                                                Text="Bloqueado" Visible='<%# (bool)Eval("estadoBloqueado") %>' />
                                        </td>
                                        <td><%# Eval("ultimoAcceso", "{0:dd/MM/yyyy HH:mm}") %></td>
                                        <td class="acciones">
                                            <asp:LinkButton runat="server" CssClass="boton-icono" ToolTip="Editar"
                                                CommandName="Editar" CommandArgument='<%# Eval("DNI") %>'>&#9998;</asp:LinkButton>
                                            <asp:LinkButton runat="server" CssClass="boton-icono" ToolTip="Activar/Desactivar"
                                                CommandName="CambiarEstado" CommandArgument='<%# Eval("DNI") %>'
                                                OnClientClick="return confirm('¿Confirmás cambiar el estado de este usuario?');">&#8635;</asp:LinkButton>
                                            <asp:LinkButton runat="server" CssClass="boton-icono" ToolTip="Desbloquear"
                                                CommandName="Desbloquear" CommandArgument='<%# Eval("DNI") %>'
                                                OnClientClick="return confirm('¿Confirmás desbloquear a este usuario? Se le va a enviar un mail para que defina una nueva contraseña.');">&#128275;</asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>

                    <asp:Label ID="lbNoResultado" runat="server" Text="No hay usuarios que coincidan con la búsqueda." Visible="false" CssClass="sin-resultados" />
                </div>
            </div>
        </div>

        <asp:Panel ID="pnlModalUsuario" runat="server" ClientIDMode="Static" CssClass="modal-fondo" Style="display: none;">
            <div class="modal-caja">
                <div class="modal-encabezado">
                    <h3><asp:Literal ID="litTituloModal" runat="server" Text="Nuevo Usuario" /></h3>
                    <span class="modal-cerrar" onclick="cerrarModalUsuario()">&times;</span>
                </div>

                <asp:HiddenField ID="hfDNI" runat="server" />
                <asp:HiddenField ID="hfEsEdicion" runat="server" Value="false" />

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="DNI *" AssociatedControlID="tbDNI" />
                    <asp:TextBox ID="tbDNI" runat="server" CssClass="modal-campo" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="tbDNI" CssClass="campo-error"
                        ErrorMessage="Ingrese el DNI" Display="Dynamic" ValidationGroup="vgUsuario" />
                    <asp:RangeValidator runat="server" ControlToValidate="tbDNI" CssClass="campo-error"
                        Type="Integer" MinimumValue="1000000" MaximumValue="99999999"
                        ErrorMessage="DNI inválido" Display="Dynamic" ValidationGroup="vgUsuario" />
                </div>

                <div class="fila-formulario">
                    <div class="grupo-campo">
                        <asp:Label runat="server" Text="Nombre *" AssociatedControlID="tbNombre" />
                        <asp:TextBox ID="tbNombre" runat="server" CssClass="modal-campo" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="tbNombre" CssClass="campo-error"
                            ErrorMessage="Ingrese el nombre" Display="Dynamic" ValidationGroup="vgUsuario" />
                    </div>
                    <div class="grupo-campo">
                        <asp:Label runat="server" Text="Apellido *" AssociatedControlID="tbApellido" />
                        <asp:TextBox ID="tbApellido" runat="server" CssClass="modal-campo" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="tbApellido" CssClass="campo-error"
                            ErrorMessage="Ingrese el apellido" Display="Dynamic" ValidationGroup="vgUsuario" />
                    </div>
                </div>

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Correo Electrónico *" AssociatedControlID="tbEmail" />
                    <asp:TextBox ID="tbEmail" runat="server" CssClass="modal-campo" TextMode="Email" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="tbEmail" CssClass="campo-error"
                        ErrorMessage="Ingrese el email" Display="Dynamic" ValidationGroup="vgUsuario" />
                </div>

                <div class="fila-formulario">
                    <div class="grupo-campo">
                        <asp:Label runat="server" Text="Rol" AssociatedControlID="ddlRol" />
                        <asp:DropDownList ID="ddlRol" runat="server" CssClass="modal-campo">
                            <asp:ListItem Text="Administrador" Value="1" />
                            <asp:ListItem Text="Analista" Value="2" />
                            <asp:ListItem Text="Gerente" Value="3" />
                        </asp:DropDownList>
                    </div>
                    <div class="grupo-campo">
                        <asp:Label runat="server" Text="Empresa" AssociatedControlID="ddlEmpresa" />
                        <asp:DropDownList ID="ddlEmpresa" runat="server" CssClass="modal-campo" />
                    </div>
                </div>

                <div class="grupo-campo" id="grupoContrasenia" runat="server">
                    <asp:Label runat="server" Text="Contraseña *" AssociatedControlID="tbContrasenia" />
                    <asp:TextBox ID="tbContrasenia" runat="server" CssClass="modal-campo" TextMode="Password" />
                    <asp:RequiredFieldValidator ID="rfvContrasenia" runat="server" ControlToValidate="tbContrasenia" CssClass="campo-error"
                        ErrorMessage="Ingrese la contraseña" Display="Dynamic" ValidationGroup="vgUsuario" />
                    <asp:RegularExpressionValidator ID="revContrasenia" runat="server" ControlToValidate="tbContrasenia" CssClass="campo-error"
                        ValidationExpression="^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&amp;*(),.?&quot;:{}|&lt;&gt;_\-]).{8,}$"
                        ErrorMessage="Mínimo 8 caracteres, con mayúscula, número y carácter especial"
                        Display="Dynamic" ValidationGroup="vgUsuario" />
                </div>

                <div class="grupo-campo grupo-casilla">
                    <asp:CheckBox ID="chkActivo" runat="server" Checked="true" />
                    <asp:Label runat="server" Text="Usuario activo" AssociatedControlID="chkActivo" />
                </div>

                <div class="modal-botones">
                    <asp:Button ID="btnCancelarModal" runat="server" CssClass="modal-boton-cancelar"
                        Text="Cancelar" OnClick="btnCancelarModal_Click" CausesValidation="false" />
                    <asp:Button ID="btnGuardarUsuario" runat="server" CssClass="modal-boton-guardar"
                        Text="Crear Usuario" OnClick="btnGuardarUsuario_Click" ValidationGroup="vgUsuario" />
                </div>
            </div>
        </asp:Panel>
    </form>
</body>
</html>
