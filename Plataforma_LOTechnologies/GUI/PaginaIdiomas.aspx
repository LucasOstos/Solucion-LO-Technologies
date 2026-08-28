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
                        <p class="subtitulo">CUS 016 — Agregar y activar/desactivar idiomas disponibles</p>
                    </div>
                    <asp:Button ID="btnAgregarIdioma" runat="server" CssClass="boton-nuevo" Text="Agregar Idioma" OnClick="btnAgregarIdioma_Click" CausesValidation="false" />
                </div>

                <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" Visible="false" />

                <asp:Repeater ID="rptIdiomas" runat="server" OnItemCommand="rptIdiomas_ItemCommand">
                    <HeaderTemplate>
                        <div class="idi-grilla">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="idi-tarjeta">
                            <div class="idi-tarjeta-header">
                                <span class="idi-bandera"><%# Eval("Codigo").ToString().ToUpper() %></span>
                                <div class="idi-nombre-bloque">
                                    <span class="texto-principal">
                                        <%# Eval("Nombre") %>
                                        <asp:Label runat="server" CssClass="etiqueta etiqueta-info" Text="Predeterminado" Visible='<%# (bool)Eval("Predeterminado") %>' />
                                    </span>
                                    <div class="texto-secundario"><%# Eval("NombreNativo") %> &middot; <%# Eval("Codigo").ToString().ToUpper() %></div>
                                </div>
                                <label class="idi-switch">
                                    <asp:CheckBox runat="server" Checked='<%# (int)Eval("Estado") == 1 %>' Enabled='<%# !(bool)Eval("Predeterminado") %>' />
                                </label>
                            </div>

                            <div class="texto-secundario">Completitud de traducción</div>
                            <div class="idi-barra">
                                <div class='<%# "idi-barra-relleno " + ObtenerClaseCompletitud(Eval("Completitud")) %>' style='<%# "width:" + Eval("Completitud") + "%" %>'></div>
                            </div>

                            <div class="idi-tarjeta-footer">
                                <span class='<%# (int)Eval("Estado") == 1 ? "etiqueta etiqueta-activo" : "etiqueta etiqueta-inactivo" %>'>
                                    <%# (int)Eval("Estado") == 1 ? "Activo" : "Inactivo" %>
                                </span>
                                <asp:LinkButton runat="server" CssClass="boton-icono" ToolTip="Editar traducciones"
                                    CommandName="EditarTraducciones" CommandArgument='<%# Eval("CodigoIdioma") %>'>Editar traducciones</asp:LinkButton>
                            </div>
                        </div>
                    </ItemTemplate>
                    <FooterTemplate></div></FooterTemplate>
                </asp:Repeater>

            </div>
        </div>

        <asp:Panel ID="pnlModalIdioma" runat="server" ClientIDMode="Static" CssClass="modal-fondo" Style="display: none;">
            <div class="modal-caja">
                <div class="modal-encabezado">
                    <h3>Agregar Idioma</h3>
                    <span class="modal-cerrar" onclick="cerrarModal('pnlModalIdioma')">&times;</span>
                </div>

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Idioma *" AssociatedControlID="ddlNuevoIdioma" />
                    <asp:DropDownList ID="ddlNuevoIdioma" runat="server" CssClass="modal-campo">
                        <asp:ListItem Text="Italiano" Value="it|Italiano" />
                        <asp:ListItem Text="Alemán" Value="de|Deutsch" />
                        <asp:ListItem Text="Chino" Value="zh|中文" />
                        <asp:ListItem Text="Portugués" Value="pt|Português" />
                        <asp:ListItem Text="Francés" Value="fr|Français" />
                    </asp:DropDownList>
                </div>

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Código ISO" AssociatedControlID="tbCodigoIso" />
                    <asp:TextBox ID="tbCodigoIso" runat="server" CssClass="modal-campo" placeholder="Ej: de, it, zh" MaxLength="10" />
                </div>

                <div class="grupo-campo grupo-casilla">
                    <asp:CheckBox ID="chkActivarAlAgregar" runat="server" />
                    <asp:Label runat="server" Text="Activar al agregar" AssociatedControlID="chkActivarAlAgregar" />
                </div>

                <p class="texto-secundario">
                    Al confirmar, el sistema traduce automáticamente todas las claves
                    existentes desde Español usando el servicio de traducción
                    configurado. Después vas a poder revisar y corregir cualquier
                    traducción desde "Editar traducciones".
                </p>

                <div class="modal-botones">
                    <asp:Button ID="btnCancelarIdioma" runat="server" CssClass="modal-boton-cancelar"
                        Text="Cancelar" OnClick="btnCancelarIdioma_Click" CausesValidation="false" />
                    <asp:Button ID="btnGuardarIdioma" runat="server" CssClass="modal-boton-guardar"
                        Text="Agregar Idioma" OnClick="btnGuardarIdioma_Click" CausesValidation="false" />
                </div>
            </div>
        </asp:Panel>
    </form>
</body>
</html>
