using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Sportex.Infrastructure.Services;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public void Send(string to, string subject, string body)
    {
        var from = _config["Email:From"];
        var host = _config["Email:Host"];
        var port = int.Parse(_config["Email:Port"]!);
        var username = _config["Email:Username"];
        var password = _config["Email:Password"];

        if (string.IsNullOrEmpty(from))
            throw new Exception("Email From missing");

        var smtp = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(username, password),
            EnableSsl = true
        };

        var mail = new MailMessage(from!, to, subject, body);
        smtp.Send(mail);
    }
}
