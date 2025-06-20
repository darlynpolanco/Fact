using Fact.Data;
using Fact.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fact.Features.Clientes.Commands.Handlers
{
    public class UpdateClienteHandler : IRequestHandler<UpdateClienteCommand, ClienteDto>
    {
        private readonly AppDbContext _context;

        public UpdateClienteHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ClienteDto> Handle(UpdateClienteCommand request, CancellationToken cancellationToken)
        {
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (cliente == null)
                throw new KeyNotFoundException($"Cliente con ID {request.Id} no encontrado");

            cliente.Nombre = request.Nombre;
            cliente.Email = request.Email;
            cliente.Telefono = request.Telefono;
            cliente.Direccion = request.Direccion;

            await _context.SaveChangesAsync(cancellationToken);

            return new ClienteDto(
                cliente.Id,
                cliente.Nombre,
                cliente.Email,
                cliente.Telefono,
                cliente.Direccion);
        }
    }
}
