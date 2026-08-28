using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    #region Filtros
    public class UsuarioListado
    {
        public int DNI { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public int Rol { get; set; }
        public bool estadoActivo { get; set; }
        public bool estadoBloqueado { get; set; }
        public DateTime ultimoAcceso { get; set; }
        public int? CodigoEmpresa { get; set; }
        public string NombreEmpresa { get; set; }
    }
    #endregion
    public class Usuario
    {
        public int DNI {  get; set; }
        public string Nombre {  get; set; }
        public string Apellido {  get; set; }
        public string Email {  get; set; }
        public string Contrasenia {  get; set; }
        public int Rol {  get; set; }
        public bool estadoActivo {  get; set; }
        public bool estadoBloqueado {  get; set; }
        public int intentosAcceso {  get; set; }
        public DateTime ultimoAcceso { get; set; }
        public int Idioma {  get; set; }
        public int CodigoEmpresa { get; set; }
        public Usuario() { }
        public Usuario(int pDNI, string pNombre, string pApellido, string pEmail, string pContrasenia, int pRol, bool pEstadoActivo, bool pEstadoBloqueado, int pIntentosAcceso, DateTime pUltimoAcceso, int pIdioma, int pCodigoEmpresa)
        {
            DNI = pDNI;
            Nombre = pNombre;
            Apellido = pApellido;
            Email = pEmail;
            Contrasenia = pContrasenia;
            Rol = pRol;
            estadoActivo = pEstadoActivo;
            estadoBloqueado = pEstadoBloqueado;
            intentosAcceso = pIntentosAcceso;
            ultimoAcceso = pUltimoAcceso;
            Idioma = pIdioma;
            CodigoEmpresa = pCodigoEmpresa;
        }
    }
}
