function mostrarPestania(nombre) {
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