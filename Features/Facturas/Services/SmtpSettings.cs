using System.Text.Json.Serialization;

namespace Fact.Features.Facturas.Services
{
    public class SmtpSettings
    {
        public string Host { get; set; } = "smtp.gmail.com";
        public int Port { get; set; } = 587;
        public bool EnableSsl { get; set; } = true;

        // Mapeo explícito para 'User' en los secrets
        [ConfigurationKeyName("User")]  // Esto mapea 'User' a 'Username'
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
    }
}
