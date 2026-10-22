using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;

namespace SERVICIO.Logica
{
    public class Traductor
    {
        private static readonly Traductor instancia = new Traductor();
        public static Traductor Instancia { get { return instancia; } }
        private Traductor() { }
        private readonly TraduccionDAL traduccionDAL = new TraduccionDAL();
        private readonly Dictionary<int, Dictionary<string, string>> cache = new Dictionary<int, Dictionary<string, string>>();
        private readonly object bloqueo = new object();
        public string Traducir(int pCodigoIdioma, int pCodigoPredeterminado, string pClave)
        {
            if (string.IsNullOrEmpty(pClave)) return pClave;
            string texto;
            if (ObtenerTextos(pCodigoIdioma).TryGetValue(pClave, out texto) && !string.IsNullOrEmpty(texto)) return texto;
            if (pCodigoPredeterminado != pCodigoIdioma
                && ObtenerTextos(pCodigoPredeterminado).TryGetValue(pClave, out texto) && !string.IsNullOrEmpty(texto)) return texto;
            return pClave;
        }
        private Dictionary<string, string> ObtenerTextos(int pCodigoIdioma)
        {
            lock (bloqueo)
            {
                Dictionary<string, string> textos;
                if (!cache.TryGetValue(pCodigoIdioma, out textos))
                {
                    textos = pCodigoIdioma > 0 ? traduccionDAL.ObtenerTextos(pCodigoIdioma) : new Dictionary<string, string>();
                    cache[pCodigoIdioma] = textos;
                }
                return textos;
            }
        }
        public void Invalidar(int pCodigoIdioma)
        {
            lock (bloqueo) { cache.Remove(pCodigoIdioma); }
        }
        public void InvalidarTodo()
        {
            lock (bloqueo) { cache.Clear(); }
        }
    }
}
