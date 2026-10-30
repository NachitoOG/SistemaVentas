using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text.Json;

namespace Vista_usuario
{
    public static class EnviarMail
    {
        public static void sendMail(string to, string asunto, string body)
        {
            string ruta = Path.Combine(AppContext.BaseDirectory, "correo.json");

            if (!File.Exists(ruta))
            {
                throw new Exception("Falta configurar el correo del sistema: copiá correo.ejemplo.json como correo.json y completá la cuenta.");
            }

            string cuenta;
            string clave;
            string nombreVisible;

            using (JsonDocument documento = JsonDocument.Parse(File.ReadAllText(ruta)))
            {
                cuenta = documento.RootElement.GetProperty("Cuenta").GetString();
                clave = documento.RootElement.GetProperty("Clave").GetString();
                nombreVisible = documento.RootElement.GetProperty("NombreVisible").GetString();
            }

            try
            {
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(cuenta, nombreVisible)
                };
                mail.To.Add(to);
                mail.Subject = asunto;
                mail.Body = body;
                mail.IsBodyHtml = true;

                SmtpClient client = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(cuenta, clave),
                    EnableSsl = true
                };
                client.Send(mail);
            }
            catch
            {
                throw new Exception("No se pudo enviar el correo. Revisá la conexión a internet y que los datos sean correctos.");
            }
        }
    }
}
