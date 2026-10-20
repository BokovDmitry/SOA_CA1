using DotNetEnv;

namespace SOA_CA1;

public class CryptoService : ICryptoService
{
    
    private readonly HttpClient _httpClient;
    
    public CryptoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<IReadOnlyList<Crypto>> GetCryptosAsync(int amount,
        CancellationToken cancellationToken = default)
    {
        string url = $"coins?limit={amount}";
        var response = await _httpClient.GetFromJsonAsync<CoinsResponse>(url, cancellationToken);
        return response?.Result ?? [];
    }
    
    public async Task<Crypto> GetCryptoAsync(string id, CancellationToken cancellationToken = default)
    {
        string url = $"coins/{Uri.EscapeDataString(id)}";
        return await _httpClient.GetFromJsonAsync<Crypto>(url, cancellationToken)
               ?? throw new InvalidOperationException($"No data returned for '{id}'.");
    }

    public async Task<List<ChartPoint>> GetChartPointsAsync(string id, ChartPeriod period,
        CancellationToken cancellationToken = default)
    {
        string apiPeriod = period switch
        {
            ChartPeriod.Week => "1w",
            ChartPeriod.Month => "1m",
            ChartPeriod.SixMonths => "6m",
            _ => throw new ArgumentOutOfRangeException(nameof(period), period, "Unsupported chart period.")
        };
        string url = $"coins/{Uri.EscapeDataString(id)}/charts?period={apiPeriod}";
        
        var rows = await _httpClient.GetFromJsonAsync<List<decimal[]>>(url, cancellationToken)
                   ?? throw new InvalidOperationException($"No chart data returned for '{id}'.");
        return rows
            .Where(row => row.Length >= 2)
            .Select(row => new ChartPoint(DateTimeOffset.FromUnixTimeSeconds((long)row[0]).UtcDateTime, row[1]))
            .ToList();
    }

    private sealed record CoinsResponse(List<Crypto> Result);
}