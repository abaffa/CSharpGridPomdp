using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace GridPomdp.Utils
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

        public void SendMail(String subject, String body, String sender, String recipients, String cc = "", String[] attachments = null)
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
                foreach (String e in emails)
                    message.To.Add(new MailAddress(e.Trim()));
            }
            else
                message.To.Add(new MailAddress(recipients));

            if (cc.Trim().Length > 0)
            {
                if (cc.IndexOf(',') > 0)
                {
                    String[] emails = recipients.Split(',');
                    foreach (String e in emails)
                        message.CC.Add(new MailAddress(e.Trim()));
                }
                else
                    message.CC.Add(new MailAddress(cc));
            }

            if(attachments != null && attachments.Length > 0)
            {
                foreach(String filename in attachments)
                {
                    if (File.Exists(filename))
                        message.Attachments.Add(new Attachment(filename));
                }
                
            }

            smtp.Send(message);
        }
    }
}
