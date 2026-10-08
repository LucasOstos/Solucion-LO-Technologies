(function () {
    var SELECTOR = '.mensaje, .dv-mensaje';
    var DEMORA_EXITO = 5000;
    var DEMORA_ERROR = 8000;
    var DEMORA_TRAS_HOVER = 2000;

    function esError(elemento) {
        return /error/.test(elemento.className);
    }

    function ocultar(elemento) {
        var reducirMovimiento = window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches;
        if (reducirMovimiento) {
            elemento.style.display = 'none';
            return;
        }

        elemento.style.overflow = 'hidden';
        elemento.style.maxHeight = elemento.offsetHeight + 'px';
        elemento.style.transition =
            'opacity 0.6s ease, max-height 0.4s ease 0.6s, margin 0.4s ease 0.6s, ' +
            'padding 0.4s ease 0.6s, border-width 0.4s ease 0.6s';
        void elemento.offsetHeight;

        elemento.style.opacity = '0';
        elemento.style.maxHeight = '0';
        elemento.style.marginTop = '0';
        elemento.style.marginBottom = '0';
        elemento.style.paddingTop = '0';
        elemento.style.paddingBottom = '0';
        elemento.style.borderWidth = '0';

        setTimeout(function () { elemento.style.display = 'none'; }, 1100);
    }

    function programar(elemento) {
        var temporizador = setTimeout(function () { ocultar(elemento); },
            esError(elemento) ? DEMORA_ERROR : DEMORA_EXITO);

        elemento.addEventListener('mouseenter', function () {
            clearTimeout(temporizador);
        });
        elemento.addEventListener('mouseleave', function () {
            clearTimeout(temporizador);
            temporizador = setTimeout(function () { ocultar(elemento); }, DEMORA_TRAS_HOVER);
        });
    }

    document.addEventListener('DOMContentLoaded', function () {
        var mensajes = document.querySelectorAll(SELECTOR);
        for (var i = 0; i < mensajes.length; i++) {
            var mensaje = mensajes[i];
            if (mensaje.closest('.modal-caja, .modal-box')) continue;
            if (!mensaje.textContent.trim()) continue;
            programar(mensaje);
        }
    });
})();