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
    public enum ResultadoGuardarLocal
    {
        Exitoso,
        DatosInvalidos,
        EmpresaInactiva,
        NombreRepetido,
        SuperficieInvalida,
        TelefonoInvalido
    }
    public class LocalBLL
    {
        private LocalDAL localDAL = new LocalDAL();
        private DigitoVerificador digitos = new DigitoVerificador();

        public List<LocalListado> ObtenerLocalesListado(string pBusqueda, int? pCodigoEmpresa, string pEstado)
        {
            return localDAL.ObtenerLocalesListado(pBusqueda, ControlAcceso.FiltrarEmpresa(pCodigoEmpresa), pEstado);
        }
        public LocalBE ObtenerLocalPorCodigo(int pCodigoLocal)
        {
            ControlAcceso.Validar(EntidadEmpresa.Local, pCodigoLocal);
            return localDAL.ObtenerLocalPorCodigo(pCodigoLocal);
        }
        public ResultadoGuardarLocal CrearLocal(LocalBE pLocal)
        {
            pLocal.CodigoEmpresa = ControlAcceso.EmpresaAlcance ?? pLocal.CodigoEmpresa;
            ResultadoGuardarLocal validacion = Validar(pLocal);
            if (validacion != ResultadoGuardarLocal.Exitoso) return validacion;
            int codigoLocalNuevo = localDAL.AgregarLocal(pLocal);
            digitos.ActualizarDigitoFila("Local", codigoLocalNuevo);
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "Registrar local", "Gestión locales", 2);
            return ResultadoGuardarLocal.Exitoso;
        }
        public ResultadoGuardarLocal ModificarLocal(LocalBE pLocal)
        {
            ControlAcceso.Validar(EntidadEmpresa.Local, pLocal.CodigoLocal);
            pLocal.CodigoEmpresa = ControlAcceso.EmpresaAlcance ?? pLocal.CodigoEmpresa;
            ResultadoGuardarLocal validacion = Validar(pLocal);
            if (validacion != ResultadoGuardarLocal.Exitoso) return validacion;
            localDAL.ModificarLocal(pLocal);
            digitos.ActualizarDigitoFila("Local", pLocal.CodigoLocal);
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "Modificar local", "Gestión locales", 2);
            return ResultadoGuardarLocal.Exitoso;
        }
        private ResultadoGuardarLocal Validar(LocalBE pLocal)
        {
            pLocal.Nombre = (pLocal.Nombre ?? "").Trim();
            pLocal.Direccion = (pLocal.Direccion ?? "").Trim();
            pLocal.Observaciones = (pLocal.Observaciones ?? "").Trim();
            if (pLocal.Nombre == "" || pLocal.Direccion == "" || pLocal.Nombre.Length > 100 || pLocal.Direccion.Length > 200 || pLocal.Observaciones.Length > 500)
                return ResultadoGuardarLocal.DatosInvalidos;
            EmpresaBE empresa = new EmpresaDAL().ObtenerEmpresaPorCodigo(pLocal.CodigoEmpresa);
            if (empresa == null || !empresa.Estado) return ResultadoGuardarLocal.EmpresaInactiva;
            if (pLocal.SuperficieAprox != null && (pLocal.SuperficieAprox <= 0 || pLocal.SuperficieAprox > 1000000)) return ResultadoGuardarLocal.SuperficieInvalida;
            string telefono;
            if (!EmpresaBLL.NormalizarTelefono(pLocal.Telefono, out telefono)) return ResultadoGuardarLocal.TelefonoInvalido;
            pLocal.Telefono = telefono;
            if (localDAL.ExisteNombre(pLocal.CodigoEmpresa, pLocal.Nombre, pLocal.CodigoLocal)) return ResultadoGuardarLocal.NombreRepetido;
            return ResultadoGuardarLocal.Exitoso;
        }
        public void CambiarEstado(int pCodigoLocal, string pNuevoEstado)
        {
            ControlAcceso.Validar(EntidadEmpresa.Local, pCodigoLocal);
            localDAL.CambiarEstado(pCodigoLocal, pNuevoEstado);
            digitos.ActualizarDigitoFila("Local", pCodigoLocal);
            string accion = pNuevoEstado == "Activo" ? "Activar local" : "Desactivar local";
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", accion, "Gestión locales", 2);
        }
    }
}
