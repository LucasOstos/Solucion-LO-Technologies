using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
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
    public class ProductoBE
    {
        public int CodigoProducto { get; set; }
        public int CodigoCategoria { get; set; }
        public int? CodigoSector { get; set; }
        public int NumeroSerie { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal? Precio { get; set; }
        public bool Estado {  get; set; }
        public ProductoBE(int codigoProducto, int codigoCategoria, int? codigoSector, int numeroSerie, string nombre, string descripcion, decimal? precio, bool estado)
        {
            CodigoProducto = codigoProducto;
            CodigoCategoria = codigoCategoria;
            CodigoSector = codigoSector;
            NumeroSerie = numeroSerie;
            Nombre = nombre;
            Descripcion = descripcion;
            Precio = precio;
            Estado = estado;
        }
    }
}
