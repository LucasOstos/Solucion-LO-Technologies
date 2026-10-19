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
    public class EscenarioBLL
    {
        private EscenarioDAL escenarioDAL = new EscenarioDAL();
        private DigitoVerificador digitos = new DigitoVerificador();
        public List<EscenarioListado> ObtenerEscenariosPorLocal(int pCodigoLocal)
        {
            ControlAcceso.Validar(EntidadEmpresa.Local, pCodigoLocal);
            return escenarioDAL.ObtenerEscenariosPorLocal(pCodigoLocal);
        }
        public EscenarioBE ObtenerEscenarioPorCodigo(int pCodigoEscenario)
        {
            ControlAcceso.Validar(EntidadEmpresa.Escenario, pCodigoEscenario);
            return escenarioDAL.ObtenerEscenarioPorCodigo(pCodigoEscenario);
        }
        public List<EscenarioCambio> ObtenerCambiosPorEscenario(int pCodigoEscenario)
        {
            ControlAcceso.Validar(EntidadEmpresa.Escenario, pCodigoEscenario);
            return escenarioDAL.ObtenerCambiosPorEscenario(pCodigoEscenario);
        }
        public int CrearEscenario(EscenarioBE pEscenario, List<EscenarioCambio> pCambios)
        {
            ControlAcceso.Validar(EntidadEmpresa.Local, pEscenario.CodigoLocal);
            int codigoNuevo = escenarioDAL.AgregarEscenario(pEscenario);
            foreach (EscenarioCambio cambio in pCambios)
            {
                cambio.CodigoEscenario = codigoNuevo;
                escenarioDAL.AgregarCambio(cambio);
            }
            digitos.ActualizarDigitoFila("Escenario", codigoNuevo);
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "Crear escenario de simulación", "Gestión simulación", 2);
            return codigoNuevo;
        }
        public void EliminarEscenario(int pCodigoEscenario)
        {
            ControlAcceso.Validar(EntidadEmpresa.Escenario, pCodigoEscenario);
            escenarioDAL.EliminarEscenario(pCodigoEscenario);
            digitos.ActualizarDigitoTabla("Escenario");
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "Eliminar escenario de simulación", "Gestión simulación", 2);
        }
    }
}
