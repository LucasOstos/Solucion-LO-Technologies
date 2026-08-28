using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class EscenarioCambio
    {
        public int CodigoCambio { get; set; }
        public int CodigoEscenario { get; set; }
        public string TipoCambio { get; set; }
        public string Descripcion { get; set; }
    }
    public class EscenarioBE
    {
        public int CodigoEscenario { get; set; }
        public int CodigoLocal { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Objetivo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Estado { get; set; }
    }
}
