using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;
using DAL;
using SERVICIO.Logica;

namespace BLL
{
    public enum ResultadoGuardarEmpresa
    {
        Exitoso,
        CuitYaRegistrado,
        DatosInvalidos,
        CuitInvalido,
        CorreoInvalido,
        TelefonoInvalido
    }
    public class EmpresaBLL
    {
        private DigitoVerificador digitos = new DigitoVerificador();
        private EmpresaDAL empresaDAL = new EmpresaDAL();
        public List<EmpresaBE> ObtenerEmpresas()
        {
            return FiltrarPorAlcance(empresaDAL.ObtenerEmpresas());
        }
        public List<EmpresaBE> ObtenerEmpresasListado(string pBusqueda, bool? pEstado)
        {
            return FiltrarPorAlcance(empresaDAL.ObtenerEmpresasListado(pBusqueda, pEstado));
        }
        public EmpresaBE ObtenerEmpresaPorCodigo(int pCodigoEmpresa)
        {
            ControlAcceso.ValidarEmpresa(pCodigoEmpresa);
            return empresaDAL.ObtenerEmpresaPorCodigo(pCodigoEmpresa);
        }
        private List<EmpresaBE> FiltrarPorAlcance(List<EmpresaBE> pEmpresas)
        {
            int? alcance = ControlAcceso.EmpresaAlcance;
            return alcance == null ? pEmpresas : pEmpresas.Where(e => e.CodigoEmpresa == alcance.Value).ToList();
        }
        public ResultadoGuardarEmpresa CrearEmpresa(EmpresaBE pEmpresa)
        {
            ControlAcceso.ValidarSuperUsuario();
            ResultadoGuardarEmpresa validacion = Validar(pEmpresa);
            if (validacion != ResultadoGuardarEmpresa.Exitoso) return validacion;
            if (empresaDAL.ExisteCUIT(pEmpresa.CUIT))
            {
                return ResultadoGuardarEmpresa.CuitYaRegistrado;
            }
            int codigoEmpresaNueva = empresaDAL.AgregarEmpresa(pEmpresa);
            digitos.ActualizarDigitoFila("Empresa", codigoEmpresaNueva);
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "Registrar empresa", "Gestión empresas", 2);
            return ResultadoGuardarEmpresa.Exitoso;
        }
        public ResultadoGuardarEmpresa ModificarEmpresa(EmpresaBE pEmpresa)
        {
            ControlAcceso.ValidarEmpresa(pEmpresa.CodigoEmpresa);
            ResultadoGuardarEmpresa validacion = Validar(pEmpresa);
            if (validacion != ResultadoGuardarEmpresa.Exitoso) return validacion;
            if (empresaDAL.ExisteCUIT(pEmpresa.CUIT, pEmpresa.CodigoEmpresa))
            {
                return ResultadoGuardarEmpresa.CuitYaRegistrado;
            }
            empresaDAL.ModificarEmpresa(pEmpresa);
            digitos.ActualizarDigitoFila("Empresa", pEmpresa.CodigoEmpresa);
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "Modificar empresa", "Gestión empresas", 2);
            return ResultadoGuardarEmpresa.Exitoso;
        }
        // Valida y normaliza los datos (CUIT con guiones, teléfono sin espacios, textos sin espacios de más)
        private static ResultadoGuardarEmpresa Validar(EmpresaBE pEmpresa)
        {
            pEmpresa.RazonSocial = (pEmpresa.RazonSocial ?? "").Trim();
            pEmpresa.Direccion = (pEmpresa.Direccion ?? "").Trim();
            pEmpresa.Correo = (pEmpresa.Correo ?? "").Trim();
            if (pEmpresa.RazonSocial == "" || pEmpresa.RazonSocial.Length > 150 || pEmpresa.Direccion.Length > 200) return ResultadoGuardarEmpresa.DatosInvalidos;
            string cuit = NormalizarCuit(pEmpresa.CUIT);
            if (cuit == null) return ResultadoGuardarEmpresa.CuitInvalido;
            pEmpresa.CUIT = cuit;
            if (!CorreoValido(pEmpresa.Correo)) return ResultadoGuardarEmpresa.CorreoInvalido;
            string telefono;
            if (!NormalizarTelefono(pEmpresa.Telefono, out telefono)) return ResultadoGuardarEmpresa.TelefonoInvalido;
            pEmpresa.Telefono = telefono;
            return ResultadoGuardarEmpresa.Exitoso;
        }
        public static string NormalizarCuit(string pCuit)
        {
            string digitos = new string((pCuit ?? "").Where(char.IsDigit).ToArray());
            if (digitos.Length != 11 || (pCuit ?? "").Any(c => !char.IsDigit(c) && c != '-' && c != ' ')) return null;
            int[] pesos = { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
            int suma = 0;
            for (int i = 0; i < 10; i++) suma += (digitos[i] - '0') * pesos[i];
            int resto = 11 - (suma % 11);
            int verificador = resto == 11 ? 0 : resto == 10 ? 9 : resto;
            if (verificador != digitos[10] - '0') return null;
            return digitos.Substring(0, 2) + "-" + digitos.Substring(2, 8) + "-" + digitos.Substring(10, 1);
        }
        public static bool CorreoValido(string pCorreo)
        {
            return !string.IsNullOrWhiteSpace(pCorreo) && pCorreo.Length <= 150
                && System.Text.RegularExpressions.Regex.IsMatch(pCorreo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
        public static bool NormalizarTelefono(string pTelefono, out string pNormalizado)
        {
            pNormalizado = (pTelefono ?? "").Trim();
            if (pNormalizado == "") return true;
            return System.Text.RegularExpressions.Regex.IsMatch(pNormalizado, @"^\+?\d{6,15}$");
        }
        public int CambiarEstado(int pCodigoEmpresa, bool pNuevoEstado)
        {
            ControlAcceso.ValidarSuperUsuario();
            empresaDAL.CambiarEstado(pCodigoEmpresa, pNuevoEstado);
            digitos.ActualizarDigitoFila("Empresa", pCodigoEmpresa);
            string accion = pNuevoEstado ? "Activar empresa" : "Desactivar empresa";
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", accion, "Gestión empresas", 2);
            return empresaDAL.ContarUsuariosActivos(pCodigoEmpresa);
        }
    }
}
