using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class EmpresaBE
    {
        public int CodigoEmpresa { get; set; }
        public string RazonSocial { get; set; }
        public string CUIT { get; set; }
        public int? Telefono { get; set; }
        public string Direccion { get; set; }
        public bool Estado { get; set; }
        public EmpresaBE(int pCodigo, string pRazon, string pCUIT, int pTelefono, string pDireccion, bool pEstado)
        {
            CodigoEmpresa = pCodigo;
            RazonSocial = pRazon;
            CUIT = pCUIT;
            Telefono = pTelefono;
            Direccion = pDireccion;
            Estado = pEstado;
        }
    }
}
