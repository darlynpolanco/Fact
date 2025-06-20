using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using Fact.Models;
using System.Text;

namespace Fact.Features.Facturas.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailSender(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings?.Value ?? throw new ArgumentNullException(nameof(smtpSettings));

            if (string.IsNullOrWhiteSpace(_smtpSettings.Host))
                throw new ArgumentException("El host SMTP no está configurado");

            if (_smtpSettings.Port <= 0 || _smtpSettings.Port > 65535)
                throw new ArgumentException("Puerto SMTP inválido");

            if (string.IsNullOrWhiteSpace(_smtpSettings.Username))
                throw new ArgumentException("Usuario SMTP no configurado");

            if (string.IsNullOrWhiteSpace(_smtpSettings.Password))
                throw new ArgumentException("Contraseña SMTP no configurada");

            if (string.IsNullOrWhiteSpace(_smtpSettings.SenderEmail))
                throw new ArgumentException("Email del remitente no configurado");
        }

        // Auxiliares 
        private string GetDetallesFacturaTexto(FacturaDto factura)
        {
            if (factura.Items == null || !factura.Items.Any())
                return $"Total: {factura.Total:C}";

            var sb = new StringBuilder();
            foreach (var detalle in factura.Items)
            {
                sb.AppendLine($"- {detalle.ProductoNombre}: {detalle.Cantidad} x {detalle.PrecioUnitario:C} = {detalle.Total:C}");
            }
            return sb.ToString();
        }

        private string GetDetallesFacturaHTML(FacturaDto factura)
        {
            if (factura.Items == null || !factura.Items.Any())
                return "";

            var sb = new StringBuilder();
            sb.AppendLine("<div style='margin-top: 15px;'>");
            sb.AppendLine("<div class='detalle-item' style='font-weight: bold;'>");
            sb.AppendLine("   <div>Producto/Servicio</div>");
            sb.AppendLine("   <div>Cantidad</div>");
            sb.AppendLine("   <div>Precio Unitario</div>");
            sb.AppendLine("   <div>Subtotal</div>");
            sb.AppendLine("</div>");

            foreach (var detalle in factura.Items)
            {
                sb.AppendLine("<div class='detalle-item'>");
                sb.AppendLine($"   <div>{detalle.ProductoNombre}</div>");
                sb.AppendLine($"   <div>{detalle.Cantidad}</div>");
                sb.AppendLine($"   <div>{detalle.PrecioUnitario:C}</div>");
                sb.AppendLine($"   <div>{detalle.Total:C}</div>");
                sb.AppendLine("</div>");
            }

            sb.AppendLine("</div>");
            return sb.ToString();
        }
        public async Task SendFacturaEmailAsync(
            string email,
            string nombreCliente,
            Models.FacturaDto factura,
            byte[] pdfBytes)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("El email del destinatario no puede estar vacío");

            if (string.IsNullOrWhiteSpace(nombreCliente))
                throw new ArgumentException("El nombre del cliente no puede estar vacío");

            if (factura == null)
                throw new ArgumentNullException(nameof(factura));

            if (pdfBytes == null || pdfBytes.Length == 0)
                throw new ArgumentException("El PDF de la factura no puede estar vacío");

            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(
                    _smtpSettings.SenderName ?? "Sistema de Facturación", // Valor por defecto sin sobre escribir...
                    _smtpSettings.SenderEmail));

                message.To.Add(new MailboxAddress(nombreCliente, email));
                message.Subject = $"Factura #{factura.Id} - {factura.Fecha:dd/MM/yyyy}";

                var builder = new BodyBuilder
                {
                    TextBody = $@"📄 Factura #{factura.Id}

                        ¡Hola {nombreCliente}!

                        Adjunto encontrarás la factura #{factura.Id} emitida el {factura.Fecha:dd/MM/yyyy}.

                        🔍 Detalles de la factura:
                        {GetDetallesFacturaTexto(factura)}

                        💳 Total: {factura.Total:C}
                        📅 Vencimiento: {(factura.Fecha.AddDays(15)):dd/MM/yyyy}
                        🆔 Referencia: PAGO-{factura.Id}

                        ¿Necesitas ayuda? Estamos aquí para ayudarte:
                        📧 soporte@gmail.com
                        📞 +123 456 7890

                        Gracias por tu preferencia,
                        ✨ Equipo de Facturación",

                                            HtmlBody = $@"
                            <!DOCTYPE html>
                            <html>
                            <head>
                                <meta charset='utf-8'>
                                <style>
                                    body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; color: #333; }}
                                    .container {{ max-width: 600px; margin: 0 auto; }}
                                    .header {{ background-color: #f8f9fa; padding: 20px; text-align: center; }}
                                    .content {{ padding: 30px; }}
                                    .factura-card {{ background: #ffffff; border: 1px solid #e0e0e0; border-radius: 8px; padding: 25px; margin-bottom: 20px; }}
                                    .total {{ font-size: 1.3em; font-weight: bold; color: #2e7d32; }}
                                    .detalle-item {{ display: grid; grid-template-columns: 2fr 1fr 1fr 1fr; padding: 8px 0; border-bottom: 1px solid #f0f0f0; }}
                                    .footer {{ text-align: center; padding: 20px; color: #757575; font-size: 0.9em; }}
                                    .emoji {{ font-size: 1.5em; margin-right: 10px; vertical-align: middle; }}
                                    .btn {{ 
                                        display: inline-block; 
                                        background: #4a6cf7; 
                                        color: white !important; 
                                        padding: 12px 25px; 
                                        text-decoration: none; 
                                        border-radius: 4px; 
                                        margin: 15px 0; 
                                    }}
                                    .detalle-header {{ font-weight: bold; background-color: #f5f5f5; }}
                                </style>
                            </head>
                            <body>
                                <div class='container'>
                                    <div class='header'>
                                        <h2 style='margin: 0;'>Factura #{factura.Id}</h2>
                                        <p style='margin: 5px 0;'>{factura.Fecha:dd/MM/yyyy}</p>
                                    </div>
            
                                    <div class='content'>
                                        <p>¡Hola <strong>{nombreCliente}</strong>!</p>
                                        <p>Gracias por tu compra. Adjunto encontrarás la factura #{factura.Id} en formato PDF.</p>
                
                                        <div class='factura-card'>
                                            <div style='display: flex; align-items: center; margin-bottom: 15px;'>
                                                <span class='emoji'>📄</span>
                                                <h3 style='margin: 0;'>Detalles de la Factura</h3>
                                            </div>
                    
                                            <div class='detalle-item detalle-header'>
                                                <div>Producto/Servicio</div>
                                                <div>Cantidad</div>
                                                <div>P. Unitario</div>
                                                <div>Subtotal</div>
                                            </div>
                    
                                            {GetDetallesFacturaHTML(factura)}
                    
                                            <div style='margin-top: 20px; text-align: right;'>
                                                <div>Total:</div>
                                                <div class='total'>{factura.Total:C}</div>
                                            </div>
                    
                                            <div style='margin-top: 20px;'>
                                                <div><span class='emoji'>📅</span> Vencimiento: {(factura.Fecha.AddDays(15)):dd/MM/yyyy}</div>
                                                <div><span class='emoji'>🆔</span> Referencia: PAGO-{factura.Id}</div>
                                            </div>
                                        </div>
                
                                        <div style='background: #f8f9fa; padding: 20px; border-radius: 8px; margin-top: 30px;'>
                                            <div style='display: flex; align-items: center;'>
                                                <span class='emoji'>❓</span>
                                                <h3 style='margin: 0;'>¿Necesitas ayuda?</h3>
                                            </div>
                                            <p>Estamos aquí para ayudarte con cualquier pregunta sobre tu factura.</p>
                                            <p>📧 <a href='mailto:soporte@gmail.com'>soporte@tudominio.com</a></p>
                                            <p>📞 +123 456 7890</p>
                                        </div>
                                    </div>
            
                                    <div class='footer'>
                                        <p>© {DateTime.Now.Year} Tu Empresa - Todos los derechos reservados</p>
                                        <p><a href='https://tudominio.com'>tudominio.com</a> | <a href='https://tudominio.com/privacidad'>Política de Privacidad</a></p>
                                    </div>
                                </div>
                            </body>
                            </html>"
                };

                builder.Attachments.Add(
                    $"factura-{factura.Id}.pdf",
                    pdfBytes,
                    new ContentType("application", "pdf"));

                message.Body = builder.ToMessageBody();

                using var client = new SmtpClient();

                await client.ConnectAsync(
                    _smtpSettings.Host, 
                    _smtpSettings.Port,
                    _smtpSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);

                await client.AuthenticateAsync(
                    _smtpSettings.Username,
                    _smtpSettings.Password);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al enviar el email con la factura", ex);
            }
        }
    }
}