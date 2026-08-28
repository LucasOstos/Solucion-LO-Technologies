function ocultarContrasenia() {
    var input = document.getElementById('tbContrasenia');
    input.type = (input.type === 'password') ? 'text' : 'password';
}

function abrirModalRecuperar() {
    document.getElementById('modal-recuperar').style.display = 'flex';
    document.getElementById('txtEmailRecuperar').value = '';
    ocultarMensajeModal();
}

function ocultarMensajeModal() {
    var mensaje = document.getElementById('modalMensaje');
    mensaje.style.display = 'none';
    mensaje.textContent = '';
}

function cerrarModalRecuperar() {
    document.getElementById('modal-recuperar').style.display = 'none';
}

function mostrarMensajeModal(texto) {
    var mensaje = document.getElementById('modalMensaje');
    mensaje.textContent = texto;
    mensaje.style.display = 'block';
}

function enviarSolicitudRecuperacion() {
    var email = document.getElementById('txtEmailRecuperar').value.trim();

    if (!email) {
        mostrarMensajeModal('Ingresá tu email.');
        return;
    }

    var boton = document.getElementById('btnEnviarRecuperar');
    boton.disabled = true;
    boton.textContent = 'Enviando...';

    fetch('RecuperarContraseniaWS.asmx/SolicitarRecuperacion', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json; charset=utf-8' },
        body: JSON.stringify({ pEmail: email })
    })
        .then(function (respuesta) { return respuesta.json(); })
        .then(function (data) {
            mostrarMensajeModal(data.d);
        })
        .catch(function () {
            mostrarMensajeModal('Ocurrió un error. Intentá de nuevo más tarde.');
        })
        .finally(function () {
            boton.disabled = false;
            boton.textContent = 'Enviar';
        });
}