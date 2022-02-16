using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace CertCheck;

public class CertificateValidationService
{
    private readonly HttpClient _client;

    private X509Certificate2 _certificate = null!;

    public CertificateValidationService()
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, cert, _, _) =>
            {
                _certificate = new X509Certificate2(cert.GetRawCertData());
                return true;
            }
        };

        _client = new HttpClient(handler);
    }

    public async Task<int> GetDaysToExpiration(string host)
    {
        if (string.IsNullOrEmpty(host))
            throw new ArgumentException("Need a valid URL to verify.");

        if (!host.StartsWith("https://"))
            throw new InvalidOperationException("Cannot validate a non-https domain");

        await _client.SendAsync(new HttpRequestMessage(HttpMethod.Head, host));

        if (_certificate is null) throw new InvalidOperationException("Failed to retrieve certificate for host {host}");

        var expirationDate = _certificate.NotAfter;

        var expiration = (expirationDate - DateTime.Now).Days;

        return expiration;
    }
}