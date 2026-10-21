using System.Net;
using System.Text;

namespace SOA_CA1.Tests.Api;

public class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly string _content;
    private readonly HttpStatusCode _statusCode;

    public Uri? LastRequestUri { get; private set; }

    public FakeHttpMessageHandler(string content, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        _content = content;
        _statusCode = statusCode;
    }

    public HttpClient CreateClient() => new(this) { BaseAddress = new Uri("https://api.test/") };

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        LastRequestUri = request.RequestUri;
        return Task.FromResult(new HttpResponseMessage(_statusCode)
        {
            Content = new StringContent(_content, Encoding.UTF8, "application/json")
        });
    }
}
