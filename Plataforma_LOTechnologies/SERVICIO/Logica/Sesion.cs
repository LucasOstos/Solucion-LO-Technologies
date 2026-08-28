using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;

namespace SERVICIO.Logica
{
    public class Sesion
    {
        public Usuario Usuario;
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
    }
}
