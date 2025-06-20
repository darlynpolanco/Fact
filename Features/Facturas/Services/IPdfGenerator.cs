namespace Fact.Features.Facturas.Services
{
    public interface IPdfGenerator
    {
        byte[] GenerateFacturaPdf(Data.Entities.Factura factura);
    }
}
