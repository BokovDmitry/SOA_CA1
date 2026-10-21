namespace SOA_CA1;

public interface ICryptoService
{
    Task<IReadOnlyList<Crypto>> GetCryptosAsync(int amount, CancellationToken cancellationToken = default);

    Task<Crypto> GetCryptoAsync(string id, CancellationToken cancellationToken = default);

    Task<List<ChartPoint>> GetChartPointsAsync(string id, ChartPeriod period,
        CancellationToken cancellationToken = default);
}
