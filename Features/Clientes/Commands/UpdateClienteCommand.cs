using Fact.Models;
using MediatR;

namespace Fact.Features.Clientes.Commands
{
    public record UpdateClienteCommand(
    int Id,
    string Nombre,
    string Email,
    string Telefono,
    string Direccion) : IRequest<ClienteDto>;
}
