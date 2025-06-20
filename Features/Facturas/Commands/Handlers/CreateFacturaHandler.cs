using Fact.Data.Entities;
using Fact.Data;
using Fact.Features.Facturas.Services;
using Fact.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fact.Features.Facturas.Commands.Handlers
{
    public class CreateFacturaHandler : IRequestHandler<CreateFacturaCommand, FacturaDto>
    {
        private readonly AppDbContext _context;
        private readonly IPdfGenerator _pdfGenerator;
        private readonly IEmailSender _emailSender;

        public CreateFacturaHandler(
            AppDbContext context,
            IPdfGenerator pdfGenerator,
            IEmailSender emailSender)
        {
            _context = context;
            _pdfGenerator = pdfGenerator;
            _emailSender = emailSender;
        }

        public async Task<FacturaDto> Handle(CreateFacturaCommand request, CancellationToken cancellationToken)
        {
            // Validar cliente
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.Id == request.ClienteId, cancellationToken);

            if (cliente == null)
                throw new ArgumentException("Cliente no encontrado");

            // Crear factura
            var factura = new Factura
            {
                ClienteId = request.ClienteId,
                Fecha = DateTime.UtcNow,
                Detalles = new List<DetalleFactura>()
            };

            // Procesar items
            foreach (var item in request.Items)
            {
                var producto = await _context.Productos
                    .FirstOrDefaultAsync(p => p.Id == item.ProductoId, cancellationToken);

                if (producto == null)
                    throw new ArgumentException($"Producto con ID {item.ProductoId} no encontrado");

                factura.Detalles.Add(new DetalleFactura
                {
                    ProductoId = item.ProductoId,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = producto.Precio
                });
            }

            // Calcular total
            factura.Total = factura.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario);

            // Guardar
            _context.Facturas.Add(factura);
            await _context.SaveChangesAsync(cancellationToken);

            // Cargar datos completos para DTO
            var facturaCompleta = await _context.Facturas
                .Include(f => f.Cliente)
                .Include(f => f.Detalles)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(f => f.Id == factura.Id, cancellationToken);

            // Generar DTO
            var facturaDto = new FacturaDto(
                facturaCompleta.Id,
                facturaCompleta.ClienteId,
                facturaCompleta.Cliente.Nombre,
                facturaCompleta.Cliente.Email,
                facturaCompleta.Fecha,
                facturaCompleta.Total,
                facturaCompleta.Detalles.Select(d => new DetalleFacturaDto(
                    d.ProductoId,
                    d.Producto.Nombre,
                    d.Cantidad,
                    d.PrecioUnitario,
                    d.Cantidad * d.PrecioUnitario
                )).ToList()
            );

            // Generar PDF
            var pdfBytes = _pdfGenerator.GenerateFacturaPdf(facturaCompleta);

            // Enviar email
            await _emailSender.SendFacturaEmailAsync(
                cliente.Email,
                cliente.Nombre,
                facturaDto,
                pdfBytes
            );

            return facturaDto;
        }
    }
}
