namespace Fact.Data.Entities
{
    public class Factura
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public int ClienteId { get; set; }
        public decimal Total { get; set; }
        public Cliente Cliente { get; set; } = null!;
        public List<DetalleFactura> Detalles { get; set; } = new();
    }

}
