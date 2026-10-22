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
                        <h1 runat="server" Clave="principal.escenarios.titulo">Escenarios de Simulación</h1>
                        <p class="subtitulo" runat="server" Clave="escenarios.subtitulo">CUN 007 — Crear y gestionar escenarios alternativos</p>
                    </div>
                    <asp:Button ID="btnNuevoEscenario" runat="server" CssClass="boton-nuevo" Text="+ Nuevo Escenario" Clave="escenarios.btnNuevo" OnClick="btnNuevoEscenario_Click" CausesValidation="false" />
                </div>

                <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" Visible="false" EnableViewState="false" />

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Local" AssociatedControlID="ddlLocal" Clave="comun.local" />
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
                                <span class='<%# "etiqueta " + ObtenerClaseEstado(Eval("Estado").ToString()) %>'><%# TraducirValor(Eval("Estado")) %></span>
                            </div>
                            <div class="texto-secundario"><%# Eval("Descripcion") %></div>

                            <div class="esc-stats">
                                <div>
                                    <div class="texto-secundario"><%# Traducir("escenarios.ventasEst") %></div>
                                    <div class="texto-principal"><%# Eval("VentasEstimadas") %></div>
                                </div>
                                <div>
                                    <div class="texto-secundario"><%# Traducir("layout.colCirculacion") %></div>
                                    <div class="texto-principal"><%# Eval("CirculacionEstimada") %>%</div>
                                </div>
                                <div>
                                    <div class="texto-secundario"><%# Traducir("escenarios.satisfaccion") %></div>
                                    <div class="texto-principal"><%# Eval("SatisfaccionEstimada") %>/100</div>
                                </div>
                            </div>

                            <div class="esc-tarjeta-botones">
                                <asp:LinkButton runat="server" CssClass="boton-secundario esc-boton-full" ToolTip="Ver detalle" ClaveTitle="layout.verDetalle"
                                    CommandName="VerDetalle" CommandArgument='<%# Eval("CodigoEscenario") %>' Text="Ver detalle" Clave="layout.verDetalle" />
                                <asp:LinkButton runat="server" CssClass="boton-nuevo esc-boton-full" Visible='<%# Eval("Estado").ToString() != "Base" %>'
                                    ToolTip="Comparar" ClaveTitle="escenarios.comparar" CommandName="Comparar" CommandArgument='<%# Eval("CodigoEscenario") %>' Text="Comparar" Clave="escenarios.comparar" />
                            </div>
                        </div>
                    </ItemTemplate>
                    <FooterTemplate></div></FooterTemplate>
                </asp:Repeater>

                <asp:Label ID="lblSinEscenarios" runat="server" Text="Este local todavía no tiene escenarios creados." Clave="escenarios.sinEscenarios" Visible="false" CssClass="sin-resultados" />

            </div>
        </div>

        <asp:Panel ID="pnlModalEscenario" runat="server" ClientIDMode="Static" CssClass="modal-fondo" Style="display: none;">
            <div class="modal-caja">
                <div class="modal-encabezado">
                    <h3 runat="server" Clave="escenarios.modalNuevo">Nuevo Escenario</h3>
                    <span class="modal-cerrar" onclick="cerrarModal('pnlModalEscenario')">&times;</span>
                </div>

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Nombre del Escenario *" AssociatedControlID="tbNombreEscenario" Clave="escenarios.nombre" />
                    <asp:TextBox ID="tbNombreEscenario" runat="server" CssClass="modal-campo" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="tbNombreEscenario" CssClass="campo-error"
                        ErrorMessage="Ingrese el nombre" Clave="comun.errNombre" Display="Dynamic" ValidationGroup="vgEscenario" />
                </div>

                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Descripción" AssociatedControlID="tbDescripcionEscenario" Clave="comun.descripcion" />
                    <asp:TextBox ID="tbDescripcionEscenario" runat="server" CssClass="modal-campo" TextMode="MultiLine" Rows="2" />
                </div>
                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Objetivo" AssociatedControlID="tbObjetivoEscenario" Clave="escenarios.objetivo" />
                    <asp:TextBox ID="tbObjetivoEscenario" runat="server" CssClass="modal-campo"
                        placeholder="Ej: Aumentar la circulación en el sector Electrónica" ClavePlaceholder="escenarios.ejObjetivo" />
                </div>
                <div class="grupo-campo">
                    <label runat="server" Clave="escenarios.cambiosPropuestosSectores">Cambios propuestos de sectores</label>
                    <%-- Se puebla dinámicamente desde los Sectores reales del
                         local seleccionado (tabla Sector), no hardcodeado. --%>
                    <asp:Repeater ID="rptCambiosPorSector" runat="server">
                        <ItemTemplate>
                            <div class="esc-fila-sector">
                                <span class="texto-secundario"><%# Eval("Nombre") %></span>
                                <asp:HiddenField runat="server" Value='<%# Eval("CodigoSector") %>' />
                                <asp:DropDownList ID="ddlCambioSector" runat="server" CssClass="modal-campo esc-select-cambio">
                                    <asp:ListItem Text="Sin cambios" Value="" Clave="escenarios.sinCambios" />
                                    <asp:ListItem Text="Mover al frente" Value="Mover al frente" Clave="valor.moverFrente" />
                                    <asp:ListItem Text="Mover al fondo" Value="Mover al fondo" Clave="valor.moverFondo" />
                                    <asp:ListItem Text="Ampliar superficie" Value="Ampliar superficie" Clave="valor.ampliarSuperficie" />
                                    <asp:ListItem Text="Reducir superficie" Value="Reducir superficie" Clave="valor.reducirSuperficie" />
                                    <asp:ListItem Text="Reubicar productos destacados" Value="Reubicar productos destacados" Clave="valor.reubicarDestacados" />
                                </asp:DropDownList>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>

                <div class="modal-botones">
                    <asp:Button ID="btnCancelarEscenario" runat="server" CssClass="modal-boton-cancelar"
                        Text="Cancelar" Clave="comun.cancelar" OnClick="btnCancelarEscenario_Click" CausesValidation="false" />
                    <asp:Button ID="btnGuardarEscenario" runat="server" CssClass="modal-boton-guardar"
                        Text="Crear Escenario" Clave="escenarios.btnCrear" OnClick="btnGuardarEscenario_Click" ValidationGroup="vgEscenario" />
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlDetalleEscenario" runat="server" ClientIDMode="Static" CssClass="modal-fondo" Style="display: none;">
            <div class="modal-caja">
                <div class="modal-encabezado">
                    <h3>
                        <asp:Literal ID="litNombreDetalleEscenario" runat="server" /></h3>
                    <span class="modal-cerrar" onclick="cerrarModal('pnlDetalleEscenario')">&times;</span>
                </div>

                <p><strong runat="server" Clave="layout.estado">Estado:</strong>
                    <asp:Literal ID="litEstadoDetalleEscenario" runat="server" /></p>
                <p><strong runat="server" Clave="layout.descripcion">Descripción:</strong>
                    <asp:Literal ID="litDescripcionDetalleEscenario" runat="server" /></p>
                <p><strong runat="server" Clave="escenarios.objetivoDetalle">Objetivo:</strong>
                    <asp:Literal ID="litObjetivoDetalleEscenario" runat="server" /></p>

                <div class="grupo-campo">
                    <label runat="server" Clave="escenarios.cambiosPropuestos">Cambios propuestos</label>
                    <asp:Repeater ID="rptDetalleCambios" runat="server">
                        <ItemTemplate>
                            <div class="esc-cambio-item">
                                <span class="etiqueta etiqueta-info"><%# TraducirValor(Eval("TipoCambio")) %></span>
                                <span><%# TraducirCambio(Eval("Descripcion")) %></span>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Label ID="lblSinCambiosDetalle" runat="server" Text="Este escenario no tiene cambios propuestos cargados." Clave="escenarios.sinCambiosDetalle" Visible="false" CssClass="sin-resultados" />
                </div>
                <div class="modal-botones">
                    <asp:Button runat="server" CssClass="modal-boton-cancelar" Text="Cerrar" Clave="comun.cerrar"
                        OnClientClick="cerrarModal('pnlDetalleEscenario'); return false;" />
                </div>
            </div>
        </asp:Panel>
    </form>
</body>
</html>
