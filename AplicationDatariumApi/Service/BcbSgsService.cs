using System.Net.Http.Headers;
using System.Text.Json;
using AplicationDatariumApi.Domain.Interface;

namespace AplicationDatariumApi.Service;

public class BcbSgsService : IEconomicIndicatorService
{
    private readonly HttpClient _http;
    private const string BaseUrl = "https://api.bcb.gov.br/";
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public BcbSgsService(HttpClient http)
    {
        _http = http;

        // Headers para evitar 406
        if (!_http.DefaultRequestHeaders.Accept.Any())
            _http.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        if (!_http.DefaultRequestHeaders.UserAgent.Any())
            _http.DefaultRequestHeaders.UserAgent.ParseAdd("AplicationDatariumApi/1.0 (+https://localhost)");
    }

    public async Task<decimal?> GetSelicDailyAsync(DateTime? date = null, CancellationToken ct = default)
    {
        // Se vier data, busca intervalo fechado; senão, busca geral e pega o último
        string uri = date.HasValue
            ? $"{BaseUrl}dados/serie/bcdata.sgs.432/dados?formato=json&dataInicial={date:dd/MM/yyyy}&dataFinal={date:dd/MM/yyyy}"
            : $"{BaseUrl}dados/serie/bcdata.sgs.432/dados?formato=json";

        var arr = await GetArrayWithFallbackAsync(uri, ct);
        if (arr.Count == 0) return null;

        // Se foi intervalo fechado, vem 0 ou 1 item. Senão, pega o último.
        var item = date.HasValue ? arr.LastOrDefault() : arr.Last();
        return item?.TryGetValor();
    }

    public async Task<IReadOnlyList<(DateTime date, decimal value)>> GetSelicSeriesAsync(
        DateTime start, DateTime end, CancellationToken ct = default)
    {
        var uri = $"{BaseUrl}dados/serie/bcdata.sgs.432/dados?formato=json&dataInicial={start:dd/MM/yyyy}&dataFinal={end:dd/MM/yyyy}";
        var arr = await GetArrayWithFallbackAsync(uri, ct);

        return arr
            .Where(x => x.TryGetValor().HasValue)
            .Select(x => (DateTime.ParseExact(x.data, "dd/MM/yyyy", null), x.TryGetValor()!.Value))
            .ToList();
    }

    // --- helpers ---
    private async Task<List<SgsItem>> GetArrayWithFallbackAsync(string uri, CancellationToken ct)
    {
        using var resp = await _http.GetAsync(uri, ct);

        // Alguns ambientes retornam 406 com esse path; tenta rota alternativa
        if ((int)resp.StatusCode == 406)
        {
            var alt = ToAltRoute(uri); // troca /dados/serie/bcdata.sgs.432/dados -> /sgs/series/432/dados
            using var respAlt = await _http.GetAsync(alt, ct);
            respAlt.EnsureSuccessStatusCode();
            var jsonAlt = await respAlt.Content.ReadAsStringAsync(ct);
            return JsonSerializer.Deserialize<List<SgsItem>>(jsonAlt, JsonOpts) ?? [];
        }

        resp.EnsureSuccessStatusCode();
        var json = await resp.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<List<SgsItem>>(json, JsonOpts) ?? [];
    }

    private static string ToAltRoute(string original)
    {
        // Converte o caminho conhecido para a rota alternativa do SGS
        // de: https://api.bcb.gov.br/dados/serie/bcdata.sgs.432/dados?... 
        // para: https://api.bcb.gov.br/sgs/series/432/dados?...
        var idxSerie = original.IndexOf("bcdata.sgs.", StringComparison.OrdinalIgnoreCase);
        if (idxSerie >= 0)
        {
            var start = idxSerie + "bcdata.sgs.".Length;
            var dot = original.IndexOf('.', start);
            var seriesId = original.Substring(start, dot - start); // "432"
            var queryIdx = original.IndexOf('?', dot);
            var query = queryIdx >= 0 ? original.Substring(queryIdx) : string.Empty;
            return $"{BaseUrl}sgs/series/{seriesId}/dados{query}";
        }
        return original; // fallback
    }

    private record SgsItem(string data, string valor)
    {
        public decimal? TryGetValor()
            => decimal.TryParse(valor.Replace(',', '.'),
                                System.Globalization.CultureInfo.InvariantCulture,
                                out var v) ? v : null;
    }
}
