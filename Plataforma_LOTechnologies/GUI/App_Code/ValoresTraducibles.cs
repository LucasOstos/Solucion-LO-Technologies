using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de ValoresTraducibles
/// </summary>
public static class ValoresTraducibles
{
    private static readonly Dictionary<string, string> claves = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        // Estados
        { "Activo", "comun.activo" },
        { "Inactivo", "comun.inactivo" },
        { "Borrador", "valor.borrador" },
        // Tipo de local
        { "Local a la calle", "valor.localCalle" },
        { "Shopping", "valor.shopping" },
        { "Strip Center", "valor.stripCenter" },
        { "Otro", "valor.otro" },
        // Tipo de sector
        { "Frutería", "valor.fruteria" },
        { "Carnicería", "valor.carniceria" },
        { "Lácteos", "valor.lacteos" },
        { "Panadería", "valor.panaderia" },
        { "Bebidas", "valor.bebidas" },
        { "Almacén", "valor.almacen" },
        { "Limpieza", "valor.limpieza" },
        { "Perfumería", "valor.perfumeria" },
        { "Electrónica", "valor.electronica" },
        { "Indumentaria", "valor.indumentaria" },
        // Ubicación del sector
        { "Frente - Izquierda", "valor.frenteIzquierda" },
        { "Frente - Centro", "valor.frenteCentro" },
        { "Frente - Derecha", "valor.frenteDerecha" },
        { "Centro - Izquierda", "valor.centroIzquierda" },
        { "Centro - Centro", "valor.centroCentro" },
        { "Centro - Derecha", "valor.centroDerecha" },
        { "Fondo - Izquierda", "valor.fondoIzquierda" },
        { "Fondo - Centro", "valor.fondoCentro" },
        { "Fondo - Derecha", "valor.fondoDerecha" },
        // Carga de datos y estado del stock
        { "Venta", "valor.venta" },
        { "Stock", "valor.stock" },
        { "Crítico", "valor.critico" },
        { "Bajo", "valor.bajo" },
        { "Normal", "valor.normal" }
    };
    public static string ObtenerClave(string pValor)
    {
        string clave;
        return pValor != null && claves.TryGetValue(pValor.Trim(), out clave) ? clave : null;
    }
}