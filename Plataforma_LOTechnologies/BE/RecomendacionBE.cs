using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class RecomendacionBE
    {
        public int CodigoRecomendacion { get; set; }
        public int? CodigoEscenario { get; set; }
        public int CodigoLocal { get; set; }
        public int? CodigoProducto { get; set; }
        public int? CodigoSector { get; set; }
        public string Descripcion { get; set; }
        public string Motivo { get; set; }
        public string Prioridad { get; set; }
        public string TipoRecomendacion { get; set; }
        public string Estado {  get; set; }
        public DateTime FechaGeneracion { get; set; }
    }
}
