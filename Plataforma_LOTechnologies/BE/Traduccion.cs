using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class TraduccionComparada
    {
        public string Clave { get; set; }
        public string Modulo { get; set; }
        public string TextoBase { get; set; }
        public string TextoTraducido { get; set; }
        public bool TraduccionFaltante { get; set; }
    }
    public class Traduccion
    {
        public int CodigoIdioma { get; set; }
        public string Modulo { get; set; }
        public string Texto { get; set; }
        public string Clave { get; set; }
        public int CodigoTraduccion { get; set; }
    }
}
