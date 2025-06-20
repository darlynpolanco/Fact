using Fact.Models;
using MediatR;

namespace Fact.Features.Clientes.Queries
{
    public record GetAllClientesQuery : IRequest<IEnumerable<ClienteDto>>;
}
