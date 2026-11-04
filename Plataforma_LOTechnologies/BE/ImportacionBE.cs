using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class ImportacionBE
    {
        public const string TIPO_VENTA = "Venta";
        public const string TIPO_STOCK = "Stock";
        public const string ESTADO_APLICADA = "Aplicada";
        public const string ESTADO_DESHECHA = "Deshecha";
        public int CodigoImportacion { get; set; }
        public int CodigoLocal { get; set; }
        public string Tipo { get; set; }
        public string NombreArchivo { get; set; }
        public DateTime Fecha { get; set; }
        public string Usuario { get; set; }
        public int CantidadFilas { get; set; }
        public string Estado { get; set; }
        //Solo las importaciones de ventas se pueden deshacer (el stock importado reemplaza al anterior)
        public bool PuedeDeshacer
        {
            get { return Tipo == TIPO_VENTA && Estado == ESTADO_APLICADA; }
        }
    }
    public class ErrorImportacion
    {
        public int Fila { get; set; }
        public string Clave { get; set; }
        public object[] Parametros { get; set; } = new object[0];
        public ErrorImportacion(int pFila, string pClave, params object[] pParametros)
        {
            Fila = pFila;
            Clave = pClave;
            Parametros = pParametros ?? new object[0];
        }
    }
    public class ResultadoImportacion
    {
        public bool Exito { get { return Errores.Count == 0 && FilasImportadas > 0; } }
        public int CodigoImportacion { get; set; }
        public int FilasImportadas { get; set; }
        public int FilasSinUbicar { get; set; }
        public List<ErrorImportacion> Errores { get; set; } = new List<ErrorImportacion>();
    }
    public class ProductoImportable
    {
        public int CodigoProducto { get; set; }
        public int NumeroSerie { get; set; }
        public bool Activo { get; set; }
        public decimal? Precio { get; set; }
        public bool UbicadoEnLocal { get; set; }
    }
}
