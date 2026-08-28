using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class VentaBE
    {
        public int CodigoVenta { get; set; }
        public int CodigoLocal { get; set; }
        public int CodigoProducto { get; set; }
        public DateTime Fecha {  get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal ImporteTotal { get; set; }
        public string Periodo { get; set; }
    }
}
