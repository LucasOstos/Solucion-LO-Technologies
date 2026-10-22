using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class TraduccionComparada
    {
        public int CodigoTraduccion { get; set; } // de la traducción (la que se edita)
        public string Clave { get; set; }
        public string Modulo { get; set; }
        public string TextoBase { get; set; } // en español
        public string TextoTraducido { get; set; }
        public bool Pendiente { get; set; }
    }
    public class Traduccion
    {
        public int CodigoTraduccion { get; set; }
        public int CodigoIdioma { get; set; }
        public string Clave { get; set; } // EJ: "principal.btnCerrarSesion"
        public string Modulo { get; set; } // EJ: "principal"
        public string Texto { get; set; }
        public bool Pendiente { get; set; } // true = no estaba en el .resx y tiene el texto en español
        public Traduccion() { }
        public Traduccion(int pCodigoTraduccion, int pCodigoIdioma, string pClave, string pModulo, string pTexto, bool pPendiente)
        {
            CodigoTraduccion = pCodigoTraduccion;
            CodigoIdioma = pCodigoIdioma;
            Clave = pClave;
            Modulo = pModulo;
            Texto = pTexto;
            Pendiente = pPendiente;
        }
    }
}
