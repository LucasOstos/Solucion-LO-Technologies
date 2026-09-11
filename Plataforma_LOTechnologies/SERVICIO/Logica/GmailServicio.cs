using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace SERVICIO.Logica
{
    public class GmailServicio
    {
        private static GmailServicio instancia;
        public static GmailServicio Instancia
        {
            get
            {
                if (instancia == null) instancia = new GmailServicio(); return instancia;
            }
        }
        public void EnviarMailRecuperacion(string pDestinatario, string pNombreCompleto, Guid pToken)
        {
            string urlBase = ConfigurationManager.AppSettings["UrlBase"];
            string link = $"{urlBase}/RecuperarContrasenia.aspx?token={pToken}";
            string asunto = "LO Technologies - Recuperar contraseña";
            string cuerpo = $@"
                            <p>Hola {pNombreCompleto},</p>
                            <p>Recibimos una solicitud para restablecer tu contraseña.</p>
                            <p><a href='{link}'>Hacé clic acá reestablacer tu contraseña</a></p>
                            <p>Este link expira en 5 minutos. Si no solicitaste este cambio, ignorá este mensaje.</p>";
            EnviarMail(pDestinatario, asunto, cuerpo);
        }
        private void EnviarMail(string pDestinatario, string pAsunto, string pHtml)
        {
            string usuarioGmail = ConfigurationManager.AppSettings["GmailUsuario"];
            string passwordApp = ConfigurationManager.AppSettings["GmailAppPassword"];
            using(var mensaje = new MailMessage())
            {
                mensaje.From = new MailAddress(usuarioGmail, "LO Technologies");
                mensaje.To.Add(pDestinatario);
                mensaje.Subject = pAsunto;
                mensaje.Body = pHtml;
                mensaje.IsBodyHtml = true;
                using(var smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.EnableSsl = true;
                    smtp.Credentials = new NetworkCredential(usuarioGmail, passwordApp);
                    smtp.Send(mensaje);
                }
            }
        }
    }
}
