using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;

namespace DAL
{
    public class PermisoDAL
    {
        private AccesoDatos acceso = new AccesoDatos();
        public PermisosDatos ObtenerEstructura()
        {
            var datos = new PermisosDatos();
            using (SqlConnection CO = acceso.NuevaConexion())
            using (SqlCommand CM = new SqlCommand("usp_Permisos_ObtenerEstructura", CO))
            {
                CM.CommandType = CommandType.StoredProcedure;
                CO.Open();
                using (SqlDataReader DR = CM.ExecuteReader())
                {
                    //Permisos simples
                    while (DR.Read()) datos.Permisos.Add(new PermisoSimple(Convert.ToInt32(DR["Cod_Permiso"]), DR["Nombre_Permiso"].ToString()));

                    //Familias
                    DR.NextResult();
                    while (DR.Read()) datos.Familias.Add(new Familia(Convert.ToInt32(DR["Cod_Familia"]), DR["Nombre_Familia"].ToString()));

                    //Perfiles
                    DR.NextResult();
                    while (DR.Read()) datos.Perfiles.Add(new Perfil(Convert.ToInt32(DR["Cod_Perfil"]), DR["Nombre_Perfil"].ToString()));

                    //Relaciones planas
                    DR.NextResult(); LeerRelaciones(DR, datos.PermisoFamilia);
                    DR.NextResult(); LeerRelaciones(DR, datos.FamiliaFamilia);
                    DR.NextResult(); LeerRelaciones(DR, datos.PermisoPerfil);
                    DR.NextResult(); LeerRelaciones(DR, datos.FamiliaPerfil);
                }
            }
            return datos;
        }
        private void LeerRelaciones(SqlDataReader DR, List<RelacionPermiso> pDestino)
        {
            while (DR.Read())
            {
                pDestino.Add(new RelacionPermiso
                {
                    CodigoPadre = Convert.ToInt32(DR["CodigoPadre"]),
                    CodigoHijo = Convert.ToInt32(DR["CodigoHijo"])
                });
            }
        }

        public int CrearPerfil(string pNombre, List<Permiso> pHijos)
        {
            return EnTransaccion((CO, TR) =>
            {
                int codigo = CrearCompuesto("usp_Perfil_Crear", pNombre, CO, TR);
                foreach (Permiso hijo in pHijos) AgregarHijoAPerfil(codigo, hijo, CO, TR);
                return codigo;
            });
        }
        public void AgregarAPerfil(int pCodPerfil, List<Permiso> pHijos)
        {
            EnTransaccion((CO, TR) =>
            {
                foreach (Permiso hijo in pHijos) AgregarHijoAPerfil(pCodPerfil, hijo, CO, TR);
                return 0;
            });
        }
        public void QuitarDePerfil(int pCodPerfil, List<Permiso> pHijos)
        {
            EnTransaccion((CO, TR) =>
            {
                foreach (Permiso hijo in pHijos)
                {
                    if (hijo is PermisoSimple) Ejecutar("usp_Perfil_QuitarPermiso", CO, TR, P("@CodPerfil", pCodPerfil), P("@CodPermiso", hijo.Codigo));
                    else if (hijo is Familia) Ejecutar("usp_Perfil_QuitarFamilia", CO, TR, P("@CodPerfil", pCodPerfil), P("@CodFamilia", hijo.Codigo));
                }
                return 0;
            });
        }
        public void EliminarPerfil(int pCodPerfil)
        {
            EjecutarSinTransaccion("usp_Perfil_Eliminar", P("@CodPerfil", pCodPerfil));
        }
        private void AgregarHijoAPerfil(int pCodPerfil, Permiso pHijo, SqlConnection CO, SqlTransaction TR)
        {
            if (pHijo is PermisoSimple)
            {
                Ejecutar("usp_Perfil_AgregarPermiso", CO, TR, P("@CodPerfil", pCodPerfil), P("@CodPermiso", pHijo.Codigo));
            }
            else if (pHijo is Familia)
            {
                Ejecutar("usp_Perfil_AgregarFamilia", CO, TR, P("@CodPerfil", pCodPerfil), P("@CodFamilia", pHijo.Codigo));
            }
            else
            {
                throw new InvalidOperationException("Un perfil solo puede contener permisos simples o familias.");
            }
        }

        public int CrearFamilia(string pNombre, List<Permiso> pHijos)
        {
            return EnTransaccion((CO, TR) =>
            {
                int codigo = CrearCompuesto("usp_Familia_Crear", pNombre, CO, TR);
                foreach (Permiso hijo in pHijos) AgregarHijoAFamilia(codigo, hijo, CO, TR);
                return codigo;
            });
        }
        public void AgregarAFamilia(int pCodFamilia, List<Permiso> pHijos)
        {
            EnTransaccion((CO, TR) =>
            {
                foreach (Permiso hijo in pHijos) AgregarHijoAFamilia(pCodFamilia, hijo, CO, TR);
                return 0;
            });
        }
        public void QuitarDeFamilia(int pCodFamilia, List<Permiso> pHijos)
        {
            EnTransaccion((CO, TR) =>
            {
                foreach (Permiso hijo in pHijos)
                {
                    if (hijo is PermisoSimple)
                    {
                        Ejecutar("usp_Familia_QuitarPermiso", CO, TR, P("@CodFamilia", pCodFamilia), P("@CodPermiso", hijo.Codigo));
                    }                        
                    else if (hijo is Familia)
                    {
                        Ejecutar("usp_Familia_QuitarFamilia", CO, TR, P("@CodContenedora", pCodFamilia), P("@CodContenida", hijo.Codigo));
                    }                        
                }
                return 0;
            });
        }
        public void EliminarFamilia(int pCodFamilia)
        {
            EjecutarSinTransaccion("usp_Familia_Eliminar", P("@CodFamilia", pCodFamilia));
        }
        private void AgregarHijoAFamilia(int pCodFamilia, Permiso pHijo, SqlConnection CO, SqlTransaction TR)
        {
            if (pHijo is PermisoSimple)
            {
                Ejecutar("usp_Familia_AgregarPermiso", CO, TR, P("@CodFamilia", pCodFamilia), P("@CodPermiso", pHijo.Codigo));
            }
            else if (pHijo is Familia)
            {
                Ejecutar("usp_Familia_AgregarFamilia", CO, TR, P("@CodContenedora", pCodFamilia), P("@CodContenida", pHijo.Codigo));
            }
            else
            {
                throw new InvalidOperationException("Una familia solo puede contener permisos simples u otras familias.");
            }
        }

        private int CrearCompuesto(string pSP, string pNombre, SqlConnection CO, SqlTransaction TR)
        {
            var salida = new SqlParameter("@Codigo", SqlDbType.Int) { Direction = ParameterDirection.Output };
            Ejecutar(pSP, CO, TR, P("@Nombre", pNombre), salida);
            return (int)salida.Value;
        }
        private static SqlParameter P(string pNombre, object pValor)
        {
            return new SqlParameter(pNombre, pValor);
        }
        private void Ejecutar(string pSP, SqlConnection CO, SqlTransaction TR, params SqlParameter[] pParametros)
        {
            using (SqlCommand CM = new SqlCommand(pSP, CO, TR))
            {
                CM.CommandType = CommandType.StoredProcedure;
                CM.Parameters.AddRange(pParametros);
                CM.ExecuteNonQuery();
            }
        }
        private void EjecutarSinTransaccion(string pSP, params SqlParameter[] pParametros)
        {
            using (SqlConnection CO = acceso.NuevaConexion())
            {
                CO.Open();
                try
                {
                    Ejecutar(pSP, CO, null, pParametros);
                }
                catch (SqlException ex) when (ex.Number >= 50000)
                {
                    throw new InvalidOperationException(ex.Message, ex);
                }
            }
        }
        private T EnTransaccion<T>(Func<SqlConnection, SqlTransaction, T> pOperacion)
        {
            using (SqlConnection CO = acceso.NuevaConexion())
            {
                CO.Open();
                using (SqlTransaction TR = CO.BeginTransaction())
                {
                    try
                    {
                        T resultado = pOperacion(CO, TR);
                        TR.Commit();
                        return resultado;
                    }
                    catch (SqlException ex) when (ex.Number >= 50000)
                    {
                        RevertirSeguro(TR);
                        throw new InvalidOperationException(ex.Message, ex);
                    }
                    catch
                    {
                        RevertirSeguro(TR);
                        throw;
                    }
                }
            }
        }
        private static void RevertirSeguro(SqlTransaction TR)
        {
            try { TR.Rollback(); }
            catch (InvalidOperationException) { }
        }
    }
}
