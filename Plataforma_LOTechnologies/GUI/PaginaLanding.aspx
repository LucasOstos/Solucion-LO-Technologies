<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PaginaLanding.aspx.cs" Inherits="PaginaLanding" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>LO Technologies - Gestión inteligente de locales retail</title>
    <link rel="stylesheet" type="text/css" href="/Estilos/EstilosLanding.css" />
    <script type="text/javascript" src="/JS/LandingJS.js"></script>
</head>
<body>
    <form id="formLanding" runat="server">

        <nav class="land-navbar">
            <div class="land-navbar-marca">
                <span class="land-logo">LT</span>
                <span class="land-marca-texto">LO <b>TECHNOLOGIES</b></span>
            </div>
            <div class="land-navbar-links">
                <a href="#inicio">Inicio</a>
                <a href="#planes">Planes</a>
                <a href="#faq">FAQ</a>
                <a href="PaginaLogin.aspx" class="land-boton-navbar land-boton-navbar-outline">Iniciar Sesión</a>
                <a href="#" class="land-boton-navbar land-boton-navbar-solido" onclick="abrirModal('pnlComoAcceder'); return false;">Registrarte</a>
            </div>
        </nav>

        <section id="inicio" class="land-hero">
            <div class="land-hero-contenido">
                <h1>Gestioná tus locales retail con datos, no con intuición</h1>
                <p class="land-hero-subtitulo">
                    LO Technologies es una plataforma SaaS pensada para empresas de retail físico.
                    Registrá tus locales, sectores y productos, cargá tus ventas y stock, y descubrí
                    oportunidades de mejora con indicadores y recomendaciones basadas en datos reales.
                </p>
                <div class="land-hero-botones">
                    <a href="#planes" class="land-boton-navbar land-boton-navbar-solido">Ver planes</a>
                    <a href="#como-acceder" class="land-boton-navbar land-boton-navbar-outline" onclick="abrirModal('pnlComoAcceder'); return false;">¿Cómo empiezo?</a>
                </div>
            </div>
        </section>

        <section class="land-seccion">
            <h2>¿Qué hacemos por tu negocio?</h2>
            <p class="land-seccion-subtitulo">Todo lo que necesitás para tomar mejores decisiones sobre tus locales.</p>

            <div class="land-features-grilla">
                <div class="land-feature-tarjeta">
                    <div class="land-feature-icono">🏬</div>
                    <h3>Locales, sectores y layouts</h3>
                    <p>Registrá cada local, dividilo en sectores y cargá el plano o layout de referencia, todo centralizado en un solo lugar.</p>
                </div>
                <div class="land-feature-tarjeta">
                    <div class="land-feature-icono">📦</div>
                    <h3>Stock y ventas por producto</h3>
                    <p>Cargá tus ventas y el stock disponible de cada producto, para tener información siempre actualizada y confiable.</p>
                </div>
                <div class="land-feature-tarjeta">
                    <div class="land-feature-icono">🧪</div>
                    <h3>Simulación de escenarios</h3>
                    <p>Compará alternativas de reorganización antes de mover un solo mueble, evaluando el impacto estimado de cada cambio.</p>
                </div>
                <div class="land-feature-tarjeta">
                    <div class="land-feature-icono">📊</div>
                    <h3>Indicadores y recomendaciones</h3>
                    <p>Detectá automáticamente sectores de baja circulación, stock crítico y oportunidades de mejora en tus productos.</p>
                </div>
                <div class="land-feature-tarjeta">
                    <div class="land-feature-icono">📄</div>
                    <h3>Reportes con un clic</h3>
                    <p>Generá reportes ejecutivos en PDF con indicadores, comparaciones y recomendaciones, listos para compartir.</p>
                </div>
            </div>
        </section>

        <section id="planes" class="land-seccion">
            <h2>Planes para cada etapa de tu negocio</h2>
            <p class="land-seccion-subtitulo">Elegí el plan que se ajuste a la cantidad de locales que gestionás.</p>

            <div class="land-planes-grilla">
                <div class="land-plan-tarjeta">
                    <h3>LO Starter</h3>
                    <div class="land-plan-precio">800 USD<span>/mes</span></div>
                    <ul class="land-plan-lista">
                        <li>✓ Gestión de 1 empresa, hasta 1 local</li>
                        <li>✓ Sectores, productos y categorías</li>
                        <li>✓ Carga de ventas y stock</li>
                        <li>✓ Indicadores básicos</li>
                        <li>✓ Reportes simples</li>
                        <li>✗ Escenarios comparativos</li>
                    </ul>
                    <a href="#" class="land-plan-boton" onclick="abrirModal('pnlComoAcceder'); return false;">Empezar</a>
                </div>

                <div class="land-plan-tarjeta land-plan-destacado">
                    <span class="land-plan-badge">Más elegido</span>
                    <h3>LO Professional</h3>
                    <div class="land-plan-precio">1.500 USD<span>/mes</span></div>
                    <ul class="land-plan-lista">
                        <li>✓ Todo lo del plan Starter</li>
                        <li>✓ Hasta 5 locales</li>
                        <li>✓ Creación y comparación de escenarios</li>
                        <li>✓ Indicadores por sector y producto</li>
                        <li>✓ Detección de oportunidades de reubicación</li>
                        <li>✓ Reportes ejecutivos</li>
                    </ul>
                    <a href="#" class="land-plan-boton land-plan-boton-solido" onclick="abrirModal('pnlComoAcceder'); return false;">Empezar</a>
                </div>

                <div class="land-plan-tarjeta">
                    <h3>LO Enterprise</h3>
                    <div class="land-plan-precio">Desde 2.500 USD<span>/mes</span></div>
                    <ul class="land-plan-lista">
                        <li>✓ Todo lo del plan Professional</li>
                        <li>✓ Múltiples locales y usuarios</li>
                        <li>✓ Comparación entre sucursales</li>
                        <li>✓ Reportes personalizados</li>
                        <li>✓ Integraciones con ERP, CRM y BI</li>
                        <li>✓ Soporte prioritario</li>
                    </ul>
                    <a href="#" class="land-plan-boton" onclick="abrirModal('pnlComoAcceder'); return false;">Contactar</a>
                </div>
            </div>
            <p class="land-planes-nota">10% de descuento por contratación anual anticipada en todos los planes.</p>
        </section>

        <section class="land-seccion land-seccion-alterna">
            <h2>Lo que dicen nuestros clientes</h2>

            <div class="land-resenas-grilla">
                <div class="land-resena-tarjeta">
                    <div class="land-estrellas-fijas">★★★★★</div>
                    <p>"Detectamos que teníamos productos de alta rotación en el sector con menos circulación. Los movimos y las ventas de esa categoría subieron un 18% el primer mes."</p>
                    <div class="land-resena-autor">— María García, Falabella S.A.</div>
                </div>
                <div class="land-resena-tarjeta">
                    <div class="land-estrellas-fijas">★★★★☆</div>
                    <p>"La simulación de escenarios nos ahorró remodelar el local a ciegas. Pudimos comparar dos propuestas antes de mover un solo mueble."</p>
                    <div class="land-resena-autor">— Carlos Vega, Ripley Corp</div>
                </div>
                <div class="land-resena-tarjeta">
                    <div class="land-estrellas-fijas">★★★★★</div>
                    <p>"Los reportes en PDF nos ahorran horas armando presentaciones para el directorio cada mes."</p>
                    <div class="land-resena-autor">— Pedro Ruiz, Easy S.A.</div>
                </div>
            </div>

            <div class="land-resena-form-caja">
                <h3>Dejanos tu opinión</h3>
                <p class="land-seccion-subtitulo" style="margin-bottom: 16px;">Completá al menos uno de los dos campos.</p>

                <div class="land-estrellas-input" id="contenedorEstrellas">
                    <span class="land-estrella" data-valor="1">★</span>
                    <span class="land-estrella" data-valor="2">★</span>
                    <span class="land-estrella" data-valor="3">★</span>
                    <span class="land-estrella" data-valor="4">★</span>
                    <span class="land-estrella" data-valor="5">★</span>
                </div>
                <asp:HiddenField ID="hfPuntuacion" runat="server" ClientIDMode="Static" Value="0" />

                <asp:TextBox ID="tbComentario" runat="server" ClientIDMode="Static" CssClass="land-textarea" TextMode="MultiLine" Rows="3" placeholder="Contanos tu experiencia..." />

                <asp:CustomValidator ID="cvResena" runat="server" CssClass="campo-error" Display="Dynamic"
                    ErrorMessage="Completá al menos la puntuación o el comentario." OnServerValidate="cvResena_ServerValidate"
                    ClientValidationFunction="validarResenaCliente" />

                <asp:Button ID="btnEnviarResena" runat="server" CssClass="land-boton-navbar land-boton-navbar-solido"
                    Text="Enviar opinión" OnClick="btnEnviarResena_Click" CausesValidation="true" ValidationGroup="vgResena" Style="margin-top: 10px;" />

                <asp:Label ID="lblGraciasResena" runat="server" CssClass="land-mensaje-gracias" Visible="false" Text="¡Gracias por tu opinión!" />
            </div>
        </section>

        <section id="faq" class="land-seccion">
            <h2>Preguntas frecuentes</h2>

            <div class="land-faq-lista">
                <div class="land-faq-item">
                    <div class="land-faq-pregunta" onclick="toggleFaq(this)">¿Qué es LO Technologies? <span>+</span></div>
                    <div class="land-faq-respuesta">Es una plataforma SaaS que permite a empresas de retail físico registrar sus locales, analizar ventas y stock, simular reorganizaciones y recibir recomendaciones basadas en datos.</div>
                </div>
                <div class="land-faq-item">
                    <div class="land-faq-pregunta" onclick="toggleFaq(this)">¿Cómo empiezo a usar la plataforma? <span>+</span></div>
                    <div class="land-faq-respuesta">Primero se registra tu empresa en el sistema. Con esa empresa dada de alta, se crea un usuario Administrador, que es quien luego gestiona al resto de los usuarios de tu organización. Hacé clic en "Registrarte" para ver el detalle.</div>
                </div>
                <div class="land-faq-item">
                    <div class="land-faq-pregunta" onclick="toggleFaq(this)">¿Mis datos están seguros? <span>+</span></div>
                    <div class="land-faq-respuesta">Sí. Las contraseñas se almacenan encriptadas, el acceso está protegido por bloqueo tras intentos fallidos, y contamos con mecanismos de auditoría e integridad de datos sobre toda la información cargada.</div>
                </div>
                <div class="land-faq-item">
                    <div class="land-faq-pregunta" onclick="toggleFaq(this)">¿Puedo cambiar de plan más adelante? <span>+</span></div>
                    <div class="land-faq-respuesta">Sí, podés pasar de Basic a Pro o Enterprise cuando lo necesites, sin perder la información ya cargada.</div>
                </div>
                <div class="land-faq-item">
                    <div class="land-faq-pregunta" onclick="toggleFaq(this)">¿Qué pasa si me olvido mi contraseña? <span>+</span></div>
                    <div class="land-faq-respuesta">Desde la pantalla de inicio de sesión podés solicitar la recuperación con tu correo registrado, y vas a recibir un mail con instrucciones para definir una nueva.</div>
                </div>
            </div>
        </section>

        <footer class="land-footer">
            © 2026 LO Technologies. Todos los derechos reservados.
        </footer>

        <div id="pnlComoAcceder" class="modal-fondo" style="display: none;">
            <div class="modal-caja">
                <div class="modal-encabezado">
                    <h3>¿Cómo accedo a la plataforma?</h3>
                    <span class="modal-cerrar" onclick="cerrarModal('pnlComoAcceder')">&times;</span>
                </div>

                <p>LO Technologies es un servicio que se contrata a nivel empresa, no hay un registro individual abierto al público. El acceso funciona así:</p>

                <div class="land-paso">
                    <div class="land-paso-numero">1</div>
                    <div><strong>Se registra tu empresa.</strong> Nuestro equipo (o un administrador del sistema) da de alta a tu empresa cliente en la plataforma.</div>
                </div>
                <div class="land-paso">
                    <div class="land-paso-numero">2</div>
                    <div><strong>Se crea un usuario Administrador.</strong> Esa persona va a recibir sus credenciales para ingresar por primera vez.</div>
                </div>
                <div class="land-paso">
                    <div class="land-paso-numero">3</div>
                    <div><strong>El Administrador gestiona al resto del equipo.</strong> Desde la plataforma, crea los usuarios que necesite tu organización.</div>
                </div>

                <p style="margin-top: 16px;">¿Querés contratar el servicio para tu empresa? Escribinos a <a href="mailto:lotechnologies.contacto@gmail.com">lotechnologies.contacto@gmail.com</a>.</p>

                <div class="modal-botones">
                    <button type="button" class="modal-boton-cancelar" onclick="cerrarModal('pnlComoAcceder')">Cerrar</button>
                </div>
            </div>
        </div>

    </form>
</body>
</html>
