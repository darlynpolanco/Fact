using Fact.Data;
using Fact.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fact.Features.Clientes.Queries.Handlers
{
    public class GetAllClientesHandler : IRequestHandler<GetAllClientesQuery, IEnumerable<ClienteDto>>
    {
        private readonly AppDbContext _context;

        public GetAllClientesHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ClienteDto>> Handle(GetAllClientesQuery request, CancellationToken cancellationToken)
        {
            return await _context.Clientes
                .Select(c => new ClienteDto(
                    c.Id,
                    c.Nombre,
                    c.Email,
                    c.Telefono,
                    c.Direccion))
                .ToListAsync(cancellationToken);
        }
    }
}
