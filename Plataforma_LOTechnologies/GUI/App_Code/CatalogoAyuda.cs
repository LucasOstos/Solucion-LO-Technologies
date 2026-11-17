using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BE;

/// <summary>
/// Descripción breve de CatalogoAyuda
/// </summary>
public static class CatalogoAyuda
{
    // Tema general que aparece en todas las pantallas
    public static readonly TemaAyuda PRIMEROS_PASOS = new TemaAyuda("general.primerosPasos", "&#9733;", null, true);

    private static readonly Dictionary<string, PantallaAyuda> PANTALLAS = new Dictionary<string, PantallaAyuda>(StringComparer.OrdinalIgnoreCase)
    {
        {
            "principal", Pantalla("principal", "ayuda.principal.pantalla", "PaginaPrincipal.aspx", null, new[] { "locales", "indicadores" },
                new TemaAyuda("principal.modulos", "&#9638;", ".principal-seccion", false, "principal.sinModulo"),
                new TemaAyuda("principal.idioma", "&#127760;", "#ddlIdioma", false),
                new TemaAyuda("principal.contrasenia", "&#128273;", "#tbContraseniaActual|#btnAbrirCambiarContrasenia", false),
                new TemaAyuda("principal.sinModulo", "?", ".principal-seccion", false, "principal.modulos"))
        },
        {
            "locales", Pantalla("locales", "principal.locales.titulo", "PaginaLocales.aspx", PermisoNombres.GestionarLocales, new[] { "layout", "productos" },
                new TemaAyuda("locales.crear", "+", "#tbNombre|#btnNuevoLocal", true, "locales.datos", "locales.errores"),
                new TemaAyuda("locales.datos", "&#9745;", ".loc-datos", false, "locales.crear"),
                new TemaAyuda("locales.editar", "&#9998;", ".loc-acciones", false, "locales.buscar"),
                new TemaAyuda("locales.buscar", "&#128269;", "#tbBuscar", false),
                new TemaAyuda("locales.errores", "!", "#lblMensajeModal|#tbNombre|#lblMensaje|#btnNuevoLocal", false, "locales.crear"))
        },
        {
            "layout", Pantalla("layout", "principal.layout.titulo", "PaginaLayoutSectores.aspx", PermisoNombres.GestionarLayoutSectores, new[] { "locales", "productos", "escenarios" },
                new TemaAyuda("layout.cargar", "&#8679;", "#fuArchivoLayout|#btnCargarLayout", true, "layout.vigente", "layout.errores"),
                new TemaAyuda("layout.vigente", "&#9873;", "#panelLayout|#tabLayout", false, "layout.cargar"),
                new TemaAyuda("layout.sectores", "+", "#tbNombreSector|#btnNuevoSector|#tabSectores", false, "layout.circulacion"),
                new TemaAyuda("layout.circulacion", "&#8644;", "#tbCirculacionSector|#pnlMapaSectores|#tabSectores", false, "layout.sectores"),
                new TemaAyuda("layout.errores", "!", "#fuArchivoLayout|#lblMensaje|#btnCargarLayout", false, "layout.cargar"))
        },
        {
            "productos", Pantalla("productos", "principal.productos.titulo", "PaginaProductos.aspx", PermisoNombres.GestionarProductos, new[] { "layout", "stock" },
                new TemaAyuda("productos.categorias", "&#9776;", "#tbNombreCategoria|#btnNuevaCategoria|#tabCategorias", false, "productos.crear"),
                new TemaAyuda("productos.crear", "+", "#tbCosto|#btnNuevo", true, "productos.categorias", "productos.ubicar"),
                new TemaAyuda("productos.ubicar", "&#8982;", "#ddlSectorProducto|#ddlFiltroSector|#tabProductos", false, "productos.crear"),
                new TemaAyuda("productos.estado", "&#9998;", "#panelProductos|#tabProductos", false, "productos.ubicar"))
        },
        {
            "stock", Pantalla("stock", "principal.stockVentas.titulo", "PaginaStockVentas.aspx", PermisoNombres.CargarStockVentas, new[] { "productos", "indicadores" },
                new TemaAyuda("stock.importar", "&#8679;", "#fuArchivo|#btnImportar", true, "stock.errores", "stock.deshacer"),
                new TemaAyuda("stock.manual", "&#9998;", "#ddlTipoCarga|#btnCargaManual", false, "stock.importar"),
                new TemaAyuda("stock.consultar", "&#9636;", "#ddlPeriodo|#tabVentas", false, "stock.deshacer"),
                new TemaAyuda("stock.deshacer", "&#8630;", "#panelImportaciones|#tabImportaciones", false, "stock.importar"),
                new TemaAyuda("stock.errores", "!", "#pnlErroresImportacion|#fuArchivo|#lblMensaje|#btnImportar", false, "stock.importar"))
        },
        {
            "escenarios", Pantalla("escenarios", "principal.escenarios.titulo", "PaginaEscenarios.aspx", PermisoNombres.GestionarEscenarios, new[] { "comparacion", "indicadores" },
                new TemaAyuda("escenarios.queEs", "&#9432;", null, false, "escenarios.crear", "escenarios.supuestos"),
                new TemaAyuda("escenarios.crear", "+", ".esc-cambios-sectores|#btnNuevoEscenario", true, "escenarios.supuestos", "escenarios.errores"),
                new TemaAyuda("escenarios.variaciones", "&#9636;", "#pnlSituacionActual|.esc-stats", false, "escenarios.queEs"),
                new TemaAyuda("escenarios.estado", "&#9873;", ".esc-estado-detalle|.esc-tarjeta-botones", true, "escenarios.eliminar"),
                new TemaAyuda("escenarios.supuestos", "&#8721;", ".esc-supuestos", false, "escenarios.variaciones"),
                new TemaAyuda("escenarios.eliminar", "&#10005;", ".esc-tarjeta-acciones", false, "escenarios.estado"),
                new TemaAyuda("escenarios.errores", "!", "#lblMensajeModal|#tbNombreEscenario|#lblMensaje|#btnNuevoEscenario", false, "escenarios.crear"))
        },
        {
            "comparacion", Pantalla("comparacion", "principal.comparacion.titulo", "PaginaComparacionEscenarios.aspx", PermisoNombres.CompararEscenarios, new[] { "escenarios", "reportes" },
                new TemaAyuda("comparacion.comparar", "&#8646;", "#ddlEscenarioB|#btnComparar", false, "comparacion.resultados", "comparacion.errores"),
                new TemaAyuda("comparacion.resultados", "&#9636;", ".ce-conclusion|#pnlResultados", false, "comparacion.reporte"),
                new TemaAyuda("comparacion.reporte", "&#128196;", "#btnGuardarComparacion", false, "comparacion.resultados"),
                new TemaAyuda("comparacion.errores", "!", "#lblMensaje|#btnComparar", false, "comparacion.comparar"))
        },
        {
            "indicadores", Pantalla("indicadores", "principal.indicadores.titulo", "PaginaIndicadores.aspx", PermisoNombres.VisualizarIndicadores, new[] { "escenarios", "analisis", "reportes" },
                new TemaAyuda("indicadores.leer", "&#9636;", ".ind-como-se-calcula|#pnlResumenLocal", false, "indicadores.faltanDatos"),
                new TemaAyuda("indicadores.recomendaciones", "&#10148;", ".ind-recomendacion-acciones|#lblSinRecomendaciones", false, "indicadores.leer"),
                new TemaAyuda("indicadores.mapa", "&#9638;", "#ddlIndicadorMapa|#pnlMapa", false, "indicadores.leer"),
                new TemaAyuda("indicadores.faltanDatos", "!", "#lblSinCostos|#lblSinSuperficie|#lblSinIndicadores|#lblSinRecomendaciones", false, "indicadores.leer"))
        },
        {
            "analisis", Pantalla("analisis", "principal.analisisProductos.titulo", "PaginaAnalisisProductos.aspx", PermisoNombres.AnalizarProductos, new[] { "productos", "escenarios", "stock" },
                new TemaAyuda("analisis.analizar", "&#9881;", "#btnDetectar", false, "analisis.tipos"),
                new TemaAyuda("analisis.tipos", "&#9432;", ".ap-como-se-detecta", false, "analisis.acciones"),
                new TemaAyuda("analisis.acciones", "&#10148;", "#ddlFiltroTipo|#btnDetectar", true, "analisis.tipos"))
        },
        {
            "reportes", Pantalla("reportes", "principal.reportes.titulo", "PaginaReportes.aspx", PermisoNombres.GenerarReportes, new[] { "indicadores", "comparacion" },
                new TemaAyuda("reportes.generar", "&#128196;", "#btnGenerarReporte", true, "reportes.tipos", "reportes.errores"),
                new TemaAyuda("reportes.tipos", "&#9776;", "#ddlTipoReporte", false, "reportes.generar"),
                new TemaAyuda("reportes.recientes", "&#8681;", ".rep-item-acciones|#lblSinReportes", false),
                new TemaAyuda("reportes.errores", "!", "#lblMensaje|#btnGenerarReporte", false, "reportes.generar"))
        },
        {
            "usuarios", Pantalla("usuarios", "principal.usuarios.titulo", "PaginaUsuarios.aspx", PermisoNombres.GestionarUsuarios, new[] { "permisos", "empresas" },
                new TemaAyuda("usuarios.crear", "+", "#tbDNI|#btnAgregarUsuario", true, "usuarios.errores", "usuarios.desbloquear"),
                new TemaAyuda("usuarios.estado", "&#9998;", "td.acciones|#tbBuscar", false, "usuarios.desbloquear"),
                new TemaAyuda("usuarios.desbloquear", "&#128275;", ".etiqueta-bloqueado|td.acciones", false, "usuarios.estado"),
                new TemaAyuda("usuarios.errores", "!", "#lbMensajeModal|#tbDNI|#lbMensaje|#btnAgregarUsuario", false, "usuarios.crear"))
        },
        {
            "permisos", Pantalla("permisos", "principal.permisos.titulo", "PaginaPermisos.aspx", PermisoNombres.GestionarPermisos, new[] { "usuarios" },
                new TemaAyuda("permisos.conceptos", "&#9432;", ".perm-columnas", false, "permisos.crear"),
                new TemaAyuda("permisos.crear", "+", "#tbNombrePerfil", false, "permisos.modificar"),
                new TemaAyuda("permisos.modificar", "&#9998;", "#ddlPerfiles", false, "permisos.errores"),
                new TemaAyuda("permisos.errores", "!", "#lblMensaje|#ddlPerfiles", false, "permisos.conceptos"))
        },
        {
            "empresas", Pantalla("empresas", "principal.empresas.titulo", "PaginaEmpresas.aspx", PermisoNombres.GestionarEmpresas, new[] { "usuarios", "locales" },
                new TemaAyuda("empresas.crear", "+", "#tbCUIT|#btnNuevaEmpresa", false, "empresas.errores"),
                new TemaAyuda("empresas.estado", "&#9998;", "td.acciones|#tbBuscar", false, "empresas.crear"),
                new TemaAyuda("empresas.errores", "!", "#lblMensajeModal|#tbCUIT|#lblMensaje|#btnNuevaEmpresa", false, "empresas.crear"))
        },
        {
            "idiomas", Pantalla("idiomas", "principal.idiomas.titulo", "PaginaIdiomas.aspx", PermisoNombres.GestionarIdiomas, null,
                new TemaAyuda("idiomas.agregar", "+", "#ddlNuevoIdioma|#btnAgregarIdioma", false, "idiomas.activar"),
                new TemaAyuda("idiomas.activar", "&#10003;", ".idi-tarjeta-footer", false, "idiomas.agregar"),
                new TemaAyuda("idiomas.sincronizar", "&#8635;", "#btnSincronizar", false, "idiomas.activar"))
        },
        {
            "traducciones", Pantalla("traducciones", "idiomas.editarTraducciones", "PaginaEditarTraducciones.aspx", PermisoNombres.GestionarIdiomas, new[] { "idiomas" },
                new TemaAyuda("traducciones.editar", "&#9998;", "#ddlModulo|#btnGuardar", true, "traducciones.marcadores"),
                new TemaAyuda("traducciones.marcadores", "!", "#lblMensaje|#btnGuardar", false, "traducciones.editar"))
        },
        {
            "auditoria", Pantalla("auditoria", "principal.bitacora.titulo", "PaginaAuditorias.aspx", PermisoNombres.ConsultarBitacora, new[] { "backup", "usuarios" },
                new TemaAyuda("auditoria.filtrar", "&#128269;", "#tbFechaDesde|#btnFiltrar", false, "auditoria.niveles"),
                new TemaAyuda("auditoria.niveles", "&#9888;", "#ddlCriticidad", false, "auditoria.filtrar"),
                new TemaAyuda("auditoria.exportar", "&#8681;", "#btnExportarLog", false, "auditoria.filtrar"))
        },
        {
            "backup", Pantalla("backup", "principal.backup.titulo", "PaginaBackupRestore.aspx", PermisoNombres.RealizarBackupRestore, new[] { "auditoria" },
                new TemaAyuda("backup.crear", "+", "#btnCrearBackup", false, "backup.restaurar"),
                new TemaAyuda("backup.restaurar", "&#8630;", "#tbConfirmarRestauracion|.tabla .boton-icono|#btnCrearBackup", false, "backup.crear"))
        },
        {
            "integridad", SinPrimerosPasos(Pantalla("integridad", "integridad.encabezado", "PaginaDigitosVerificadores.aspx", PermisoNombres.RecalcularDigitos, new[] { "backup", "auditoria" },
                new TemaAyuda("integridad.queEs", "&#9432;", ".dv-tarjeta", false, "integridad.opciones"),
                new TemaAyuda("integridad.opciones", "&#9881;", ".dv-acciones", false, "integridad.queEs")))
        },
        {
            "login", SinPrimerosPasos(Pantalla("login", "ayuda.login.pantalla", "PaginaLogin.aspx", null, null,
                new TemaAyuda("login.ingresar", "&#128100;", "#tbEmail", false, "login.errores"),
                new TemaAyuda("login.olvide", "&#128273;", "#linkOlvideContrasenia", false, "login.errores"),
                new TemaAyuda("login.errores", "!", "#lbError|#btnIngresar", false, "login.olvide")))
        },
        {
            "recuperar", SinPrimerosPasos(Pantalla("recuperar", "ayuda.recuperar.pantalla", "RecuperarContrasenia.aspx", null, null,
                new TemaAyuda("recuperar.nueva", "&#128273;", "#tbNuevaContrasenia|#lbError", false)))
        }
    };
    private static PantallaAyuda SinPrimerosPasos(PantallaAyuda pPantalla)
    {
        pPantalla.ConPrimerosPasos = false;
        return pPantalla;
    }

    private static PantallaAyuda Pantalla(string pCodigo, string pClaveTitulo, string pUrl, string pPermiso, string[] pRelacionadas, params TemaAyuda[] pTemas)
    {
        return new PantallaAyuda
        {
            Codigo = pCodigo,
            ClaveTitulo = pClaveTitulo,
            Url = pUrl,
            Permiso = pPermiso,
            Temas = new List<TemaAyuda>(pTemas),
            Relacionadas = pRelacionadas ?? new string[0]
        };
    }

    public static PantallaAyuda ObtenerPantalla(string pCodigo)
    {
        PantallaAyuda pantalla;
        return !string.IsNullOrEmpty(pCodigo) && PANTALLAS.TryGetValue(pCodigo, out pantalla) ? pantalla : null;
    }
    // Todos los temas que se cargan en el panel de una pantalla: los propios y el general
    public static List<TemaAyuda> TemasDelPanel(PantallaAyuda pPantalla)
    {
        var temas = new List<TemaAyuda>();
        if (pPantalla != null) temas.AddRange(pPantalla.Temas);
        if (pPantalla == null || pPantalla.ConPrimerosPasos) temas.Add(PRIMEROS_PASOS);
        return temas;
    }
}
public class TemaAyuda
{
    public string Clave { get; private set; }
    public string Icono { get; private set; }
    // Selectores CSS de lo que marca "Mostrármelo en la pantalla", separados por "|": se usa el primero visible
    public string Resaltar { get; private set; }
    public string[] Relacionados { get; private set; }
    public bool TieneConsejo { get; private set; }

    public TemaAyuda(string pClave, string pIcono, string pResaltar, bool pTieneConsejo, params string[] pRelacionados)
    {
        Clave = pClave;
        Icono = pIcono;
        Resaltar = pResaltar;
        TieneConsejo = pTieneConsejo;
        Relacionados = pRelacionados ?? new string[0];
    }
    public string ClaveTitulo { get { return "ayuda." + Clave + ".titulo"; } }
    public string ClaveCuerpo { get { return "ayuda." + Clave + ".cuerpo"; } }
    public string ClavePasos { get { return "ayuda." + Clave + ".pasos"; } }
    public string ClaveConsejo { get { return "ayuda." + Clave + ".consejo"; } }
}
public class PantallaAyuda
{
    public string Codigo { get; set; }
    public string ClaveTitulo { get; set; }      // nombre de la pantalla (el mismo del menú principal)
    public string Url { get; set; }
    public string Permiso { get; set; }          // para no ofrecer como relacionada una pantalla sin acceso
    public List<TemaAyuda> Temas { get; set; }
    public string[] Relacionadas { get; set; }   // códigos de otras pantallas
    // false en las pantallas fuera del uso diario (inicio de sesión, integridad): no muestran la guía "Primeros pasos"
    public bool ConPrimerosPasos { get; set; } = true;
}