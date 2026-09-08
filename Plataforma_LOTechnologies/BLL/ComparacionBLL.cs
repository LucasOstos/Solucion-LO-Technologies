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
    public enum ResultadoComparacion
    {
        Exitoso,
        EscenariosDeDistintoLocal,
        DatosInsuficientes
    }
    public class ComparacionBLL
    {
        private ComparacionDAL comparacionDAL = new ComparacionDAL();
        private DigitoVerificador digitos = new DigitoVerificador();
        public int GuardarComparacion(ComparacionBE pComparacion)
        {
            int codigoNuevo = comparacionDAL.AgregarComparacion(pComparacion);
            digitos.ActualizarDigitoFila("Comparacion", codigoNuevo);
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "Comparar escenarios", "Gestión simulación", 2);
            return codigoNuevo;
        }    
    }
}
