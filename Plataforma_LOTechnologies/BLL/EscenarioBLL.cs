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
            return escenarioDAL.ObtenerEscenariosPorLocal(pCodigoLocal);
        }
        public EscenarioBE ObtenerEscenarioPorCodigo(int pCodigoEscenario)
        {
            return escenarioDAL.ObtenerEscenarioPorCodigo(pCodigoEscenario);
        }
        public List<EscenarioCambio> ObtenerCambiosPorEscenario(int pCodigoEscenario)
        {
            return escenarioDAL.ObtenerCambiosPorEscenario(pCodigoEscenario);
        }
        public int CrearEscenario(EscenarioBE pEscenario, List<EscenarioCambio> pCambios)
        {
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
            escenarioDAL.EliminarEscenario(pCodigoEscenario);
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "Eliminar escenario de simulación", "Gestión simulación", 2);
        }
    }
}
