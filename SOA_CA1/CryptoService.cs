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
    
    private sealed record CoinsResponse(List<Crypto> Result);
}