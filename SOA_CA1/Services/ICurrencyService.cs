namespace SOA_CA1;

public interface ICurrencyService
{
    Task<IReadOnlyList<Currency>> GetCurrenciesAsync(CancellationToken cancellationToken = default);

    Task<ConversionResult> ConvertAsync(string from, string to, decimal amount,
        CancellationToken cancellationToken = default);
}
