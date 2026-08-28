<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PaginaEscenarios.aspx.cs" Inherits="PaginaEscenarios" %>

<%@ Register TagPrefix="uc" TagName="BotonVolverMenu" Src="~/Controles/BotonVolverMenu.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Escenarios - LO Technologies</title>
    <link rel="stylesheet" type="text/css" href="/Estilos/BotonVolverMenu.css" />
    <link rel="stylesheet" type="text/css" href="/Estilos/Comun.css" />
    <link rel="stylesheet" type="text/css" href="/Estilos/EstilosEscenarios.css" />
    <script type="text/javascript" src="/JS/EscenariosJS.js"></script>
</head>
<body>
    <form id="formEscenarios" runat="server">
        <div class="pagina">
            <div class="contenedor">

                <uc:BotonVolverMenu ID="volverMenu" runat="server" />

                <div class="encabezado">
                    <div>
                        <h1>Escenarios de Simulación</h1>
                        <p class="subtitulo">CUN 007 — Crear y gestionar escenarios alternativos</p>
                    </div>
                    <asp:Button ID="btnNuevoEscenario" runat="server" CssClass="boton-nuevo" Text="+ Nuevo Escenario" OnClick="btnNuevoEscenario_Click" CausesValidation="false" />
                </div>

                <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" Visible="false" />

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Local" AssociatedControlID="ddlLocal" />
                    <asp:DropDownList ID="ddlLocal" runat="server" CssClass="selector" AutoPostBack="true" OnSelectedIndexChanged="ddlLocal_SelectedIndexChanged" />
                </div>

                <asp:Repeater ID="rptEscenarios" runat="server" OnItemCommand="rptEscenarios_ItemCommand">
                    <HeaderTemplate>
                        <div class="esc-grilla">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="esc-tarjeta">
                            <div class="esc-tarjeta-header">
                                <span class="texto-principal">&#128100; <%# Eval("Nombre") %></span>
                                <span class='<%# "etiqueta " + ObtenerClaseEstado(Eval("Estado").ToString()) %>'><%# Eval("Estado") %></span>
                            </div>
                            <div class="texto-secundario"><%# Eval("Descripcion") %></div>

                            <div class="esc-stats">
                                <div>
                                    <div class="texto-secundario">Ventas est.</div>
                                    <div class="texto-principal"><%# Eval("VentasEstimadas") %></div>
                                </div>
                                <div>
                                    <div class="texto-secundario">Circulación</div>
                                    <div class="texto-principal"><%# Eval("CirculacionEstimada") %>%</div>
                                </div>
                                <div>
                                    <div class="texto-secundario">Satisfacción</div>
                                    <div class="texto-principal"><%# Eval("SatisfaccionEstimada") %>/100</div>
                                </div>
                            </div>

                            <div class="esc-tarjeta-botones">
                                <asp:LinkButton runat="server" CssClass="boton-secundario esc-boton-full" ToolTip="Ver detalle"
                                    CommandName="VerDetalle" CommandArgument='<%# Eval("CodigoEscenario") %>'>Ver detalle</asp:LinkButton>
                                <asp:LinkButton runat="server" CssClass="boton-nuevo esc-boton-full" Visible='<%# Eval("Estado").ToString() != "Base" %>'
                                    ToolTip="Comparar" CommandName="Comparar" CommandArgument='<%# Eval("CodigoEscenario") %>'>Comparar</asp:LinkButton>
                            </div>
                        </div>
                    </ItemTemplate>
                    <FooterTemplate></div></FooterTemplate>
                </asp:Repeater>

                <asp:Label ID="lblSinEscenarios" runat="server" Text="Este local todavía no tiene escenarios creados." Visible="false" CssClass="sin-resultados" />

            </div>
        </div>

        <asp:Panel ID="pnlModalEscenario" runat="server" ClientIDMode="Static" CssClass="modal-fondo" Style="display: none;">
            <div class="modal-caja">
                <div class="modal-encabezado">
                    <h3>Nuevo Escenario</h3>
                    <span class="modal-cerrar" onclick="cerrarModal('pnlModalEscenario')">&times;</span>
                </div>

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Nombre del Escenario *" AssociatedControlID="tbNombreEscenario" />
                    <asp:TextBox ID="tbNombreEscenario" runat="server" CssClass="modal-campo" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="tbNombreEscenario" CssClass="campo-error"
                        ErrorMessage="Ingrese el nombre" Display="Dynamic" ValidationGroup="vgEscenario" />
                </div>

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Descripción" AssociatedControlID="tbDescripcionEscenario" />
                    <asp:TextBox ID="tbDescripcionEscenario" runat="server" CssClass="modal-campo" TextMode="MultiLine" Rows="2" />
                </div>

                <div class="grupo-campo">
                    <label>Cambios propuestos de sectores</label>
                    <%-- Se puebla dinámicamente desde los Sectores reales del
                         local seleccionado (tabla Sector), no hardcodeado. --%>
                    <asp:Repeater ID="rptCambiosPorSector" runat="server">
                        <ItemTemplate>
                            <div class="esc-fila-sector">
                                <span class="texto-secundario"><%# Eval("Nombre") %></span>
                                <asp:HiddenField runat="server" Value='<%# Eval("CodigoSector") %>' />
                                <asp:DropDownList runat="server" CssClass="modal-campo esc-select-cambio">
                                    <asp:ListItem Text="Sin cambios" Value="" />
                                    <asp:ListItem Text="Mover al frente" Value="Mover al frente" />
                                    <asp:ListItem Text="Mover al fondo" Value="Mover al fondo" />
                                    <asp:ListItem Text="Ampliar superficie" Value="Ampliar superficie" />
                                    <asp:ListItem Text="Reducir superficie" Value="Reducir superficie" />
                                    <asp:ListItem Text="Reubicar productos destacados" Value="Reubicar productos destacados" />
                                </asp:DropDownList>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>

                <div class="modal-botones">
                    <asp:Button ID="btnCancelarEscenario" runat="server" CssClass="modal-boton-cancelar"
                        Text="Cancelar" OnClick="btnCancelarEscenario_Click" CausesValidation="false" />
                    <asp:Button ID="btnGuardarEscenario" runat="server" CssClass="modal-boton-guardar"
                        Text="Crear Escenario" OnClick="btnGuardarEscenario_Click" ValidationGroup="vgEscenario" />
                </div>
            </div>
        </asp:Panel>
    </form>
</body>
</html>
