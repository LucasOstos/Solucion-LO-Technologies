var PESTANIAS_STOCK_VENTAS = {
    ventas: { tab: 'tabVentas', panel: 'panelVentas' },
    stock: { tab: 'tabStock', panel: 'panelStock' },
    importaciones: { tab: 'tabImportaciones', panel: 'panelImportaciones' }
};
function mostrarPestania(nombre) {
    if (!PESTANIAS_STOCK_VENTAS[nombre]) nombre = 'ventas';
    var hf = document.getElementById('hfPestania');
    if (hf) hf.value = nombre;
    for (var clave in PESTANIAS_STOCK_VENTAS) {
        var activa = clave === nombre;
        var p = PESTANIAS_STOCK_VENTAS[clave];
        document.getElementById(p.panel).style.display = activa ? 'block' : 'none';
        var tab = document.getElementById(p.tab);
        tab.className = activa ? 'pestania pestania-activa' : 'pestania';
        tab.setAttribute('aria-selected', activa ? 'true' : 'false');
    }
}
// Accesibilidad: las pestañas se pueden activar con Enter o Espacio
function teclaPestania(evento, nombre) {
    if (evento.key === 'Enter' || evento.key === ' ') {
        evento.preventDefault();
        mostrarPestania(nombre);
    }
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
    mostrarPestania(hf && hf.value ? hf.value : 'ventas');
});
// En la carga manual se muestran solo los campos del tipo elegido: importe para venta, mínimo y máximo para stock
function actualizarCamposCarga() {
    var tipo = document.getElementById('ddlTipoCarga');
    if (!tipo) return;
    var esVenta = tipo.value === 'Venta';
    document.getElementById('grpImporteCarga').style.display = esVenta ? '' : 'none';
    document.getElementById('filaMinMaxCarga').style.display = esVenta ? 'none' : '';
}
// En la importación se muestra el formato (columnas y plantillas) del tipo elegido
function actualizarAyudaImportacion() {
    var tipo = document.getElementById('ddlTipoImportacion');
    if (!tipo) return;
    var esVenta = tipo.value === 'Venta';
    document.getElementById('ayudaImportVentas').style.display = esVenta ? '' : 'none';
    document.getElementById('ayudaImportStock').style.display = esVenta ? 'none' : '';
}
document.addEventListener('DOMContentLoaded', actualizarCamposCarga);
document.addEventListener('DOMContentLoaded', actualizarAyudaImportacion);