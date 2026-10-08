using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DAL;

namespace SERVICIO.Logica
{
    public class RegistroCorrupto
    {
        public string NombreTabla { get; set; }
        public string CodigoRegistro { get; set; }
        public string Problema { get; set; }
    }
    public class DigitoVerificador
    {
        private const string TODA_LA_TABLA = "Toda la tabla";
        DigitoVerificadorDAL accesoDAL = new DigitoVerificadorDAL();
        private readonly Dictionary<string, string> TablasConDVH = new Dictionary<string, string>
        {
            { "Empresa", "CodigoEmpresa" },
            { "Usuario", "UsuarioDNI" },
            { "Local", "CodigoLocal"},
            { "Sector", "CodigoSector" },
            { "Layout", "CodigoLayout"},
            { "Categoria", "CodigoCategoria" },
            { "Producto", "CodigoProducto" },
            { "Venta", "CodigoVenta" },
            { "Stock", "CodigoStock" },
            { "Escenario", "CodigoEscenario" },
            { "Comparacion", "CodigoComparacion" },
            { "Indicador", "CodigoIndicador" },
            { "Recomendacion", "CodigoRecomendacion" },
            { "Reporte", "CodigoReporte" }
        };
        public void ActualizarDigitoTabla(string pNombreTabla)
        {
            if(!TablasConDVH.TryGetValue(pNombreTabla, out string pColumnaPK))
            {
                throw new ArgumentException($"La tabla '{pNombreTabla}' no cuenta con la columna DVH");
            }
            CalcularDigitosTabla(pNombreTabla, pColumnaPK);
        }
        public void ActualizarDigitoFila(string pNombreTabla, object pValorPK)
        {
            if (!TablasConDVH.TryGetValue(pNombreTabla, out string pColumnaPK))
            {
                throw new ArgumentException($"La tabla '{pNombreTabla}' no cuenta con la columna DVH");
            }
            DataTable DT = accesoDAL.ObtenerTabla(pNombreTabla, pColumnaPK);
            var dvhs = new List<string>();
            foreach(DataRow DR in DT.Rows)
            {
                string dvh;
                if (DR[pColumnaPK].ToString() == pValorPK.ToString())
                {
                    dvh = CalcularDVH(DR, DT);
                    accesoDAL.ActualizarDVH(pNombreTabla, pColumnaPK, DR[pColumnaPK], dvh);
                }
                else
                {
                    dvh = DR["DVH"] == DBNull.Value ? null : DR["DVH"].ToString();
                }
                dvhs.Add(dvh);
            }
            string dvv = CalcularDVV(dvhs);
            accesoDAL.GuardarDVV(pNombreTabla, dvv, DT.Rows.Count);
        }
        public bool CalcularTablas()
        {
            try
            {
                foreach (var tabla in TablasConDVH)
                {
                    CalcularDigitosTabla(tabla.Key, tabla.Value);
                }
                return true;
            }
            catch { return false; }
        }
        private void CalcularDigitosTabla(string pNombreTabla, string pColumnaPK)
        {
            DataTable DT = accesoDAL.ObtenerTabla(pNombreTabla, pColumnaPK);
            var dvh = new List<string>();
            foreach(DataRow DR in DT.Rows)
            {
                string pDVH = CalcularDVH(DR, DT);
                dvh.Add(pDVH);
                accesoDAL.ActualizarDVH(pNombreTabla, pColumnaPK, DR[pColumnaPK], pDVH);
            }
            string pDVV = CalcularDVV(dvh);
            accesoDAL.GuardarDVV(pNombreTabla, pDVV, DT.Rows.Count);
        }
        public bool ValidarIntegridadDatos()
        {
            foreach (var tabla in TablasConDVH)
            {
                if (DiagnosticarTabla(tabla.Key, tabla.Value).Count > 0)
                {
                    return false;
                }
            }
            return true;
        }
        private List<RegistroCorrupto> DiagnosticarTabla(string pNombreTabla, string pColumnaPK)
        {
            var problemas = new List<RegistroCorrupto>();
            var dvvGuardado = accesoDAL.ObtenerDVV(pNombreTabla);
            if (dvvGuardado == null)
            {
                problemas.Add(NuevoProblema(pNombreTabla, TODA_LA_TABLA,
                    "No existe el dígito verificador vertical (DVV) de la tabla. Pudo haber sido eliminado de la tabla DigitoVerificador."));
                return problemas;
            }
            DataTable DT = accesoDAL.ObtenerTabla(pNombreTabla, pColumnaPK);
            var dvhsRecalculados = new List<string>();
            foreach (DataRow DR in DT.Rows)
            {
                string dvhGuardado = DR["DVH"] == DBNull.Value ? null : DR["DVH"].ToString();
                string dvhRecalculado = CalcularDVH(DR, DT);
                dvhsRecalculados.Add(dvhRecalculado);
                if (dvhGuardado == null)
                {
                    problemas.Add(NuevoProblema(pNombreTabla, DR[pColumnaPK].ToString(),
                        "El registro no tiene dígito verificador (DVH). Pudo haber sido insertado por fuera del sistema."));
                }
                else if (dvhGuardado != dvhRecalculado)
                {
                    problemas.Add(NuevoProblema(pNombreTabla, DR[pColumnaPK].ToString(),
                        "El DVH no coincide con los datos del registro. Se modificó un dato o el propio DVH."));
                }
            }
            int cantidadGuardada = dvvGuardado.Value.pCantidadRegistros;
            if (DT.Rows.Count != cantidadGuardada)
            {
                problemas.Add(NuevoProblema(pNombreTabla, TODA_LA_TABLA,
                    $"La tabla tiene {DT.Rows.Count} registros y se esperaban {cantidadGuardada}. Se agregaron o eliminaron registros por fuera del sistema."));
            }
            else if (CalcularDVV(dvhsRecalculados) != dvvGuardado.Value.pDVV && problemas.Count == 0)
            {
                problemas.Add(NuevoProblema(pNombreTabla, TODA_LA_TABLA,
                    "El dígito verificador vertical (DVV) no coincide. Se modificaron registros recalculando su DVH, o se alteró el DVV guardado."));
            }
            return problemas;
        }
        private RegistroCorrupto NuevoProblema(string pNombreTabla, string pRegistro, string pProblema)
        {
            return new RegistroCorrupto { NombreTabla = pNombreTabla, CodigoRegistro = pRegistro, Problema = pProblema };
        }        
        public List<RegistroCorrupto> ObtenerRegistrosCorruptos()
        {
            List<RegistroCorrupto> corruptos = new List<RegistroCorrupto>();
            foreach (var tabla in TablasConDVH)
            {
                corruptos.AddRange(DiagnosticarTabla(tabla.Key, tabla.Value));
            }
            return corruptos;
        }
        private string CalcularDVH(DataRow pFila, DataTable pTabla)
        {
            StringBuilder filaConcatenada = new StringBuilder();
            foreach(DataColumn DC in pTabla.Columns)
            {
                if(DC.ColumnName == "DVH")
                {
                    continue;
                }
                filaConcatenada.Append(ConvertirValorAString(pFila[DC])).Append("-");
            }
            return Encriptar(filaConcatenada.ToString());
        }
        private string CalcularDVV(List<string> pDVHs)
        {
            string acumulado = "";
            foreach(string dvh in pDVHs)
            {
                acumulado = Encriptar(acumulado + dvh);
            }
            return acumulado;
        }
        private string ConvertirValorAString(object item)
        {
            if (item == null || item == DBNull.Value)
            {
                return "";
            }

            switch (item)
            {
                case DateTime time:
                    return time.ToString("o", CultureInfo.InvariantCulture);
                case decimal d:
                    return d.ToString(CultureInfo.InvariantCulture);
                case double db:
                    return db.ToString(CultureInfo.InvariantCulture);
                case float f:
                    return f.ToString(CultureInfo.InvariantCulture);
                default:
                    return item.ToString();
            }
        }
        private string Encriptar(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(bytes);
                var builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2")); // "x2" para formato hexadecimal
                }
                return builder.ToString();
            }
        }
    }
}
