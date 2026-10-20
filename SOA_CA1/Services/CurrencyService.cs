namespace SOA_CA1;

public class CurrencyService : ICurrencyService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public CurrencyService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["CURRENCY_API_KEY"]!;
    }

    public async Task<IReadOnlyList<Currency>> GetCurrenciesAsync(CancellationToken cancellationToken = default)
    {
        string url = $"symbols?access_key={Uri.EscapeDataString(_apiKey)}";
        var response = await _httpClient.GetFromJsonAsync<SymbolsResponse>(url, cancellationToken);
        return response?.Symbols
            .Select(symbol => new Currency(symbol.Key, symbol.Value))
            .Order()
            .ToList() ?? [];
    }

    public async Task<ConversionResult> ConvertAsync(string from, string to, decimal amount,
        CancellationToken cancellationToken = default)
    {
        string url = $"latest?access_key={Uri.EscapeDataString(_apiKey)}" +
                     $"&symbols={Uri.EscapeDataString(from)},{Uri.EscapeDataString(to)}";
        var response = await _httpClient.GetFromJsonAsync<LatestRatesResponse>(url, cancellationToken)
                       ?? throw new InvalidOperationException($"No rates returned for '{from}' to '{to}'.");

        decimal rate = CurrencyConverter.CrossRate(response.Rates, from, to);
        return new ConversionResult(from, to, amount, rate, CurrencyConverter.Convert(amount, rate), response.Date);
    }

    private sealed record SymbolsResponse(Dictionary<string, string> Symbols);

    private sealed record LatestRatesResponse(DateOnly Date, Dictionary<string, decimal> Rates);
}
