(function () {
    var hayCambios = false;

    // Al escribir en una traducción: se marca la fila como modificada y como revisada
    document.addEventListener('input', function (evento) {
        var campo = evento.target;
        if (!campo.classList || !campo.classList.contains('trd-texto')) return;
        hayCambios = true;
        var fila = campo.closest('tr');
        if (!fila) return;
        fila.classList.add('trd-fila-modificada');
        var revisada = fila.querySelector('.trd-revisada input[type="checkbox"]');
        if (revisada) revisada.checked = true;
    });
    document.addEventListener('change', function (evento) {
        if (!evento.target.closest || !evento.target.closest('.trd-revisada')) return;
        hayCambios = true;
        var fila = evento.target.closest('tr');
        if (fila) fila.classList.add('trd-fila-modificada');
    });

    // Si hay cambios sin guardar, el navegador pregunta antes de salir o filtrar
    window.addEventListener('beforeunload', function (evento) {
        if (!hayCambios) return;
        evento.preventDefault();
        evento.returnValue = '';
    });

    // Lo llama el botón Guardar (OnClientClick)
    window.trdGuardando = function () {
        hayCambios = false;
    };
})();