using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace skillExchange.App_Start
{
    public static class EmailService
    {
        public static bool TrySend(string recipient, string subject, string body, out string error)
        {
            error = "";
            if (!Boolean.TryParse(ConfigurationManager.AppSettings["SmtpEnabled"], out bool enabled) || !enabled)
            {
                error = "SMTP is disabled in Web.config.";
                return false;
            }

            try
            {
                string username = ConfigurationManager.AppSettings["SmtpUsername"];
                string password = ConfigurationManager.AppSettings["SmtpAppPassword"];
                using (MailMessage message = new MailMessage(ConfigurationManager.AppSettings["SmtpFrom"], recipient, subject, body))
                using (SmtpClient client = new SmtpClient(ConfigurationManager.AppSettings["SmtpHost"], Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"])))
                {
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(username, password);
                    client.Send(message);
                }
                return true;
            }
            catch (SmtpException ex) { error = "Gmail rejected the SMTP login or connection (" + ex.StatusCode + ")."; return false; }
            catch (Exception) { error = "SMTP settings are incomplete or invalid."; return false; }
        }
    }
}
