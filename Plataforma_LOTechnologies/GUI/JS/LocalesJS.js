function abrirModalLocal() {
    document.getElementById('pnlModalLocal').style.display = 'flex';
}

function cerrarModalLocal() {
    document.getElementById('pnlModalLocal').style.display = 'none';
}
function filtrarTelefono(campo) {
    var valor = campo.value;
    var mas = valor.trim().charAt(0) === '+' ? '+' : '';
    campo.value = mas + valor.replace(/\D/g, '').substring(0, 15);
}