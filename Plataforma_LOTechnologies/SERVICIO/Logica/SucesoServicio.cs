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
        SucesoDAL sucesoDAL = new SucesoDAL();
        private static SucesoServicio instancia;
        public static SucesoServicio Instancia
        {
            get { if(instancia == null) instancia = new SucesoServicio(); return instancia; }            
        }
        public void RegistrarSuceso(string pUsuario, string pDescripcion, string pTipoSuceso, int pCriticidad)
        {
            Suceso suceso = new Suceso(DateTime.Now, pUsuario, pDescripcion, pTipoSuceso, pCriticidad);
            sucesoDAL.RegistrarSuceso(suceso);
        }
        public List<Suceso> ObtenerSucesosUltimosTresDias()
        {
            return sucesoDAL.LeerSucesosUltimosTresDias();
        }
        public List<Suceso> ObtenerSucesos()
        {
            return sucesoDAL.LeerSucesos();
        }
        public List<Suceso> FiltrarSucesos(DateTime? pDesde, DateTime? pHasta, string pBusqueda, string pTipoSuceso, int? pCriticidad)
        {
            return sucesoDAL.FiltrarSucesos(pDesde, pHasta, pBusqueda, pTipoSuceso, pCriticidad);
        }
        public List<string> ObtenerTiposSuceso()
        {
            return sucesoDAL.ObtenerTiposSuceso();
        }
    }
}
