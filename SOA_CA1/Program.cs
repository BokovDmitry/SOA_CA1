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

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Typed HttpClient: every CryptoService gets a client already pointed at the CoinStats API with the key attached
builder.Services.AddHttpClient<ICryptoService, CryptoService>(client =>
{
    client.BaseAddress = new Uri(cryptoApiUri);
    client.DefaultRequestHeaders.Add("X-API-KEY", cryptoApiKey);
});

var app = builder.Build();

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
