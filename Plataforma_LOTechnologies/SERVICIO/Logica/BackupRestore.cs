using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;

namespace SERVICIO.Logica
{
    public class Backup
    {
        public string NombreArchivo { get; set; }
        public DateTime Fecha { get; set; }
        public string Tamanio { get; set; }
        public string Tipo { get; set; }
        public bool Exitoso { get; set; }
        public string RutaArchivo { get; set; }
    }
    public class BackupRestore
    {
        private BackupRestoreDAL backupRestoreDAL = new BackupRestoreDAL();
        private const string rutaBackups = @"C:\BackupsLOTechnologies\";
        public List<Backup> ObtenerBackups()
        {
            var backups = new List<Backup>();
            if (!Directory.Exists(rutaBackups))
            {
                return backups;
            }
            var archivos = Directory.GetFiles(rutaBackups, "*.bak").Select(ruta => new FileInfo(ruta)).OrderByDescending(f => f.CreationTime);
            foreach (var archivo in archivos)
            {
                backups.Add(new Backup
                {
                    NombreArchivo = archivo.Name,
                    Fecha = archivo.CreationTime,
                    Tamanio = FormatearTamanio(archivo.Length),
                    Tipo = "Manual",
                    Exitoso = true,
                    RutaArchivo = archivo.FullName
                });
            }
            return backups;
        }
        public Backup ObtenerBackupPorNombreArchivo(string pNombreArchivo)
        {
            return ObtenerBackups().Find(b => b.NombreArchivo == pNombreArchivo);
        }
        public Backup RealizarBackup()
        {
            if (!Directory.Exists(rutaBackups))
            {
                Directory.CreateDirectory(rutaBackups);
            }
            string nombreArchivo = backupRestoreDAL.RealizarBackup(rutaBackups);
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "Realizar backup/restore", "Seguridad", 1);
            return ObtenerBackupPorNombreArchivo(nombreArchivo);
        }
        public void RealizarRestore(string pNombreArchivo)
        {
            Backup backup = ObtenerBackupPorNombreArchivo(pNombreArchivo);
            if (backup == null)
            {
                throw new FileNotFoundException("El backup seleccionado ya no existe en el servidor.");
            }
            backupRestoreDAL.RealizarRestore(backup.RutaArchivo);
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "Realizar backup/restore", "Seguridad", 1);
        }
        public void RealizarRestoreDigito(string pRutaArchivo)
        {
            backupRestoreDAL.RealizarRestore(pRutaArchivo);

            if (Sesion.Instancia.IsLogueado())
            {
                SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "Restore (integridad)", "Seguridad", 1);
            }
        }
        public bool ValidarArchivoBackup(string pRutaArchivo)
        {
            return backupRestoreDAL.ValidarArchivoBackup(pRutaArchivo);
        }
        private string FormatearTamanio(long bytes)
        {
            double mb = bytes / 1024.0 / 1024.0;
            return mb < 1 ? $"{bytes / 1024.0:0} KB" : $"{mb:0.0} MB";
        }
    }
}
