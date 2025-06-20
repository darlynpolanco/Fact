namespace Fact.Features.Facturas.Services
{
    public interface IEmailSender
    {
        Task SendFacturaEmailAsync(
            string email,
            string nombreCliente,
            Models.FacturaDto factura,
            byte[] pdfBytes);
    }
}
