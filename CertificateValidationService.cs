using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace CertCheck;

public class CertificateValidationService
{
    public async Task<int> GetDaysToExpiration(string host)
    {
        if (string.IsNullOrEmpty(host))
            throw new ArgumentException("Need a valid URL to verify.");

        if (!host.StartsWith("https://"))
            throw new InvalidOperationException("Cannot validate a non-https domain");

        X509Certificate2? certificate = null;
        var httpClientHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, cert, _, _) =>
            {
                certificate = new X509Certificate2(cert.GetRawCertData());
                return true;
            }
        };

        var httpClient = new HttpClient(httpClientHandler);
        await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, host));

        if (certificate is null) throw new InvalidOperationException("Failed to retrieve certificate for host {host}");

        var expirationDate = certificate.NotAfter;

        var expiration = (expirationDate - DateTime.Now).Days;

        return expiration;
    }
}