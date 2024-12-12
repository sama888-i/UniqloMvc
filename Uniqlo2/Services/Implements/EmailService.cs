using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using Uniqlo2.Helpers;
using Uniqlo2.Services.Abstracts;

namespace Uniqlo2.Services.Implements
{
    public class EmailService : IEmailService
    {
        readonly SmtpClient _client;
        readonly MailAddress _from;
        readonly HttpContext Context;
        public EmailService(IOptions<SmtpOptions> opts,IHttpContextAccessor acc)
        {
            var opt = opts.Value;
            _client = new(opt.Host,opt.Port);
            _client.EnableSsl = true;
            _client.Credentials = new NetworkCredential(opt.Sender, opt.Password);
            _from = new MailAddress(opt.Sender, "Uniqlo");
            Context = acc.HttpContext;
        }

        public void SendEmailconfirmation(string reciever,string name,string token)
        {
            MailAddress to = new(reciever);
            MailMessage message = new MailMessage(_from, to);
            message.Subject = "Confirm your email adress";
            string url = Context.Request.Scheme + "://" + Context.Request.Host + "/Account/VerifyEmail?token=" + token+"&user="+name;
            message.Body = EmailTemplates.VerifyEmail.Replace("__$name", name).Replace("__$link", url);
           _client.Send(message);
            
        }

        Task IEmailService.SendEmailconfirmation(string reciever, string name, string token)
        {
            throw new NotImplementedException();
        }
    }
}
