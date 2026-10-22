using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SERVICIO.Logica
{
    public class ExcepcionTraducible : Exception
    {
        public string Clave { get; private set; }
        public object[] Parametros { get; private set; }

        public ExcepcionTraducible(string pClave, params object[] pParametros) : base(pClave)
        {
            Clave = pClave;
            Parametros = pParametros ?? new object[0];
        }
    }
}
