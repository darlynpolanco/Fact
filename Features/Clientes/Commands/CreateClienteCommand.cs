using Fact.Models;
using MediatR;

namespace Fact.Features.Clientes.Commands
{
    public record CreateClienteCommand(
    string Nombre,
    string Email,
    string Telefono,
    string Direccion) : IRequest<ClienteDto>;
}
