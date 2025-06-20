using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Fact.Features.Facturas.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailSender(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task SendFacturaEmailAsync(
            string email,
            string nombreCliente,
            Models.FacturaDto factura,
            byte[] pdfBytes)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(
                _smtpSettings.SenderName,
                _smtpSettings.SenderEmail));

            message.To.Add(new MailboxAddress(nombreCliente, email));
            message.Subject = $"Factura #{factura.Id} - {factura.Fecha:dd/MM/yyyy}";

            var builder = new BodyBuilder
            {
                TextBody = $"Estimado {nombreCliente},\n\nAdjunto encontrará la factura #{factura.Id}.\n\nSaludos,\nEquipo de Facturación",
                HtmlBody = $@"
                <h1>Factura #{factura.Id}</h1>
                <p>Estimado {nombreCliente},</p>
                <p>Adjunto encontrará la factura #{factura.Id} emitida el {factura.Fecha:dd/MM/yyyy}.</p>
                <p>Total: <strong>{factura.Total:C}</strong></p>
                <p>Saludos,<br>Equipo de Facturación</p>
            "
            };

            // Adjuntar PDF
            builder.Attachments.Add(
                $"factura-{factura.Id}.pdf",
                pdfBytes,
                new ContentType("application", "pdf"));

            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();

            await client.ConnectAsync(
                _smtpSettings.Server,
                _smtpSettings.Port,
                SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(
                _smtpSettings.Username,
                _smtpSettings.Password);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}