var SECCIONES_POR_TIPO = {
    'Completo': ['chkResumenEjecutivo', 'chkEvolucionVentas', 'chkIndicadoresPrincipales', 'chkRecomendaciones', 'chkComparacionEscenarios',
        'chkOportunidades', 'chkEstadoStock', 'chkHistorialVentas', 'chkDetalleProductos'],
    'Indicadores': ['chkResumenEjecutivo', 'chkEvolucionVentas', 'chkIndicadoresPrincipales', 'chkRecomendaciones', 'chkHistorialVentas'],
    'Comparación': ['chkResumenEjecutivo', 'chkIndicadoresPrincipales', 'chkComparacionEscenarios'],
    'Stock': ['chkResumenEjecutivo', 'chkRecomendaciones', 'chkOportunidades', 'chkEstadoStock']
};
var TODAS_LAS_SECCIONES = SECCIONES_POR_TIPO['Completo'];

function aplicarTipoReporte(tipo) {
    var elegidas = SECCIONES_POR_TIPO[tipo];
    if (!elegidas) return;
    for (var i = 0; i < TODAS_LAS_SECCIONES.length; i++) {
        var casilla = document.getElementById(TODAS_LAS_SECCIONES[i]);
        if (casilla) casilla.checked = elegidas.indexOf(TODAS_LAS_SECCIONES[i]) >= 0;
    }
}