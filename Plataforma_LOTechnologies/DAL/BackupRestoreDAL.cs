using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class BackupRestoreDAL
    {
        private const string NOMBRE_BD = "BD_LOTechnologies";
        private AccesoDatos acceso = new AccesoDatos();
        public string RealizarBackup(string pRutaCarpeta)
        {
            string nombreArchivo = $"BCK_{NOMBRE_BD}_{DateTime.Now:ddMMyy_HHmm}.bak";
            string rutaCompleta = Path.Combine(pRutaCarpeta, nombreArchivo);
            string query = $@"BACKUP DATABASE [{NOMBRE_BD}] TO DISK = @RutaCompleta WITH FORMAT, INIT, SKIP, STATS = 10";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@RutaCompleta", rutaCompleta);
                CO.Open();
                CM.ExecuteNonQuery();
            }
            return nombreArchivo;
        }
        public void RealizarRestore(string pRutaArchivo)
        {
            // Limpia el pool de conexiones de ADO.NET antes de restaurar.
            // Sin esto, SQL Server puede rechazar el ALTER DATABASE SET SINGLE_USER si todavía hay conexiones inactivas del pool.
            SqlConnection.ClearAllPools();
            string query = $@"USE master; ALTER DATABASE [{NOMBRE_BD}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; RESTORE DATABASE [{NOMBRE_BD}] FROM DISK = @RutaArchivo WITH REPLACE, STATS = 10;
                              ALTER DATABASE [{NOMBRE_BD}] SET MULTI_USER;";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@RutaArchivo", pRutaArchivo);
                CO.Open();
                CM.ExecuteNonQuery();
            }
        }
        public bool ValidarArchivoBackup(string pRutaArchivo)
        {
            string query = "RESTORE HEADERONLY FROM DISK = @RutaArchivo";
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand(query, CO))
            {
                CM.Parameters.AddWithValue("@RutaArchivo", pRutaArchivo);
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    if (DR.Read())
                    {
                        return DR["DatabaseName"].ToString() == NOMBRE_BD;
                    }
                }
            }
            return false;
        }
    }
}
