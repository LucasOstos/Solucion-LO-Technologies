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
        public List<CategoriaBE> ObtenerCategorias()
        {
            return categoriaDAL.ObtenerCategorias();
        }
        public CategoriaBE ObtenerCategoriaPorCodigo(int pCodigoCategoria)
        {
            return categoriaDAL.ObtenerCategoriaPorCodigo(pCodigoCategoria);
        }
        public void CrearCategoria(CategoriaBE pCategoria)
        {
            int codigoNuevo = categoriaDAL.AgregarCategoria(pCategoria);
            digitos.ActualizarDigitoFila("Categoria", codigoNuevo);
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "ABM de productos y/o categorías", "Gestión productos", 2);
        }
        public void ModificarCategoria(CategoriaBE pCategoria)
        {
            categoriaDAL.ModificarCategoria(pCategoria);
            digitos.ActualizarDigitoFila("Categoria", pCategoria.CodigoCategoria);
            SucesoServicio.Instancia.RegistrarSuceso($"{Sesion.Instancia.Usuario.Nombre} {Sesion.Instancia.Usuario.Apellido}", "ABM de productos y/o categorías", "Gestión productos", 2);
        }
    }
}
