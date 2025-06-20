using MediatR;

namespace Fact.Features.Productos.Commands
{
    public record DeleteProductoCommand(int Id) : IRequest<bool>;
}
