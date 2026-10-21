# Xchangeit

Xchangeit is a Blazor Server web app for following the crypto market and converting between world currencies. It combines two third-party web services:

| Provider | Used for |
|---|---|
| [CoinStats API](https://docs.coinstats.app/) | Coin list, live coin details (price, 1h change, all-time high/low, volume, market cap) and price history charts |
| [exchangeratesapi.io](https://exchangeratesapi.io/documentation/) | List of supported currencies and the latest exchange rates |

## Features

### Crypto Market (`/`)
- Choose a coin from a list of the top cryptocurrencies by market cap.
- See the current price and the 1-hour change as both an amount and a percentage, shown green for a rise and red for a fall.
- View a price chart for 1 week, 1 month or 6 months. The chart colour follows the trend over that period.
- See all-time high, all-time low, 24h volume and market cap, with large values shortened (e.g. `$1.66T`).

### Currency Converter (`/currencies`)
- Convert any amount between the supported currencies, listed in alphabetical order by code.
- Swap the two currencies with one click.
- See the exchange rate used and the date of the rates.
- The API returns rates against EUR, so rates between other pairs are calculated as cross rates (e.g. USD → GBP = `rate(GBP) / rate(USD)`).

### Error handling
- If an API call fails or returns data that can't be read, a friendly message appears on the page instead of the app crashing.
- Required configuration is checked at start-up, and the app stops with a clear message saying which value is missing.
- The coin and currency lists are fetched once and saved to `SOA_CA1/Data/*.json`. If that fetch fails, a warning is logged and the page explains how to retry.

## Running the app

### Requirements
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- A CoinStats API key and an exchangeratesapi.io access key

### Configuration
API settings are read from `SOA_CA1/.env`, which is not committed to the repository. Create it with these keys:

```env
CRYPTO_API_URI="https://api.coinstats.app/v1/"
CRYPTO_API_KEY="<your CoinStats API key>"
CRYPTO_IDS_NUMBER=10
CURRENCY_API_URI="https://api.exchangeratesapi.io/v1/"
CURRENCY_API_KEY="<your exchangeratesapi.io access key>"
```

`CRYPTO_IDS_NUMBER` sets how many coins appear in the Crypto Market dropdown.

### Seed data
`SOA_CA1/Data/cryptos.json` and `SOA_CA1/Data/currencies.json` are included, so the dropdowns work immediately. To download fresh lists, delete either file and restart the app. It will be fetched again from the API.

### Run
```bash
dotnet run --project SOA_CA1
```
Then open the URL shown in the console.

## Tests

```bash
dotnet test
```

The `SOA_CA1.Tests` project uses xUnit and is split into two parts:

- **`Logic/`**: pure logic with no network access.
  - `CurrencyConverter`: cross-rate calculation and amount conversion.
  - `Currency`: sorting by code.
  - `Crypto`: negative values set to 0, the 1h change limit, and the 1h change amount.
- **`Api/`**: `CryptoService` and `CurrencyService` tested against a `FakeHttpMessageHandler` that returns fixed JSON, so no real API is called. The tests check that each service:
  - builds the correct request URL;
  - parses the response into coins, chart points, sorted currencies and conversion results.

Theories (`[Theory]` with `[InlineData]` / `[MemberData]`) run the same test across several inputs and edge cases.

## Project structure

```
SOA_CA1/
├── Components/
│   ├── Layout/         Header navigation and page layout
│   └── Pages/          CryptoMarket and Currencies pages, with scoped CSS
├── Data/               Seeded coin and currency lists (JSON)
├── Models/             Crypto, Currency, ChartPoint, ConversionResult, ChartPeriod
├── Services/           API service interfaces and implementations, CurrencyConverter, DataFileSeeder
└── Program.cs          Configuration checks, dependency injection, typed HttpClients, start-up seeding
SOA_CA1.Tests/
├── Logic/              Pure logic tests
└── Api/                Service tests using a fake HTTP handler
```

### C# and design highlights
- **Interfaces and dependency injection**: pages depend on `ICryptoService` and `ICurrencyService`, never on `HttpClient` directly.
- **Typed `HttpClient`s**: each service receives a client already set up with its API's base address (and the API key header for CoinStats).
- **Encapsulation**: `Crypto` properties validate their values. Negative amounts become 0, and an impossible 1h change throws an exception.
- **Sorting and comparison**: `Currency` implements `IComparable<Currency>`, so the currency list can be sorted with `Order()`.
- **Records**: `ChartPoint` and `ConversionResult` are immutable records with value equality.
- **Enums and switch expressions**: `ChartPeriod` is mapped to the API's period codes with a switch expression.
- **Generics**: `DataFileSeeder.SeedFileAsync<T>` seeds any JSON data file.
- **LINQ**: used to filter and map chart rows and to sort currencies.
- **`decimal`**: used for all money values, to avoid floating-point rounding errors.
- **Exception filters**: `catch (...) when (...)` catches only the failures each part of the app expects.

## References

### Web services
- [CoinStats API documentation](https://docs.coinstats.app/)
- [exchangeratesapi.io documentation](https://exchangeratesapi.io/documentation/)

### Libraries and assets
- [Blazor-ApexCharts](https://github.com/apexcharts/Blazor-ApexCharts): crypto price chart
- [DotNetEnv](https://github.com/tonerdo/dotnet-env): loads `.env` into configuration
- [Bootstrap](https://getbootstrap.com/): base styles from the Blazor template
- [Google Fonts: Inter](https://fonts.google.com/specimen/Inter) and [Plus Jakarta Sans](https://fonts.google.com/specimen/Plus+Jakarta+Sans)
- [Google Material Symbols: swap_vert](https://fonts.google.com/icons?selected=Material+Symbols+Outlined:swap_vert): swap button icon
- [xUnit](https://xunit.net/): test framework

### Microsoft documentation
- [ASP.NET Core Blazor](https://learn.microsoft.com/en-us/aspnet/core/blazor/)
- [Asynchronous programming with async and await](https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/)
- [Blazor CSS isolation](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/css-isolation)
- [IHttpClientFactory and typed clients](https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory)
- [HttpClient JSON extension methods (`GetFromJsonAsync`)](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.json.httpclientjsonextensions)
- [HTTP error handling with HttpClient](https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient#http-error-handling)
- [`HttpRequestException`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httprequestexception)
- [Records](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/record)
- [The `sealed` modifier](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/sealed)
- [Serialize and deserialize JSON with System.Text.Json](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/how-to)
- [`JsonSerializerOptions.Web`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions.web)
- [`JsonException`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonexception)
- [`DateTimeOffset.FromUnixTimeSeconds`](https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset.fromunixtimeseconds)
- [Standard numeric format strings](https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings)
- [`CultureInfo`](https://learn.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo)

### Testing
- [Unit testing C# with xUnit](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-csharp-with-xunit)
- [Creating parameterised tests in xUnit with [InlineData], [ClassData], and [MemberData]](https://andrewlock.net/creating-parameterised-tests-in-xunit-with-inlinedata-classdata-and-memberdata/)
- [`HttpMessageHandler`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpmessagehandler): base class of `FakeHttpMessageHandler`
- [Memory configuration provider (`AddInMemoryCollection`)](https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration-providers#memory-configuration-provider)
