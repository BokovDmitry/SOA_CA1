using System.Net;
using Microsoft.Extensions.Configuration;

namespace SOA_CA1.Tests.Api;

public class CurrencyServiceTests
{
    private static CurrencyService CreateService(FakeHttpMessageHandler handler)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["CURRENCY_API_KEY"] = "test-key" })
            .Build();
        return new CurrencyService(handler.CreateClient(), configuration);
    }

    [Fact]
    public async Task GetCurrenciesAsync_ValidResponse_ReturnsCurrenciesSortedByCode()
    {
        var handler = new FakeHttpMessageHandler("""
            {
              "success": true,
              "symbols": { "USD": "United States Dollar", "AUD": "Australian Dollar", "EUR": "Euro" }
            }
            """);

        var currencies = await CreateService(handler).GetCurrenciesAsync();

        Assert.Equal("/symbols?access_key=test-key", handler.LastRequestUri!.PathAndQuery);
        Assert.Equal(["AUD", "EUR", "USD"], currencies.Select(currency => currency.Code));
        Assert.Equal("Australian Dollar", currencies[0].Name);
    }

    [Fact]
    public async Task ConvertAsync_ValidResponse_ReturnsConvertedResult()
    {
        var handler = new FakeHttpMessageHandler("""
            {
              "success": true,
              "base": "EUR",
              "date": "2026-10-21",
              "rates": { "USD": 2, "GBP": 0.5 }
            }
            """);

        var result = await CreateService(handler).ConvertAsync("USD", "GBP", 100m);

        Assert.Equal("/latest?access_key=test-key&symbols=USD,GBP", handler.LastRequestUri!.PathAndQuery);
        Assert.Equal(new ConversionResult("USD", "GBP", 100m, 0.25m, 25m, new DateOnly(2026, 10, 21)), result);
    }
    
}
