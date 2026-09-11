<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PaginaPermisos.aspx.cs" Inherits="PaginaPermisos" %>
<%@ Register TagPrefix="uc" TagName="BotonVolverMenu" Src="~/Controles/BotonVolverMenu.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Maestro Permisos - LO Technologies</title>
    <link rel="stylesheet" type="text/css" href="/Estilos/BotonVolverMenu.css" />
    <link rel="stylesheet" type="text/css" href="/Estilos/Comun.css" />
    <link rel="stylesheet" type="text/css" href="/Estilos/EstilosPermisos.css" />
</head>
<body>
    <form id="formPermisos" runat="server">
        <div class="pagina">
            <div class="contenedor">

                <uc:botonvolvermenu id="volverMenu" runat="server" />

                <div class="encabezado">
                    <div>
                        <h1>Maestro Permisos</h1>
                        <p class="subtitulo">Administración de Perfiles y Familias</p>
                    </div>
                </div>

                <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" Visible="false" />

                <div class="perm-columnas">

                    <div class="tarjeta perm-panel">
                        <h2>Perfiles</h2>

                        <div class="perm-fila">
                            <div class="perm-bloque">
                                <div class="grupo-campo">
                                    <asp:Label runat="server" Text="Nombre del Perfil" AssociatedControlID="tbNombrePerfil" />
                                    <asp:TextBox ID="tbNombrePerfil" runat="server" CssClass="modal-campo" placeholder="Ej: Gerente de Local" />
                                </div>

                                <label>Permisos disponibles</label>
                                <asp:CheckBoxList ID="cblPermisosDisponibles" runat="server" CssClass="perm-lista-checks">
                                    <asp:ListItem Text="Login" Value="Login" />
                                    <asp:ListItem Text="Logout" Value="Logout" />
                                    <asp:ListItem Text="Cambiar Contraseña" Value="CambiarContrasenia" />
                                    <asp:ListItem Text="Cambiar Idioma" Value="CambiarIdioma" />
                                </asp:CheckBoxList>
                                <label>Familias disponibles</label>
                                <asp:CheckBoxList ID="cblFamiliasDisponiblesEnPerfil" runat="server" CssClass="perm-lista-checks">
                                </asp:CheckBoxList>
                            </div>

                            <div class="perm-bloque-botones">
                                <asp:Button ID="btnAgregarPermisosPerfil" runat="server" CssClass="boton-accion boton-primario"
                                    Text="Agregar Permisos" CausesValidation="false" OnClick="btnAgregarPermisosPerfil_Click" />
                                <asp:Button ID="btnQuitarPermisosPerfil" runat="server" CssClass="boton-accion boton-secundario"
                                    Text="Quitar Permisos" CausesValidation="false" OnClick="btnQuitarPermisosPerfil_Click" />
                                <asp:Button ID="btnCrearPerfil" runat="server" CssClass="boton-accion boton-primario"
                                    Text="Crear Perfil" CausesValidation="false" OnClick="btnCrearPerfil_Click" />
                                <asp:Button ID="btnBorrarPerfil" runat="server" CssClass="boton-accion boton-peligro"
                                    Text="Borrar Perfil" CausesValidation="false" OnClick="btnBorrarPerfil_Click" />
                            </div>

                            <div class="perm-bloque">
                                <div class="grupo-campo">
                                    <asp:Label runat="server" Text="Perfiles" AssociatedControlID="ddlPerfiles" />
                                    <asp:DropDownList ID="ddlPerfiles" runat="server" CssClass="modal-campo" AutoPostBack="true" OnSelectedIndexChanged="ddlPerfiles_SelectedIndexChanged">
                                        <asp:ListItem Text="— Seleccionar —" Value="" />
                                    </asp:DropDownList>
                                </div>

                                <label>Contenido asignado (permisos y familias)</label>
                                <asp:ListBox ID="lbContenidoPerfil" runat="server" CssClass="perm-lista-caja" />
                            </div>
                        </div>
                    </div>

                    <div class="tarjeta perm-panel">
                        <h2>Familias</h2>

                        <div class="perm-fila">
                            <div class="perm-bloque">
                                <div class="grupo-campo">
                                    <asp:Label runat="server" Text="Nombre de la Familia" AssociatedControlID="tbNombreFamilia" />
                                    <asp:TextBox ID="tbNombreFamilia" runat="server" CssClass="modal-campo" placeholder="Ej: Administración General" />
                                </div>

                                <label>Permisos disponibles</label>
                                <asp:CheckBoxList ID="cblPermisosDisponiblesEnFamilia" runat="server" CssClass="perm-lista-checks">
                                    <asp:ListItem Text="Login" Value="Login" />
                                    <asp:ListItem Text="Logout" Value="Logout" />
                                    <asp:ListItem Text="Cambiar Contraseña" Value="CambiarContrasenia" />
                                    <asp:ListItem Text="Cambiar Idioma" Value="CambiarIdioma" />
                                </asp:CheckBoxList>
                                <label>Familias disponibles</label>
                                <asp:CheckBoxList ID="cblFamiliasDisponibles" runat="server" CssClass="perm-lista-checks">
                                </asp:CheckBoxList>
                            </div>

                            <div class="perm-bloque-botones">
                                <asp:Button ID="btnAgregarPermisosFamilia" runat="server" CssClass="boton-accion boton-primario"
                                    Text="Agregar Permisos" CausesValidation="false" OnClick="btnAgregarPermisosFamilia_Click" />
                                <asp:Button ID="btnQuitarPermisosFamilia" runat="server" CssClass="boton-accion boton-secundario"
                                    Text="Quitar Permisos" CausesValidation="false" OnClick="btnQuitarPermisosFamilia_Click" />
                                <asp:Button ID="btnCrearFamilia" runat="server" CssClass="boton-accion boton-primario"
                                    Text="Crear Familia" CausesValidation="false" OnClick="btnCrearFamilia_Click" />
                                <asp:Button ID="btnBorrarFamilia" runat="server" CssClass="boton-accion boton-peligro"
                                    Text="Borrar Familia" CausesValidation="false" OnClick="btnBorrarFamilia_Click" />
                            </div>

                            <div class="perm-bloque">
                                <div class="grupo-campo">
                                    <asp:Label runat="server" Text="Familias" AssociatedControlID="ddlFamilias" />
                                    <asp:DropDownList ID="ddlFamilias" runat="server" CssClass="modal-campo" AutoPostBack="true" OnSelectedIndexChanged="ddlFamilias_SelectedIndexChanged">
                                        <asp:ListItem Text="— Seleccionar —" Value="" />
                                    </asp:DropDownList>
                                </div>

                                <label>Contenido asignado (permisos y familias)</label>
                                <asp:ListBox ID="lbContenidoFamilia" runat="server" CssClass="perm-lista-caja" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
