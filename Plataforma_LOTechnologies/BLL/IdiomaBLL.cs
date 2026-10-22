using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BE;
using DAL;
using SERVICIO.Logica;

namespace BLL
{
    public enum ResultadoAgregarIdioma { Exitoso, NoEstaEnCatalogo, YaExiste }
    public enum ResultadoCambioEstadoIdioma { Exitoso, NoExiste, EsPredeterminado, EnUso }

    public class ResultadoSincronizacion
    {
        public int ClavesAgregadas { get; set; }
        public int ClavesPendientes { get; set; }
        public int IdiomasActualizados { get; set; }
    }
    public class ResultadoGuardarTraducciones
    {
        public int Modificadas { get; set; }
        public List<string> ClavesVacias { get; } = new List<string>();
        public List<string> ClavesMuyLargas { get; } = new List<string>();
        public List<string> ClavesMarcadores { get; } = new List<string>();
        public bool Exitoso { get { return ClavesVacias.Count == 0 && ClavesMuyLargas.Count == 0 && ClavesMarcadores.Count == 0; } }
    }
    public class IdiomaBLL
    {
        private readonly IdiomaDAL idiomaDAL = new IdiomaDAL();
        private readonly TraduccionDAL traduccionDAL = new TraduccionDAL();
        private readonly DigitoVerificador digitos = new DigitoVerificador();
        private readonly CatalogoTraducciones catalogo = CatalogoTraducciones.Instancia;
        public List<Idioma> ObtenerIdiomas()
        {
            ControlAcceso.ValidarSuperUsuario();
            return idiomaDAL.ListarIdiomasConCompletitud();
        }
        public List<Idioma> ObtenerIdiomasParaAgregar()
        {
            ControlAcceso.ValidarSuperUsuario();
            HashSet<string> cargados = new HashSet<string>(idiomaDAL.ListarIdiomas().Select(i => i.Codigo), StringComparer.OrdinalIgnoreCase);
            return catalogo.ObtenerIdiomas().Where(i => !cargados.Contains(i.Codigo)).ToList();
        }
        public ResultadoAgregarIdioma AgregarIdioma(string pCodigo, out int pPendientes)
        {
            ControlAcceso.ValidarSuperUsuario();
            pPendientes = 0;
            Idioma idioma = catalogo.ObtenerIdiomas().FirstOrDefault(i => string.Equals(i.Codigo, pCodigo, StringComparison.OrdinalIgnoreCase));
            if (idioma == null) return ResultadoAgregarIdioma.NoEstaEnCatalogo;
            List<Idioma> cargados = idiomaDAL.ListarIdiomas();
            if (cargados.Any(i => string.Equals(i.Codigo, idioma.Codigo, StringComparison.OrdinalIgnoreCase))) return ResultadoAgregarIdioma.YaExiste;
            Idioma predeterminado = cargados.First(i => i.Predeterminado);
            Dictionary<string, string> textosBase = traduccionDAL.ObtenerTextos(predeterminado.CodigoIdioma);
            Dictionary<string, string> textosCatalogo = catalogo.ObtenerTextos(idioma.Codigo);
            List<Traduccion> traducciones = new List<Traduccion>();
            foreach (KeyValuePair<string, string> claveBase in textosBase.OrderBy(c => c.Key))
            {
                string texto;
                bool encontrada = textosCatalogo.TryGetValue(claveBase.Key, out texto);
                traducciones.Add(new Traduccion(0, 0, claveBase.Key, ObtenerModulo(claveBase.Key), encontrada ? texto : claveBase.Value, !encontrada));
            }
            pPendientes = traducciones.Count(t => t.Pendiente);
            idioma.Estado = false;
            List<int> codigosTraduccion;
            int codigoIdioma = idiomaDAL.AgregarConTraducciones(idioma, traducciones, out codigosTraduccion);
            digitos.ActualizarDigitoFila("Idioma", codigoIdioma);
            digitos.ActualizarDigitoFilas("Traduccion", codigosTraduccion.Cast<object>());
            RegistrarSuceso($"Agregar idioma {idioma.Nombre} ({traducciones.Count} claves, {pPendientes} pendientes)", 1);
            GestorIdioma.Instancia.RecargarIdiomas();
            return ResultadoAgregarIdioma.Exitoso;
        }
        public ResultadoCambioEstadoIdioma CambiarEstado(int pCodigoIdioma, bool pActivar, out int pUsuarios, out int pPendientes)
        {
            ControlAcceso.ValidarSuperUsuario();
            pUsuarios = 0;
            pPendientes = 0;
            Idioma idioma = idiomaDAL.ListarIdiomas().FirstOrDefault(i => i.CodigoIdioma == pCodigoIdioma);
            if (idioma == null) return ResultadoCambioEstadoIdioma.NoExiste;
            if (!pActivar)
            {
                if (idioma.Predeterminado) return ResultadoCambioEstadoIdioma.EsPredeterminado;
                pUsuarios = idiomaDAL.ContarUsuarios(pCodigoIdioma);
                if (pUsuarios > 0) return ResultadoCambioEstadoIdioma.EnUso;
            }
            else
            {
                pPendientes = traduccionDAL.ContarPendientes(pCodigoIdioma);
            }
            if (idioma.Estado == pActivar) return ResultadoCambioEstadoIdioma.Exitoso;
            idiomaDAL.CambiarEstado(pCodigoIdioma, pActivar);
            digitos.ActualizarDigitoFila("Idioma", pCodigoIdioma);
            RegistrarSuceso($"{(pActivar ? "Activar" : "Desactivar")} idioma {idioma.Nombre}", 1);
            GestorIdioma.Instancia.RecargarIdiomas();
            return ResultadoCambioEstadoIdioma.Exitoso;
        }
        public ResultadoSincronizacion SincronizarClaves()
        {
            ControlAcceso.ValidarSuperUsuario();
            ResultadoSincronizacion resultado = new ResultadoSincronizacion();
            Dictionary<string, string> textosBase = catalogo.ObtenerTextos(CatalogoTraducciones.CODIGO_BASE);
            List<Traduccion> nuevas = new List<Traduccion>();
            foreach (Idioma idioma in idiomaDAL.ListarIdiomas())
            {
                HashSet<string> existentes = new HashSet<string>(traduccionDAL.ObtenerTextos(idioma.CodigoIdioma).Keys);
                List<string> faltantes = textosBase.Keys.Where(c => !existentes.Contains(c)).OrderBy(c => c).ToList();
                if (faltantes.Count == 0) continue;

                Dictionary<string, string> textosIdioma = idioma.Predeterminado ? textosBase : catalogo.ObtenerTextos(idioma.Codigo);
                foreach (string clave in faltantes)
                {
                    string texto;
                    bool encontrada = textosIdioma.TryGetValue(clave, out texto);
                    nuevas.Add(new Traduccion(0, idioma.CodigoIdioma, clave, ObtenerModulo(clave), encontrada ? texto : textosBase[clave], !encontrada));
                }
                resultado.IdiomasActualizados++;
            }
            if (nuevas.Count == 0) return resultado;
            List<int> codigos = traduccionDAL.Agregar(nuevas);
            digitos.ActualizarDigitoFilas("Traduccion", codigos.Cast<object>());
            resultado.ClavesAgregadas = nuevas.Count;
            resultado.ClavesPendientes = nuevas.Count(t => t.Pendiente);
            RegistrarSuceso($"Sincronizar claves de idiomas ({resultado.ClavesAgregadas} agregadas, {resultado.ClavesPendientes} pendientes)", 1);
            GestorIdioma.Instancia.RecargarIdiomas();
            return resultado;
        }

        public const int LARGO_MAXIMO_TEXTO = 1000;
        private static readonly Regex MARCADOR = new Regex(@"\{(\d+)[^{}]*\}");
        public Idioma ObtenerIdioma(int pCodigoIdioma)
        {
            ControlAcceso.ValidarSuperUsuario();
            return idiomaDAL.ListarIdiomas().FirstOrDefault(i => i.CodigoIdioma == pCodigoIdioma);
        }
        public List<TraduccionComparada> ObtenerTraducciones(int pCodigoIdioma)
        {
            ControlAcceso.ValidarSuperUsuario();
            Idioma predeterminado = idiomaDAL.ListarIdiomas().First(i => i.Predeterminado);
            return traduccionDAL.ObtenerComparadas(pCodigoIdioma, predeterminado.CodigoIdioma);
        }
        public List<TraduccionComparada> FiltrarTraducciones(List<TraduccionComparada> pTraducciones, string pBusqueda, string pModulo, bool pSoloPendientes)
        {
            string busqueda = (pBusqueda ?? "").Trim();
            return pTraducciones.Where(t => (string.IsNullOrEmpty(pModulo) || t.Modulo == pModulo) && (!pSoloPendientes || t.Pendiente)
                   && (busqueda.Length == 0 || t.Clave.IndexOf(busqueda, StringComparison.OrdinalIgnoreCase) >= 0 || t.TextoBase.IndexOf(busqueda, StringComparison.OrdinalIgnoreCase) >= 0
                   || t.TextoTraducido.IndexOf(busqueda, StringComparison.OrdinalIgnoreCase) >= 0)).ToList();
        }
        public ResultadoGuardarTraducciones GuardarTraducciones(int pCodigoIdioma, List<Traduccion> pEditadas)
        {
            ControlAcceso.ValidarSuperUsuario();
            ResultadoGuardarTraducciones resultado = new ResultadoGuardarTraducciones();
            Idioma idioma = idiomaDAL.ListarIdiomas().FirstOrDefault(i => i.CodigoIdioma == pCodigoIdioma);
            if (idioma == null) return resultado;

            Dictionary<int, TraduccionComparada> actuales = ObtenerTraducciones(pCodigoIdioma).ToDictionary(t => t.CodigoTraduccion);
            List<Traduccion> cambios = new List<Traduccion>();
            foreach (Traduccion editada in pEditadas)
            {
                TraduccionComparada actual;
                if (!actuales.TryGetValue(editada.CodigoTraduccion, out actual)) continue;
                string texto = (editada.Texto ?? "").Trim();
                bool pendiente = !idioma.Predeterminado && editada.Pendiente;
                if (texto == actual.TextoTraducido && pendiente == actual.Pendiente) continue;
                if (texto.Length == 0) { resultado.ClavesVacias.Add(actual.Clave); continue; }
                if (texto.Length > LARGO_MAXIMO_TEXTO) { resultado.ClavesMuyLargas.Add(actual.Clave); continue; }
                if (!idioma.Predeterminado && !MismosMarcadores(actual.TextoBase, texto)) { resultado.ClavesMarcadores.Add(actual.Clave); continue; }
                cambios.Add(new Traduccion(actual.CodigoTraduccion, pCodigoIdioma, actual.Clave, actual.Modulo, texto, pendiente));
            }
            if (!resultado.Exitoso || cambios.Count == 0) return resultado;

            traduccionDAL.ActualizarTextos(pCodigoIdioma, cambios);
            digitos.ActualizarDigitoFilas("Traduccion", cambios.Select(c => (object)c.CodigoTraduccion));
            Traductor.Instancia.Invalidar(pCodigoIdioma);
            resultado.Modificadas = cambios.Count;
            RegistrarSuceso($"Editar traducciones de {idioma.Nombre} ({cambios.Count} modificadas)", 1);
            return resultado;
        }
        private static bool MismosMarcadores(string pTextoBase, string pTexto)
        {
            Func<string, string> marcadores = t => string.Join(",", MARCADOR.Matches(t ?? "").Cast<Match>()
                .Select(m => m.Groups[1].Value).Distinct().OrderBy(v => v));
            return marcadores(pTextoBase) == marcadores(pTexto);
        }
        private static string ObtenerModulo(string pClave)
        {
            int punto = pClave.IndexOf('.');
            return punto > 0 ? pClave.Substring(0, punto) : "general";
        }
        private void RegistrarSuceso(string pDescripcion, int pCriticidad)
        {
            Usuario usuario = Sesion.Instancia.Usuario;
            SucesoServicio.Instancia.RegistrarSuceso($"{usuario.Nombre} {usuario.Apellido}", pDescripcion, "Idiomas", pCriticidad);
        }
    }
}
