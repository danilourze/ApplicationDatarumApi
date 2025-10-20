using Microsoft.EntityFrameworkCore;

namespace AplicationDatariumApi.Domain
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Portifolio> Portifolios { get; set; }
        public DbSet<Asset> Assets { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração do enum RiskProfile (usando int para armazenar o valor do enum no banco)
            modelBuilder.Entity<Portifolio>()
                .Property(p => p.RiskProfile)
                .HasConversion<int>();

        }
    }
}
