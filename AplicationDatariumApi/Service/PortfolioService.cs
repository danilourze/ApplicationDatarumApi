using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using AplicationDatariumApi.Domain;
using AplicationDatariumApi.Domain.Interface;

namespace AplicationDatariumApi.Service;

public class PortfolioService : IPortfolioService
{
    private readonly ConcurrentDictionary<long, Portifolio> _store = new();
    private long _seq = 0;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public PortfolioService() { }

    public Task<List<Portifolio>> GetAllAsync()
    {
        var list = _store.Values.OrderBy(p => p.Id).ToList();
        return Task.FromResult(list);
    }

    public Task<Portifolio?> GetByIdAsync(long id)
    {
        _store.TryGetValue(id, out var p);
        return Task.FromResult(p);
    }

    public Task<Portifolio> CreateAsync(Portifolio input)
    {
        var id = Interlocked.Increment(ref _seq);
        var clone = DeepClone(input);
        clone.Id = id;
        _store[id] = clone;
        return Task.FromResult(clone);
    }

    public Task UpdateAsync(Portifolio input)
    {
        if (!_store.ContainsKey(input.Id))
            throw new KeyNotFoundException($"Portfolio {input.Id} não encontrado.");

        var clone = DeepClone(input);
        _store[input.Id] = clone;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(long id)
    {
        _store.TryRemove(id, out _);
        return Task.CompletedTask;
    }

    public Task<(byte[] content, string contentType, string fileName)> ExportAsync(long id)
    {
        if (!_store.TryGetValue(id, out var entity))
            throw new KeyNotFoundException($"Portfolio {id} não encontrado.");

        var json = JsonSerializer.Serialize(entity, _jsonOptions);
        var bytes = Encoding.UTF8.GetBytes(json);
        var fileName = $"portfolio-{id}.json";
        return Task.FromResult<(byte[], string, string)>((bytes, "application/json", fileName));
    }

    public async Task<Portifolio> ImportAsync(Stream jsonStream)
    {
        using var reader = new StreamReader(jsonStream, Encoding.UTF8);
        var json = await reader.ReadToEndAsync();

        var portfolio = JsonSerializer.Deserialize<Portifolio>(json, _jsonOptions)
                        ?? throw new InvalidOperationException("JSON de portfolio inválido.");

        portfolio.Id = 0; // sempre cria novo registro
        return await CreateAsync(portfolio);
    }

    private static Portifolio DeepClone(Portifolio p)
    {
        var json = JsonSerializer.Serialize(p);
        return JsonSerializer.Deserialize<Portifolio>(json)!;
    }
}
