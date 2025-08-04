using Microsoft.EntityFrameworkCore;

namespace backend
{
    public class PedidoContext : DbContext
    {
        public PedidoContext(DbContextOptions<PedidoContext> options) : base(options) { }

        public DbSet<Pedido> Pedidos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // modelBuilder
            //     .Entity<Pedido>()
            //     .Property(d => d.Status)
            //     .HasConversion<string>();
        }
    }
}
