using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public enum EntidadEmpresa
    {
        Local,
        Sector,
        Layout,
        Stock,
        Escenario,
        Reporte,
        Producto,
        Categoria,
        Usuario
    }
    public class AlcanceDAL
    {
        private AccesoDatos acceso = new AccesoDatos();
        private static readonly Dictionary<EntidadEmpresa, string> Consultas = new Dictionary<EntidadEmpresa, string>
        {
            { EntidadEmpresa.Local,     "SELECT CodigoEmpresa FROM Local WHERE CodigoLocal = @Codigo" },
            { EntidadEmpresa.Sector,    "SELECT l.CodigoEmpresa FROM Sector s INNER JOIN Local l ON s.CodigoLocal = l.CodigoLocal WHERE s.CodigoSector = @Codigo" },
            { EntidadEmpresa.Layout,    "SELECT l.CodigoEmpresa FROM Layout y INNER JOIN Local l ON y.CodigoLocal = l.CodigoLocal WHERE y.CodigoLayout = @Codigo" },
            { EntidadEmpresa.Stock,     "SELECT l.CodigoEmpresa FROM Stock st INNER JOIN Local l ON st.CodigoLocal = l.CodigoLocal WHERE st.CodigoStock = @Codigo" },
            { EntidadEmpresa.Escenario, "SELECT l.CodigoEmpresa FROM Escenario e INNER JOIN Local l ON e.CodigoLocal = l.CodigoLocal WHERE e.CodigoEscenario = @Codigo" },
            { EntidadEmpresa.Reporte,   "SELECT l.CodigoEmpresa FROM Reporte r INNER JOIN Local l ON r.CodigoLocal = l.CodigoLocal WHERE r.CodigoReporte = @Codigo" },
            { EntidadEmpresa.Producto,  "SELECT CodigoEmpresa FROM Producto WHERE CodigoProducto = @Codigo" },
            { EntidadEmpresa.Categoria, "SELECT CodigoEmpresa FROM Categoria WHERE CodigoCategoria = @Codigo" },
            { EntidadEmpresa.Usuario,   "SELECT UsuarioCodigoEmpresa FROM Usuario WHERE UsuarioDNI = @Codigo" }
        };
        public int? ObtenerEmpresa(EntidadEmpresa pEntidad, int pCodigo)
        {
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(Consultas[pEntidad], CO))
            {
                CM.Parameters.AddWithValue("@Codigo", pCodigo);
                CO.Open();
                object resultado = CM.ExecuteScalar();
                return (resultado == null || resultado == DBNull.Value) ? (int?)null : Convert.ToInt32(resultado);
            }
        }
    }
}
