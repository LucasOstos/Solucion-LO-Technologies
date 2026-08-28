using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class LocalListado
    {
        public int CodigoLocal { get; set; }
        public int CodigoEmpresa { get; set; }
        public string NombreEmpresa { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string TipoLocal { get; set; }
        public decimal? SuperficieAprox { get; set; }
        public int Estado { get; set; }
    }
    public class LocalBE
    {
        public int CodigoLocal { get; set; }
        public int CodigoEmpresa { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string TipoLocal { get; set; }
        public decimal? SuperficieAprox { get; set; }
        public string Telefono { get; set; }
        public string Observaciones { get; set; }
        public string Estado { get; set; }
    }
}
