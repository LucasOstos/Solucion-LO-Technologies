using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;
using DAL;
using SERVICIO.Logica;

namespace BLL
{
    public class CategoriaBLL
    {
        private CategoriaDAL categoriaDAL = new CategoriaDAL();
        private DigitoVerificador digitos = new DigitoVerificador();
        public List<CategoriaBE> ObtenerCategorias(int pCodigoLocal)
        {
            int empresa = ControlAcceso.ResolverEmpresaPorLocal(pCodigoLocal);
            return empresa > 0 ? categoriaDAL.ObtenerCategorias(empresa) : new List<CategoriaBE>();
        }
        public CategoriaBE ObtenerCategoriaPorCodigo(int pCodigoCategoria)
        {
            ControlAcceso.Validar(EntidadEmpresa.Categoria, pCodigoCategoria);
            return categoriaDAL.ObtenerCategoriaPorCodigo(pCodigoCategoria);
        }
        public void CrearCategoria(CategoriaBE pCategoria, int pCodigoLocal)
        {
            pCategoria.CodigoEmpresa = ControlAcceso.ResolverEmpresaPorLocal(pCodigoLocal);
            if (pCategoria.CodigoEmpresa <= 0) throw new ExcepcionTraducible("productos.errLocalCategoria");
            int codigoNuevo = categoriaDAL.AgregarCategoria(pCategoria);
            digitos.ActualizarDigitoFila("Categoria", codigoNuevo);
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "ABM de productos y/o categorías", "Gestión productos", 2);
        }
        public void ModificarCategoria(CategoriaBE pCategoria)
        {
            ControlAcceso.Validar(EntidadEmpresa.Categoria, pCategoria.CodigoCategoria);
            categoriaDAL.ModificarCategoria(pCategoria);
            digitos.ActualizarDigitoFila("Categoria", pCategoria.CodigoCategoria);
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "ABM de productos y/o categorías", "Gestión productos", 2);
        }
    }
}
