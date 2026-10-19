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
    public class ControlAcceso
    {
        private static readonly AlcanceDAL alcanceDAL = new AlcanceDAL();

        public static bool EsSuperUsuario
        {
            get { return Sesion.Instancia.EsSuperUsuario; }
        }
        public static int? EmpresaAlcance
        {
            get { return Sesion.Instancia.CodigoEmpresaAlcance; }
        }
        public static int? FiltrarEmpresa(int? pCodigoEmpresaPedida)
        {
            return EmpresaAlcance ?? pCodigoEmpresaPedida;
        }
        public static int Validar(EntidadEmpresa pEntidad, int pCodigo)
        {
            if (!Sesion.Instancia.IsLogueado()) Denegar("sin sesión");
            if (pCodigo <= 0) return EmpresaAlcance ?? 0;
            int? empresaRegistro = alcanceDAL.ObtenerEmpresa(pEntidad, pCodigo);
            if (empresaRegistro == null)
            {
                if (EsSuperUsuario) return 0;
                Denegar($"{pEntidad} {pCodigo}");
            }
            if (EmpresaAlcance != null && empresaRegistro.Value != EmpresaAlcance.Value)
            {
                Denegar($"{pEntidad} {pCodigo}");
            }
            return empresaRegistro.Value;
        }
        public static int ResolverEmpresaPorLocal(int pCodigoLocal)
        {
            if (pCodigoLocal > 0) return Validar(EntidadEmpresa.Local, pCodigoLocal);
            return EmpresaAlcance ?? 0;
        }
        public static void ValidarEmpresa(int pCodigoEmpresa)
        {
            if (!Sesion.Instancia.IsLogueado()) Denegar("sin sesión");
            if (EmpresaAlcance != null && pCodigoEmpresa != EmpresaAlcance.Value)
            {
                Denegar($"Empresa {pCodigoEmpresa}");
            }
        }
        public static void ValidarSuperUsuario()
        {
            if (!EsSuperUsuario) Denegar("operación de superusuario");
        }
        private static void Denegar(string pDetalle)
        {
            Usuario usuario = Sesion.Instancia.Usuario;
            string nombre = usuario == null ? "Sin sesión" : $"{usuario.Nombre} {usuario.Apellido}";
            SucesoServicio.Instancia.RegistrarSuceso(nombre, $"Acceso denegado: {pDetalle}", "Seguridad", 3);
            throw new UnauthorizedAccessException("No tenés acceso a ese registro.");
        }
    }
}
