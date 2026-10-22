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

                <uc:BotonVolverMenu ID="volverMenu" runat="server" />

                <div class="encabezado">
                    <div>
                        <h1 runat="server" clave="permisos.encabezado">Maestro Permisos</h1>
                    </div>
                </div>

                <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" Visible="false" EnableViewState="false" />

                <div class="perm-columnas">
                    <div class="tarjeta perm-panel">
                        <h2 runat="server" clave="permisos.perfiles">Perfiles</h2>

                        <div class="perm-grilla">
                            <div class="perm-bloque">
                                <div class="grupo-campo">
                                    <asp:Label runat="server" Text="Nombre del perfil nuevo" AssociatedControlID="tbNombrePerfil" Clave="permisos.nombrePerfilNuevo" />
                                    <asp:TextBox ID="tbNombrePerfil" runat="server" CssClass="modal-campo" placeholder="Ej: Gerente de Local" ClavePlaceholder="permisos.ejPerfil" />
                                </div>

                                <span class="perm-titulo" runat="server" clave="permisos.permisosDisponibles">Permisos disponibles</span>
                                <asp:CheckBoxList ID="cblPermisosDisponibles" runat="server" CssClass="perm-lista-checks"
                                    RepeatLayout="UnorderedList">
                                </asp:CheckBoxList>

                                <span class="perm-titulo" runat="server" clave="permisos.familiasDisponibles">Familias disponibles</span>
                                <asp:CheckBoxList ID="cblFamiliasDisponiblesEnPerfil" runat="server" CssClass="perm-lista-checks"
                                    RepeatLayout="UnorderedList">
                                </asp:CheckBoxList>
                            </div>

                            <div class="perm-bloque perm-bloque-asignado">
                                <div class="grupo-campo">
                                    <asp:Label runat="server" Text="Perfil seleccionado" AssociatedControlID="ddlPerfiles" Clave="permisos.perfilSeleccionado" />
                                    <asp:DropDownList ID="ddlPerfiles" runat="server" CssClass="modal-campo" AutoPostBack="true"
                                        OnSelectedIndexChanged="ddlPerfiles_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </div>

                                <span class="perm-titulo" runat="server" clave="permisos.contenidoAsignado">Contenido asignado</span>
                                <div class="perm-caja-asignado">
                                    <asp:CheckBoxList ID="cblContenidoPerfil" runat="server" CssClass="perm-lista-checks perm-lista-asignado"
                                        RepeatLayout="UnorderedList">
                                    </asp:CheckBoxList>
                                </div>
                                <span class="perm-ayuda" runat="server" clave="permisos.ayudaQuitarPerfil">Marcá lo que quieras quitar del perfil.</span>
                            </div>

                            <div class="perm-botones">
                                <asp:Button ID="btnCrearPerfil" runat="server" CssClass="boton-accion boton-primario"
                                    Text="Crear Perfil" Clave="permisos.btnCrearPerfil" CausesValidation="false" OnClick="btnCrearPerfil_Click" />
                                <asp:Button ID="btnAgregarPermisosPerfil" runat="server" CssClass="boton-accion boton-primario"
                                    Text="Agregar Permisos" Clave="permisos.btnAgregarPermisos" CausesValidation="false" OnClick="btnAgregarPermisosPerfil_Click" />
                            </div>
                            <div class="perm-botones">
                                <asp:Button ID="btnQuitarPermisosPerfil" runat="server" CssClass="boton-accion boton-secundario"
                                    Text="Quitar Permisos" Clave="permisos.btnQuitarPermisos" CausesValidation="false" OnClick="btnQuitarPermisosPerfil_Click" />
                                <asp:Button ID="btnBorrarPerfil" runat="server" CssClass="boton-accion boton-peligro"
                                    Text="Borrar Perfil" Clave="permisos.btnBorrarPerfil" CausesValidation="false" OnClick="btnBorrarPerfil_Click" />
                            </div>
                        </div>
                    </div>

                    <div class="tarjeta perm-panel">
                        <h2 runat="server" clave="permisos.familias">Familias</h2>

                        <div class="perm-grilla">
                            <div class="perm-bloque">
                                <div class="grupo-campo">
                                    <asp:Label runat="server" Text="Nombre de la familia nueva" AssociatedControlID="tbNombreFamilia" Clave="permisos.nombreFamiliaNueva" />
                                    <asp:TextBox ID="tbNombreFamilia" runat="server" CssClass="modal-campo" placeholder="Ej: Administración General" ClavePlaceholder="permisos.ejFamilia" />
                                </div>

                                <span class="perm-titulo" runat="server" clave="permisos.permisosDisponibles">Permisos disponibles</span>
                                <asp:CheckBoxList ID="cblPermisosDisponiblesEnFamilia" runat="server" CssClass="perm-lista-checks"
                                    RepeatLayout="UnorderedList">
                                </asp:CheckBoxList>

                                <span class="perm-titulo" runat="server" clave="permisos.familiasDisponibles">Familias disponibles</span>
                                <asp:CheckBoxList ID="cblFamiliasDisponibles" runat="server" CssClass="perm-lista-checks"
                                    RepeatLayout="UnorderedList">
                                </asp:CheckBoxList>
                            </div>

                            <div class="perm-bloque perm-bloque-asignado">
                                <div class="grupo-campo">
                                    <asp:Label runat="server" Text="Familia seleccionada" AssociatedControlID="ddlFamilias" Clave="permisos.familiaSeleccionada" />
                                    <asp:DropDownList ID="ddlFamilias" runat="server" CssClass="modal-campo" AutoPostBack="true"
                                        OnSelectedIndexChanged="ddlFamilias_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </div>

                                <span class="perm-titulo" runat="server" clave="permisos.contenidoAsignado">Contenido asignado</span>
                                <div class="perm-caja-asignado">
                                    <asp:CheckBoxList ID="cblContenidoFamilia" runat="server" CssClass="perm-lista-checks perm-lista-asignado"
                                        RepeatLayout="UnorderedList">
                                    </asp:CheckBoxList>
                                </div>
                                <span class="perm-ayuda" runat="server" clave="permisos.ayudaQuitarFamilia">Marcá lo que quieras quitar de la familia.</span>
                            </div>

                            <div class="perm-botones">
                                <asp:Button ID="btnCrearFamilia" runat="server" CssClass="boton-accion boton-primario"
                                    Text="Crear Familia" Clave="permisos.btnCrearFamilia" CausesValidation="false" OnClick="btnCrearFamilia_Click" />
                                <asp:Button ID="btnAgregarPermisosFamilia" runat="server" CssClass="boton-accion boton-primario"
                                    Text="Agregar Permisos" Clave="permisos.btnAgregarPermisos" CausesValidation="false" OnClick="btnAgregarPermisosFamilia_Click" />
                            </div>
                            <div class="perm-botones">
                                <asp:Button ID="btnQuitarPermisosFamilia" runat="server" CssClass="boton-accion boton-secundario"
                                    Text="Quitar Permisos" Clave="permisos.btnQuitarPermisos" CausesValidation="false" OnClick="btnQuitarPermisosFamilia_Click" />
                                <asp:Button ID="btnBorrarFamilia" runat="server" CssClass="boton-accion boton-peligro"
                                    Text="Borrar Familia" Clave="permisos.btnBorrarFamilia" CausesValidation="false" OnClick="btnBorrarFamilia_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
