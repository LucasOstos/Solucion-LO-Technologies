using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class LayoutBE
    {
        public int CodigoLayout { get; set; }
        public int CodigoLocal { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string NombreArchivo { get; set; }
        public string TamanioArchivo { get; set; }
        public DateTime Fecha { get; set; }
        public string Estado { get; set; }
    }
}
