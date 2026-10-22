using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;

namespace SERVICIO.Logica
{
    public interface ISujetoIdioma
    {
        void Suscribir(IObservadorIdioma pObservador);
        void Desuscribir(IObservadorIdioma pObservador);
        void Notificar(Idioma pIdioma);
    }
}
