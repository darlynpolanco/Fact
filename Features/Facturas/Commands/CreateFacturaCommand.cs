using Fact.Models;
using MediatR;

namespace Fact.Features.Facturas.Commands
{
    public record CreateFacturaCommand : IRequest<FacturaDto>
    {
        public int ClienteId { get; init; }
        public List<DetalleFacturaItem> Items { get; init; } = new();
    }

    public record DetalleFacturaItem
    {
        public int ProductoId { get; init; }
        public int Cantidad { get; init; }
    }
}
