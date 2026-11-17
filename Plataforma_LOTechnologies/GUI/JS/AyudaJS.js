// Ayuda en línea: panel acoplado a la derecha (lo arma Controles/PanelAyuda.ascx).
// - Botón "? Ayuda" o F1: abre o cierra. Con un modal abierto, abre el tema de ese modal (atributo data-ayuda-tema).
// - Esc: cierra y devuelve el foco al botón.
// - El estado (abierto y tema) se guarda en sessionStorage para que sobreviva a los postbacks.
// - "Mostrármelo en la pantalla" marca el primer elemento visible de los selectores del tema.
(function () {
    var CLAVE_ESTADO = 'loAyuda';
    var CLASE_ABIERTA = 'ayuda-abierta';
    var CLASE_RESALTADO = 'ayuda-resaltado';
    var panel, boton, buscador, botonVolver, anuncio, resaltado, aviso, temporizador;

    document.addEventListener('DOMContentLoaded', iniciar);

    function iniciar() {
        panel = document.getElementById('panelAyuda');
        boton = document.getElementById('btnAyuda');
        if (!panel || !boton) return;
        buscador = document.getElementById('ayudaBuscar');
        botonVolver = panel.querySelector('[data-ayuda-volver]');
        anuncio = panel.querySelector('.ayuda-anuncio');

        boton.addEventListener('click', function () {
            if (estaAbierto()) cerrar(); else abrir(temaDelContexto());
        });
        panel.addEventListener('click', alHacerClic);
        if (buscador) buscador.addEventListener('input', function () { buscar(buscador.value); });
        document.addEventListener('keydown', alPresionarTecla);

        // Al llegar desde "Pantallas relacionadas" (?ayuda=1) o después de un postback con la ayuda abierta
        var estado = leerEstado();
        if (/[?&]ayuda=1/.test(window.location.search)) abrir(null, true);
        else if (estado && estado.abierto && estado.pantalla === panel.getAttribute('data-pantalla')) abrir(estado.tema, true);
    }

    function alHacerClic(e) {
        var objetivo = e.target.closest('[data-tema], [data-ayuda-cerrar], [data-ayuda-volver], [data-resaltar]');
        if (!objetivo) return;
        if (objetivo.hasAttribute('data-ayuda-cerrar')) cerrar();
        else if (objetivo.hasAttribute('data-ayuda-volver')) mostrarInicio(true);
        else if (objetivo.hasAttribute('data-resaltar')) resaltar(objetivo.getAttribute('data-resaltar'), objetivo);
        else mostrarTema(objetivo.getAttribute('data-tema'), true);
    }

    function alPresionarTecla(e) {
        if (e.key === 'F1') {
            e.preventDefault();
            if (estaAbierto()) cerrar(); else abrir(temaDelContexto());
        } else if (e.key === 'Escape' && estaAbierto() && panel.contains(document.activeElement)) {
            e.preventDefault();
            cerrar();
        }
    }

    // Si hay un modal visible con data-ayuda-tema, la ayuda abre ese tema
    function temaDelContexto() {
        var modales = document.querySelectorAll('.modal-fondo[data-ayuda-tema]');
        for (var i = 0; i < modales.length; i++) {
            if (esVisible(modales[i])) return modales[i].getAttribute('data-ayuda-tema');
        }
        return null;
    }

    function estaAbierto() {
        return !panel.hasAttribute('hidden');
    }

    function abrir(tema, sinFoco) {
        panel.removeAttribute('hidden');
        document.body.classList.add(CLASE_ABIERTA);
        boton.setAttribute('aria-expanded', 'true');
        boton.classList.add('activo');
        if (tema && panel.querySelector('[data-articulo="' + tema + '"]')) mostrarTema(tema, !sinFoco);
        else mostrarInicio(!sinFoco);
    }

    function cerrar() {
        quitarResaltado();
        panel.setAttribute('hidden', 'hidden');
        document.body.classList.remove(CLASE_ABIERTA);
        boton.setAttribute('aria-expanded', 'false');
        boton.classList.remove('activo');
        guardarEstado(false, null);
        boton.focus();
    }

    function mostrarInicio(conFoco) {
        ocultarArticulos();
        panel.querySelector('.ayuda-inicio').hidden = false;
        botonVolver.hidden = true;
        guardarEstado(true, null);
        if (conFoco) document.getElementById('ayudaTitulo').focus();
    }

    function mostrarTema(tema, conFoco) {
        var articulo = panel.querySelector('[data-articulo="' + tema + '"]');
        if (!articulo) return;
        quitarResaltado();
        ocultarArticulos();
        panel.querySelector('.ayuda-inicio').hidden = true;
        articulo.hidden = false;
        botonVolver.hidden = false;
        panel.querySelector('.ayuda-cuerpo').scrollTop = 0;
        guardarEstado(true, tema);
        if (conFoco) articulo.querySelector('h3').focus();
    }

    function ocultarArticulos() {
        var articulos = panel.querySelectorAll('[data-articulo]');
        for (var i = 0; i < articulos.length; i++) articulos[i].hidden = true;
    }

    // Filtra los temas por título y contenido, sin distinguir tildes ni mayúsculas
    function buscar(texto) {
        var termino = normalizar(texto.trim());
        if (termino) mostrarInicio(false);
        var temas = panel.querySelectorAll('.ayuda-inicio [data-buscar]');
        var hay = 0;
        for (var i = 0; i < temas.length; i++) {
            var coincide = !termino || temas[i].getAttribute('data-buscar').indexOf(termino) >= 0;
            var contenedor = temas[i].closest('li') || temas[i];
            contenedor.hidden = !coincide;
            if (coincide) hay++;
        }
        var grupos = panel.querySelectorAll('.ayuda-inicio .ayuda-grupo');
        for (var j = 0; j < grupos.length; j++) {
            if (grupos[j].classList.contains('ayuda-grupo-relacionadas')) { grupos[j].hidden = !!termino; continue; }
            grupos[j].hidden = !grupos[j].querySelector('li:not([hidden])');
        }
        var sinResultados = panel.querySelector('.ayuda-sin-resultados');
        sinResultados.hidden = !termino || hay > 0;
        sinResultados.textContent = sinResultados.getAttribute('data-texto').replace('{0}', texto.trim());
    }

    function normalizar(texto) {
        return texto.toLowerCase().normalize('NFD').replace(/[̀-ͯ]/g, '');
    }

    // "Mostrármelo en la pantalla": marca el primer elemento visible de la lista de selectores
    function resaltar(selectores, botonMostrar) {
        quitarResaltado();
        var lista = selectores.split('|');
        for (var i = 0; i < lista.length; i++) {
            var elementos = document.querySelectorAll(lista[i]);
            for (var j = 0; j < elementos.length; j++) {
                if (esVisible(elementos[j]) && !panel.contains(elementos[j])) {
                    resaltado = elementos[j];
                    resaltado.classList.add(CLASE_RESALTADO);
                    resaltado.scrollIntoView({ behavior: 'smooth', block: 'center' });
                    anunciar(anuncio.getAttribute('data-texto-marcado'));
                    temporizador = setTimeout(quitarResaltado, 6000);
                    return;
                }
            }
        }
        // Nada visible: además del aviso para lectores de pantalla, se muestra el aviso debajo del botón
        var texto = anuncio.getAttribute('data-texto-no-visible');
        anunciar(texto);
        if (botonMostrar) {
            aviso = document.createElement('p');
            aviso.className = 'ayuda-aviso';
            aviso.textContent = texto;
            botonMostrar.parentNode.insertBefore(aviso, botonMostrar.nextSibling);
            temporizador = setTimeout(quitarResaltado, 6000);
        }
    }

    function quitarResaltado() {
        if (temporizador) clearTimeout(temporizador);
        if (resaltado) resaltado.classList.remove(CLASE_RESALTADO);
        resaltado = null;
        if (aviso && aviso.parentNode) aviso.parentNode.removeChild(aviso);
        aviso = null;
    }

    function anunciar(texto) {
        anuncio.textContent = '';
        setTimeout(function () { anuncio.textContent = texto; }, 50);
    }

    function esVisible(elemento) {
        if (!elemento.getClientRects().length) return false;
        var estilo = window.getComputedStyle(elemento);
        return estilo.visibility !== 'hidden' && estilo.display !== 'none';
    }

    function guardarEstado(abierto, tema) {
        try {
            sessionStorage.setItem(CLAVE_ESTADO, JSON.stringify({ abierto: abierto, tema: tema, pantalla: panel.getAttribute('data-pantalla') }));
        } catch (e) { /* sin almacenamiento la ayuda funciona igual, solo no se recuerda */ }
    }

    function leerEstado() {
        try { return JSON.parse(sessionStorage.getItem(CLAVE_ESTADO) || 'null'); }
        catch (e) { return null; }
    }
})();