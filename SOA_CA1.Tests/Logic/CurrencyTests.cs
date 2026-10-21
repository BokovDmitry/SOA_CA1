namespace SOA_CA1.Tests.Logic;

public class CurrencyTests
{
    [Fact]
    public void CompareTo_Null_ReturnsPositive()
    {
        var currency = new Currency("EUR", "Euro");

        Assert.True(currency.CompareTo(null) > 0);
    }

    [Theory]
    [InlineData("AUD", "EUR", -1)]
    [InlineData("EUR", "AUD", 1)]
    [InlineData("EUR", "EUR", 0)]
    [InlineData("EUR", "eur", -1)]
    public void CompareTo_OtherCurrency_ComparesCodesOrdinally(string code, string otherCode, int expectedSign)
    {
        var currency = new Currency(code, "First");
        var other = new Currency(otherCode, "Second");

        Assert.Equal(expectedSign, Math.Sign(currency.CompareTo(other)));
    }

    [Fact]
    public void Order_UnsortedList_SortsByCode()
    {
        List<Currency> currencies =
        [
            new("USD", "US Dollar"),
            new("AUD", "Australian Dollar"),
            new("GBP", "British Pound"),
            new("EUR", "Euro")
        ];

        var codes = currencies.Order().Select(currency => currency.Code);

        Assert.Equal(["AUD", "EUR", "GBP", "USD"], codes);
    }
}
