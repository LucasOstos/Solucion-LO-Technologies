function mostrarPestania(nombre) {
    var hf = document.getElementById('hfPestania');
    if (hf) hf.value = nombre;
    var esLayout = nombre === 'layout';
    document.getElementById('panelLayout').style.display = esLayout ? 'block' : 'none';
    document.getElementById('panelSectores').style.display = esLayout ? 'none' : 'block';
    document.getElementById('tabLayout').className = esLayout ? 'pestania pestania-activa' : 'pestania';
    document.getElementById('tabSectores').className = esLayout ? 'pestania' : 'pestania pestania-activa';
}
function abrirModal(idPanel) {
    document.getElementById(idPanel).style.display = 'flex';
}
function cerrarModal(idPanel) {
    document.getElementById(idPanel).style.display = 'none';
}
// La pestaña elegida se guarda en hfPestania: así un postback (botones, filtros, modales) vuelve a la misma pestaña
document.addEventListener('DOMContentLoaded', function () {
    var hf = document.getElementById('hfPestania');
    if (hf && hf.value) mostrarPestania(hf.value);
});