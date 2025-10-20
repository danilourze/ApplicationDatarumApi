using AplicationDatariumApi.Domain;
using AplicationDatariumApi.Domain.Interface;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace AplicationDatariumApi.Service
{
    public class PortfolioService : IPortfolioService
    {
        private readonly ApplicationDbContext _context;

        // JsonOptions para exportação
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public PortfolioService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Obter todos os portfólios
        public async Task<List<Portifolio>> GetAllAsync()
        {
            return await _context.Portifolios
                                 .Include(p => p.Assets)  // Incluindo os assets associados
                                 .OrderBy(p => p.Id)
                                 .ToListAsync();
        }

        // Obter portfólio por ID
        public async Task<Portifolio?> GetByIdAsync(long id)
        {
            return await _context.Portifolios
                                 .Include(p => p.Assets)  // Incluindo os assets associados
                                 .FirstOrDefaultAsync(p => p.Id == id);
        }

        // Criar um novo portfólio
        public async Task<Portifolio> CreateAsync(Portifolio input)
        {
            // Adiciona o novo portfólio ao contexto
            _context.Portifolios.Add(input);
            await _context.SaveChangesAsync();
            return input;
        }

        // Atualizar um portfólio existente
        public async Task UpdateAsync(Portifolio input)
        {
            var existingPortfolio = await _context.Portifolios
                                                  .Include(p => p.Assets)  // Incluindo os assets associados
                                                  .FirstOrDefaultAsync(p => p.Id == input.Id);

            if (existingPortfolio == null)
            {
                throw new KeyNotFoundException($"Portfolio {input.Id} não encontrado.");
            }

            // Atualize os campos conforme necessário
            existingPortfolio.Name = input.Name;
            existingPortfolio.RiskProfile = input.RiskProfile;
            existingPortfolio.TotalAmount = input.TotalAmount;
            existingPortfolio.Assets = input.Assets;  // Atualiza a lista de assets

            await _context.SaveChangesAsync();
        }

        // Deletar um portfólio pelo ID
        public async Task DeleteAsync(long id)
        {
            var portfolio = await _context.Portifolios
                                          .FirstOrDefaultAsync(p => p.Id == id);

            if (portfolio == null)
            {
                throw new KeyNotFoundException($"Portfolio {id} não encontrado.");
            }

            _context.Portifolios.Remove(portfolio);
            await _context.SaveChangesAsync();
        }

        // Exportar um portfólio como JSON
        public async Task<(byte[] content, string contentType, string fileName)> ExportAsync(long id)
        {
            var entity = await _context.Portifolios
                                       .Include(p => p.Assets)  // Incluindo os assets associados
                                       .FirstOrDefaultAsync(p => p.Id == id);

            if (entity == null)
                throw new KeyNotFoundException($"Portfolio {id} não encontrado.");

            var json = JsonSerializer.Serialize(entity, _jsonOptions);
            var bytes = Encoding.UTF8.GetBytes(json);
            var fileName = $"portfolio-{id}.json";

            return (bytes, "application/json", fileName);
        }

        // Importar um portfólio a partir de um JSON
        public async Task<Portifolio> ImportAsync(Stream jsonStream)
        {
            using var reader = new StreamReader(jsonStream, Encoding.UTF8);
            var json = await reader.ReadToEndAsync();

            var portfolio = JsonSerializer.Deserialize<Portifolio>(json, _jsonOptions)
                            ?? throw new InvalidOperationException("JSON de portfolio inválido.");

            // Criação de um novo portfólio
            _context.Portifolios.Add(portfolio);
            await _context.SaveChangesAsync();

            return portfolio;
        }
    }
}
