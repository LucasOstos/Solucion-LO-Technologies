using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BE;
using DAL;

namespace SERVICIO.Logica
{
    public class GestorIdioma
    {
        private static readonly GestorIdioma instancia = new GestorIdioma();
        public static GestorIdioma Instancia { get { return instancia; } }
        private GestorIdioma() { }
        private const string CLAVE_OBSERVADORES = "ObservadoresIdioma";
        private readonly IdiomaDAL idiomaDAL = new IdiomaDAL();
        private readonly UsuarioDAL usuarioDAL = new UsuarioDAL();
        private readonly object bloqueo = new object();
        private List<Idioma> idiomas;

        private List<IObservadorIdioma> Observadores
        {
            get
            {
                List<IObservadorIdioma> observadores = HttpContext.Current.Items[CLAVE_OBSERVADORES] as List<IObservadorIdioma>;
                if (observadores == null)
                {
                    observadores = new List<IObservadorIdioma>();
                    HttpContext.Current.Items[CLAVE_OBSERVADORES] = observadores;
                }
                return observadores;
            }
        }
        public void Suscribir(IObservadorIdioma pObservador)
        {
            if (!Observadores.Contains(pObservador)) Observadores.Add(pObservador);
        }
        public void Desuscribir(IObservadorIdioma pObservador)
        {
            Observadores.Remove(pObservador);
        }
        private void Notificar(Idioma pIdioma)
        {
            foreach (IObservadorIdioma observador in Observadores.ToList())
            {
                observador.ActualizarIdioma(pIdioma);
            }
        }

        public List<Idioma> ObtenerIdiomas()
        {
            lock (bloqueo)
            {
                if (idiomas == null) idiomas = idiomaDAL.ListarIdiomas();
                return idiomas;
            }
        }
        public List<Idioma> ObtenerIdiomasActivos()
        {
            return ObtenerIdiomas().Where(i => i.Estado).ToList();
        }
        public Idioma IdiomaPredeterminado
        {
            get { return ObtenerIdiomas().FirstOrDefault(i => i.Predeterminado); }
        }
        public Idioma IdiomaActual
        {
            get
            {
                Usuario usuario = Sesion.Instancia.Usuario;
                Idioma idioma = usuario == null ? null : ObtenerIdiomas().FirstOrDefault(i => i.CodigoIdioma == usuario.Idioma && i.Estado);
                return idioma ?? IdiomaPredeterminado;
            }
        }
        public void RecargarIdiomas()
        {
            lock (bloqueo) { idiomas = null; }
            Traductor.Instancia.InvalidarTodo();
        }

        public void CambiarIdioma(int pCodigoIdioma)
        {
            Usuario usuario = Sesion.Instancia.Usuario;
            if (usuario == null) throw new InvalidOperationException("No hay una sesión iniciada.");
            Idioma nuevo = ObtenerIdiomasActivos().FirstOrDefault(i => i.CodigoIdioma == pCodigoIdioma);
            if (nuevo == null) throw new ExcepcionTraducible("error.idiomaNoDisponible");
            if (usuario.Idioma == nuevo.CodigoIdioma) return;
            usuario.Idioma = nuevo.CodigoIdioma;
            if (!usuario.EsSuperUsuario)
            {
                usuarioDAL.ActualizarIdioma(usuario.DNI, nuevo.CodigoIdioma);
                new DigitoVerificador().ActualizarDigitoFila("Usuario", usuario.DNI);
            }
            SucesoServicio.Instancia.RegistrarSuceso($"{usuario.Nombre} {usuario.Apellido}", $"Cambio de idioma a {nuevo.Nombre}", "Idiomas", 1);
            Notificar(nuevo);
        }
        public string Traducir(string pClave)
        {
            Idioma actual = IdiomaActual;
            Idioma predeterminado = IdiomaPredeterminado;
            if (actual == null || predeterminado == null) return pClave;
            return Traductor.Instancia.Traducir(actual.CodigoIdioma, predeterminado.CodigoIdioma, pClave);
        }
    }
}
