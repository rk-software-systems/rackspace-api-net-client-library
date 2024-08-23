using System.Globalization;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;

namespace RKSoftware.Rackspace.ApiClient;

public class ObjectRackspaceService(IHttpClientFactory httpClientFactory, ILogger<ObjectRackspaceService> logger) : IObjectRackspaceService
{
    #region fields       

    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ILogger _logger = logger;

    #endregion

    #region methods

    public async Task<Stream?> GetObject(RackspaceLoginResponse login, RackspaceObjectModel obj)
    {
        ArgumentNullException.ThrowIfNull(login, nameof(login));

        ArgumentNullException.ThrowIfNull(obj, nameof(obj));

        var client = _httpClientFactory.CreateClient(nameof(Rackspace));
        client.DefaultRequestHeaders.Add(RackspaceConstants.AuthTokenHeader, login.Token);

        var baseUrl = login.Endpoints.GetValueOrDefault(obj.Region);
        if (baseUrl == null)
        {
            RackspaceLoggingConstants.LogEndpointUrlNotFoundError(_logger, obj.Region, nameof(GetObject), null);
            return null;
        }

        var url = GetObjectUrl(baseUrl, obj);
        var response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            var str = await response.Content.ReadAsStringAsync();
            RackspaceLoggingConstants.LogRequestError(_logger, nameof(GetObject), str, null);
            return null;
        }

        return await response.Content.ReadAsStreamAsync();
    }

    public async Task<bool> DeleteObject(RackspaceLoginResponse login, RackspaceObjectModel obj)
    {
        ArgumentNullException.ThrowIfNull(login, nameof(login));

        ArgumentNullException.ThrowIfNull(obj, nameof(obj));

        var client = _httpClientFactory.CreateClient(nameof(Rackspace));
        client.DefaultRequestHeaders.Add(RackspaceConstants.AuthTokenHeader, login.Token);

        var baseUrl = login.Endpoints.GetValueOrDefault(obj.Region);
        if (baseUrl == null)
        {
            RackspaceLoggingConstants.LogEndpointUrlNotFoundError(_logger, obj.Region, nameof(DeleteObject), null);
            return false;
        }

        var url = GetObjectUrl(baseUrl, obj);
        var response = await client.DeleteAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            var str = await response.Content.ReadAsStringAsync();
            RackspaceLoggingConstants.LogRequestError(_logger, nameof(DeleteObject), str, null);
        }

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UploadObject(RackspaceLoginResponse login, RackspaceObjectModel obj)
    {
        ArgumentNullException.ThrowIfNull(login, nameof(login));

        ArgumentNullException.ThrowIfNull(obj, nameof(obj));

        if (obj.File == null)
        {
            return false;
        }

        var client = _httpClientFactory.CreateClient(nameof(Rackspace));
        client.DefaultRequestHeaders.TransferEncodingChunked = true;
        client.DefaultRequestHeaders.Add(RackspaceConstants.AuthTokenHeader, login.Token);

        if(obj.DeleteAfter.HasValue)
        {
            client.DefaultRequestHeaders.Add("X-Delete-After", obj.DeleteAfter.Value.ToString(CultureInfo.InvariantCulture));
        }

        var baseUrl = login.Endpoints.GetValueOrDefault(obj.Region);
        if (baseUrl == null)
        {
            RackspaceLoggingConstants.LogEndpointUrlNotFoundError(_logger, obj.Region, nameof(UploadObject), null);
            return false;
        }

        var url = GetObjectUrl(baseUrl, obj);

        using var fileContent = new StreamContent(obj.File);

        if(!string.IsNullOrEmpty(obj.ContentType) && MediaTypeHeaderValue.TryParse(obj.ContentType, out var parsedContentType))
        {
            fileContent.Headers.ContentType = parsedContentType;
        }       
        fileContent.Headers.ContentLength = obj.File.Length;

        var response = await client.PutAsync(url, fileContent);

        if (!response.IsSuccessStatusCode)
        {
            var str = await response.Content.ReadAsStringAsync();
            RackspaceLoggingConstants.LogRequestError(_logger, nameof(UploadObject), str, null);
        }

        return response.IsSuccessStatusCode;
    }
    #endregion

    #region helpers

    private static Uri GetObjectUrl(string baseUrl, RackspaceObjectModel obj)
    {
        string url;
        if (baseUrl.EndsWith('/'))
        {
            url = $"{baseUrl}{obj.ContainerName}/{obj.Name}";
        }
        else
        {
            url = $"{baseUrl}/{obj.ContainerName}/{obj.Name}";
        }
        var uri = new Uri(url);
        return uri;
    }
    #endregion
}