using Fact.Models;
using MediatR;

namespace Fact.Features.Facturas.Queries
{
    public record GetFacturaByIdQuery(int Id) : IRequest<FacturaDto?>;
}
