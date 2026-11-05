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
        private LayoutDAL layoutDAL = new LayoutDAL();
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
            pEscenario.Nombre = (pEscenario.Nombre ?? "").Trim();
            if (pEscenario.Nombre == "" || pEscenario.Nombre.Length > 100) throw new ExcepcionTraducible("comun.datosInvalidos");
            if (pCambios == null || pCambios.Count == 0) throw new ExcepcionTraducible("escenarios.errSinCambios");
            if (escenarioDAL.ExisteNombre(pEscenario.CodigoLocal, pEscenario.Nombre)) throw new ExcepcionTraducible("escenarios.errNombreRepetido");
            pEscenario.Estado = EscenarioBE.ESTADO_BORRADOR;
            if (pEscenario.CodigoLayout == null)
            {
                LayoutBE vigente = layoutDAL.ObtenerLayoutVigente(pEscenario.CodigoLocal);
                pEscenario.CodigoLayout = vigente?.CodigoLayout;
            }
            int codigoNuevo = escenarioDAL.AgregarEscenarioConCambios(pEscenario, pCambios);
            digitos.ActualizarDigitoFila("Escenario", codigoNuevo);
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "Crear escenario de simulación", "Gestión simulación", 2);
            return codigoNuevo;
        }
        public void CambiarEstado(int pCodigoEscenario, string pEstado)
        {
            ControlAcceso.Validar(EntidadEmpresa.Escenario, pCodigoEscenario);
            if (!EscenarioBE.ESTADOS.Contains(pEstado)) throw new ExcepcionTraducible("comun.datosInvalidos");
            EscenarioBE escenario = escenarioDAL.ObtenerEscenarioPorCodigo(pCodigoEscenario);
            if (escenario == null || escenario.Estado == pEstado) return;
            escenarioDAL.CambiarEstado(pCodigoEscenario, pEstado);
            digitos.ActualizarDigitoFila("Escenario", pCodigoEscenario);
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}",
                $"Cambiar estado de escenario ({escenario.Nombre}: {escenario.Estado} -> {pEstado})", "Gestión simulación", 2);
        }
        // Devuelve cuántas comparaciones guardadas se borraron junto con el escenario
        public int EliminarEscenario(int pCodigoEscenario)
        {
            ControlAcceso.Validar(EntidadEmpresa.Escenario, pCodigoEscenario);
            ResultadoEliminarEscenario resultado = escenarioDAL.EliminarEscenario(pCodigoEscenario);
            digitos.ActualizarDigitoVertical("Escenario");
            if (resultado.ComparacionesBorradas > 0) digitos.ActualizarDigitoVertical("Comparacion");
            if (resultado.IndicadoresBorrados > 0) digitos.ActualizarDigitoVertical("Indicador");
            if (resultado.RecomendacionesBorradas > 0) digitos.ActualizarDigitoVertical("Recomendacion");
            if (resultado.ReportesModificados.Count > 0) digitos.ActualizarDigitoFilas("Reporte", resultado.ReportesModificados.Cast<object>());
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "Eliminar escenario de simulación", "Gestión simulación", 2);
            return resultado.ComparacionesBorradas;
        }
    }
}
