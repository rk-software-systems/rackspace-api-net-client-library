using Microsoft.Extensions.Logging;

namespace RKSoftware.Rackspace.ApiClient;

public class ContainerRackspaceService(IHttpClientFactory httpClientFactory, ILogger<ContainerRackspaceService> logger) : IContainerRackspaceService
{
    #region constants

    private const string _cdnEnabledHeader = "X-Cdn-Enabled";
    private const string _cdnUriHeader = "X-Cdn-Uri";
    private const string _cdnSslUriHeader = "X-Cdn-Ssl-Uri";
    #endregion

    #region fields   

    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ILogger _logger = logger;

    #endregion

    #region methods

    public async Task<List<string>> GetContainers(RackspaceLoginResponse login, string region = RackspaceConstants.DefaultRegion)
    {
        ArgumentNullException.ThrowIfNull(login, nameof(login));

        var client = _httpClientFactory.CreateClient(nameof(Rackspace));
        client.DefaultRequestHeaders.Add(RackspaceConstants.AuthTokenHeader, login.Token);

        var baseUrl = login.CdnEndpoints.GetValueOrDefault(region);
        if (baseUrl == null)
        {
            RackspaceLoggingConstants.LogCdnEndpointUrlNotFoundError(_logger, region, nameof(GetContainers), null);
            throw new RackspaceLoginException(RackspaceLoggingConstants.CdnEndpointUrlNotFoundError);
        }

#pragma warning disable CA2234 // Pass system uri objects instead of strings
        var response = await client.GetAsync(baseUrl);
#pragma warning restore CA2234 // Pass system uri objects instead of strings

        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {

            RackspaceLoggingConstants.LogRequestError(_logger, nameof(GetContainers), content, null);
            throw new RackspaceLoginException(RackspaceLoggingConstants.RequestError);
        }

        return content.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    public async Task<RackspaceContainerInfoModel?> GetContainerCdnInfoOrNullIfNotFound(RackspaceLoginResponse login, string containerName, string region = RackspaceConstants.DefaultRegion)
    {
        ArgumentNullException.ThrowIfNull(login, nameof(login));

        var client = _httpClientFactory.CreateClient(nameof(Rackspace));
        client.DefaultRequestHeaders.Add(RackspaceConstants.AuthTokenHeader, login.Token);

        var baseUrl = login.CdnEndpoints.GetValueOrDefault(region);
        if (baseUrl == null)
        {
            RackspaceLoggingConstants.LogCdnEndpointUrlNotFoundError(_logger, region, nameof(GetContainerCdnInfoOrNullIfNotFound), null);
            throw new RackspaceLoginException(RackspaceLoggingConstants.CdnEndpointUrlNotFoundError);
        }

        var url = GetContainerUrl(baseUrl, containerName);
        using var request = new HttpRequestMessage(HttpMethod.Head, url);
        var response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var str = await response.Content.ReadAsStringAsync();
            RackspaceLoggingConstants.LogRequestError(_logger, nameof(GetContainerCdnInfoOrNullIfNotFound), str, null);
            return null;
        }

        var info = new RackspaceContainerInfoModel
        {
            Name = containerName,
            CdnUri = GetHeaderUri(response, _cdnUriHeader),
            CdnSslUri = GetHeaderUri(response, _cdnSslUriHeader),
        };

        if (response.Headers.TryGetValues(_cdnEnabledHeader, out var values))
        {
            info.IsCdnEnabled = values.Any(x => x.Equals("True", StringComparison.OrdinalIgnoreCase));
        }

        return info;
    }

    public async Task<RackspaceContainerInfoModel> GetContainerCdnInfo(RackspaceLoginResponse login, string containerName, string region = RackspaceConstants.DefaultRegion)
    {
        var containerInfo = await GetContainerCdnInfoOrNullIfNotFound(login, containerName, region);
        return containerInfo ?? throw new RackspaceContainerCdnInfoNotFoundException();
    }

    public async Task<RackspaceContainerInfoModel> GetOrCreateContainerCdnInfo(RackspaceLoginResponse login, string containerName, string region = RackspaceConstants.DefaultRegion)
    {
        var containerInfo = await GetContainerCdnInfoOrNullIfNotFound(login, containerName, region);

        if (containerInfo == null || !containerInfo.IsCdnEnabled)
        {
            if (containerInfo == null)
            {
                await CreateContainer(login, containerName, region);
            }
            await EnableContainerCdn(login, containerName, region);
            containerInfo = await GetContainerCdnInfo(login, containerName, region);
        }

        return containerInfo;
    }

    public async Task<bool> CreateContainer(RackspaceLoginResponse login, string containerName, string region = RackspaceConstants.DefaultRegion)
    {
        ArgumentNullException.ThrowIfNull(login, nameof(login));

        var client = _httpClientFactory.CreateClient(nameof(Rackspace));
        client.DefaultRequestHeaders.Add(RackspaceConstants.AuthTokenHeader, login.Token);

        var baseUrl = login.Endpoints.GetValueOrDefault(region);
        if (baseUrl == null)
        {
            RackspaceLoggingConstants.LogEndpointUrlNotFoundError(_logger, region, nameof(CreateContainer), null);
            return false;
        }

        var url = GetContainerUrl(baseUrl, containerName);
        var response = await client.PutAsync(url, null);

        if (!response.IsSuccessStatusCode)
        {
            var str = await response.Content.ReadAsStringAsync();
            RackspaceLoggingConstants.LogRequestError(_logger, nameof(CreateContainer), str, null);
        }

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> EnableContainerCdn(RackspaceLoginResponse login, string containerName, string region = RackspaceConstants.DefaultRegion)
    {
        ArgumentNullException.ThrowIfNull(login, nameof(login));

        var client = _httpClientFactory.CreateClient(nameof(Rackspace));
        client.DefaultRequestHeaders.Add(RackspaceConstants.AuthTokenHeader, login.Token);
        client.DefaultRequestHeaders.Add(_cdnEnabledHeader, "True");

        var baseUrl = login.CdnEndpoints.GetValueOrDefault(region);
        if (baseUrl == null)
        {
            RackspaceLoggingConstants.LogCdnEndpointUrlNotFoundError(_logger, region, nameof(EnableContainerCdn), null);
            return false;
        }

        var url = GetContainerUrl(baseUrl, containerName);
        var response = await client.PutAsync(url, null);

        if (!response.IsSuccessStatusCode)
        {
            var str = await response.Content.ReadAsStringAsync();
            RackspaceLoggingConstants.LogRequestError(_logger, nameof(EnableContainerCdn), str, null);
        }

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteContainer(RackspaceLoginResponse login, string containerName, string region = RackspaceConstants.DefaultRegion)
    {
        ArgumentNullException.ThrowIfNull(login, nameof(login));

        var client = _httpClientFactory.CreateClient(nameof(Rackspace));
        client.DefaultRequestHeaders.Add(RackspaceConstants.AuthTokenHeader, login.Token);

        var baseUrl = login.Endpoints.GetValueOrDefault(region);
        if (baseUrl == null)
        {
            RackspaceLoggingConstants.LogEndpointUrlNotFoundError(_logger, region, nameof(DeleteContainer), null);
            return false;
        }

        var url = GetContainerUrl(baseUrl, containerName);
        var response = await client.DeleteAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            var str = await response.Content.ReadAsStringAsync();
            RackspaceLoggingConstants.LogRequestError(_logger, nameof(DeleteContainer), str, null);
        }

        return response.IsSuccessStatusCode;
    }
    #endregion

    #region helpers

    private static Uri GetContainerUrl(string baseUrl, string containerName)
    {
        string url;
        if (baseUrl.EndsWith('/'))
        {
            url = $"{baseUrl}{containerName}";
        }
        else
        {
            url = $"{baseUrl}/{containerName}";
        }
        var uri = new Uri(url);
        return uri;
    }

    private static string? GetHeaderUri(HttpResponseMessage response, string headerName)
    {
        if (response.Headers.TryGetValues(headerName, out var values))
        {
            var value = values.FirstOrDefault();
            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }
        }
        return null;
    }

    #endregion
}
