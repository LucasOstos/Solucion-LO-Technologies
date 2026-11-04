using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    #region Auxiliar
    public class EscenarioListado
    {
        public int CodigoEscenario { get; set; }
        public int CodigoLocal { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Estado { get; set; }
        public int CantidadCambios { get; set; }
        public decimal VentasEstimadas { get; set; }
        public decimal CirculacionEstimada { get; set; }
        public decimal SatisfaccionEstimada { get; set; }
        public int IndiceGlobalEstimado { get; set; }
        public int? CodigoLayout { get; set; }
        public string NombreLayout { get; set; }
    }
    public class EscenarioCambio
    {
        public int CodigoCambio { get; set; }
        public int CodigoEscenario { get; set; }
        public string TipoCambio { get; set; }
        public string Descripcion { get; set; }
        public int? CodigoSector { get; set; }
        public string Accion { get; set; }
    }
    #endregion
    public class EscenarioBE
    {
        public int CodigoEscenario { get; set; }
        public int CodigoLocal { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Objetivo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Estado { get; set; }
        public int? CodigoLayout { get; set; }
        public EscenarioBE(int codigoLocal, string nombre, string descripcion, string objetivo, DateTime fechaCreacion, string estado)
        {
            CodigoLocal = codigoLocal;
            Nombre = nombre;
            Descripcion = descripcion;
            Objetivo = objetivo;
            FechaCreacion = fechaCreacion;
            Estado = estado;
        }
        public EscenarioBE(int codigoEscenario, int codigoLocal, string nombre, string descripcion, string objetivo, DateTime fechaCreacion, string estado)
        {
            CodigoEscenario = codigoEscenario;
            CodigoLocal = codigoLocal;
            Nombre = nombre;
            Descripcion = descripcion;
            Objetivo = objetivo;
            FechaCreacion = fechaCreacion;
            Estado = estado;
        }
    }
}
