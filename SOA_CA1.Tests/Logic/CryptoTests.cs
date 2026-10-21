namespace SOA_CA1.Tests.Logic;

public class CryptoTests
{
    public static TheoryData<decimal, decimal> NonNegativeValues => new()
    {
        { -5m, 0m },
        { 0m, 0m },
        { 12.5m, 12.5m }
    };

    public static TheoryData<decimal, decimal, decimal> PriceChangeAmounts => new()
    {
        { 110m, 10m, 10m },
        { 90m, -10m, -10m },
        { 100m, 0m, 0m }
    };

    [Theory]
    [MemberData(nameof(NonNegativeValues))]
    public void AmountSetters_ClampNegativeValuesToZero(decimal value, decimal expected)
    {
        var crypto = new Crypto
        {
            Price = value,
            Volume = value,
            MarketCap = value,
            TotalSupply = value,
            AllTimeHigh = value,
            AllTimeLow = value
        };

        Assert.Equal(expected, crypto.Price);
        Assert.Equal(expected, crypto.Volume);
        Assert.Equal(expected, crypto.MarketCap);
        Assert.Equal(expected, crypto.TotalSupply);
        Assert.Equal(expected, crypto.AllTimeHigh);
        Assert.Equal(expected, crypto.AllTimeLow);
    }

    [Theory]
    [InlineData(-100)]
    [InlineData(-150.5)]
    public void PriceChange1h_AtOrBelowMinusHundred_Throws(double change)
    {
        var crypto = new Crypto();

        Assert.Throws<ArgumentOutOfRangeException>(() => crypto.PriceChange1h = (decimal)change);
    }

    [Theory]
    [MemberData(nameof(PriceChangeAmounts))]
    public void PriceChange1hAmount_ReturnsDifferenceFromPriceOneHourAgo(decimal price, decimal change,
        decimal expected)
    {
        var crypto = new Crypto { Price = price, PriceChange1h = change };

        Assert.Equal(expected, crypto.PriceChange1hAmount);
    }
}
