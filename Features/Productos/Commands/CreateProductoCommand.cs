using Fact.Models;
using MediatR;

namespace Fact.Features.Productos.Commands
{
    public record CreateProductoCommand(
    string Nombre,
    string Descripcion,
    decimal Precio,
    string ImagenUrl) : IRequest<ProductoDto>;
}
