namespace SOA_CA1;

public interface ICryptoService
{
    Task<IReadOnlyList<Crypto>> GetCryptosAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);

    Task<Crypto> GetCryptoAsync(string id, CancellationToken cancellationToken = default);
}