using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Idioma
    {
        public int CodigoIdioma { get; set; }
        public string Nombre { get; set; } // En español, para el administrador: "Alemán"
        public string NombreNativo { get; set; } // En su propio idioma, para el selector: "Deutsch"
        public string Codigo { get; set; } // ISO, igual al sufijo del .resx: "de"
        public bool Predeterminado { get; set; } // Idioma base y de respaldo (español)
        public bool Estado { get; set; } // true = activo (los usuarios lo pueden elegir)
        public int Completitud { get; set; } // Porcentaje de traducciones revisadas, se usa para la pagina: PaginaIdiomas
        public Idioma() { }
        public Idioma(int pCodigoIdioma, string pNombre, string pNombreNativo, string pCodigo, bool pPredeterminado, bool pEstado)
        {
            CodigoIdioma = pCodigoIdioma;
            Nombre = pNombre;
            NombreNativo = pNombreNativo;
            Codigo = pCodigo;
            Predeterminado = pPredeterminado;
            Estado = pEstado;
        }
    }
}
