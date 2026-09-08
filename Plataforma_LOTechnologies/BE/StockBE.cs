using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class StockListado
    {
        public int CodigoStock { get; set; }
        public int CodigoProducto { get; set; }
        public string Producto { get; set; }
        public string Categoria { get; set; }
        public int CantidadDisponible { get; set; }
        public int? Minimo { get; set; }
        public int? Maximo { get; set; }
        public string EstadoStock { get; set; }
        public string ClaseEstadoStock { get; set; }
        public int PorcentajeStock { get; set; }
    }
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
        public StockBE(int codigoStock, int codigoLocal, int codigoProducto, int cantidadDisponible, int? minimo, int? maximo, string periodo, DateTime fechaActualizacion)
        {
            CodigoStock = codigoStock;
            CodigoLocal = codigoLocal;
            CodigoProducto = codigoProducto;
            CantidadDisponible = cantidadDisponible;
            Minimo = minimo;
            Maximo = maximo;
            Periodo = periodo;
            FechaActualizacion = fechaActualizacion;
        }
    }
}
