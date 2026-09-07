using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class StockBE
    {
        public int CodigoStock { get; set; }
        public int CodigoLocal { get; set; }
        public int CodigoProducto { get; set; }
        public int CantidadDisponible { get; set; }
        public int? Minimo { get; set; }
        public int? Maximo { get; set; }
        public string Periodo { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
