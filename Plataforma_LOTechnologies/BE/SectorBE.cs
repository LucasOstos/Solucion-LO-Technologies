using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class SectorBE
    {
        public int CodigoSector { get; set; }
        public int CodigoLocal { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public bool Estado { get; set; }
        public string Ubicacion { get; set; }
        public decimal? Circulacion { get; set; }
        public string Descripcion { get; set; }
    }
}
