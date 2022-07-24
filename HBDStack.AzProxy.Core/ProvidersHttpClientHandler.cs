using System.Diagnostics;
using System.Net.Http.Headers;
using HBDStack.AzProxy.Core.Providers;

namespace HBDStack.AzProxy.Core;

public class ProvidersHttpClientHandler : HttpClientHandler, IHttpClientBuilder
{
    #region Fields

    private readonly IDictionary<string, string> cache;
    private readonly List<IHeaderValuesProvider> headerProviders;
    private ICertProvider certProvider;
    private DateTime lastCacheUpdated = DateTime.MinValue;

    #endregion Fields

    #region Constructors

    public ProvidersHttpClientHandler() : this(null as ICertProvider, null)
    {
    }

    public ProvidersHttpClientHandler(params IHeaderValuesProvider[] headerProviders)
        : this(null, headerProviders) { }

    public ProvidersHttpClientHandler(ICertProvider certProvider)
        : this(certProvider, null) { }

    public ProvidersHttpClientHandler(ICertProvider certProvider, IHeaderValuesProvider[] headerProviders)
    {
        this.certProvider = certProvider;
        this.headerProviders = new List<IHeaderValuesProvider>();
        if (headerProviders != null)
            this.headerProviders.AddRange(headerProviders);
        cache = new Dictionary<string, string>();
    }

    #endregion Constructors

    #region Properties

    public TimeSpan CacheExpiration { get; set; } = TimeSpan.FromMinutes(10);

    public string HeaderCertificateForwardingKey { get; set; } = "X-ARR-ClientCert";

    /// <summary>
    /// Ifnore if cert is not found else throw exception
    /// </summary>
    public bool IgnoreNotFoundCertificate { get; set; }

    /// <summary>
    /// Add Certificate to X-ARR-ClientCert header
    /// </summary>
    public bool IncludesCertificateForwarding { get; set; }

    #endregion Properties

    #region Methods

    public IHttpClientBuilder AddHeaderValuesProvider(IHeaderValuesProvider provider)
    {
        if (headerProviders.Contains(provider)) return this;
        if (provider == null) throw new ArgumentNullException(nameof(provider));

        headerProviders.Add(provider);
        return this;
    }

    public IHttpClientBuilder ClientCertificate(ICertProvider certProvider)
    {
        if (this.certProvider != null) throw new ArgumentException($"{nameof(CertProvider)} is already provided.");
        this.certProvider = certProvider ?? throw new ArgumentNullException(nameof(certProvider));

        return this;
    }

    public TProvider Get<TProvider>() where TProvider : IHeaderValuesProvider => (TProvider)headerProviders.FirstOrDefault(p => p is TProvider);

    public TProvider Remove<TProvider>() where TProvider : IHeaderValuesProvider
    {
        var p = Get<TProvider>();
        if (p != null)
            headerProviders.Remove(p);
        return p;
    }

    public IHttpClientBuilder SetCacheExpiration(TimeSpan expiration)
    {
        if (expiration.TotalSeconds <= 0)
            throw new ArgumentException(nameof(expiration));

        CacheExpiration = expiration;
        return this;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        certProvider?.Dispose();
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        await AddCertIfAvailable(cancellationToken);
        IncludesCertificateForwardingIfAvailable(request);

        await AddHeaderValuesIfAvailable(request, cancellationToken);

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    private async Task AddCertIfAvailable(CancellationToken cancellationToken)
    {
        if (certProvider == null || ClientCertificates.Count > 0) return;
        var cert = await certProvider.GetCertAsync(cancellationToken);

        if (cert != null)
        {
            ClientCertificates.Add(cert);
        }
        else if (!IgnoreNotFoundCertificate)
            throw new ArgumentException($"The Client Certificate is not found.");
    }

    private async Task AddHeaderValuesIfAvailable(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (headerProviders == null) return;

        await LoadKeysToCacheIfExpiredAsync(cancellationToken);

        foreach (var item in cache)
        {
            if (item.Key.Equals(HeaderKeys.Authorization, StringComparison.OrdinalIgnoreCase))
            {
                var auth = request.Headers.Authorization;
                request.Headers.Authorization = new AuthenticationHeaderValue(auth?.Scheme ?? "Bearer", item.Value);

                continue;
            }

            if (request.Headers.Contains(item.Key))
                request.Headers.Remove(item.Key);

            request.Headers.Add(item.Key, item.Value);
        }
    }

    private void IncludesCertificateForwardingIfAvailable(HttpRequestMessage request)
    {
        if (!IncludesCertificateForwarding || ClientCertificates.Count == 0) return;

        var cert = ClientCertificates[0];
        request.Headers.Add(HeaderCertificateForwardingKey, cert.GetRawCertDataString());
    }

    private bool IsCacheExpired() => cache.Count == 0 || (DateTime.Now - lastCacheUpdated).TotalMinutes >= CacheExpiration.TotalMinutes;

    private async Task LoadKeysToCacheIfExpiredAsync(CancellationToken cancellationToken)
    {
        if (headerProviders == null) throw new InvalidOperationException($"There is no {nameof(IHeaderValuesProvider)} found.");

        if (!IsCacheExpired())
        {
            Trace.TraceInformation("ProvidersHttpClientHandler: Cache is still valid.");
            return;
        }
        else Trace.TraceInformation("ProvidersHttpClientHandler: Cache is expired. Data will be reload.");

        cache.Clear();
        lastCacheUpdated = DateTime.Now;

        foreach (var provider in headerProviders)
        await foreach (var item in provider.GetHeaderAsync(cancellationToken))
        {
            if (string.IsNullOrEmpty(item.Key))
            {
                Trace.WriteLine("ProvidersHttpClientHandler: The empty key had been ignored.");
                continue;
            }
            if (string.IsNullOrEmpty(item.Value))
            {
                Trace.WriteLine($"ProvidersHttpClientHandler: The empty value of key {item.Key} had been ignored.");
                continue;
            }
            cache.Add(item.Key, item.Value);
        }
    }

    #endregion Methods
}