using DotNetEnv;

namespace SOA_CA1;

public class CryptoService : ICryptoService
{
    
    private readonly HttpClient _httpClient;
    
    public CryptoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<IReadOnlyList<string>> GetCryptoIdsAsync(int ids_amount,
        CancellationToken cancellationToken = default)
    {
        string url = $"coins?limit={ids_amount}";
        var response = await _httpClient.GetFromJsonAsync<CoinsResponse>(url, cancellationToken);
        return response?.Result?.Select(coin => coin.Id).ToList() ?? [];
    }
    
    public async Task<Crypto> GetCryptoAsync(string id, CancellationToken cancellationToken = default)
    {
        string url = $"coins/{Uri.EscapeDataString(id)}";
        return await _httpClient.GetFromJsonAsync<Crypto>(url, cancellationToken)
               ?? throw new InvalidOperationException($"No data returned for '{id}'.");
    }
    
    private sealed record CoinsResponse(List<Crypto> Result);
}