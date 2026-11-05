using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    #region Auxiliares
    [Serializable]
    public class ProductoAnalisisListado
    {
        public const string TIPO_ESTRELLA = "EstrellaEscondida";
        public const string TIPO_LUGAR_PRIVILEGIADO = "LugarPrivilegiado";
        public const string TIPO_SIN_MOVIMIENTO = "SinMovimiento";
        public const string TIPO_RIESGO_QUIEBRE = "RiesgoQuiebre";
        public const string TIPO_SIN_UBICAR = "SinUbicar";
        public int CodigoProducto { get; set; }
        public string CodigoInterno { get; set; } // PROD-001
        public string NombreProducto { get; set; }
        public string NombreCategoria { get; set; }
        public int? CodigoSectorActual { get; set; }
        public string SectorActual { get; set; } // null = sin ubicar en el local
        public int? CodigoSectorSugerido { get; set; }
        public string SectorSugerido { get; set; }
        public int Ventas { get; set; } // unidades vendidas en el período
        public decimal Importe { get; set; } // importe vendido en el período
        public decimal? Margen { get; set; } // % (null = el producto no tiene costo)
        public int Stock { get; set; }
        public decimal? CoberturaDias { get; set; } // null = sin ventas en el período
        public decimal Circulacion { get; set; } // circulación del sector actual
        public string Tipo { get; set; }
        public string Prioridad { get; set; } // Alta / Media / Baja ("" = sin observaciones)
        public string ClaveMotivo { get; set; }
        public object[] Parametros { get; set; }
        public string AccionEscenario { get; set; } // cambio a simular en el sector sugerido (null = no aplica)
    }
    public class ProductoListado
    {
        public int CodigoProducto { get; set; }
        public string CodigoInterno { get; set; }
        public string Nombre { get; set; }
        public string NombreCategoria { get; set; }
        public string NombreSector { get; set; }
        public decimal? Precio { get; set; }
        public int Stock { get; set; }
        public int Estado { get; set; }
    }
    public class ProductoMetrica
    {
        public int CodigoProducto { get; set; }
        public int NumeroSerie { get; set; }
        public string NombreProducto { get; set; }
        public string NombreCategoria { get; set; }
        public decimal? Costo { get; set; }
        public int? CodigoSectorActual { get; set; } // null = sin ubicar en el local
        public string NombreSectorActual { get; set; }
        public decimal CirculacionActual { get; set; }
        public int Ventas { get; set; } // unidades
        public decimal Importe { get; set; }
        public int Stock { get; set; }
    }
    #endregion
    public class ProductoUbicacionBE
    {
        public int CodigoUbicacion { get; set; }
        public int CodigoProducto { get; set; }
        public int CodigoLocal { get; set; }
        public int CodigoSector { get; set; }
        public ProductoUbicacionBE() { }
        public ProductoUbicacionBE(int pCodigoUbicacion, int pCodigoProducto, int pCodigoLocal, int pCodigoSector)
        {
            CodigoUbicacion = pCodigoUbicacion;
            CodigoProducto = pCodigoProducto;
            CodigoLocal = pCodigoLocal;
            CodigoSector = pCodigoSector;
        }
    }
    public class ProductoBE
    {
        public int CodigoProducto { get; set; }
        public int CodigoCategoria { get; set; }
        public int NumeroSerie { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal? Precio { get; set; }
        public decimal? Costo { get; set; }
        public bool Estado { get; set; }
        public int CodigoEmpresa { get; set; }
        public ProductoBE(int codigoProducto, int codigoCategoria, int numeroSerie, string nombre, string descripcion, decimal? precio, bool estado, decimal? costo = null)
        {
            Costo = costo;
            CodigoProducto = codigoProducto;
            CodigoCategoria = codigoCategoria;
            NumeroSerie = numeroSerie;
            Nombre = nombre;
            Descripcion = descripcion;
            Precio = precio;
            Estado = estado;
        }
    }
}
