using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SERVICIO.Logica
{
    public enum ResultadoLogin
    {
        Exitoso,
        CredencialesIncorrectas,
        UsuarioBloqueado,
        UsuarioRecienBloqueado,
        UsuarioDeshabilitado
    }
}
