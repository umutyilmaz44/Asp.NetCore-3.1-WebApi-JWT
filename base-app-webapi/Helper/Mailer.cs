using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using base_app_common;
using Microsoft.Extensions.Configuration;

namespace base_app_webapi.Helper
{
    public interface IMailer
    {
        ServiceResult Send(string[] recipients, string[] bccList=null, string[] ccList=null, string subject="", string body="", string[] attachments=null);
        Task<ServiceResult> SendAsync(string[] recipients, string[] bccList=null, string[] ccList=null, string subject="", string body="", string[] attachments=null);
    }
    public class Mailer : IMailer
    {
        public IConfiguration Configuration { get; }
        
        public Mailer(IConfiguration configuration)
        {
            Configuration = configuration;            
        }

        public async Task<ServiceResult> SendAsync(string[] recipients, string[] bccList, string[] ccList, string subject, string body, string[] attachments)
        {
            if(recipients == null || recipients.Length == 0)
            {
                return new ServiceResult(false, "Recipients empty!");
            }

            try
            {
                using(System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                { 
                    // yeni mail oluşturduk
                    mail.From = new System.Net.Mail.MailAddress(Configuration["Smtp:Username"], Configuration["Smtp:MailDisplayName"]); // maili gönderecek hesabı belirttik
                    foreach(string recipient in recipients)
                    {
                        mail.To.Add(recipient); // mail gönderilecek adres
                    }
                    foreach(string bcc in bccList??(new string[]{}))
                    {
                        mail.Bcc.Add(bcc); // Bcc içerisinde mail gönderilecek adres
                    }
                    foreach(string cc in ccList??(new string[]{}))
                    {
                        mail.CC.Add(cc); // CC içerisinde mail gönderilecek adres
                    }
                    
                    mail.Subject = subject; // mailin konusu
                    mail.IsBodyHtml = true; // mail içeriği html olarak gönderilsin
                    mail.Body = body; // mailin içeriği
                    mail.Attachments.Clear(); // mail eklerini temizledik
                    
                    // MailEkleri parametresinde mailie ekleyeceğimiz tüm dosyaları aralarına " / " koyarak birbilerine ekledik
                    foreach (string attachment in attachments??(new string[]{}))
                    {
                        if (!string.IsNullOrEmpty(attachment) && File.Exists(attachment))
                        {
                            mail.Attachments.Add(new Attachment(attachment));
                        }
                    }
                    // göndereceğimiz maili hazırladık.
                    using(SmtpClient smtpClient = new SmtpClient(Configuration["Smtp:Host"]))
                    {
                        smtpClient.Port = int.Parse(Configuration["Smtp:Port"]);
                        smtpClient.Credentials = new NetworkCredential(Configuration["Smtp:Username"], Configuration["Smtp:Password"]);
                        smtpClient.EnableSsl = bool.Parse(Configuration["Smtp:EnableSsl"]);

                        await smtpClient.SendMailAsync(mail); // mailimizi gönderdik.
                    }
                }

                return new ServiceResult(true,"");
            }
            catch (Exception ex)
            {
                return new ServiceResult(false, ex.ToString());
            }
        } 

        public ServiceResult Send(string[] recipients, string[] bccList, string[] ccList, string subject, string body, string[] attachments)
        {
            if(recipients == null || recipients.Length == 0)
            {
                return new ServiceResult(false, "Recipients empty!");
            }

            try
            {
                using(System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                { 
                    // yeni mail oluşturduk
                    mail.From = new System.Net.Mail.MailAddress(Configuration["Smtp:Host"], Configuration["Smtp:MailDisplayName"]); // maili gönderecek hesabı belirttik
                    foreach(string recipient in recipients)
                    {
                        mail.To.Add(recipient); // mail gönderilecek adres
                    }
                    foreach(string bcc in bccList)
                    {
                        mail.Bcc.Add(bcc); // Bcc içerisinde mail gönderilecek adres
                    }
                    foreach(string cc in ccList)
                    {
                        mail.CC.Add(cc); // CC içerisinde mail gönderilecek adres
                    }
                    
                    mail.Subject = subject; // mailin konusu
                    mail.IsBodyHtml = true; // mail içeriği html olarak gönderilsin
                    mail.Body = body; // mailin içeriği
                    mail.Attachments.Clear(); // mail eklerini temizledik
                    
                    // MailEkleri parametresinde mailie ekleyeceğimiz tüm dosyaları aralarına " / " koyarak birbilerine ekledik
                    foreach (string attachment in attachments)
                    {
                        if (!string.IsNullOrEmpty(attachment) && File.Exists(attachment))
                        {
                            mail.Attachments.Add(new Attachment(attachment));
                        }
                    }
                    // göndereceğimiz maili hazırladık.
                    using(SmtpClient smtpClient = new SmtpClient(Configuration["Smtp:Host"]))
                    {
                        smtpClient.Port = int.Parse(Configuration["Smtp:Port"]);
                        smtpClient.Credentials = new NetworkCredential(Configuration["Smtp:Username"], Configuration["Smtp:Password"]);
                        smtpClient.EnableSsl = bool.Parse(Configuration["Smtp:EnableSsl"]);

                        smtpClient.Send(mail); // mailimizi gönderdik.
                    }
                }

                return new ServiceResult(true,"");
            }
            catch (Exception ex)
            {
                return new ServiceResult(false, ex.ToString());
            }
        } 
    }
}