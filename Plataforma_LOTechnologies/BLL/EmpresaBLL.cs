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
            return empresaDAL.ObtenerEmpresaPorCodigo(pCodigoEmpresa);
        }
        public ResultadoGuardarEmpresa CrearEmpresa(EmpresaBE pEmpresa)
        {
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
            empresaDAL.ModificarEmpresa(pEmpresa);
            digitos.ActualizarDigitoFila("Empresa", pEmpresa.CodigoEmpresa);
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "Modificar empresa", "Gestión empresas", 2);
            return ResultadoGuardarEmpresa.Exitoso;
        }
        public void CambiarEstado(int pCodigoEmpresa, bool pNuevoEstado)
        {
            empresaDAL.CambiarEstado(pCodigoEmpresa, pNuevoEstado);
            digitos.ActualizarDigitoFila("Empresa", pCodigoEmpresa);
            string accion = pNuevoEstado ? "Activar empresa" : "Desactivar empresa";
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", accion, "Gestión empresas", 2);
        }
    }
}
