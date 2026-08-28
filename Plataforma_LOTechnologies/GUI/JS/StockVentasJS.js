function mostrarPestania(nombre) {
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