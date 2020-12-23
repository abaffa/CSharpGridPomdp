using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MailTest
{
    public class GmailSender
    {
        private String mailhost = "smtp.gmail.com";
        private String user;
        private String password;
        private int port = 587;

        public GmailSender(String user, String password)
        {
            this.user = user;
            this.password = password;

        }

        public void SendMail(String subject, String body, String sender, String recipients)
        {

            var smtp = new SmtpClient
            {
                Host = mailhost,
                Port = port,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(user, password)
            };
            var message = new MailMessage()
            {
                From = new MailAddress(sender),
                Subject = subject,
                Body = body
            };

            if (recipients.IndexOf(',') > 0)
            {
                String[] emails = recipients.Split(',');
                foreach(String e in emails)
                    message.To.Add(new MailAddress(e.Trim()));
            }
            else
                message.To.Add(new MailAddress(recipients));

            smtp.Send(message);

        }
    }
}
