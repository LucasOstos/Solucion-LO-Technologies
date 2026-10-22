<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PaginaPrincipal.aspx.cs" Inherits="PaginaPrincipal" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Página Principal - LO Technologies</title>
    <link rel="stylesheet" type="text/css" href="/Estilos/Comun.css" />
    <link rel="stylesheet" type="text/css" href="/Estilos/EstilosPrincipal.css" />
    <script type="text/javascript" src="/JS/PrincipalJS.js"></script>
</head>
<body>
    <form id="formPaginaPrincipal" runat="server">
        <div class="pagina">
            <div class="contenedor principal-contenedor">

                <div class="principal-hero">
                    <div class="principal-hero-usuario">
                        <span class="avatar principal-avatar" aria-hidden="true">
                            <asp:Literal ID="litInicialesUsuario" runat="server" />
                        </span>
                        <div>
                            <div class="principal-hero-saludo" runat="server" Clave="principal.saludo">Hola,</div>
                            <div class="principal-hero-nombre">
                                <asp:Literal ID="litNombreUsuario" runat="server" /></div>
                            <span class="principal-hero-rol">
                                <asp:Literal ID="litRolUsuario" runat="server" /></span>
                        </div>
                    </div>

                    <div class="principal-hero-acciones">
                        <div id="pnlIdioma" runat="server" class="principal-idioma">
                            <asp:Label runat="server" Text="Idioma" AssociatedControlID="ddlIdioma" CssClass="principal-idioma-etiqueta" Clave="principal.idioma" />
                            <asp:DropDownList ID="ddlIdioma" runat="server" CssClass="principal-idioma-lista"
                                AutoPostBack="true" OnSelectedIndexChanged="ddlIdioma_SelectedIndexChanged" CausesValidation="false" />
                        </div>
                        <button type="button" id="btnAbrirCambiarContrasenia" runat="server" class="principal-accion" onclick="abrirModal('pnlCambiarContrasenia')">
                            <span aria-hidden="true">&#128273;</span>
                            <span runat="server" Clave="principal.btnCambiarContrasenia">Cambiar contraseña</span>
                        </button>
                        <asp:Button ID="btnLogout" runat="server" CssClass="principal-accion principal-accion-salir"
                            Text="Cerrar sesión" Clave="principal.btnCerrarSesion" OnClick="btnLogout_Click" CausesValidation="false" />
                    </div>
                </div>

                <asp:Label ID="lblMensajePrincipal" runat="server" CssClass="mensaje" Visible="false" />

                <div class="principal-seccion">
                    <h2 class="principal-seccion-titulo" runat="server" Clave="principal.seccionLocales">Locales y empresa</h2>
                    <div class="principal-modulos">
                        <asp:HyperLink ID="lnkEmpresas" runat="server" NavigateUrl="~/PaginaEmpresas.aspx" CssClass="principal-modulo" Permiso="Gestionar empresas">
                            <span class="principal-modulo-titulo" runat="server" Clave="principal.empresas.titulo">Empresas</span>
                            <span class="principal-modulo-desc" runat="server" Clave="principal.empresas.desc">Empresas cliente de la plataforma</span>
                        </asp:HyperLink>
                        <asp:HyperLink ID="lnkLocales" runat="server" NavigateUrl="~/PaginaLocales.aspx" CssClass="principal-modulo" Permiso="Gestionar locales">
                            <span class="principal-modulo-titulo" runat="server" Clave="principal.locales.titulo">Locales</span>
                            <span class="principal-modulo-desc" runat="server" Clave="principal.locales.desc">Puntos de venta de cada empresa</span>
                        </asp:HyperLink>
                        <asp:HyperLink ID="lnkLayoutSectores" runat="server" NavigateUrl="~/PaginaLayoutSectores.aspx" CssClass="principal-modulo" Permiso="Gestionar layout y sectores">
                            <span class="principal-modulo-titulo" runat="server" Clave="principal.layout.titulo">Layout y sectores</span>
                            <span class="principal-modulo-desc" runat="server" Clave="principal.layout.desc">Plano del local y sus zonas</span>
                        </asp:HyperLink>
                        <asp:HyperLink ID="lnkProductos" runat="server" NavigateUrl="~/PaginaProductos.aspx" CssClass="principal-modulo" Permiso="Gestionar productos y categorías">
                            <span class="principal-modulo-titulo" runat="server" Clave="principal.productos.titulo">Productos y categorías</span>
                            <span class="principal-modulo-desc" runat="server" Clave="principal.productos.desc">Catálogo y ubicación por sector</span>
                        </asp:HyperLink>
                        <asp:HyperLink ID="lnkStockVentas" runat="server" NavigateUrl="~/PaginaStockVentas.aspx" CssClass="principal-modulo" Permiso="Cargar stock y ventas">
                            <span class="principal-modulo-titulo" runat="server" Clave="principal.stockVentas.titulo">Stock y ventas</span>
                            <span class="principal-modulo-desc" runat="server" Clave="principal.stockVentas.desc">Carga de datos operativos</span>
                        </asp:HyperLink>
                    </div>
                </div>

                <div class="principal-seccion">
                    <h2 class="principal-seccion-titulo" runat="server" Clave="principal.seccionSimulacion">Simulación y análisis</h2>
                    <div class="principal-modulos">
                        <asp:HyperLink ID="lnkEscenarios" runat="server" NavigateUrl="~/PaginaEscenarios.aspx" CssClass="principal-modulo" Permiso="Gestionar escenarios">
                            <span class="principal-modulo-titulo" runat="server" Clave="principal.escenarios.titulo">Escenarios de simulación</span>
                            <span class="principal-modulo-desc" runat="server" Clave="principal.escenarios.desc">Crear y editar escenarios</span>
                        </asp:HyperLink>
                        <asp:HyperLink ID="lnkComparacionEscenarios" runat="server" NavigateUrl="~/PaginaComparacionEscenarios.aspx" CssClass="principal-modulo" Permiso="Comparar escenarios">
                            <span class="principal-modulo-titulo" runat="server" Clave="principal.comparacion.titulo">Comparación de escenarios</span>
                            <span class="principal-modulo-desc" runat="server" Clave="principal.comparacion.desc">Enfrentar dos escenarios</span>
                        </asp:HyperLink>
                        <asp:HyperLink ID="lnkIndicadores" runat="server" NavigateUrl="~/PaginaIndicadores.aspx" CssClass="principal-modulo" Permiso="Visualizar indicadores">
                            <span class="principal-modulo-titulo" runat="server" Clave="principal.indicadores.titulo">Indicadores y recomendaciones</span>
                            <span class="principal-modulo-desc" runat="server" Clave="principal.indicadores.desc">Rendimiento por sector</span>
                        </asp:HyperLink>
                        <asp:HyperLink ID="lnkAnalisisProductos" runat="server" NavigateUrl="~/PaginaAnalisisProductos.aspx" CssClass="principal-modulo" Permiso="Analizar ubicación de productos">
                            <span class="principal-modulo-titulo" runat="server" Clave="principal.analisisProductos.titulo">Análisis de productos</span>
                            <span class="principal-modulo-desc" runat="server" Clave="principal.analisisProductos.desc">Ubicación y desempeño</span>
                        </asp:HyperLink>
                        <asp:HyperLink ID="lnkReportes" runat="server" NavigateUrl="~/PaginaReportes.aspx" CssClass="principal-modulo" Permiso="Generar reportes">
                            <span class="principal-modulo-titulo" runat="server" Clave="principal.reportes.titulo">Reportes</span>
                            <span class="principal-modulo-desc" runat="server" Clave="principal.reportes.desc">Exportar resultados en PDF</span>
                        </asp:HyperLink>
                    </div>
                </div>

                <div class="principal-seccion">
                    <h2 class="principal-seccion-titulo" runat="server" Clave="principal.seccionAdministracion">Administración y seguridad</h2>
                    <div class="principal-modulos">
                        <asp:HyperLink ID="lnkUsuarios" runat="server" NavigateUrl="~/PaginaUsuarios.aspx" CssClass="principal-modulo" Permiso="Gestionar usuarios">
                            <span class="principal-modulo-titulo" runat="server" Clave="principal.usuarios.titulo">Usuarios</span>
                            <span class="principal-modulo-desc" runat="server" Clave="principal.usuarios.desc">Altas, modificaciones y desbloqueos</span>
                        </asp:HyperLink>
                        <asp:HyperLink ID="lnkPermisos" runat="server" NavigateUrl="~/PaginaPermisos.aspx" CssClass="principal-modulo" Permiso="Gestionar permisos">
                            <span class="principal-modulo-titulo" runat="server" Clave="principal.permisos.titulo">Permisos</span>
                            <span class="principal-modulo-desc" runat="server" Clave="principal.permisos.desc">Perfiles y familias de permisos</span>
                        </asp:HyperLink>
                        <asp:HyperLink ID="lnkAuditoria" runat="server" NavigateUrl="~/PaginaAuditorias.aspx" CssClass="principal-modulo" Permiso="Consultar bitácora">
                            <span class="principal-modulo-titulo" runat="server" Clave="principal.bitacora.titulo">Auditoría / Bitácora</span>
                            <span class="principal-modulo-desc" runat="server" Clave="principal.bitacora.desc">Eventos registrados en el sistema</span>
                        </asp:HyperLink>
                        <asp:HyperLink ID="lnkDigitos" runat="server" NavigateUrl="~/PaginaDigitosVerificadores.aspx" CssClass="principal-modulo" Permiso="Recalcular dígitos verificadores">
                            <span class="principal-modulo-titulo" runat="server" Clave="principal.integridad.titulo">Integridad del sistema</span>
                            <span class="principal-modulo-desc" runat="server" Clave="principal.integridad.desc">Dígitos verificadores</span>
                        </asp:HyperLink>
                        <asp:HyperLink ID="lnkBackupRestore" runat="server" NavigateUrl="~/PaginaBackupRestore.aspx" CssClass="principal-modulo" Permiso="Realizar backup y restore">
                            <span class="principal-modulo-titulo" runat="server" Clave="principal.backup.titulo">Backup / Restore</span>
                            <span class="principal-modulo-desc" runat="server" Clave="principal.backup.desc">Copias de seguridad de la base</span>
                        </asp:HyperLink>
                        <asp:HyperLink ID="lnkIdiomas" runat="server" NavigateUrl="~/PaginaIdiomas.aspx" CssClass="principal-modulo" Permiso="Gestionar idiomas">
                            <span class="principal-modulo-titulo" runat="server" Clave="principal.idiomas.titulo">Idiomas</span>
                            <span class="principal-modulo-desc" runat="server" Clave="principal.idiomas.desc">Idiomas y traducciones</span>
                        </asp:HyperLink>
                    </div>
                </div>
            </div>
        </div>

        <asp:Panel ID="pnlCambiarContrasenia" runat="server" ClientIDMode="Static" CssClass="modal-fondo" Style="display: none;">
            <div class="modal-caja" role="dialog" aria-modal="true" aria-labelledby="tituloCambiarContrasenia">
                <div class="modal-encabezado">
                    <h3 id="tituloCambiarContrasenia" runat="server" ClientIDMode="Static" Clave="principal.btnCambiarContrasenia">Cambiar contraseña</h3>
                    <button type="button" runat="server" class="modal-cerrar principal-modal-cerrar" aria-label="Cerrar" ClaveAria="comun.cerrar"
                        onclick="cerrarModal('pnlCambiarContrasenia')">
                        &times;</button>
                </div>
                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Contraseña actual *" AssociatedControlID="tbContraseniaActual" Clave="principal.contraseniaActual" />
                    <asp:TextBox ID="tbContraseniaActual" runat="server" CssClass="modal-campo" TextMode="Password" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="tbContraseniaActual" CssClass="campo-error"
                        ErrorMessage="Ingrese su contraseña actual" Clave="principal.errContraseniaActual" Display="Dynamic" ValidationGroup="vgContrasenia" />
                </div>
                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Contraseña nueva *" AssociatedControlID="tbContraseniaNueva" Clave="principal.contraseniaNueva" />
                    <asp:TextBox ID="tbContraseniaNueva" runat="server" CssClass="modal-campo" TextMode="Password" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="tbContraseniaNueva" CssClass="campo-error"
                        ErrorMessage="Ingrese la contraseña nueva" Clave="principal.errContraseniaNueva" Display="Dynamic" ValidationGroup="vgContrasenia" />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="tbContraseniaNueva" CssClass="campo-error"
                        ValidationExpression="^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&amp;*(),.?&quot;:{}|&lt;&gt;_\-]).{8,}$"
                        ErrorMessage="Mínimo 8 caracteres, con mayúscula, número y carácter especial" Clave="principal.errContraseniaFormato"
                        Display="Dynamic" ValidationGroup="vgContrasenia" />
                </div>
                <div class="grupo-campo">
                    <asp:Label runat="server" Text="Confirmar contraseña nueva *" AssociatedControlID="tbConfirmarContrasenia" Clave="principal.contraseniaConfirmar" />
                    <asp:TextBox ID="tbConfirmarContrasenia" runat="server" CssClass="modal-campo" TextMode="Password" />
                    <asp:CompareValidator runat="server" ControlToValidate="tbConfirmarContrasenia" ControlToCompare="tbContraseniaNueva"
                        CssClass="campo-error" ErrorMessage="Las contraseñas no coinciden" Clave="principal.errContraseniaNoCoincide" Display="Dynamic" ValidationGroup="vgContrasenia" />
                </div>
                <asp:Label ID="lblMensajeContrasenia" runat="server" CssClass="mensaje" Visible="false" />
                <div class="modal-botones">
                    <asp:Button ID="btnCancelarContrasenia" runat="server" CssClass="modal-boton-cancelar"
                        Text="Cancelar" Clave="comun.cancelar" OnClick="btnCancelarContrasenia_Click" CausesValidation="false" />
                    <asp:Button ID="btnGuardarContrasenia" runat="server" CssClass="modal-boton-guardar"
                        Text="Guardar" Clave="comun.guardar" OnClick="btnGuardarContrasenia_Click" ValidationGroup="vgContrasenia" />
                </div>
            </div>
        </asp:Panel>
    </form>
</body>
</html>
