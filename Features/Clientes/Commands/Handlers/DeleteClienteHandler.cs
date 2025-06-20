using Fact.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fact.Features.Clientes.Commands.Handlers
{
    public class DeleteClienteHandler : IRequestHandler<DeleteClienteCommand, bool>
    {
        private readonly AppDbContext _context;

        public DeleteClienteHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteClienteCommand request, CancellationToken cancellationToken)
        {
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (cliente == null)
                return false;

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
