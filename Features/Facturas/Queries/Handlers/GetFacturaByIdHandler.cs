using Fact.Data;
using Fact.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fact.Features.Facturas.Queries.Handlers
{   
        public class GetFacturaByIdHandler : IRequestHandler<GetFacturaByIdQuery, FacturaDto?>
        {
            private readonly AppDbContext _context;

            public GetFacturaByIdHandler(AppDbContext context)
            {
                _context = context;
            }

            public async Task<FacturaDto?> Handle(GetFacturaByIdQuery request, CancellationToken cancellationToken)
            {
                var factura = await _context.Facturas
                    .Include(f => f.Cliente)
                    .Include(f => f.Detalles)
                        .ThenInclude(d => d.Producto)
                    .FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken);

                if (factura == null) return null;

                return new FacturaDto(
                    factura.Id,
                    factura.ClienteId,
                    factura.Cliente.Nombre,
                    factura.Cliente.Email,
                    factura.Fecha,
                    factura.Total,
                    factura.Detalles.Select(d => new DetalleFacturaDto(
                        d.ProductoId,
                        d.Producto.Nombre,
                        d.Cantidad,
                        d.PrecioUnitario,
                        d.Cantidad * d.PrecioUnitario
                    )).ToList()
                );
            }
        }
    }

