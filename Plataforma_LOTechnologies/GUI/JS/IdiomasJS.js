function abrirModal(idPanel) {
    document.getElementById(idPanel).style.display = 'flex';
}

function cerrarModal(idPanel) {
    document.getElementById(idPanel).style.display = 'none';
}

// En "Agregar idioma": los campos de código y nombres solo se muestran con "Otro idioma..."
function mostrarOtroIdioma(valor) {
    var panel = document.getElementById('pnlOtroIdioma');
    if (!panel) return;
    panel.style.display = valor === 'otro' ? 'block' : 'none';
    if (valor === 'otro') document.getElementById('tbCodigoIso').focus();
}