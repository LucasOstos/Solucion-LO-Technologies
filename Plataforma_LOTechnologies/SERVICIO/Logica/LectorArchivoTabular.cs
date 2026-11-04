using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SERVICIO.Logica
{
    public static class LectorArchivoTabular
    {
        private static readonly XNamespace NS_HOJA = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
        private static readonly XNamespace NS_RELACION = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
        private static readonly XNamespace NS_PAQUETE = "http://schemas.openxmlformats.org/package/2006/relationships";

        public static bool EsFormatoSoportado(string pNombreArchivo)
        {
            string extension = Path.GetExtension(pNombreArchivo ?? "").ToLowerInvariant();
            return extension == ".csv" || extension == ".xlsx";
        }
        // Devuelve las filas no vacías del archivo. Lanza ExcepcionTraducible si el archivo no se puede leer.
        public static List<string[]> Leer(Stream pArchivo, string pNombreArchivo)
        {
            string extension = Path.GetExtension(pNombreArchivo ?? "").ToLowerInvariant();
            try
            {
                byte[] contenido;
                using (var memoria = new MemoryStream())
                {
                    pArchivo.CopyTo(memoria);
                    contenido = memoria.ToArray();
                }
                List<string[]> filas = extension == ".xlsx" ? LeerExcel(new MemoryStream(contenido)) : LeerCsv(contenido);
                return filas.Where(f => f.Any(c => !string.IsNullOrWhiteSpace(c))).ToList();
            }
            catch (ExcepcionTraducible) { throw; }
            catch (Exception)
            {
                throw new ExcepcionTraducible("importar.errArchivoIlegible");
            }
        }

        #region CSV
        // Separador ";" o ",": se toma el que aparezca más en la primera línea (encabezado).
        // Respeta comillas dobles (un campo "1.234,50" o con el separador adentro).
        private static List<string[]> LeerCsv(byte[] pContenido)
        {
            string texto;
            try
            {
                // UTF-8 estricto: si el archivo no es UTF-8 válido lanza una excepción
                texto = new UTF8Encoding(false, true).GetString(pContenido);
            }
            catch (DecoderFallbackException)
            {
                // Un CSV guardado desde Excel en Windows suele venir en ANSI (Windows-1252)
                texto = Encoding.GetEncoding(1252).GetString(pContenido);
            }
            texto = texto.TrimStart('﻿');
            string primeraLinea = texto.Split('\n')[0];
            char separador = primeraLinea.Count(c => c == ';') >= primeraLinea.Count(c => c == ',') ? ';' : ',';

            var filas = new List<string[]>();
            var campos = new List<string>();
            var campo = new StringBuilder();
            bool entreComillas = false;
            for (int i = 0; i < texto.Length; i++)
            {
                char c = texto[i];
                if (entreComillas)
                {
                    if (c == '"' && i + 1 < texto.Length && texto[i + 1] == '"') { campo.Append('"'); i++; }
                    else if (c == '"') entreComillas = false;
                    else campo.Append(c);
                }
                else if (c == '"') entreComillas = true;
                else if (c == separador) { campos.Add(campo.ToString().Trim()); campo.Clear(); }
                else if (c == '\n')
                {
                    campos.Add(campo.ToString().Trim()); campo.Clear();
                    filas.Add(campos.ToArray()); campos.Clear();
                }
                else if (c != '\r') campo.Append(c);
            }
            if (campo.Length > 0 || campos.Count > 0)
            {
                campos.Add(campo.ToString().Trim());
                filas.Add(campos.ToArray());
            }
            return filas;
        }
        #endregion

        #region Excel
        private static List<string[]> LeerExcel(Stream pArchivo)
        {
            using (var zip = new ZipArchive(pArchivo, ZipArchiveMode.Read))
            {
                List<string> textosCompartidos = LeerTextosCompartidos(zip);
                XDocument hoja = AbrirXml(zip, ObtenerRutaPrimeraHoja(zip));
                if (hoja == null) throw new ExcepcionTraducible("importar.errArchivoIlegible");

                var filas = new List<string[]>();
                foreach (XElement fila in hoja.Descendants(NS_HOJA + "row"))
                {
                    var celdas = new SortedDictionary<int, string>();
                    int columnaSiguiente = 0;
                    foreach (XElement celda in fila.Elements(NS_HOJA + "c"))
                    {
                        // La referencia (A1, C7...) dice la columna: Excel omite las celdas vacías
                        string referencia = (string)celda.Attribute("r");
                        int columna = referencia == null ? columnaSiguiente : IndiceColumna(referencia);
                        celdas[columna] = ValorCelda(celda, textosCompartidos);
                        columnaSiguiente = columna + 1;
                    }
                    if (celdas.Count == 0) continue;
                    var valores = new string[celdas.Keys.Max() + 1];
                    for (int i = 0; i < valores.Length; i++) valores[i] = celdas.ContainsKey(i) ? celdas[i] : "";
                    filas.Add(valores);
                }
                return filas;
            }
        }
        // Los textos de las celdas se guardan una sola vez en sharedStrings.xml y la celda guarda el índice
        private static List<string> LeerTextosCompartidos(ZipArchive pZip)
        {
            var textos = new List<string>();
            XDocument documento = AbrirXml(pZip, "xl/sharedStrings.xml");
            if (documento == null) return textos;
            foreach (XElement item in documento.Root.Elements(NS_HOJA + "si"))
            {
                // Un texto con formato mixto viene partido en varios <r><t>: se concatenan
                textos.Add(string.Concat(item.Descendants(NS_HOJA + "t").Select(t => t.Value)));
            }
            return textos;
        }
        // La primera hoja según el orden de workbook.xml (no necesariamente sheet1.xml)
        private static string ObtenerRutaPrimeraHoja(ZipArchive pZip)
        {
            XDocument libro = AbrirXml(pZip, "xl/workbook.xml");
            XDocument relaciones = AbrirXml(pZip, "xl/_rels/workbook.xml.rels");
            XElement primera = libro?.Descendants(NS_HOJA + "sheet").FirstOrDefault();
            if (primera != null && relaciones != null)
            {
                string id = (string)primera.Attribute(NS_RELACION + "id");
                XElement relacion = relaciones.Root.Elements(NS_PAQUETE + "Relationship").FirstOrDefault(r => (string)r.Attribute("Id") == id);
                string destino = (string)relacion?.Attribute("Target");
                if (!string.IsNullOrEmpty(destino))
                {
                    return destino.StartsWith("/") ? destino.TrimStart('/') : "xl/" + destino;
                }
            }
            return "xl/worksheets/sheet1.xml";
        }
        private static string ValorCelda(XElement pCelda, List<string> pTextosCompartidos)
        {
            string tipo = (string)pCelda.Attribute("t");
            if (tipo == "inlineStr")
            {
                return string.Concat(pCelda.Descendants(NS_HOJA + "t").Select(t => t.Value)).Trim();
            }
            string valor = (string)pCelda.Element(NS_HOJA + "v") ?? "";
            if (tipo == "s")
            {
                int indice;
                return int.TryParse(valor, out indice) && indice >= 0 && indice < pTextosCompartidos.Count ? pTextosCompartidos[indice].Trim() : "";
            }
            return valor.Trim();
        }
        private static int IndiceColumna(string pReferencia)
        {
            int indice = 0;
            foreach (char c in pReferencia)
            {
                if (!char.IsLetter(c)) break;
                indice = indice * 26 + (char.ToUpperInvariant(c) - 'A' + 1);
            }
            return indice - 1;
        }
        private static XDocument AbrirXml(ZipArchive pZip, string pRuta)
        {
            ZipArchiveEntry entrada = pZip.Entries.FirstOrDefault(e => string.Equals(e.FullName, pRuta, StringComparison.OrdinalIgnoreCase));
            if (entrada == null) return null;
            using (Stream flujo = entrada.Open())
            {
                return XDocument.Load(flujo);
            }
        }
        #endregion
    }
}
