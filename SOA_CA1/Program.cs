using System.Text.Json;
using ApexCharts;
using DotNetEnv;
using SOA_CA1;
using SOA_CA1.Components;

// Loads SOA_CA1/.env into environment variables. Must run before CreateBuilder so builder.Configuration sees them.
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Fail at startup with a clear message instead of a null reference on the first API call
var cryptoApiUri = builder.Configuration["CRYPTO_API_URI"]
    ?? throw new InvalidOperationException("CRYPTO_API_URI is missing. Add it to SOA_CA1/.env.");
var cryptoApiKey = builder.Configuration["CRYPTO_API_KEY"]
    ?? throw new InvalidOperationException("CRYPTO_API_KEY is missing. Add it to SOA_CA1/.env.");
var cryptosNumber = int.TryParse(builder.Configuration["CRYPTO_IDS_NUMBER"], out var number) && number > 0
    ? number
    : throw new InvalidOperationException("CRYPTO_IDS_NUMBER is missing or not a positive number. Add it to SOA_CA1/.env.");
var cryptosFile = Path.Combine(builder.Environment.ContentRootPath,
    builder.Configuration["CryptosFile"] ?? throw new InvalidOperationException("CryptosFile is missing in appsettings.json."));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Blazor-ApexCharts services used by the Crypto Market price chart
builder.Services.AddApexCharts();

// Typed HttpClient: every CryptoService gets a client already pointed at the CoinStats API with the key attached
builder.Services.AddHttpClient<ICryptoService, CryptoService>(client =>
{
    client.BaseAddress = new Uri(cryptoApiUri);
    client.DefaultRequestHeaders.Add("X-API-KEY", cryptoApiKey);
});

var app = builder.Build();

// Fetch the coin list once on first start-up and save it; the Crypto Market dropdown reads it from the file.
// Delete the file to fetch a fresh list. A failed fetch only logs a warning, the page then shows the list as unavailable.
if (!File.Exists(cryptosFile))
{
    try
    {
        var cryptos = await app.Services.GetRequiredService<ICryptoService>().GetCryptosAsync(cryptosNumber);
        Directory.CreateDirectory(Path.GetDirectoryName(cryptosFile)!);
        await File.WriteAllTextAsync(cryptosFile,
            JsonSerializer.Serialize(cryptos, new JsonSerializerOptions(JsonSerializerOptions.Web) { WriteIndented = true }));
    }
    catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException or ArgumentException or IOException)
    {
        app.Logger.LogWarning(ex, "Could not create the coin list file {File}.", cryptosFile);
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
