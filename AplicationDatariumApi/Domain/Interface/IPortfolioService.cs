namespace AplicationDatariumApi.Domain.Interface;

public interface IPortfolioService
{
    Task<List<Portifolio>> GetAllAsync();
    Task<Portifolio?> GetByIdAsync(long id);
    Task<Portifolio> CreateAsync(Portifolio input);
    Task UpdateAsync(Portifolio input);
    Task DeleteAsync(long id);

    // Manipulação de arquivos JSON
    Task<(byte[] content, string contentType, string fileName)> ExportAsync(long id);
    Task<Portifolio> ImportAsync(Stream jsonStream);
}