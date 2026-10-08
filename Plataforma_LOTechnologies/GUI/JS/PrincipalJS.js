function abrirModal(idPanel) {
    var panel = document.getElementById(idPanel);
    panel.style.display = 'flex';
    var primerCampo = panel.querySelector('input[type="password"], input[type="text"]');
    if (primerCampo) primerCampo.focus();
}

function cerrarModal(idPanel) {
    document.getElementById(idPanel).style.display = 'none';
}

document.addEventListener('keydown', function (evento) {
    if (evento.key === 'Escape') {
        var modal = document.getElementById('pnlCambiarContrasenia');
        if (modal && modal.style.display !== 'none') cerrarModal('pnlCambiarContrasenia');
    }
});

document.addEventListener('click', function (evento) {
    if (evento.target && evento.target.id === 'pnlCambiarContrasenia') {
        cerrarModal('pnlCambiarContrasenia');
    }
});