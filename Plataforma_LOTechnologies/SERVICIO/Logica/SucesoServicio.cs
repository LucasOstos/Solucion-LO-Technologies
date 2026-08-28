using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;
using DAL;
namespace SERVICIO.Logica
{
    public class SucesoServicio
    {
        SucesoDAL accesoDAL = new SucesoDAL();
        private static SucesoServicio instancia;
        public static SucesoServicio Instancia
        {
            get { if(instancia == null) instancia = new SucesoServicio(); return instancia; }            
        }
        public void RegistrarSuceso(string pUsuario, string pDescripcion, string pTipoSuceso, int pCriticidad)
        {
            Suceso suceso = new Suceso(DateTime.Now, pUsuario, pDescripcion, pTipoSuceso, pCriticidad);
            accesoDAL.RegistrarSuceso(suceso);
        }
        public List<Suceso> ObtenerSucesosUltimosTresDias()
        {
            return accesoDAL.LeerSucesosUltimosTresDias();
        }
        public List<Suceso> ObtenerSucesos()
        {
            return accesoDAL.LeerSucesos();
        }
    }
}
