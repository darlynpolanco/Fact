using Fact.Data;
using Fact.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fact.Features.Productos.Commands.Handlers
{
    public class UpdateProductoHandler : IRequestHandler<UpdateProductoCommand, ProductoDto>
    {
        private readonly AppDbContext _context;

        public UpdateProductoHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProductoDto> Handle(UpdateProductoCommand request, CancellationToken cancellationToken)
        {
            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (producto == null)
                throw new KeyNotFoundException($"Producto con ID {request.Id} no encontrado");

            producto.Nombre = request.Nombre;
            producto.Descripcion = request.Descripcion;
            producto.Precio = request.Precio;
            producto.ImagenUrl = request.ImagenUrl;

            await _context.SaveChangesAsync(cancellationToken);

            return new ProductoDto(
                producto.Id,
                producto.Nombre,
                producto.Descripcion,
                producto.Precio,
                producto.ImagenUrl);
        }
    }
}
