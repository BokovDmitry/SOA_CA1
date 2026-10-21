namespace SOA_CA1.Tests.Logic;

public class CurrencyConverterTests
{
    private static readonly IReadOnlyDictionary<string, decimal> BaseRates = new Dictionary<string, decimal>
    {
        ["USD"] = 2m,
        ["GBP"] = 0.5m,
        ["JPY"] = 160m
    };

    public static TheoryData<decimal, decimal, decimal> Conversions => new()
    {
        { 100m, 1.1m, 110m },
        { 0m, 1.5m, 0m },
        { 2.5m, 0.4m, 1.0m }
    };

    [Theory]
    [InlineData("USD", 2)]
    [InlineData("GBP", 0.5)]
    public void CrossRate_FromBaseCurrency_ReturnsTargetRate(string to, double expected)
    {
        decimal rate = CurrencyConverter.CrossRate(BaseRates, CurrencyConverter.BaseCurrency, to);

        Assert.Equal((decimal)expected, rate);
    }

    [Fact]
    public void CrossRate_ToBaseCurrency_ReturnsInverseRate()
    {
        decimal rate = CurrencyConverter.CrossRate(BaseRates, "USD", CurrencyConverter.BaseCurrency);

        Assert.Equal(0.5m, rate);
    }

    [Theory]
    [InlineData("USD", "GBP", 0.25)]
    [InlineData("GBP", "USD", 4)]
    [InlineData("USD", "JPY", 80)]
    public void CrossRate_BetweenNonBaseCurrencies_DividesTargetByFrom(string from, string to, double expected)
    {
        decimal rate = CurrencyConverter.CrossRate(BaseRates, from, to);

        Assert.Equal((decimal)expected, rate);
    }

    [Theory]
    [InlineData("XYZ", "USD")]
    [InlineData("USD", "XYZ")]
    public void CrossRate_UnknownCurrency_Throws(string from, string to)
    {
        Assert.Throws<KeyNotFoundException>(() => CurrencyConverter.CrossRate(BaseRates, from, to));
    }

    [Theory]
    [MemberData(nameof(Conversions))]
    public void Convert_MultipliesAmountByRate(decimal amount, decimal rate, decimal expected)
    {
        Assert.Equal(expected, CurrencyConverter.Convert(amount, rate));
    }
}
