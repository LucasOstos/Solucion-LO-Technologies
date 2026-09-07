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
        public string NombreTabla {  get; set; }
        public string CodigoRegistro {  get; set; }
    }
    public class DigitoVerificador
    {
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
            { "Stock", "CodigoStock" }
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
            foreach (var Tabla in TablasConDVH)
            {
                if (!ValidarTabla(Tabla.Key))
                {
                    return false;
                }
            }
            return true;
        }
        private bool ValidarTabla(string pNombreTabla)
        {
            var dvvActual = accesoDAL.ObtenerDVV(pNombreTabla);
            if (dvvActual == null) return false;
            DataTable DT = accesoDAL.ObtenerTabla(pNombreTabla, TablasConDVH[pNombreTabla]);
            if (DT.Rows.Count != dvvActual.Value.pCantidadRegistros) return false;
            var dvhsRecalculados = new List<string>();
            foreach (DataRow DR in DT.Rows)
            {
                string dvhActual = DR["DVH"] == DBNull.Value ? null : DR["DVH"].ToString();
                string dvhRecalculado = CalcularDVH(DR, DT);
                if(dvhActual != dvhRecalculado) return false;
                dvhsRecalculados.Add(dvhRecalculado);
            }
            if(dvhsRecalculados.Count != dvvActual.Value.pCantidadRegistros) return false;
            string dvvRecalculado = CalcularDVV(dvhsRecalculados);
            return dvvRecalculado == dvvActual.Value.pDVV;
        }
        public List<RegistroCorrupto> ObtenerRegistrosCorruptos()
        {
            List<RegistroCorrupto> corruptos = new List<RegistroCorrupto>();
            foreach(var tabla in TablasConDVH)
            {
                if (ValidarTabla(tabla.Key)) continue;
                DataTable DT = accesoDAL.ObtenerTabla(tabla.Key, tabla.Value);
                foreach(DataRow DR in DT.Rows)
                {
                    string dvhAlmacenado = DR["DVH"] == DBNull.Value ? null : DR["DVH"].ToString();
                    string dvhRecalculado = CalcularDVH(DR, DT);
                    if(dvhAlmacenado != dvhRecalculado)
                    {
                        corruptos.Add(new RegistroCorrupto { NombreTabla = tabla.Key, CodigoRegistro = DR[tabla.Value].ToString() }); 
                    }
                }
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
