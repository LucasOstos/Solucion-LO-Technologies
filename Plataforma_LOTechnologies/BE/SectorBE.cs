using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class SectorListado
    {
        public int CodigoSector { get; set; }
        public int CodigoLocal { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public string Ubicacion { get; set; }
        public decimal? Circulacion { get; set; }
        public string Descripcion { get; set; }
        public bool Estado { get; set; }
        public int CantidadProductos { get; set; }
    }
    public class SectorBE
    {
        public int CodigoSector { get; set; }
        public int CodigoLocal { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public bool Estado { get; set; }
        public string Ubicacion { get; set; }
        public decimal? Circulacion { get; set; }
        public string Descripcion { get; set; }
        public SectorBE(int pCodigoSector, int pCodigoLocal, string pNombre, string pTipo, string pUbicacion, decimal? pCirculacion, string pDescripcion, bool pEstado)
        {
            CodigoSector = pCodigoSector;
            CodigoLocal = pCodigoLocal;
            Nombre = pNombre;
            Tipo = pTipo;
            Ubicacion = pUbicacion;
            Circulacion = pCirculacion;
            Descripcion = pDescripcion;
            Estado = pEstado;
        }
    }
}
