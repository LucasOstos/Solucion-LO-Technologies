function ocultarContrasenia() {
    var input = document.getElementById('tbContrasenia');
    input.type = (input.type === 'password') ? 'text' : 'password';
}

function abrirModalRecuperar() {
    var input = document.getElementById('txtEmailRecuperar');
    input.value = '';
    input.style.display = '';
    document.getElementById('subtextoRecuperar').style.display = '';
    document.getElementById('btnEnviarRecuperar').style.display = '';
    document.getElementById('btnCancelarRecuperar').textContent = 'Cancelar';
    ocultarMensajeModal();
    document.getElementById('modal-recuperar').style.display = 'flex';
    input.focus();
}

function cerrarModalRecuperar() {
    document.getElementById('modal-recuperar').style.display = 'none';
}

function ocultarMensajeModal() {
    var mensaje = document.getElementById('modalMensaje');
    mensaje.style.display = 'none';
    mensaje.textContent = '';
}

function mostrarMensajeModal(texto) {
    var mensaje = document.getElementById('modalMensaje');
    mensaje.textContent = texto;
    mensaje.style.display = 'block';
}

function mostrarEstadoEnviado() {
    document.getElementById('txtEmailRecuperar').style.display = 'none';
    document.getElementById('subtextoRecuperar').style.display = 'none';
    document.getElementById('btnEnviarRecuperar').style.display = 'none';
    var cancelar = document.getElementById('btnCancelarRecuperar');
    cancelar.textContent = 'Cerrar';
    cancelar.focus();
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
            mostrarEstadoEnviado();
        })
        .catch(function () {
            mostrarMensajeModal('Ocurrió un error. Intentá de nuevo más tarde.');
        })
        .finally(function () {
            boton.disabled = false;
            boton.textContent = 'Enviar';
        });
}

document.addEventListener('keydown', function (evento) {
    var modal = document.getElementById('modal-recuperar');
    if (!modal || modal.style.display === 'none') return;

    if (evento.key === 'Escape') {
        cerrarModalRecuperar();
    } else if (evento.key === 'Enter' && evento.target.id === 'txtEmailRecuperar') {
        evento.preventDefault();
        enviarSolicitudRecuperacion();
    }
});

document.addEventListener('click', function (evento) {
    if (evento.target && evento.target.id === 'modal-recuperar') {
        cerrarModalRecuperar();
    }
});