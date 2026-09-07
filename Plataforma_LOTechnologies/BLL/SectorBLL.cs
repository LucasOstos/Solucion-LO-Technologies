using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;
using DAL;
using SERVICIO.Logica;

namespace BLL
{
    public class SectorBLL
    {
        private SectorDAL sectorDAL = new SectorDAL();
        private DigitoVerificador digitos = new DigitoVerificador();
        public List<SectorListado> ObtenerSectoresPorLocal(int pCodigoLocal)
        {
            return sectorDAL.ObtenerSectoresPorLocal(pCodigoLocal);
        }
        public SectorBE ObtenerSectorPorCodigo(int pCodigoSector)
        {
            return sectorDAL.ObtenerSectorPorCodigo(pCodigoSector);
        }
        public void CrearSector(SectorBE pSector)
        {
            int codigoSectorNuevo = sectorDAL.AgregarSector(pSector);
            digitos.ActualizarDigitoFila("Sector", codigoSectorNuevo);
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "ABM de sectores del local", "Gestión locales", 2);
        }
        public void ModificarSector(SectorBE pSector)
        {
            sectorDAL.ModificarSector(pSector);
            digitos.ActualizarDigitoFila("Sector", pSector.CodigoSector);
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "ABM de sectores del local", "Gestión locales", 2);
        }
    }
}
