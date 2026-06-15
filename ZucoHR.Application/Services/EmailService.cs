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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ZucoHR.Application.Services
{
   
    

        public class EmailService : IEmailService
        {
            private readonly EmailSettings _settings;
        private readonly IConfiguration _config;

            public EmailService(IOptions<EmailSettings> settings, IConfiguration config)
            {
                _settings = settings.Value;
            _config = config;
            }

        public async Task SendBookDemoEmail(
    BookDemoDto dto)
        {
            var recipient =
                _config["EmailSettings:SalesEmail"];

            var subject =
                $"New Demo Booking - {dto.Company}";

            var body = $@"
<html>

<body style='font-family:Arial'>

<h2>New ZucoHR Demo Request</h2>

<table cellpadding='8' cellspacing='0' border='1' style='border-collapse:collapse;'>

<tr>
<td><strong>Name</strong></td>
<td>{dto.FullName}</td>
</tr>

<tr>
<td><strong>Company</strong></td>
<td>{dto.Company}</td>
</tr>

<tr>
<td><strong>Email</strong></td>
<td>{dto.Email}</td>
</tr>

<tr>
<td><strong>Phone</strong></td>
<td>{dto.Phone}</td>
</tr>

<tr>
<td><strong>Employees</strong></td>
<td>{dto.Employees}</td>
</tr>

<tr>
<td><strong>Country</strong></td>
<td>{dto.Country}</td>
</tr>

<tr>
<td><strong>Preferred Date</strong></td>
<td>{dto.PreferredDate:dddd, dd MMMM yyyy}</td>
</tr>

<tr>
<td><strong>Preferred Time</strong></td>
<td>{dto.PreferredTime}</td>
</tr>

<tr>
<td><strong>Message</strong></td>
<td>{dto.Message}</td>
</tr>

</table>

</body>

</html>";
            EmailRequest req = new EmailRequest()
            {
                Subject = subject,
                To = recipient,
                Body = body
            };
            await SendEmailAsync(
               req
            );

            // Send acknowledgement to requester

            var customerBody = $@"
<html>

<body>

<h2>Hello {dto.FullName},</h2>

<p>
Thank you for requesting a live demonstration of
<b>ZucoHR</b>.
</p>

<p>
Our sales team will contact you shortly to confirm
your preferred schedule.
</p>

<p>
We look forward to showing you how ZucoHR can help
streamline your HR, Payroll, Recruitment, Leave,
Performance Management, Expenses, Assets, and more.
</p>

<br/>

Regards,<br/>
<b>ZucoHR Team</b>

</body>

</html>";
            EmailRequest request = new EmailRequest();
            request.Subject = "Your ZucoHR Demo Request";
            request.Body = customerBody;
            request.To = dto.Email;


            await SendEmailAsync(
                request
            );
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
