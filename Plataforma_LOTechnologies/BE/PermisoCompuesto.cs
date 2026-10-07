using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public abstract class PermisoCompuesto : Permiso
    {
        private readonly List<Permiso> hijos = new List<Permiso>();
        protected PermisoCompuesto(int pCodigo, string pNombre) : base(pCodigo, pNombre) { }
        public override bool EsCompuesto { get { return true; } }
        public override List<Permiso> ObtenerHijos()
        {
            return new List<Permiso>(hijos);
        }
        public override bool TienePermiso(string pNombrePermiso)
        {
            return hijos.Any(h => h.TienePermiso(pNombrePermiso));
        }
        public override bool Contiene(Permiso pPermiso)
        {
            return EsMismo(pPermiso) || hijos.Any(h => h.Contiene(pPermiso));
        }
        public void Agregar(Permiso pPermiso)
        {
            if (pPermiso == null) throw new ArgumentNullException(nameof(pPermiso));

            ValidarHijo(pPermiso);

            if (hijos.Any(h => h.EsMismo(pPermiso)))
                throw new InvalidOperationException($"'{pPermiso.Nombre}' ya está asignado directamente a '{Nombre}'.");

            hijos.Add(pPermiso);
        }
        public void Quitar(Permiso pPermiso)
        {
            hijos.RemoveAll(h => h.EsMismo(pPermiso));
        }
        public List<PermisoSimple> ObtenerPermisosSimples()
        {
            var resultado = new List<PermisoSimple>();
            foreach (Permiso hijo in hijos)
            {
                if (hijo is PermisoSimple simple) resultado.Add(simple);
                else if (hijo is PermisoCompuesto compuesto) resultado.AddRange(compuesto.ObtenerPermisosSimples());
            }
            return resultado.GroupBy(p => p.Codigo).Select(g => g.First()).ToList();
        }
        protected abstract void ValidarHijo(Permiso pPermiso);
    }
    public class Familia : PermisoCompuesto
    {
        public Familia(int pCodigo, string pNombre) : base(pCodigo, pNombre) { }
        protected override void ValidarHijo(Permiso pPermiso)
        {
            if (pPermiso is Perfil) throw new InvalidOperationException("Una familia no puede contener un perfil.");
            if (pPermiso is Familia familia)
            {
                if (familia.EsMismo(this)) throw new InvalidOperationException("Una familia no puede contenerse a sí misma.");
                if (familia.Contiene(this)) throw new InvalidOperationException($"No se puede agregar '{familia.Nombre}' a '{Nombre}' porque '{familia.Nombre}' ya contiene a '{Nombre}'.");
            }
        }
    }
    public class Perfil : PermisoCompuesto
    {
        public Perfil(int pCodigo, string pNombre) : base(pCodigo, pNombre) { }
        protected override void ValidarHijo(Permiso pPermiso)
        {
            if (pPermiso is Perfil) throw new InvalidOperationException("Un perfil no puede contener otro perfil.");
        }
    }
}
