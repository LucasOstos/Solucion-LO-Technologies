function mostrarPestania(nombre) {
    var hf = document.getElementById('hfPestania');
    if (hf) hf.value = nombre;
    var esVentas = nombre === 'ventas';
    document.getElementById('panelVentas').style.display = esVentas ? 'block' : 'none';
    document.getElementById('panelStock').style.display = esVentas ? 'none' : 'block';
    document.getElementById('tabVentas').className = esVentas ? 'pestania pestania-activa' : 'pestania';
    document.getElementById('tabStock').className = esVentas ? 'pestania' : 'pestania pestania-activa';
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
// En la carga manual se muestran solo los campos del tipo elegido: importe para venta, mínimo y máximo para stock
function actualizarCamposCarga() {
    var tipo = document.getElementById('ddlTipoCarga');
    if (!tipo) return;
    var esVenta = tipo.value === 'Venta';
    document.getElementById('grpImporteCarga').style.display = esVenta ? '' : 'none';
    document.getElementById('filaMinMaxCarga').style.display = esVenta ? 'none' : '';
}
document.addEventListener('DOMContentLoaded', actualizarCamposCarga);