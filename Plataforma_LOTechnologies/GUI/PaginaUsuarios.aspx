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
                        <h1 runat="server" clave="usuarios.encabezado">Maestro Usuarios</h1>
                    </div>
                    <asp:Button ID="btnAgregarUsuario" runat="server" CssClass="boton-nuevo"
                        Text="Agregar Usuario" Clave="usuarios.btnAgregar" CausesValidation="false" OnClick="btnAgregarUsuario_Click" />
                </div>
                <asp:Label ID="lbMensaje" runat="server" CssClass="mensaje" Visible="false" EnableViewState="false"></asp:Label>

                <div class="filtros">
                    <asp:TextBox ID="tbBuscar" runat="server" CssClass="campo-buscar" placeholder="Buscar usuario..." ClavePlaceholder="usuarios.buscarPlaceholder" />
                    <asp:DropDownList ID="ddlFiltroRol" runat="server" CssClass="selector" AutoPostBack="true" OnSelectedIndexChanged="Filtro_Changed">
                        <asp:ListItem Text="Todos los roles" Value="" />
                        <asp:ListItem Text="Administrador" Value="1" />
                        <asp:ListItem Text="Analista" Value="2" />
                        <asp:ListItem Text="Gerente" Value="3" />
                    </asp:DropDownList>
                    <asp:DropDownList ID="ddlFiltroEmpresa" runat="server" CssClass="selector" AutoPostBack="true" OnSelectedIndexChanged="Filtro_Changed">
                        <asp:ListItem Text="Todas las empresas" Value="" />
                    </asp:DropDownList>
                    <asp:Button ID="btnBuscar" runat="server" CssClass="boton-buscar" Text="Buscar" Clave="comun.buscar" OnClick="btnBuscar_Click" CausesValidation="false" />
                </div>

                <div class="tarjeta">
                    <table class="tabla">
                        <thead>
                            <tr>
                                <th runat="server" clave="usuarios.colUsuario">Usuario</th>
                                <th runat="server" clave="usuarios.colPerfil">Perfil</th>
                                <th runat="server" clave="comun.empresa">Empresa</th>
                                <th runat="server" clave="comun.estado">Estado</th>
                                <th runat="server" clave="usuarios.colUltimoAcceso">Último acceso</th>
                                <th runat="server" clave="comun.acciones">Acciones</th>
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
                                        <td><span class="etiqueta etiqueta-info"><%# Eval("NombrePerfil") %></span></td>
                                        <td><%# Eval("NombreEmpresa") %></td>
                                        <td>
                                            <span class='<%# (bool)Eval("estadoActivo") ? "etiqueta etiqueta-activo" : "etiqueta etiqueta-inactivo" %>'>
                                                <%# Traducir((bool)Eval("estadoActivo") ? "comun.activo" : "comun.inactivo") %>
                                            </span>
                                            <asp:Label runat="server" CssClass="etiqueta etiqueta-bloqueado"
                                                Text="Bloqueado" Clave="usuarios.bloqueado" Visible='<%# (bool)Eval("estadoBloqueado") %>' />
                                        </td>
                                        <td><%# Eval("ultimoAcceso", "{0:dd/MM/yyyy HH:mm}") %></td>
                                        <td class="acciones">
                                            <asp:LinkButton runat="server" CssClass="boton-icono" ToolTip="Editar" ClaveTitle="comun.editar"
                                                CommandName="Editar" CommandArgument='<%# Eval("DNI") %>'>&#9998;</asp:LinkButton>
                                            <asp:LinkButton runat="server" CssClass="boton-icono" ToolTip="Activar/Desactivar" ClaveTitle="comun.activarDesactivar"
                                                CommandName="CambiarEstado" CommandArgument='<%# Eval("DNI") %>'
                                                ClaveConfirmar="usuarios.confirmarEstado">&#8635;</asp:LinkButton>
                                            <asp:LinkButton runat="server" CssClass="boton-icono" ToolTip="Desbloquear" ClaveTitle="usuarios.desbloquear"
                                                CommandName="Desbloquear" CommandArgument='<%# Eval("DNI") %>'
                                                ClaveConfirmar="usuarios.confirmarDesbloqueo">&#128275;</asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>

                    <asp:Label ID="lbNoResultado" runat="server" Text="No hay usuarios que coincidan con la búsqueda." Clave="usuarios.sinResultados" Visible="false" CssClass="sin-resultados" />
                </div>
            </div>
        </div>

        <asp:Panel ID="pnlModalUsuario" runat="server" ClientIDMode="Static" CssClass="modal-fondo" Style="display: none;">
            <div class="modal-caja">
                <div class="modal-encabezado">
                    <h3>
                        <asp:Literal ID="litTituloModal" runat="server" Text="Nuevo Usuario" /></h3>
                    <span class="modal-cerrar" onclick="cerrarModalUsuario()">&times;</span>
                </div>

                <asp:HiddenField ID="hfDNI" runat="server" />
                <asp:HiddenField ID="hfEsEdicion" runat="server" Value="false" />

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="DNI *" AssociatedControlID="tbDNI" Clave="usuarios.dni" />
                    <asp:TextBox ID="tbDNI" runat="server" CssClass="modal-campo" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="tbDNI" CssClass="campo-error"
                        ErrorMessage="Ingrese el DNI" Clave="usuarios.errDni" Display="Dynamic" ValidationGroup="vgUsuario" />
                    <asp:RangeValidator runat="server" ControlToValidate="tbDNI" CssClass="campo-error"
                        Type="Integer" MinimumValue="1000000" MaximumValue="99999999"
                        ErrorMessage="DNI inválido" Clave="usuarios.errDniInvalido" Display="Dynamic" ValidationGroup="vgUsuario" />
                </div>

                <div class="fila-formulario">
                    <div class="grupo-campo">
                        <asp:Label runat="server" Text="Nombre *" AssociatedControlID="tbNombre" Clave="usuarios.nombre" />
                        <asp:TextBox ID="tbNombre" runat="server" CssClass="modal-campo" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="tbNombre" CssClass="campo-error"
                            ErrorMessage="Ingrese el nombre" Clave="usuarios.errNombre" Display="Dynamic" ValidationGroup="vgUsuario" />
                    </div>
                    <div class="grupo-campo">
                        <asp:Label runat="server" Text="Apellido *" AssociatedControlID="tbApellido" Clave="usuarios.apellido" />
                        <asp:TextBox ID="tbApellido" runat="server" CssClass="modal-campo" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="tbApellido" CssClass="campo-error"
                            ErrorMessage="Ingrese el apellido" Clave="usuarios.errApellido" Display="Dynamic" ValidationGroup="vgUsuario" />
                    </div>
                </div>

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Correo Electrónico *" AssociatedControlID="tbEmail" Clave="usuarios.email" />
                    <asp:TextBox ID="tbEmail" runat="server" CssClass="modal-campo" TextMode="Email" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="tbEmail" CssClass="campo-error"
                        ErrorMessage="Ingrese el email" Clave="usuarios.errEmail" Display="Dynamic" ValidationGroup="vgUsuario" />
                </div>

                <div class="fila-formulario">
                    <div class="grupo-campo">
                        <asp:Label runat="server" Text="Rol" AssociatedControlID="ddlRol" Clave="usuarios.rol" />
                        <asp:DropDownList ID="ddlRol" runat="server" CssClass="modal-campo"></asp:DropDownList>
                    </div>
                    <div class="grupo-campo">
                        <asp:Label runat="server" Text="Empresa" AssociatedControlID="ddlEmpresa" Clave="comun.empresa" />
                        <asp:DropDownList ID="ddlEmpresa" runat="server" CssClass="modal-campo" />
                    </div>
                </div>

                <div class="grupo-campo" id="grupoContrasenia" runat="server">
                    <asp:Label runat="server" Text="Contraseña *" AssociatedControlID="tbContrasenia" Clave="usuarios.contrasenia" />
                    <asp:TextBox ID="tbContrasenia" runat="server" CssClass="modal-campo" TextMode="Password" />
                    <asp:RequiredFieldValidator ID="rfvContrasenia" runat="server" ControlToValidate="tbContrasenia" CssClass="campo-error"
                        ErrorMessage="Ingrese la contraseña" Clave="usuarios.errContrasenia" Display="Dynamic" ValidationGroup="vgUsuario" />
                    <asp:RegularExpressionValidator ID="revContrasenia" runat="server" ControlToValidate="tbContrasenia" CssClass="campo-error"
                        ValidationExpression="^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&amp;*(),.?&quot;:{}|&lt;&gt;_\-]).{8,}$"
                        ErrorMessage="Mínimo 8 caracteres, con mayúscula, número y carácter especial" Clave="principal.errContraseniaFormato"
                        Display="Dynamic" ValidationGroup="vgUsuario" />
                </div>

                <div class="grupo-campo grupo-casilla">
                    <asp:CheckBox ID="chkActivo" runat="server" Checked="true" />
                    <asp:Label runat="server" Text="Usuario activo" AssociatedControlID="chkActivo" Clave="usuarios.activo" />
                </div>
                <asp:Label ID="lbMensajeModal" runat="server" CssClass="mensaje mensaje-error" Visible="false" EnableViewState="false" />
                <div class="modal-botones">
                    <asp:Button ID="btnCancelarModal" runat="server" CssClass="modal-boton-cancelar"
                        Text="Cancelar" Clave="comun.cancelar" OnClick="btnCancelarModal_Click" CausesValidation="false" />
                    <asp:Button ID="btnGuardarUsuario" runat="server" CssClass="modal-boton-guardar"
                        Text="Crear Usuario" OnClick="btnGuardarUsuario_Click" ValidationGroup="vgUsuario" />
                </div>
            </div>
        </asp:Panel>
    </form>
</body>
</html>
