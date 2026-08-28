using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;
using DAL;

namespace BLL
{
    public class EmpresaBLL
    {
        private EmpresaDAL empresaDAL = new EmpresaDAL();
        public List<EmpresaBE> ObtenerEmpresas()
        {
            return empresaDAL.ObtenerEmpresas();
        }
    }
}
