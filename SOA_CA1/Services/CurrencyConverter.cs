namespace SOA_CA1;

public static class CurrencyConverter
{
    public const string BaseCurrency = "EUR";

    public static decimal CrossRate(IReadOnlyDictionary<string, decimal> baseRates, string from, string to)
    {
        return RateFromBase(baseRates, to) / RateFromBase(baseRates, from);
    }

    public static decimal Convert(decimal amount, decimal rate)
    {
        return amount * rate;
    }

    private static decimal RateFromBase(IReadOnlyDictionary<string, decimal> baseRates, string code)
    {
        if (code == BaseCurrency)
        {
            return 1m;
        }

        return baseRates[code];
    }
}
