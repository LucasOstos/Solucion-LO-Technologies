function mostrarPestania(nombre) {
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