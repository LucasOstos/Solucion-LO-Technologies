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
    public class PermisoBLL
    {
        private PermisoDAL accesoDAL = new PermisoDAL();
        public PermisosDatos ObtenerEstructura()
        {
            PermisosDatos datos = accesoDAL.ObtenerEstructura();
            Dictionary<int, PermisoSimple> permisos = datos.Permisos.ToDictionary(p => p.Codigo);
            Dictionary<int, Familia> familias = datos.Familias.ToDictionary(f => f.Codigo);
            Dictionary<int, Perfil> perfiles = datos.Perfiles.ToDictionary(p => p.Codigo);
            // Primero las hojas y después los compuestos
            foreach (RelacionPermiso r in datos.PermisoFamilia) familias[r.CodigoPadre].Agregar(permisos[r.CodigoHijo]);
            foreach (RelacionPermiso r in datos.PermisoPerfil) perfiles[r.CodigoPadre].Agregar(permisos[r.CodigoHijo]);
            foreach (RelacionPermiso r in datos.FamiliaFamilia) familias[r.CodigoPadre].Agregar(familias[r.CodigoHijo]);
            foreach (RelacionPermiso r in datos.FamiliaPerfil) perfiles[r.CodigoPadre].Agregar(familias[r.CodigoHijo]);
            return datos;
        }
        public Perfil ObtenerPerfil(int pCodigoPerfil)
        {
            return ObtenerEstructura().Perfiles.FirstOrDefault(p => p.Codigo == pCodigoPerfil);
        }
        public bool EsPerfilAsignable(Perfil pPerfil)
        {
            if (pPerfil == null || !Sesion.Instancia.IsLogueado()) return false;
            return pPerfil.ObtenerPermisosSimples().Where(p => !PermisoNombres.EsGlobal(p.Nombre)).All(p => Sesion.Instancia.TienePermiso(p.Nombre));
        }
        public List<Perfil> ObtenerPerfilesAsignables()
        {
            return ObtenerEstructura().Perfiles.Where(EsPerfilAsignable).ToList();
        }
        public int CrearPerfil(string pNombre, List<int> pCodPermisos, List<int> pCodFamilias)
        {
            ControlAcceso.ValidarSuperUsuario();
            PermisosDatos estructura = ObtenerEstructura();
            string nombre = ValidarNombre(pNombre, estructura.Perfiles.Select(p => p.Nombre), "permisos.errPerfilExiste");
            List<Permiso> hijos = Resolver(estructura, pCodPermisos, pCodFamilias);
            if (hijos.Count == 0) throw new ExcepcionTraducible("permisos.errSinSeleccionPerfil");
            var nuevo = new Perfil(0, nombre);
            foreach (Permiso hijo in hijos) AgregarValidando(nuevo, hijo);
            int codigo = accesoDAL.CrearPerfil(nombre, hijos);
            RegistrarSuceso("Crear perfil");
            return codigo;
        }
        public void AgregarAPerfil(int pCodPerfil, List<int> pCodPermisos, List<int> pCodFamilias)
        {
            ControlAcceso.ValidarSuperUsuario();
            PermisosDatos estructura = ObtenerEstructura();
            Perfil perfil = BuscarPerfil(estructura, pCodPerfil);
            List<Permiso> hijos = Resolver(estructura, pCodPermisos, pCodFamilias);
            if (hijos.Count == 0) throw new ExcepcionTraducible("permisos.errSinSeleccionAgregar");
            foreach (Permiso hijo in hijos) AgregarValidando(perfil, hijo);
            accesoDAL.AgregarAPerfil(perfil.Codigo, hijos);
            ActualizarPerfilEnSesion(estructura);
            RegistrarSuceso("Modificar perfil");
        }
        public void QuitarDePerfil(int pCodPerfil, List<int> pCodPermisos, List<int> pCodFamilias)
        {
            ControlAcceso.ValidarSuperUsuario();
            PermisosDatos estructura = ObtenerEstructura();
            Perfil perfil = BuscarPerfil(estructura, pCodPerfil);
            List<Permiso> hijos = Resolver(estructura, pCodPermisos, pCodFamilias);
            if (hijos.Count == 0) throw new ExcepcionTraducible("permisos.errSinSeleccionQuitar");
            foreach (Permiso hijo in hijos) perfil.Quitar(hijo);
            ValidarNoAutoBloqueo(estructura);
            accesoDAL.QuitarDePerfil(perfil.Codigo, hijos);
            ActualizarPerfilEnSesion(estructura);
            RegistrarSuceso("Modificar perfil");
        }
        public void EliminarPerfil(int pCodPerfil)
        {
            ControlAcceso.ValidarSuperUsuario();
            PermisosDatos estructura = ObtenerEstructura();
            Perfil perfil = BuscarPerfil(estructura, pCodPerfil);
            accesoDAL.EliminarPerfil(perfil.Codigo);
            RegistrarSuceso("Eliminar perfil");
        }

        public int CrearFamilia(string pNombre, List<int> pCodPermisos, List<int> pCodFamilias)
        {
            ControlAcceso.ValidarSuperUsuario();
            PermisosDatos estructura = ObtenerEstructura();
            string nombre = ValidarNombre(pNombre, estructura.Familias.Select(f => f.Nombre), "permisos.errFamiliaExiste");
            List<Permiso> hijos = Resolver(estructura, pCodPermisos, pCodFamilias);
            if (hijos.Count == 0) throw new ExcepcionTraducible("permisos.errSinSeleccionFamilia");
            var nueva = new Familia(0, nombre);
            foreach (Permiso hijo in hijos) AgregarValidando(nueva, hijo);
            int codigo = accesoDAL.CrearFamilia(nombre, hijos);
            RegistrarSuceso("Crear familia");
            return codigo;
        }
        public void AgregarAFamilia(int pCodFamilia, List<int> pCodPermisos, List<int> pCodFamilias)
        {
            ControlAcceso.ValidarSuperUsuario();
            PermisosDatos estructura = ObtenerEstructura();
            Familia familia = BuscarFamilia(estructura, pCodFamilia);
            List<Permiso> hijos = Resolver(estructura, pCodPermisos, pCodFamilias);
            if (hijos.Count == 0) throw new ExcepcionTraducible("permisos.errSinSeleccionAgregar");
            foreach (Permiso hijo in hijos) AgregarValidando(familia, hijo);
            accesoDAL.AgregarAFamilia(familia.Codigo, hijos);
            ActualizarPerfilEnSesion(estructura);
            RegistrarSuceso("Modificar familia");
        }
        public void QuitarDeFamilia(int pCodFamilia, List<int> pCodPermisos, List<int> pCodFamilias)
        {
            ControlAcceso.ValidarSuperUsuario();
            PermisosDatos estructura = ObtenerEstructura();
            Familia familia = BuscarFamilia(estructura, pCodFamilia);
            List<Permiso> hijos = Resolver(estructura, pCodPermisos, pCodFamilias);
            if (hijos.Count == 0) throw new ExcepcionTraducible("permisos.errSinSeleccionQuitar");
            foreach (Permiso hijo in hijos) familia.Quitar(hijo);
            ValidarNoAutoBloqueo(estructura);
            accesoDAL.QuitarDeFamilia(familia.Codigo, hijos);
            ActualizarPerfilEnSesion(estructura);
            RegistrarSuceso("Modificar familia");
        }
        public void EliminarFamilia(int pCodFamilia)
        {
            ControlAcceso.ValidarSuperUsuario();
            PermisosDatos estructura = ObtenerEstructura();
            Familia familia = BuscarFamilia(estructura, pCodFamilia);
            List<string> usadaPor = estructura.Perfiles.Where(p => p.ObtenerHijos().Any(h => h.EsMismo(familia))).Select(p => $"'{p.Nombre}'")
                                    .Concat(estructura.Familias.Where(f => f.ObtenerHijos().Any(h => h.EsMismo(familia))).Select(f => $"'{f.Nombre}'")).ToList();
            if (usadaPor.Count > 0) throw new ExcepcionTraducible("permisos.errFamiliaEnUso", familia.Nombre, string.Join(", ", usadaPor));
            accesoDAL.EliminarFamilia(familia.Codigo);
            RegistrarSuceso("Eliminar familia");
        }
        private void AgregarValidando(PermisoCompuesto pDestino, Permiso pHijo)
        {
            if (!pHijo.EsMismo(pDestino) && pDestino.Contiene(pHijo)) throw new ExcepcionTraducible("permisos.errYaIncluido", pHijo.Nombre, pDestino.Nombre);
            pDestino.Agregar(pHijo);
        }
        private List<Permiso> Resolver(PermisosDatos pEstructura, List<int> pCodPermisos, List<int> pCodFamilias)
        {
            var resultado = new List<Permiso>();
            foreach (int codigo in pCodPermisos ?? new List<int>())
            {
                PermisoSimple permiso = pEstructura.Permisos.FirstOrDefault(p => p.Codigo == codigo);
                if (permiso == null) throw new ExcepcionTraducible("permisos.errPermisoNoExiste");
                resultado.Add(permiso);
            }
            foreach (int codigo in pCodFamilias ?? new List<int>())
            {
                Familia familia = pEstructura.Familias.FirstOrDefault(f => f.Codigo == codigo);
                if (familia == null) throw new ExcepcionTraducible("permisos.errFamiliaSelNoExiste");
                resultado.Add(familia);
            }
            return resultado;
        }
        private Perfil BuscarPerfil(PermisosDatos pEstructura, int pCodigo)
        {
            Perfil perfil = pEstructura.Perfiles.FirstOrDefault(p => p.Codigo == pCodigo);
            if (perfil == null) throw new ExcepcionTraducible("permisos.errPerfilNoExiste");
            return perfil;
        }
        private Familia BuscarFamilia(PermisosDatos pEstructura, int pCodigo)
        {
            Familia familia = pEstructura.Familias.FirstOrDefault(f => f.Codigo == pCodigo);
            if (familia == null) throw new ExcepcionTraducible("permisos.errFamiliaNoExiste");
            return familia;
        }
        private string ValidarNombre(string pNombre, IEnumerable<string> pExistentes, string pClaveYaExiste)
        {
            string nombre = (pNombre ?? "").Trim();
            if (nombre.Length == 0) throw new ExcepcionTraducible("permisos.errNombreVacio");
            if (nombre.Length > 50) throw new ExcepcionTraducible("permisos.errNombreLargo", 50);
            if (pExistentes.Any(n => string.Equals(n, nombre, StringComparison.OrdinalIgnoreCase))) throw new ExcepcionTraducible(pClaveYaExiste);
            return nombre;
        }
        private void ValidarNoAutoBloqueo(PermisosDatos pEstructura)
        {
            Usuario usuario = Sesion.Instancia.Usuario;
            if (usuario == null || usuario.Perfil == null) return;
            Perfil perfilPropio = pEstructura.Perfiles.FirstOrDefault(p => p.Codigo == usuario.Perfil.Codigo);
            if (perfilPropio != null && !perfilPropio.TienePermiso(PermisoNombres.GestionarPermisos)) throw new ExcepcionTraducible("permisos.errAutoBloqueo", PermisoNombres.GestionarPermisos);
        }
        private void ActualizarPerfilEnSesion(PermisosDatos pEstructura)
        {
            Usuario usuario = Sesion.Instancia.Usuario;
            if (usuario == null || usuario.Perfil == null) return;
            Perfil perfilPropio = pEstructura.Perfiles.FirstOrDefault(p => p.Codigo == usuario.Perfil.Codigo);
            if (perfilPropio != null) usuario.Perfil = perfilPropio;
        }
        private void RegistrarSuceso(string pDescripcion)
        {
            Usuario usuario = Sesion.Instancia.Usuario;
            SucesoServicio.Instancia.RegistrarSuceso($"{usuario.Nombre} {usuario.Apellido}", pDescripcion, "Seguridad", 1);
        }
    }
}
