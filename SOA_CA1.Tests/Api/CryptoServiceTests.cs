using System.Net;

namespace SOA_CA1.Tests.Api;

public class CryptoServiceTests
{
    [Fact]
    public async Task GetCryptosAsync_ValidResponse_RequestsLimitAndReturnsCryptos()
    {
        var handler = new FakeHttpMessageHandler("""
            {
              "result": [
                { "id": "bitcoin", "name": "Bitcoin", "symbol": "BTC", "price": 60000.5, "priceChange1h": -0.2 },
                { "id": "ethereum", "name": "Ethereum", "symbol": "ETH", "price": 3000 }
              ]
            }
            """);
        var service = new CryptoService(handler.CreateClient());

        var cryptos = await service.GetCryptosAsync(2);

        Assert.Equal("/coins?limit=2", handler.LastRequestUri!.PathAndQuery);
        Assert.Equal(["bitcoin", "ethereum"], cryptos.Select(crypto => crypto.Id));
        Assert.Equal(60000.5m, cryptos[0].Price);
        Assert.Equal(-0.2m, cryptos[0].PriceChange1h);
    }

    [Theory]
    [InlineData(ChartPeriod.Week, "1w")]
    [InlineData(ChartPeriod.Month, "1m")]
    [InlineData(ChartPeriod.SixMonths, "6m")]
    public async Task GetChartPointsAsync_Period_RequestsMatchingApiPeriod(ChartPeriod period, string apiPeriod)
    {
        var handler = new FakeHttpMessageHandler("[]");
        var service = new CryptoService(handler.CreateClient());

        await service.GetChartPointsAsync("bitcoin", period);

        Assert.Equal($"/coins/bitcoin/charts?period={apiPeriod}", handler.LastRequestUri!.PathAndQuery);
    }

    [Fact]
    public async Task GetChartPointsAsync_ValidRows_ConvertsUnixTimeAndSkipsIncompleteRows()
    {
        var handler = new FakeHttpMessageHandler("[[1767225600, 60000.5, 1, 2], [1767312000], [1767312000, 61000]]");
        var service = new CryptoService(handler.CreateClient());

        var points = await service.GetChartPointsAsync("bitcoin", ChartPeriod.Week);

        Assert.Equal(
        [
            new ChartPoint(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), 60000.5m),
            new ChartPoint(new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc), 61000m)
        ], points);
    }
}
