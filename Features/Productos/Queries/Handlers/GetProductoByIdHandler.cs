using Fact.Data;
using Fact.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fact.Features.Productos.Queries.Handlers
{
    public class GetProductoByIdHandler : IRequestHandler<GetProductoByIdQuery, ProductoDto?>
    {
        private readonly AppDbContext _context;

        public GetProductoByIdHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProductoDto?> Handle(GetProductoByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Productos
                .Where(p => p.Id == request.Id)
                .Select(p => new ProductoDto(
                    p.Id,
                    p.Nombre,
                    p.Descripcion,
                    p.Precio,
                    p.ImagenUrl))
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
