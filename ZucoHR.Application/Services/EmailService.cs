using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using global::ZucoHR.Application.Interfaces;
using global::ZucoHR.Domain.DTO;
using global::ZucoHR.Domain.Entities;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ZucoHR.Application.Services
{
   
    

        public class EmailService : IEmailService
        {
            private readonly EmailSettings _settings;

            public EmailService(IOptions<EmailSettings> settings)
            {
                _settings = settings.Value;
            }

            public async Task SendEmailAsync(EmailRequest request)
            {
                var email = new MimeMessage();

                email.From.Add(
                    new MailboxAddress(
                        _settings.DisplayName,
                        _settings.From
                    )
                );

                email.To.Add(
                    MailboxAddress.Parse(request.To)
                );

                email.Subject = request.Subject;

                var builder = new BodyBuilder
                {
                    HtmlBody = request.Body
                };

                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();

                await smtp.ConnectAsync(
                    _settings.Host,
                    _settings.Port,
                    SecureSocketOptions.StartTls
                );

                await smtp.AuthenticateAsync(
                    _settings.Username,
                    _settings.Password
                );

                await smtp.SendAsync(email);

                await smtp.DisconnectAsync(true);
            }
        }
    }
