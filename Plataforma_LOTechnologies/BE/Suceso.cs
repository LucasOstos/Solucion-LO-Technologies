using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Suceso
    {
        public int CodigoSuceso {  get; set; }
        public DateTime Fecha {  get; set; }
        public string Usuario {  get; set; }
        public string Descripcion {  get; set; }
        public string TipoSuceso { get; set; }
        public int Criticidad {  get; set; }
        
        //Constructor para lectura
        public Suceso(int pCodigo, DateTime pFecha, string pUsuario, string pDescripcion, string pTipoSuceso, int pCriticidad)
        {
            CodigoSuceso = pCodigo;
            Fecha = pFecha;
            Usuario = pUsuario;
            Descripcion = pDescripcion;
            TipoSuceso = pTipoSuceso;
            Criticidad = pCriticidad;
        }

        //Constructor para escritura
        public Suceso(DateTime pFecha, string pUsuario, string pDescripcion, string pTipoSuceso, int pCriticidad)
        {
            Fecha = pFecha;
            Usuario = pUsuario;
            Descripcion = pDescripcion;
            TipoSuceso = pTipoSuceso;
            Criticidad = pCriticidad;
        }
    }
}
