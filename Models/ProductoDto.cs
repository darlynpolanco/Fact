namespace Fact.Models
{
    public record ProductoDto(
    int Id,
    string Nombre,
    string Descripcion,
    decimal Precio,
    string ImagenUrl);
}
