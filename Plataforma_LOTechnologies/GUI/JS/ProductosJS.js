function mostrarPestania(nombre) {
    var hf = document.getElementById('hfPestania');
    if (hf) hf.value = nombre;
    var esProductos = nombre === 'productos';
    document.getElementById('panelProductos').style.display = esProductos ? 'block' : 'none';
    document.getElementById('panelCategorias').style.display = esProductos ? 'none' : 'block';
    document.getElementById('tabProductos').className = esProductos ? 'pestania pestania-activa' : 'pestania';
    document.getElementById('tabCategorias').className = esProductos ? 'pestania' : 'pestania pestania-activa';
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