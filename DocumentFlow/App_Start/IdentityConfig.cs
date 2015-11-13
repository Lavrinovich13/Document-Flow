using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace DocumentFlow.App_Start
{
    public class IdentityConfig
    {
        public class EmailService : IIdentityMessageService
        {
            public Task SendAsync(IdentityMessage message)
            {
                var from = "lavrinovich.kg.13@gmail.com";
                var pass = "Supermario_1";

                SmtpClient client = new SmtpClient("smtp.gmail.com", 25);

                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(from, pass);
                client.EnableSsl = true;

                var mail = new MailMessage(from, message.Destination);
                mail.Subject = message.Subject;
                mail.Body = message.Body;
                mail.IsBodyHtml = true;

                return client.SendMailAsync(mail);
            }
        }
    }
}