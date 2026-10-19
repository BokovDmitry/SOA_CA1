namespace SOA_CA1;

public interface ICryptoService
{
    Task<IReadOnlyList<string>> GetCryptoIdsAsync(int ids_amount, CancellationToken cancellationToken = default);

    Task<Crypto> GetCryptoAsync(string id, CancellationToken cancellationToken = default);
}