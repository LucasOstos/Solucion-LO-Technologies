function abrirModalEmpresa() {
    document.getElementById('pnlModalEmpresa').style.display = 'flex';
}
function cerrarModalEmpresa() {
    document.getElementById('pnlModalEmpresa').style.display = 'none';
}
function formatearCuit(campo) {
    var digitos = campo.value.replace(/\D/g, '').substring(0, 11);
    var texto = digitos;
    if (digitos.length > 2) texto = digitos.substring(0, 2) + '-' + digitos.substring(2);
    if (digitos.length > 10) texto = digitos.substring(0, 2) + '-' + digitos.substring(2, 10) + '-' + digitos.substring(10);
    campo.value = texto;
}
function filtrarTelefono(campo) {
    var valor = campo.value;
    var mas = valor.trim().charAt(0) === '+' ? '+' : '';
    campo.value = mas + valor.replace(/\D/g, '').substring(0, 15);
}