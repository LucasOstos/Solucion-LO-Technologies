using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Idioma
    {
        public int CodigoIdioma { get; set; }
        public string Nombre { get; set; }  // Ej: "Español"
        public string ISO { get; set; } // ISO: "es"
        public bool Activo { get; set; }
        public List<Traduccion> traducciones {  get; set; }
    }
}
