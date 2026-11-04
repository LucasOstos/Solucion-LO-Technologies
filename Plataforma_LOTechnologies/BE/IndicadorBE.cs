using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class IndicadorSectorListado
    {
        public int CodigoSector { get; set; }
        public string Sector { get; set; }
        public decimal Ventas { get; set; }
        public decimal? Margen { get; set; }      //% sobre las ventas de productos con costo cargado (null = sin costos)
        public decimal? Superficie { get; set; }  //m2 del sector
        public decimal? VentasPorM2 { get; set; } //ventas / superficie (null = sector sin superficie)
        public decimal Circulacion { get; set; }
        public decimal Rotacion { get; set; }        //% del stock que se vende por mes (unidades por mes / stock), sin tope
        public decimal? CoberturaDias { get; set; }  //días que alcanza el stock al ritmo de venta del período (null = sin ventas o sin stock)
        public int IndiceGlobal { get; set; }
        public string ClaseIndiceGlobal { get; set; }
        public int UnidadesVendidas { get; set; }
        public int StockDisponible { get; set; }
        public decimal VentasAnterior { get; set; }  //ventas del mismo período anterior (para la variación)
        public decimal? VariacionVentas { get; set; } //% contra el período anterior (null = sin ventas en el período anterior)
    }
    public class VentaMensual
    {
        public string Periodo { get; set; }
        public decimal Monto { get; set; }
        public bool EnCurso { get; set; }
    }
    public class IndicadorBE
    {
        public int CodigoIndicador { get; set; }
        public int CodigoLocal { get; set; }
        public int? CodigoSector { get; set; }
        public int? CodigoProducto { get; set; }
        public int? CodigoEscenario { get; set; }
        public string Nombre { get; set; }
        public string TipoIndicador { get; set; }
        public DateTime Fecha { get; set; }
        public string Periodo { get; set; }
        public decimal Valor { get; set; }
        public string UnidadMedida { get; set; }
    }
}
