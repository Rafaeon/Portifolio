using System.Net;
using System.Net.Mail;

namespace Portifolio.Services;

public class EmailService(IConfiguration configuration, ILogger<EmailService> logger)
{
    public bool TryEnviar(string destinatario, string assunto, string corpoHtml)
    {
        try
        {
            var host = configuration["SMTP:Host"];
            var porta = configuration["SMTP:Porta"];
            var usuario = configuration["SMTP:UserName"];
            var senha = configuration["SMTP:Senha"];
            var remetenteNome = configuration["SMTP:Nome"] ?? "Portifolio";

            if (string.IsNullOrWhiteSpace(host) ||
                string.IsNullOrWhiteSpace(porta) ||
                string.IsNullOrWhiteSpace(usuario) ||
                string.IsNullOrWhiteSpace(senha))
            {
                logger.LogWarning("SMTP não configurado; e-mail de aviso não enviado.");
                return false;
            }

            using var client = new SmtpClient(host)
            {
                Port = int.Parse(porta),
                Credentials = new NetworkCredential(usuario, senha),
                EnableSsl = true
            };

            using var message = new MailMessage
            {
                From = new MailAddress(usuario!, remetenteNome),
                Subject = assunto,
                Body = corpoHtml,
                IsBodyHtml = true
            };
            message.To.Add(destinatario);

            client.Send(message);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha ao enviar e-mail para {Destino}.", destinatario);
            return false;
        }
    }
}
