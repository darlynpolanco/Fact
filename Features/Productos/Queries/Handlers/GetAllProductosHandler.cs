using Fact.Data;
using Fact.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fact.Features.Productos.Queries.Handlers
{
    public class GetAllProductosHandler : IRequestHandler<GetAllProductosQuery, IEnumerable<ProductoDto>>
    {
        private readonly AppDbContext _context;

        public GetAllProductosHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductoDto>> Handle(GetAllProductosQuery request, CancellationToken cancellationToken)
        {
            return await _context.Productos
                .Select(p => new ProductoDto(
                    p.Id,
                    p.Nombre,
                    p.Descripcion,
                    p.Precio,
                    p.ImagenUrl))
                .ToListAsync(cancellationToken);
        }
    }
}
