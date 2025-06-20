namespace Fact.Models
{
    public record FacturaDto(
        int Id,
        int ClienteId,
        string ClienteNombre,
        string ClienteEmail,
        DateTime Fecha,
        decimal Total,
    List<DetalleFacturaDto> Items);

    public record DetalleFacturaDto(
        int ProductoId,
        string ProductoNombre,
        int Cantidad,
        decimal PrecioUnitario,
        decimal Total);
}
