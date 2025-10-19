namespace AplicationDatariumApi.Domain.Interface
{
    public interface IEconomicIndicatorService
    {

        Task<decimal?> GetSelicDailyAsync(DateTime? date = null, CancellationToken ct = default);
        Task<IReadOnlyList<(DateTime date, decimal value)>> GetSelicSeriesAsync(
            DateTime start, DateTime end, CancellationToken ct = default);
    }
}
