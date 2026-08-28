using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using BLL;

/// <summary>
/// Descripción breve de RecuperarContraseniaWS
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
[System.Web.Script.Services.ScriptService]
public class RecuperarContraseniaWS : System.Web.Services.WebService
{
    public RecuperarContraseniaWS()
    {

        //Elimine la marca de comentario de la línea siguiente si utiliza los componentes diseñados 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string SolicitarRecuperacion(string pEmail)
    {
        string mensaje = "Si la dirección email está registrada, vas a recibir un mail con instrucciones para recuperar tu contraseña.";
        new UsuarioBLL().SolicitarRecuperacion(pEmail);
        return mensaje;
    }

}
