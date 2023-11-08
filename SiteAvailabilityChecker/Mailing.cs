using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using MailKit.Net.Smtp;
using MailKit;
using MimeKit;


namespace SiteAvailabilityChecker
{
    internal class Mailing
    {




        public void SendEmail( string from, string to, string subject, string bodyText)
        {
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress("Sender Name", from));
            email.To.Add(new MailboxAddress("Receiver Name", to));
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = bodyText
            };

            using (var smtp = new SmtpClient())
            {
                smtp.Connect("smtp.gmail.com", 587, false);

                // Note: only needed if the SMTP server requires authentication
                smtp.Authenticate("smtp_username", "smtp_password");

                smtp.Send(email);
                smtp.Disconnect(true);
            }
        }


    }
}
