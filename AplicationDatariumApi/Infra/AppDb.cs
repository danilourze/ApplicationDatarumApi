using System.Collections.Generic;
using System.Text.Json;
using AplicationDatariumApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace AplicationDatariumApi.Infra;

public class AppDb : DbContext
{
    public AppDb(DbContextOptions<AppDb> options) : base(options) { }

    public DbSet<Portifolio> Portfolios => Set<Portifolio>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
 
        modelBuilder.Entity<Portifolio>(e =>
        {
            e.ToTable("portfolios");
            e.HasKey(p => p.Id);

            e.Property(p => p.Assets)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<Asset>>(v, (JsonSerializerOptions?)null)!
                )
                .HasColumnType("jsonb"); // pode continuar jsonb; o driver envia string e o Postgres valida
        });
    }
}