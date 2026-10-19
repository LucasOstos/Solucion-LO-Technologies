using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class PermisoSimple : Permiso
    {
        public PermisoSimple(int pCodigo, string pNombre) : base(pCodigo, pNombre) { }
        public override bool EsCompuesto { get { return false; } }
        public override List<Permiso> ObtenerHijos()
        {
            return new List<Permiso>();
        }
        public override bool TienePermiso(string pNombrePermiso)
        {
            return string.Equals(Nombre, pNombrePermiso, StringComparison.OrdinalIgnoreCase);
        }
        public override bool Contiene(Permiso pPermiso)
        {
            return EsMismo(pPermiso);
        }
    }
    public static class PermisoNombres
    {
        public const string GestionarEmpresas = "Gestionar empresas";
        public const string GestionarLocales = "Gestionar locales";
        public const string GestionarLayoutSectores = "Gestionar layout y sectores";
        public const string GestionarProductos = "Gestionar productos y categorías";
        public const string CargarStockVentas = "Cargar stock y ventas";
        public const string GestionarEscenarios = "Gestionar escenarios";
        public const string CompararEscenarios = "Comparar escenarios";
        public const string VisualizarIndicadores = "Visualizar indicadores";
        public const string GenerarReportes = "Generar reportes";
        public const string AnalizarProductos = "Analizar ubicación de productos";
        public const string GestionarUsuarios = "Gestionar usuarios";
        public const string RealizarBackupRestore = "Realizar backup y restore";
        public const string RecalcularDigitos = "Recalcular dígitos verificadores";
        public const string ConsultarBitacora = "Consultar bitácora";
        public const string GestionarIdiomas = "Gestionar idiomas";
        public const string GestionarPermisos = "Gestionar permisos";

        public static readonly string[] Globales =
        {
            GestionarEmpresas, RealizarBackupRestore, RecalcularDigitos, ConsultarBitacora, GestionarIdiomas, GestionarPermisos
        };

        public static bool EsGlobal(string pNombrePermiso)
        {
            return Globales.Any(g => string.Equals(g, pNombrePermiso, StringComparison.OrdinalIgnoreCase));
        }

        public static List<string> Todos()
        {
            return typeof(PermisoNombres).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                                         .Where(f => f.IsLiteral && f.FieldType == typeof(string))
                                         .Select(f => (string)f.GetRawConstantValue()).ToList();
        }
    }
}
