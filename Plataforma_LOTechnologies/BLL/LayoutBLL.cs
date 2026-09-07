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
        private DigitoVerificador digitos = new DigitoVerificador();
        public List<LayoutBE> ObtenerLayoutsPorLocal(int pCodigoLocal)
        {
            return layoutDAL.ObtenerLayoutsPorLocal(pCodigoLocal);
        }
        public LayoutBE ObtenerLayoutPorCodigo(int pCodigoLayout)
        {
            return layoutDAL.ObtenerLayoutPorCodigo(pCodigoLayout);
        }
        public int CrearLayout(LayoutBE pLayout)
        {
            int codigoLayoutNuevo = layoutDAL.AgregarLayout(pLayout);
            digitos.ActualizarDigitoFila("Layout", codigoLayoutNuevo);
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "Cargar layout de local", "Gestión locales", 2);

            return codigoLayoutNuevo;
        }

        public void EliminarLayout(int pCodigoLayout)
        {
            layoutDAL.EliminarLayout(pCodigoLayout);
            digitos.ActualizarDigitoTabla("Layout");
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "Eliminar layout de local", "Gestión locales", 2);
        }
    }
}
