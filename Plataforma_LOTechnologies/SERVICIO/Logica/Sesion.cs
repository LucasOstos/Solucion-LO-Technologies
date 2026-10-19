using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using BE;

namespace SERVICIO.Logica
{
    public class Sesion
    {
        public Usuario Usuario;
        public bool IntegridadComprometida;
        private Sesion() { }
        public static Sesion Instancia
        {
            get
            {
                Sesion instancia = HttpContext.Current.Session["SesionUsuario"] as Sesion;
                if (instancia == null) { instancia = new Sesion(); HttpContext.Current.Session["SesionUsuario"] = instancia; }
                return instancia;
            }
        }
        //public string IdiomaSesion;

        public void Login(Usuario pUsuario)
        {
            Usuario = pUsuario;
        }
        public void Logout()
        {
            if(Usuario != null)
            {
                HttpContext.Current.Session.Remove("SesionUsuario");
                Usuario = null;
            }
        }
        public bool IsLogueado()
        {
            return Usuario != null;
        }
        public bool EsSuperUsuario
        {
            get { return Usuario != null && Usuario.EsSuperUsuario; }
        }
        public int? CodigoEmpresaAlcance
        {
            get { return (Usuario == null || EsSuperUsuario) ? (int?)null : Usuario.CodigoEmpresa; }
        }
        public bool TienePermiso(string pNombrePermiso)
        {
            if (Usuario == null || Usuario.Perfil == null) return false;
            if (!EsSuperUsuario && PermisoNombres.EsGlobal(pNombrePermiso)) return false;
            return Usuario.Perfil.TienePermiso(pNombrePermiso);
        }
    }
    public class SuperUsuarioServicio
    {
        private const string RUTA_ARCHIVO = "~/App_Data/SuperUsuario.json";
        private const int CODIGO_PERFIL = -1;
        private static SuperUsuarioServicio instancia;
        public static SuperUsuarioServicio Instancia
        {
            get
            {
                if (instancia == null) instancia = new SuperUsuarioServicio();
                return instancia;
            }
        }
        private SuperUsuarioServicio() { }
        public Usuario ValidarCredenciales(string pEmail, string pContrasenia)
        {
            string contenido = LeerArchivo();
            if (contenido == null) return null;
            return ValidarContenido(contenido, pEmail, pContrasenia);
        }
        public Usuario ValidarContenido(string pContenido, string pEmail, string pContrasenia)
        {
            string emailHash = LeerCampo(pContenido, "Email");
            string contraseniaHash = LeerCampo(pContenido, "Contrasenia");
            string nombreCifrado = LeerCampo(pContenido, "Nombre");
            string apellidoCifrado = LeerCampo(pContenido, "Apellido");
            string verificador = LeerCampo(pContenido, "Verificador");
            if (emailHash == null || contraseniaHash == null || nombreCifrado == null || apellidoCifrado == null || verificador == null)
            {
                return null;
            }
            Encriptado encriptado = Encriptado.Instancia;
            string firmaEsperada = encriptado.FirmarHMAC($"{emailHash}|{contraseniaHash}|{nombreCifrado}|{apellidoCifrado}");
            if (!string.Equals(firmaEsperada, verificador, StringComparison.OrdinalIgnoreCase))
            {
                //RegistrarSinFallar("Superusuario", "Archivo del superusuario alterado", "Seguridad", 3);
                return null;
            }
            string emailIngresado = encriptado.EncriptarContraseña((pEmail ?? "").Trim().ToLowerInvariant());
            string contraseniaIngresada = encriptado.EncriptarContraseña(pContrasenia ?? "");
            if (emailIngresado != emailHash || contraseniaIngresada != contraseniaHash)
            {
                return null;
            }

            return new Usuario
            {
                DNI = 0,
                Nombre = encriptado.DesencriptacionReversible(nombreCifrado),
                Apellido = encriptado.DesencriptacionReversible(apellidoCifrado),
                Email = pEmail.Trim(),
                Perfil = ArmarPerfilCompleto(),
                estadoActivo = true,
                estadoBloqueado = false,
                ultimoAcceso = DateTime.Now,
                Idioma = 1,
                CodigoEmpresa = 0,
                EsSuperUsuario = true
            };
        }
        private Perfil ArmarPerfilCompleto()
        {
            Perfil perfil = new Perfil(CODIGO_PERFIL, "Personal LO");
            int codigo = -1;
            foreach (string nombrePermiso in PermisoNombres.Todos())
            {
                perfil.Agregar(new PermisoSimple(codigo--, nombrePermiso));
            }
            return perfil;
        }

        private string LeerArchivo()
        {
            try
            {
                string ruta = HttpContext.Current.Server.MapPath(RUTA_ARCHIVO);
                return File.Exists(ruta) ? File.ReadAllText(ruta, Encoding.UTF8) : null;
            }
            catch
            {
                return null;
            }
        }
        private string LeerCampo(string pContenido, string pCampo)
        {
            Match m = Regex.Match(pContenido, "\"" + pCampo + "\"\\s*:\\s*\"([^\"]*)\"");
            return m.Success ? m.Groups[1].Value : null;
        }
    }
}
