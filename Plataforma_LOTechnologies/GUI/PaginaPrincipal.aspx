<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PaginaPrincipal.aspx.cs" Inherits="PaginaPrincipal" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Página Principal - LO Technologies</title>
    <link rel="stylesheet" type="text/css" href="/Estilos/Comun.css" />
    <link rel="stylesheet" type="text/css" href="/Estilos/EstilosPrincipal.css" />
    <script type="text/javascript" src="/JS/PrincipalJS.js"></script>
</head>
<body>
    <form id="formPaginaPrincipal" runat="server">
        <div class="pagina">
            <div class="contenedor">
                <div class="principal-hero">
                    <div class="principal-hero-usuario">
                        <div>
                            <div class="principal-hero-nombre"><asp:Literal ID="litNombreUsuario" runat="server" /></div>
                            <div class="principal-hero-rol"><asp:Literal ID="litRolUsuario" runat="server" /></div>
                        </div>
                    </div>

                    <div class="principal-avatar-contenedor" id="contenedorAvatarUsuario">
                        <span class="avatar principal-avatar" onclick="toggleMenuUsuario()">
                            <asp:Literal ID="litInicialesUsuario" runat="server" />
                        </span>
                        <div class="principal-menu-desplegable" id="menuDesplegableUsuario">
                            <button type="button" class="principal-menu-item" onclick="abrirModal('pnlCambiarContrasenia')">
                                &#128272; Cambiar contraseña
                            </button>
                            <div class="principal-menu-separador"></div>
                            <button type="button" class="principal-menu-item principal-item-peligro" onclick="dispararLogout()">
                                &#8594; Cerrar sesión
                            </button>
                        </div>
                    </div>
                </div>

                <div>
                    <asp:Button ID="btnLogout" runat="server" ClientIDMode="Static" Text="Cerrar Sesión" OnClick="btnLogout_Click" Style="display:none;" />
                    <asp:Button ID="btnUsuarios" runat="server" Text="Usuarios" OnClick="btnUsuarios_Click" />
                    <asp:Button ID="btnDigitos" runat="server" Text="Integridad del sistema" OnClick="btnDigitos_Click" />
                    <asp:Button ID="btnEmpresas" runat="server" Text="Empresas" OnClick="btnEmpresas_Click" />
                    <asp:Button ID="btnBackupRestore" runat="server" Text="Backup / Restore" OnClick="btnBackupRestore_Click" />
                    <asp:Button ID="btnAuditoria" runat="server" Text="Auditoría / Bitácora" OnClick="btnAuditoria_Click" />
                    <asp:Button ID="btnIdiomas" runat="server" Text="Idiomas" OnClick="btnIdiomas_Click" />
                    <asp:Button ID="btnLocales" runat="server" Text="Locales" OnClick="btnLocales_Click" />
                    <asp:Button ID="btnLayoutSectores" runat="server" Text="Layout y Sectores" OnClick="btnLayoutSectores_Click" />
                    <asp:Button ID="btnProductos" runat="server" Text="Productos y Categorías" OnClick="btnProductos_Click" />
                    <asp:Button ID="btnStockVentas" runat="server" Text="Stock y Ventas" OnClick="btnStockVentas_Click" />
                    <asp:Button ID="btnEscenarios" runat="server" Text="Escenarios de Simulación" OnClick="btnEscenarios_Click" />
                    <asp:Button ID="btnComparacionEscenarios" runat="server" Text="Comparación de Escenarios" OnClick="btnComparacionEscenarios_Click" />
                    <asp:Button ID="btnIndicadores" runat="server" Text="Indicadores y Recomendaciones" OnClick="btnIndicadores_Click" />
                    <asp:Button ID="btnAnalisisProductos" runat="server" Text="Análisis de Productos" OnClick="btnAnalisisProductos_Click" />
                    <asp:Button ID="btnReportes" runat="server" Text="Reportes" OnClick="btnReportes_Click" />
                </div>

            </div>
        </div>

        <asp:Panel ID="pnlCambiarContrasenia" runat="server" ClientIDMode="Static" CssClass="modal-fondo" Style="display:none;">
            <div class="modal-caja">
                <div class="modal-encabezado">
                    <h3>Cambiar Contraseña</h3>
                    <span class="modal-cerrar" onclick="cerrarModal('pnlCambiarContrasenia')">&times;</span>
                </div>
                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Contraseña actual *" AssociatedControlID="tbContraseniaActual" />
                    <asp:TextBox ID="tbContraseniaActual" runat="server" CssClass="modal-campo" TextMode="Password" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="tbContraseniaActual" CssClass="campo-error"
                        ErrorMessage="Ingrese su contraseña actual" Display="Dynamic" ValidationGroup="vgContrasenia" />
                </div>
                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Contraseña nueva *" AssociatedControlID="tbContraseniaNueva" />
                    <asp:TextBox ID="tbContraseniaNueva" runat="server" CssClass="modal-campo" TextMode="Password" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="tbContraseniaNueva" CssClass="campo-error"
                        ErrorMessage="Ingrese la contraseña nueva" Display="Dynamic" ValidationGroup="vgContrasenia" />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="tbContraseniaNueva" CssClass="campo-error"
                        ValidationExpression="^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&amp;*(),.?&quot;:{}|&lt;&gt;_\-]).{8,}$"
                        ErrorMessage="Mínimo 8 caracteres, con mayúscula, número y carácter especial"
                        Display="Dynamic" ValidationGroup="vgContrasenia" />
                </div>
                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Confirmar contraseña nueva *" AssociatedControlID="tbConfirmarContrasenia" />
                    <asp:TextBox ID="tbConfirmarContrasenia" runat="server" CssClass="modal-campo" TextMode="Password" />
                    <asp:CompareValidator runat="server" ControlToValidate="tbConfirmarContrasenia" ControlToCompare="tbContraseniaNueva"
                        CssClass="campo-error" ErrorMessage="Las contraseñas no coinciden" Display="Dynamic" ValidationGroup="vgContrasenia" />
                </div>
                <asp:Label ID="lblMensajeContrasenia" runat="server" CssClass="mensaje" Visible="false" />
                <div class="modal-botones">
                    <asp:Button ID="btnCancelarContrasenia" runat="server" CssClass="modal-boton-cancelar"
                        Text="Cancelar" OnClick="btnCancelarContrasenia_Click" CausesValidation="false" />
                    <asp:Button ID="btnGuardarContrasenia" runat="server" CssClass="modal-boton-guardar"
                        Text="Guardar" OnClick="btnGuardarContrasenia_Click" ValidationGroup="vgContrasenia" />
                </div>
            </div>
        </asp:Panel>
    </form>
</body>
</html>
