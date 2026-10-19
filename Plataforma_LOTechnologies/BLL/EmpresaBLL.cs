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
        DatosInvalidos
    }
    public class EmpresaBLL
    {
        private DigitoVerificador digitos = new DigitoVerificador();
        private EmpresaDAL empresaDAL = new EmpresaDAL();
        public List<EmpresaBE> ObtenerEmpresas()
        {
            return empresaDAL.ObtenerEmpresas();
        }
        public List<EmpresaBE> ObtenerEmpresasListado(string pBusqueda, bool? pEstado)
        {
            return empresaDAL.ObtenerEmpresasListado(pBusqueda, pEstado);
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
            empresaDAL.ModificarEmpresa(pEmpresa);
            digitos.ActualizarDigitoFila("Empresa", pEmpresa.CodigoEmpresa);
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "Modificar empresa", "Gestión empresas", 2);
            return ResultadoGuardarEmpresa.Exitoso;
        }
        public void CambiarEstado(int pCodigoEmpresa, bool pNuevoEstado)
        {
            ControlAcceso.ValidarSuperUsuario();
            empresaDAL.CambiarEstado(pCodigoEmpresa, pNuevoEstado);
            digitos.ActualizarDigitoFila("Empresa", pCodigoEmpresa);
            string accion = pNuevoEstado ? "Activar empresa" : "Desactivar empresa";
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", accion, "Gestión empresas", 2);
        }
    }
}
