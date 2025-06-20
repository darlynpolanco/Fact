using Fact.Models;
using MediatR;

namespace Fact.Features.Productos.Queries
{
    public record GetAllProductosQuery : IRequest<IEnumerable<ProductoDto>>;
}
