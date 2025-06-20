using Fact.Data.Entities;
using Fact.Data;
using Fact.Models;
using MediatR;

namespace Fact.Features.Productos.Commands.Handlers
{
    public class CreateProductoHandler : IRequestHandler<CreateProductoCommand, ProductoDto>
    {
        private readonly AppDbContext _context;

        public CreateProductoHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProductoDto> Handle(CreateProductoCommand request, CancellationToken cancellationToken)
        {
            var producto = new Producto
            {
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,
                Precio = request.Precio,
                ImagenUrl = request.ImagenUrl
            };

            _context.Productos.Add(producto);
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
