<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PaginaLogin.aspx.cs" Inherits="PaginaLogin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Iniciar Sesión - LO Technologies</title>
    <link rel="stylesheet" type="text/css" href="/Estilos/EstilosLogin.css" />
    <script type="text/javascript" src="/JS/LoginJS.js"></script>
</head>
<body>
    <form id="formLogin" runat="server">
        <div class="login-pagina">
            <div class="login-wrapper">

                <div class="login-logo">LT</div>
                <h1 class="login-brand-titulo">LO TECHNOLOGIES</h1>
                <p class="login-brand-subtitulo">Plataforma para la gestión de locales retail</p>

                <div class="login-tarjeta">
                    <h2>Inicio de Sesión</h2>
                    <p class="login-subtexto">Ingrese sus credenciales.</p>

                    <asp:Label ID="lbError" runat="server" CssClass="login-error-banner" Visible="false"></asp:Label>

                    <div class="form-grupo">
                        <asp:Label ID="lbEmail" runat="server" Text="Correo Electrónico" AssociatedControlID="tbEmail"></asp:Label>
                        <div class="input-con-icono">
                            <span class="icon">&#9993;</span>
                            <asp:TextBox ID="tbEmail" runat="server" TextMode="Email"></asp:TextBox>
                        </div>
                        <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="tbEmail" CssClass="campo-error" ErrorMessage="Ingrese su correo electrónico" Display="Dynamic"></asp:RequiredFieldValidator>
                    </div>

                    <div class="form-grupo">
                        <asp:Label ID="lbContrasenia" runat="server" Text="Contraseña" AssociatedControlID="tbContrasenia"></asp:Label>
                        <div class="input-con-icono">
                            <span class="icon">&#128274;</span>
                            <asp:TextBox ID="tbContrasenia" runat="server" ClientIDMode="Static" TextMode="Password"></asp:TextBox>
                            <button type="button" class="ocultar-contrasenia" onclick="ocultarContrasenia()">&#128065;</button>
                        </div>
                        <asp:RequiredFieldValidator ID="rfvContrasenia" runat="server" ControlToValidate="tbContrasenia" CssClass="campo-error" ErrorMessage="Ingrese su contraseña" Display="Dynamic"></asp:RequiredFieldValidator>
                    </div>

                    <div class="olvide-contrasenia">
                        <a href="#" id="linkOlvideContrasenia" onclick="abrirModalRecuperar(); return false;">¿Olvidaste tu contraseña?</a>
                    </div>

                    <asp:Button ID="btnIngresar" runat="server" CssClass="btn-login" Text="Ingresar" OnClick="btnIngresar_Click" />
                </div>
                <p class="login-piepagina">© 2026 LO Technologies — v1.0</p>
            </div>
        </div>
        <div id="modal-recuperar" class="modal-overlay" style="display:none;">
            <div class="modal-box">
                <h3>Recuperar contraseña</h3>
                <p class="modal-subtexto">Ingresá tu email y te enviamos instrucciones para recuperar tu contraseña.</p>
                <input type="email" id="txtEmailRecuperar" class="modal-input"/>
                <div id="modalMensaje" class="modal-mensaje" style="display:none;"></div>
                <div class="modal-mensaje">
                    <button type="button" class="modal-btn-cancelar" onclick="cerrarModalRecuperar()">Cancelar</button>
                    <button type="button" id="btnEnviarRecuperar" class="modal-btn-enviar" onclick="enviarSolicitudRecuperacion()">Enviar</button>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
