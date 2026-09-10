function abrirModal(idPanel) {
    document.getElementById(idPanel).style.display = 'flex';
}

function cerrarModal(idPanel) {
    document.getElementById(idPanel).style.display = 'none';
}

function toggleFaq(elementoPregunta) {
    var item = elementoPregunta.parentElement;
    item.classList.toggle('abierta');
}

document.addEventListener('DOMContentLoaded', function () {
    var estrellas = document.querySelectorAll('.land-estrella');
    var hfPuntuacion = document.getElementById('hfPuntuacion');

    estrellas.forEach(function (estrella) {
        estrella.addEventListener('click', function () {
            var valor = parseInt(estrella.getAttribute('data-valor'));
            hfPuntuacion.value = valor;
            pintarEstrellas(valor);
        });
    });

    function pintarEstrellas(valor) {
        estrellas.forEach(function (e) {
            var v = parseInt(e.getAttribute('data-valor'));
            if (v <= valor) {
                e.classList.add('activa');
            } else {
                e.classList.remove('activa');
            }
        });
    }
});

function validarResenaCliente(sender, args) {
    var puntuacion = document.getElementById('hfPuntuacion').value;
    var comentario = document.getElementById('tbComentario');
    var tieneComentario = comentario && comentario.value.trim().length > 0;
    var tienePuntuacion = parseInt(puntuacion) > 0;
    args.IsValid = tieneComentario || tienePuntuacion;
}