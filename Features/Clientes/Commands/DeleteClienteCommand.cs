using MediatR;

namespace Fact.Features.Clientes.Commands
{
    public record DeleteClienteCommand(int Id) : IRequest<bool>;
}
