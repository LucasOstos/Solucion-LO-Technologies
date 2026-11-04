using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;
using DAL;
using SERVICIO.Logica;

namespace BLL
{
    public class ImportacionBLL
    {
        public const int MAXIMO_FILAS = 5000;
        public const int MAXIMO_BYTES = 2 * 1024 * 1024;

        private const string COL_CODIGO = "codigo";
        private const string COL_FECHA = "fecha";
        private const string COL_CANTIDAD = "cantidad";
        private const string COL_IMPORTE = "importe";
        private const string COL_MINIMO = "minimo";
        private const string COL_MAXIMO = "maximo";

        // Nombres aceptados para cada columna del encabezado (sin tildes, espacios ni mayúsculas)
        private static readonly Dictionary<string, string[]> ALIAS_COLUMNAS = new Dictionary<string, string[]>
        {
            { COL_CODIGO,   new[] { "codigo", "codigoproducto", "producto", "sku", "code", "productcode" } },
            { COL_FECHA,    new[] { "fecha", "fechaventa", "date" } },
            { COL_CANTIDAD, new[] { "cantidad", "unidades", "quantity", "qty" } },
            { COL_IMPORTE,  new[] { "importe", "importetotal", "monto", "total", "amount" } },
            { COL_MINIMO,   new[] { "minimo", "stockminimo", "min" } },
            { COL_MAXIMO,   new[] { "maximo", "stockmaximo", "max" } }
        };
        private static readonly string[] FORMATOS_FECHA =
        {
            "yyyy-MM-dd", "yyyy/MM/dd", "dd/MM/yyyy", "d/M/yyyy", "dd-MM-yyyy", "d-M-yyyy",
            "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy HH:mm", "yyyy-MM-ddTHH:mm:ss"
        };

        private ImportacionDAL importacionDAL = new ImportacionDAL();
        private DigitoVerificador digitos = new DigitoVerificador();

        public List<ImportacionBE> ObtenerImportacionesPorLocal(int pCodigoLocal)
        {
            ControlAcceso.Validar(EntidadEmpresa.Local, pCodigoLocal);
            return importacionDAL.ObtenerImportacionesPorLocal(pCodigoLocal, 30);
        }

        public ResultadoImportacion Importar(int pCodigoLocal, string pTipo, string pNombreArchivo, Stream pArchivo)
        {
            int codigoEmpresa = ControlAcceso.Validar(EntidadEmpresa.Local, pCodigoLocal);
            var resultado = new ResultadoImportacion();
            bool esVenta = pTipo == ImportacionBE.TIPO_VENTA;

            List<string[]> filas = LeerArchivo(pNombreArchivo, pArchivo, resultado);
            if (filas == null) return resultado;

            Dictionary<string, int> columnas = MapearColumnas(filas[0]);
            string[] obligatorias = esVenta ? new[] { COL_CODIGO, COL_FECHA, COL_CANTIDAD } : new[] { COL_CODIGO, COL_CANTIDAD };
            foreach (string columna in obligatorias.Where(c => !columnas.ContainsKey(c)))
            {
                resultado.Errores.Add(new ErrorImportacion(1, "importar.errFaltaColumna", columna));
            }
            if (resultado.Errores.Count > 0) return resultado;

            Dictionary<int, ProductoImportable> productos = importacionDAL.ObtenerProductosImportables(codigoEmpresa, pCodigoLocal);
            var ventas = new List<VentaBE>();
            var stocks = new List<StockBE>();
            var filaDeProducto = new Dictionary<int, int>(); // stock: un producto no puede venir dos veces
            for (int i = 1; i < filas.Count; i++)
            {
                int numeroFila = i + 1;
                string[] fila = filas[i];
                ProductoImportable producto = LeerProducto(Celda(fila, columnas, COL_CODIGO), productos, numeroFila, resultado);
                if (esVenta)
                {
                    VentaBE venta = LeerVenta(fila, columnas, producto, pCodigoLocal, numeroFila, resultado);
                    if (venta != null)
                    {
                        ventas.Add(venta);
                        if (!producto.UbicadoEnLocal) resultado.FilasSinUbicar++;
                    }
                }
                else
                {
                    StockBE stock = LeerStock(fila, columnas, producto, pCodigoLocal, numeroFila, resultado);
                    if (stock != null)
                    {
                        int filaAnterior;
                        if (filaDeProducto.TryGetValue(stock.CodigoProducto, out filaAnterior))
                        {
                            resultado.Errores.Add(new ErrorImportacion(numeroFila, "importar.errProductoRepetido", Celda(fila, columnas, COL_CODIGO), filaAnterior));
                            continue;
                        }
                        filaDeProducto[stock.CodigoProducto] = numeroFila;
                        stocks.Add(stock);
                    }
                }
            }
            if (resultado.Errores.Count > 0)
            {
                resultado.FilasSinUbicar = 0;
                return resultado;
            }

            Usuario usuario = Sesion.Instancia.Usuario;
            var importacion = new ImportacionBE
            {
                CodigoLocal = pCodigoLocal,
                Tipo = esVenta ? ImportacionBE.TIPO_VENTA : ImportacionBE.TIPO_STOCK,
                NombreArchivo = Path.GetFileName(pNombreArchivo),
                Fecha = DateTime.Now,
                Usuario = $"{usuario.Nombre} {usuario.Apellido}",
                CantidadFilas = esVenta ? ventas.Count : stocks.Count,
                Estado = ImportacionBE.ESTADO_APLICADA
            };
            var codigos = new List<int>();
            resultado.CodigoImportacion = esVenta ? importacionDAL.RegistrarVentas(importacion, ventas, codigos)
                                                  : importacionDAL.RegistrarStock(importacion, stocks, codigos);
            resultado.FilasImportadas = importacion.CantidadFilas;
            digitos.ActualizarDigitoFila("Importacion", resultado.CodigoImportacion);
            digitos.ActualizarDigitoFilas(esVenta ? "Venta" : "Stock", codigos.Cast<object>());
            SucesoServicio.Instancia.RegistrarSuceso(importacion.Usuario,
                $"Importar datos de {(esVenta ? "ventas" : "stock")} ({importacion.CantidadFilas} filas, {importacion.NombreArchivo})", "Gestión operaciones", 2);
            return resultado;
        }

        // Borra todas las ventas que entraron con una importación (por ejemplo, si el archivo se importó dos veces)
        public int DeshacerImportacion(int pCodigoImportacion)
        {
            ControlAcceso.Validar(EntidadEmpresa.Importacion, pCodigoImportacion);
            ImportacionBE importacion = importacionDAL.ObtenerImportacionPorCodigo(pCodigoImportacion);
            if (importacion == null || !importacion.PuedeDeshacer) throw new ExcepcionTraducible("importar.errNoDeshacer");
            int borradas = importacionDAL.DeshacerImportacion(pCodigoImportacion);
            digitos.ActualizarDigitoVertical("Venta");
            digitos.ActualizarDigitoFila("Importacion", pCodigoImportacion);
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}",
                $"Deshacer importación de ventas ({borradas} ventas, {importacion.NombreArchivo})", "Gestión operaciones", 2);
            return borradas;
        }

        #region Lectura y validación
        private static List<string[]> LeerArchivo(string pNombreArchivo, Stream pArchivo, ResultadoImportacion pResultado)
        {
            if (pArchivo == null || string.IsNullOrEmpty(pNombreArchivo))
            {
                pResultado.Errores.Add(new ErrorImportacion(0, "importar.errSinArchivo"));
                return null;
            }
            if (!LectorArchivoTabular.EsFormatoSoportado(pNombreArchivo))
            {
                pResultado.Errores.Add(new ErrorImportacion(0, "importar.errFormato"));
                return null;
            }
            if (pArchivo.CanSeek && pArchivo.Length > MAXIMO_BYTES)
            {
                pResultado.Errores.Add(new ErrorImportacion(0, "importar.errTamanio", MAXIMO_BYTES / (1024 * 1024)));
                return null;
            }
            List<string[]> filas;
            try
            {
                filas = LectorArchivoTabular.Leer(pArchivo, pNombreArchivo);
            }
            catch (ExcepcionTraducible error)
            {
                pResultado.Errores.Add(new ErrorImportacion(0, error.Clave, error.Parametros));
                return null;
            }
            if (filas.Count < 2)
            {
                pResultado.Errores.Add(new ErrorImportacion(0, "importar.errVacio"));
                return null;
            }
            if (filas.Count - 1 > MAXIMO_FILAS)
            {
                pResultado.Errores.Add(new ErrorImportacion(0, "importar.errMaximoFilas", MAXIMO_FILAS));
                return null;
            }
            return filas;
        }

        private static Dictionary<string, int> MapearColumnas(string[] pEncabezado)
        {
            var columnas = new Dictionary<string, int>();
            for (int i = 0; i < pEncabezado.Length; i++)
            {
                string nombre = Normalizar(pEncabezado[i]);
                foreach (var alias in ALIAS_COLUMNAS)
                {
                    if (!columnas.ContainsKey(alias.Key) && alias.Value.Contains(nombre)) columnas[alias.Key] = i;
                }
            }
            return columnas;
        }

        // Minúsculas, sin tildes, sin espacios ni guiones: "Código producto" -> "codigoproducto"
        private static string Normalizar(string pTexto)
        {
            string descompuesto = (pTexto ?? "").Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (char c in descompuesto)
            {
                if (char.IsLetterOrDigit(c) && CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark) sb.Append(c);
            }
            return sb.ToString();
        }

        private static string Celda(string[] pFila, Dictionary<string, int> pColumnas, string pColumna)
        {
            int indice;
            if (!pColumnas.TryGetValue(pColumna, out indice) || indice >= pFila.Length) return "";
            return (pFila[indice] ?? "").Trim();
        }

        private static ProductoImportable LeerProducto(string pCodigo, Dictionary<int, ProductoImportable> pProductos, int pFila, ResultadoImportacion pResultado)
        {
            int? numeroSerie = LeerNumeroSerie(pCodigo);
            ProductoImportable producto;
            if (numeroSerie == null)
            {
                pResultado.Errores.Add(new ErrorImportacion(pFila, "importar.errCodigo", pCodigo));
                return null;
            }
            if (!pProductos.TryGetValue(numeroSerie.Value, out producto))
            {
                pResultado.Errores.Add(new ErrorImportacion(pFila, "importar.errProductoNoExiste", pCodigo));
                return null;
            }
            if (!producto.Activo)
            {
                pResultado.Errores.Add(new ErrorImportacion(pFila, "importar.errProductoInactivo", pCodigo));
                return null;
            }
            return producto;
        }

        // Acepta "PROD-001", "prod-1", "001" o "1"
        public static int? LeerNumeroSerie(string pCodigo)
        {
            string codigo = (pCodigo ?? "").Trim().ToUpperInvariant();
            if (codigo.StartsWith("PROD-")) codigo = codigo.Substring(5);
            decimal numero;
            if (!decimal.TryParse(codigo, NumberStyles.Number, CultureInfo.InvariantCulture, out numero)) return null;
            if (numero <= 0 || numero != decimal.Truncate(numero) || numero > int.MaxValue) return null;
            return (int)numero;
        }

        private static VentaBE LeerVenta(string[] pFila, Dictionary<string, int> pColumnas, ProductoImportable pProducto, int pCodigoLocal, int pNumeroFila, ResultadoImportacion pResultado)
        {
            int erroresAntes = pResultado.Errores.Count;
            string textoFecha = Celda(pFila, pColumnas, COL_FECHA);
            DateTime? fecha = LeerFecha(textoFecha);
            if (fecha == null) pResultado.Errores.Add(new ErrorImportacion(pNumeroFila, "importar.errFecha", textoFecha));
            int? cantidad = LeerEntero(Celda(pFila, pColumnas, COL_CANTIDAD), COL_CANTIDAD, pNumeroFila, pResultado);
            string textoImporte = Celda(pFila, pColumnas, COL_IMPORTE);
            decimal? importe = null;
            if (textoImporte != "")
            {
                importe = LeerNumero(textoImporte);
                if (importe == null) pResultado.Errores.Add(new ErrorImportacion(pNumeroFila, "importar.errNumero", COL_IMPORTE, textoImporte));
            }
            if (pProducto == null || pResultado.Errores.Count > erroresAntes) return null;

            decimal importeFinal;
            string error = VentaBLL.ValidarVenta(fecha.Value, cantidad.Value, importe, pProducto.Precio, out importeFinal);
            if (error != null)
            {
                pResultado.Errores.Add(new ErrorImportacion(pNumeroFila, error));
                return null;
            }
            return VentaBLL.ArmarVenta(pCodigoLocal, pProducto.CodigoProducto, fecha.Value, cantidad.Value, importeFinal);
        }

        private static StockBE LeerStock(string[] pFila, Dictionary<string, int> pColumnas, ProductoImportable pProducto, int pCodigoLocal, int pNumeroFila, ResultadoImportacion pResultado)
        {
            int erroresAntes = pResultado.Errores.Count;
            int? cantidad = LeerEntero(Celda(pFila, pColumnas, COL_CANTIDAD), COL_CANTIDAD, pNumeroFila, pResultado);
            string textoMinimo = Celda(pFila, pColumnas, COL_MINIMO);
            string textoMaximo = Celda(pFila, pColumnas, COL_MAXIMO);
            int? minimo = textoMinimo == "" ? null : LeerEntero(textoMinimo, COL_MINIMO, pNumeroFila, pResultado);
            int? maximo = textoMaximo == "" ? null : LeerEntero(textoMaximo, COL_MAXIMO, pNumeroFila, pResultado);
            if (pProducto == null || pResultado.Errores.Count > erroresAntes) return null;

            string error = StockBLL.ValidarStock(cantidad.Value, minimo, maximo);
            if (error != null)
            {
                pResultado.Errores.Add(new ErrorImportacion(pNumeroFila, error));
                return null;
            }
            return new StockBE(0, pCodigoLocal, pProducto.CodigoProducto, cantidad.Value, minimo, maximo, DateTime.Now.ToString("yyyy-MM"), DateTime.Now);
        }

        private static int? LeerEntero(string pTexto, string pColumna, int pNumeroFila, ResultadoImportacion pResultado)
        {
            decimal? numero = LeerNumero(pTexto);
            if (numero == null || numero != decimal.Truncate(numero.Value) || Math.Abs(numero.Value) > int.MaxValue)
            {
                pResultado.Errores.Add(new ErrorImportacion(pNumeroFila, "importar.errNumero", pColumna, pTexto));
                return null;
            }
            return (int)numero.Value;
        }

        // Acepta "8400.5" (Excel / invariante), "8400,5" y "8.400,50" (formato argentino), "8,400.50" y "$ 8400"
        public static decimal? LeerNumero(string pTexto)
        {
            string texto = (pTexto ?? "").Replace("$", "").Replace(" ", "").Replace(" ", "").Trim();
            if (texto == "") return null;
            int ultimoPunto = texto.LastIndexOf('.');
            int ultimaComa = texto.LastIndexOf(',');
            if (ultimoPunto >= 0 && ultimaComa >= 0)
            {
                // El separador que aparece último es el decimal; el otro es de miles
                texto = ultimaComa > ultimoPunto ? texto.Replace(".", "").Replace(',', '.') : texto.Replace(",", "");
            }
            else if (ultimaComa >= 0)
            {
                texto = texto.Replace(',', '.');
            }
            else if (texto.Count(c => c == '.') > 1)
            {
                texto = texto.Replace(".", ""); // "1.234.567": solo puede ser separador de miles
            }
            decimal valor;
            if (decimal.TryParse(texto, NumberStyles.Float, CultureInfo.InvariantCulture, out valor)) return valor;
            return null;
        }

        // Fecha como texto (2026-10-15, 15/10/2026...) o como número de serie de Excel (días desde 1899-12-30)
        public static DateTime? LeerFecha(string pTexto)
        {
            string texto = (pTexto ?? "").Trim();
            if (texto == "") return null;
            DateTime fecha;
            if (DateTime.TryParseExact(texto, FORMATOS_FECHA, CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha)) return fecha.Date;
            double serie;
            if (texto.IndexOfAny(new[] { '-', '/' }) < 0
                && double.TryParse(texto, NumberStyles.Float, CultureInfo.InvariantCulture, out serie) && serie >= 1 && serie < 2958466)
            {
                return DateTime.FromOADate(serie).Date;
            }
            return null;
        }
        #endregion
    }
}
