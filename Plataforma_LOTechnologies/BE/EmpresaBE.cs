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
        public string Correo {  get; set; }
        public int Telefono { get; set; }
        public string Direccion { get; set; }
        public bool Estado { get; set; }
        public EmpresaBE(int pCodigo, string pRazon, string pCUIT, string pCorreo, int pTelefono, string pDireccion, bool pEstado)
        {
            CodigoEmpresa = pCodigo;
            RazonSocial = pRazon;
            CUIT = pCUIT;
            Correo = pCorreo;
            Telefono = pTelefono;
            Direccion = pDireccion;
            Estado = pEstado;
        }
    }
}
