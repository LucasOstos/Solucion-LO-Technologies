<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PaginaIdiomas.aspx.cs" Inherits="PaginaIdiomas" %>

<%@ Register TagPrefix="uc" TagName="BotonVolverMenu" Src="~/Controles/BotonVolverMenu.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Gestión de Idiomas - LO Technologies</title>
    <link rel="stylesheet" type="text/css" href="/Estilos/BotonVolverMenu.css" />
    <link rel="stylesheet" type="text/css" href="/Estilos/Comun.css" />
    <link rel="stylesheet" type="text/css" href="/Estilos/EstilosIdiomas.css" />
    <script type="text/javascript" src="/JS/IdiomasJS.js"></script>
</head>
<body>
    <form id="formIdiomas" runat="server">
        <div class="pagina">
            <div class="contenedor">

                <uc:BotonVolverMenu ID="volverMenu" runat="server" />

                <div class="encabezado">
                    <div>
                        <h1>Gestión de Idiomas</h1>
                        <p class="subtitulo">Agregá idiomas del catálogo, revisá sus traducciones y activalos para los usuarios.</p>
                    </div>
                    <div class="idi-encabezado-acciones">
                        <asp:Button ID="btnSincronizar" runat="server" CssClass="idi-boton-secundario" Text="Sincronizar claves"
                            ToolTip="Agrega a todos los idiomas las claves nuevas del catálogo, sin modificar las traducciones existentes"
                            OnClick="btnSincronizar_Click" CausesValidation="false" />
                        <asp:Button ID="btnAgregarIdioma" runat="server" CssClass="boton-nuevo" Text="Agregar idioma"
                            OnClick="btnAgregarIdioma_Click" CausesValidation="false" />
                    </div>
                </div>

                <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" Visible="false" EnableViewState="false" />

                <asp:Repeater ID="rptIdiomas" runat="server" OnItemCommand="rptIdiomas_ItemCommand">
                    <HeaderTemplate>
                        <div class="idi-grilla">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="idi-tarjeta">
                            <div class="idi-tarjeta-header">
                                <span class="idi-bandera" aria-hidden="true"><%# Eval("Codigo").ToString().ToUpper() %></span>
                                <div class="idi-nombre-bloque">
                                    <span class="texto-principal">
                                        <%#: Eval("Nombre") %>
                                        <asp:Label runat="server" CssClass="etiqueta etiqueta-info" Text="Predeterminado" Visible='<%# (bool)Eval("Predeterminado") %>' />
                                    </span>
                                    <div class="texto-secundario">
                                        <span lang='<%#: Eval("Codigo") %>'><%#: Eval("NombreNativo") %></span> &middot; <%#: Eval("Codigo").ToString().ToUpper() %>
                                    </div>
                                </div>
                                <span class='<%# (bool)Eval("Estado") ? "etiqueta etiqueta-activo" : "etiqueta etiqueta-inactivo" %>'>
                                    <%# (bool)Eval("Estado") ? "Activo" : "Inactivo" %>
                                </span>
                            </div>

                            <div class="idi-completitud-texto">
                                <span class="texto-secundario">Traducciones revisadas</span>
                                <span class="idi-completitud-valor"><%# Eval("Completitud") %>%</span>
                            </div>
                            <div class="idi-barra" role="progressbar" aria-valuemin="0" aria-valuemax="100"
                                aria-valuenow='<%# Eval("Completitud") %>' aria-label='<%#: "Traducciones revisadas de " + Eval("Nombre") %>'>
                                <div class='<%# "idi-barra-relleno " + ObtenerClaseCompletitud(Eval("Completitud")) %>' style='<%# "width:" + Eval("Completitud") + "%" %>'></div>
                            </div>

                            <div class="idi-tarjeta-footer">
                                <asp:LinkButton runat="server" CssClass="idi-accion"
                                    CommandName="EditarTraducciones" CommandArgument='<%# Eval("CodigoIdioma") %>'>Editar traducciones</asp:LinkButton>
                                <asp:LinkButton runat="server" Visible='<%# !(bool)Eval("Predeterminado") %>'
                                    CssClass='<%# (bool)Eval("Estado") ? "idi-accion idi-accion-desactivar" : "idi-accion idi-accion-activar" %>'
                                    CommandName='<%# (bool)Eval("Estado") ? "Desactivar" : "Activar" %>' CommandArgument='<%# Eval("CodigoIdioma") %>'
                                    Text='<%# (bool)Eval("Estado") ? "Desactivar" : "Activar" %>' />
                                <asp:Label runat="server" CssClass="texto-secundario" Visible='<%# (bool)Eval("Predeterminado") %>'
                                    Text="Idioma base: no se desactiva" />
                            </div>
                        </div>
                    </ItemTemplate>
                    <FooterTemplate></div></FooterTemplate>
                </asp:Repeater>

            </div>
        </div>

        <asp:Panel ID="pnlModalIdioma" runat="server" ClientIDMode="Static" CssClass="modal-fondo" Style="display: none;">
            <div class="modal-caja" role="dialog" aria-modal="true" aria-labelledby="tituloAgregarIdioma">
                <div class="modal-encabezado">
                    <h3 id="tituloAgregarIdioma">Agregar idioma</h3>
                    <button type="button" class="modal-cerrar idi-modal-cerrar" aria-label="Cerrar" onclick="cerrarModal('pnlModalIdioma')">&times;</button>
                </div>

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Idioma *" AssociatedControlID="ddlNuevoIdioma" />
                    <asp:DropDownList ID="ddlNuevoIdioma" runat="server" CssClass="modal-campo" />
                </div>

                <p class="texto-secundario idi-modal-ayuda">
                    Se cargan todas las claves del sistema con los textos del catálogo de ese idioma.
                    Las que no estén en el catálogo quedan con el texto en español, marcadas como
                    pendientes. El idioma queda <strong>inactivo</strong>: revisá las traducciones
                    desde "Editar traducciones" y después activalo.
                </p>

                <div class="modal-botones">
                    <asp:Button ID="btnCancelarIdioma" runat="server" CssClass="modal-boton-cancelar"
                        Text="Cancelar" OnClick="btnCancelarIdioma_Click" CausesValidation="false" />
                    <asp:Button ID="btnGuardarIdioma" runat="server" CssClass="modal-boton-guardar"
                        Text="Agregar idioma" OnClick="btnGuardarIdioma_Click" CausesValidation="false" />
                </div>
            </div>
        </asp:Panel>
    </form>
</body>
</html>
