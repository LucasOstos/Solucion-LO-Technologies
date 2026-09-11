function toggleMenuUsuario() {
    document.getElementById('menuDesplegableUsuario').classList.toggle('abierto');
}

document.addEventListener('click', function (evento) {
    var contenedor = document.getElementById('contenedorAvatarUsuario');
    var menu = document.getElementById('menuDesplegableUsuario');
    if (contenedor && menu && !contenedor.contains(evento.target)) {
        menu.classList.remove('abierto');
    }
});

function abrirModal(idPanel) {
    document.getElementById(idPanel).style.display = 'flex';
    document.getElementById('menuDesplegableUsuario').classList.remove('abierto');
}

function cerrarModal(idPanel) {
    document.getElementById(idPanel).style.display = 'none';
}

function dispararLogout() {
    document.getElementById('btnLogout').click();
}