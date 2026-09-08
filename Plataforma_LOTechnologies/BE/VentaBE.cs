using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class VentaListado
    {
        public string Periodo { get; set; }
        public string Sector { get; set; }
        public string Categoria { get; set; }
        public int Unidades { get; set; }
        public decimal Monto { get; set; }
        public decimal Margen { get; set; }
    }
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
        public VentaBE(int codigoLocal, int codigoProducto, DateTime fecha, int cantidad, decimal precioUnitario, decimal importeTotal, string periodo)
        {
            CodigoLocal = codigoLocal;
            CodigoProducto = codigoProducto;
            Fecha = fecha;
            Cantidad = cantidad;
            PrecioUnitario = precioUnitario;
            ImporteTotal = importeTotal;
            Periodo = periodo;
        }
        public VentaBE(int codigoVenta, int codigoLocal, int codigoProducto, DateTime fecha, int cantidad, decimal precioUnitario, decimal importeTotal, string periodo)
        {
            CodigoVenta = codigoVenta;
            CodigoLocal = codigoLocal;
            CodigoProducto = codigoProducto;
            Fecha = fecha;
            Cantidad = cantidad;
            PrecioUnitario = precioUnitario;
            ImporteTotal = importeTotal;
            Periodo = periodo;
        }
    }
}
