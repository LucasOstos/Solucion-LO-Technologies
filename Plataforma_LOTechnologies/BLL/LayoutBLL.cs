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
    public class LayoutBLL
    {
        private LayoutDAL layoutDAL = new LayoutDAL();
        private EscenarioDAL escenarioDAL = new EscenarioDAL();
        public const string ESTADO_VIGENTE = "Activo";
        private DigitoVerificador digitos = new DigitoVerificador();
        public List<LayoutBE> ObtenerLayoutsPorLocal(int pCodigoLocal)
        {
            ControlAcceso.Validar(EntidadEmpresa.Local, pCodigoLocal);
            return layoutDAL.ObtenerLayoutsPorLocal(pCodigoLocal);
        }
        public LayoutBE ObtenerLayoutPorCodigo(int pCodigoLayout)
        {
            ControlAcceso.Validar(EntidadEmpresa.Layout, pCodigoLayout);
            return layoutDAL.ObtenerLayoutPorCodigo(pCodigoLayout);
        }
        public int CrearLayout(LayoutBE pLayout)
        {
            ControlAcceso.Validar(EntidadEmpresa.Local, pLayout.CodigoLocal);
            int codigoLayoutNuevo = layoutDAL.AgregarLayout(pLayout);
            List<int> modificados = pLayout.Estado == ESTADO_VIGENTE ? layoutDAL.DesactivarOtros(pLayout.CodigoLocal, codigoLayoutNuevo) : new List<int>();
            modificados.Add(codigoLayoutNuevo);
            digitos.ActualizarDigitoFilas("Layout", modificados.Cast<object>());
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "Cargar layout de local", "Gestión locales", 2);
            return codigoLayoutNuevo;
        }
        public LayoutBE ObtenerLayoutVigente(int pCodigoLocal)
        {
            ControlAcceso.Validar(EntidadEmpresa.Local, pCodigoLocal);
            return layoutDAL.ObtenerLayoutVigente(pCodigoLocal);
        }
        public void MarcarVigente(int pCodigoLayout)
        {
            ControlAcceso.Validar(EntidadEmpresa.Layout, pCodigoLayout);
            LayoutBE layout = layoutDAL.ObtenerLayoutPorCodigo(pCodigoLayout);
            if (layout == null) return;
            layoutDAL.CambiarEstado(pCodigoLayout, ESTADO_VIGENTE);
            List<int> modificados = layoutDAL.DesactivarOtros(layout.CodigoLocal, pCodigoLayout);
            modificados.Add(pCodigoLayout);
            digitos.ActualizarDigitoFilas("Layout", modificados.Cast<object>());
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "Cargar layout de local", "Gestión locales", 2);
        }
        public void EliminarLayout(int pCodigoLayout)
        {
            ControlAcceso.Validar(EntidadEmpresa.Layout, pCodigoLayout);
            List<int> escenarios = escenarioDAL.QuitarLayoutBase(pCodigoLayout);
            if (escenarios.Count > 0) digitos.ActualizarDigitoFilas("Escenario", escenarios.Cast<object>());
            layoutDAL.EliminarLayout(pCodigoLayout);
            digitos.ActualizarDigitoTabla("Layout");
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "Eliminar layout de local", "Gestión locales", 2);
        }
    }
}
