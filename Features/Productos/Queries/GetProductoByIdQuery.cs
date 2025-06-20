using Fact.Models;
using MediatR;

namespace Fact.Features.Productos.Queries
{
    public record GetProductoByIdQuery(int Id) : IRequest<ProductoDto?>;
}
