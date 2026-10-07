using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public abstract class Permiso
    {
        public int Codigo { get; set; }
        public string Nombre { get; set; }
        protected Permiso(int pCodigo, string pNombre)
        {
            Codigo = pCodigo;
            Nombre = pNombre;
        }
        public abstract bool EsCompuesto { get; }
        public abstract List<Permiso> ObtenerHijos();
        public abstract bool TienePermiso(string pNombrePermiso);
        public abstract bool Contiene(Permiso pPermiso);
        public bool EsMismo(Permiso pOtro)
        {
            return pOtro != null && pOtro.GetType() == GetType() && pOtro.Codigo == Codigo;
        }
        public override string ToString()
        {
            return Nombre;
        }
    }
    public class RelacionPermiso
    {
        public int CodigoPadre { get; set; }
        public int CodigoHijo { get; set; }
    }
    public class PermisosDatos
    {
        public List<PermisoSimple> Permisos { get; set; } = new List<PermisoSimple>();
        public List<Familia> Familias { get; set; } = new List<Familia>();
        public List<Perfil> Perfiles { get; set; } = new List<Perfil>();
        public List<RelacionPermiso> PermisoFamilia { get; set; } = new List<RelacionPermiso>();
        public List<RelacionPermiso> FamiliaFamilia { get; set; } = new List<RelacionPermiso>();
        public List<RelacionPermiso> PermisoPerfil { get; set; } = new List<RelacionPermiso>();
        public List<RelacionPermiso> FamiliaPerfil { get; set; } = new List<RelacionPermiso>();
    }
}
