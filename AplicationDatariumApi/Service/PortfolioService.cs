using System.Text;
using System.Text.Json;
using AplicationDatariumApi.Domain;
using AplicationDatariumApi.Domain.Interface;
using AplicationDatariumApi.Infra;
using Microsoft.EntityFrameworkCore;

namespace AplicationDatariumApi.Service;

public class PortfolioService : IPortfolioService
{
    private readonly AppDb _db;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public PortfolioService(AppDb db) => _db = db;

    public async Task<List<Portifolio>> GetAllAsync()
        => await _db.Portfolios.AsNoTracking().ToListAsync();

    public async Task<Portifolio?> GetByIdAsync(long id)
        => await _db.Portfolios.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Portifolio> CreateAsync(Portifolio input)
    {
        _db.Portfolios.Add(input);
        await _db.SaveChangesAsync();
        return input;
    }

    public async Task UpdateAsync(Portifolio input)
    {
        var existing = await _db.Portfolios.FindAsync(input.Id);
        if (existing is null)
            throw new KeyNotFoundException($"Portfolio {input.Id} não encontrado.");

        existing.Name = input.Name;
        existing.RiskProfile = input.RiskProfile;
        existing.TotalAmount = input.TotalAmount;
        existing.Assets = input.Assets;

        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var existing = await _db.Portfolios.FindAsync(id);
        if (existing is null)
            throw new KeyNotFoundException($"Portfolio {id} não encontrado.");

        _db.Portfolios.Remove(existing);
        await _db.SaveChangesAsync();
    }

    public async Task<(byte[] content, string contentType, string fileName)> ExportAsync(long id)
    {
        var entity = await _db.Portfolios.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (entity is null)
            throw new KeyNotFoundException($"Portfolio {id} não encontrado.");

        var json = System.Text.Json.JsonSerializer.Serialize(entity, _jsonOptions);
        var bytes = Encoding.UTF8.GetBytes(json);
        var fileName = $"portfolio-{id}.json";
        return (bytes, "application/json", fileName);
    }

    public async Task<Portifolio> ImportAsync(Stream jsonStream)
    {
        using var reader = new StreamReader(jsonStream, Encoding.UTF8);
        var json = await reader.ReadToEndAsync();

        var portfolio = System.Text.Json.JsonSerializer.Deserialize<Portifolio>(json, _jsonOptions);
        if (portfolio is null)
            throw new InvalidOperationException("JSON de portfolio inválido.");

        portfolio.Id = 0; // insere como novo
        _db.Portfolios.Add(portfolio);
        await _db.SaveChangesAsync();
        return portfolio;
    }
}
