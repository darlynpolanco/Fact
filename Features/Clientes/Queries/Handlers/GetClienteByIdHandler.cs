using Fact.Data;
using Fact.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fact.Features.Clientes.Queries.Handlers
{
    public class GetClienteByIdHandler : IRequestHandler<GetClienteByIdQuery, ClienteDto?>
    {
        private readonly AppDbContext _context;

        public GetClienteByIdHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ClienteDto?> Handle(GetClienteByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Clientes
                .Where(c => c.Id == request.Id)
                .Select(c => new ClienteDto(
                    c.Id,
                    c.Nombre,
                    c.Email,
                    c.Telefono,
                    c.Direccion))
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
