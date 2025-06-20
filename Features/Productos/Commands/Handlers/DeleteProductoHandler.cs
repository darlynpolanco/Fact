using Fact.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fact.Features.Productos.Commands.Handlers
{
    public class DeleteProductoHandler : IRequestHandler<DeleteProductoCommand, bool>
    {
        private readonly AppDbContext _context;

        public DeleteProductoHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteProductoCommand request, CancellationToken cancellationToken)
        {
            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (producto == null)
                return false;

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
