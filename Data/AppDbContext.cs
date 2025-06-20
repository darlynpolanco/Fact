using Fact.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fact.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options){}

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<DetalleFactura> DetallesFactura { get; set; }
        //public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<DetalleFactura>()
            //    .Ignore(d => d.Total);

            // Configurar precisión para Producto
            modelBuilder.Entity<Producto>()
                .Property(p => p.Precio)
                .HasColumnType("decimal(18,2)");

            // Configurar precisión para DetalleFactura
            modelBuilder.Entity<DetalleFactura>()
                .Property(d => d.PrecioUnitario)
                .HasColumnType("decimal(18,2)");

        }
    }
}
