using Fact.Models;
using MediatR;

namespace Fact.Features.Clientes.Queries
{
    public record GetClienteByIdQuery(int Id) : IRequest<ClienteDto?>;
}
