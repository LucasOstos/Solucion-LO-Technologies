using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class TokenRecuperacion
    {
        public Guid IdToken {  get; set; }
        public int UsuarioDNI {  get; set; }
        public DateTime FechaCreacion {  get; set; }
        public DateTime FechaExpiracion { get; set; }
        public bool TokenUsado {  get; set; }
    }
}
