using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RKSoftware.Rackspace.ApiClient;

public class AuthorizationRackspaceService : IAuthorizationRackspaceService
{

    #region consts    
    
    private const string _cloudFilesCdn = "cloudFilesCDN";
    private const string _cloudFiles = "cloudFiles";

    #endregion

    #region fields   

    private readonly ILogger _logger;
    private static readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly RackspaceSettings _rackspaceSettings;

    #endregion

    #region ctors

    public AuthorizationRackspaceService(
        ILogger<AuthorizationRackspaceService> logger,
        IOptions<RackspaceSettings> rackspaceSettingsOptions, 
        IHttpClientFactory httpClientFactory)
    {
        ArgumentNullException.ThrowIfNull(rackspaceSettingsOptions, nameof(rackspaceSettingsOptions));

        _httpClientFactory = httpClientFactory;
        _rackspaceSettings = rackspaceSettingsOptions.Value;
        _logger = logger;
    }    
    #endregion

    #region methods

    public async Task<RackspaceLoginResponse> Login()
    {
        var client = _httpClientFactory.CreateClient(nameof(Rackspace));
        var body = @$"
{{
    ""auth"": {{
        ""RAX-KSKEY:apiKeyCredentials"": {{
        ""username"": ""{_rackspaceSettings.Username}"",
        ""apiKey"": ""{_rackspaceSettings.ApiKey}""
        }}
    }}
}}
";
        using var content = new StringContent(body, Encoding.UTF8, MediaTypeNames.Application.Json);
        var response = await client.PostAsync(_rackspaceSettings.TokenUrl, content);
        if(!response.IsSuccessStatusCode)
        {
            var str = await response.Content.ReadAsStringAsync();
            RackspaceLoggingConstants.LogRequestError(_logger, nameof(Login), str, null);
            throw new RackspaceLoginException(RackspaceLoggingConstants.RequestError);
        }

        var stream = await response.Content.ReadAsStreamAsync();
        
        var accessResponse = await JsonSerializer.DeserializeAsync<RackspaceAccessResponse>(stream, _options);

        var token = accessResponse?.Access?.Token?.Id;
        if(string.IsNullOrEmpty(token))
        {
            RackspaceLoggingConstants.LogAccessTokenNotFoundError(_logger, null);
            throw new RackspaceLoginException(RackspaceLoggingConstants.AccessTokenNotFoundError);
        }

        var cdnEndpoints = accessResponse?
            .Access?
            .ServiceCatalog?
            .FirstOrDefault(x => _cloudFilesCdn.Equals(x.Name, StringComparison.OrdinalIgnoreCase))?
            .Endpoints?
            .Where(x => !string.IsNullOrEmpty(x.Region) && !string.IsNullOrEmpty(x.PublicURL))
            .ToDictionary(x => x.Region!, x => x.PublicURL!);

        var endpoints = accessResponse?
            .Access?
            .ServiceCatalog?
            .FirstOrDefault(x => _cloudFiles.Equals(x.Name, StringComparison.OrdinalIgnoreCase))?
            .Endpoints?
            .Where(x => !string.IsNullOrEmpty(x.Region) && !string.IsNullOrEmpty(x.PublicURL))
            .ToDictionary(x => x.Region!, x => x.PublicURL!);

        if (cdnEndpoints == null || 
            cdnEndpoints.Count == 0 ||
            endpoints == null || 
            endpoints.Count == 0)
        {
            RackspaceLoggingConstants.LogEndpointsNotFoundError(_logger, null);
            throw new RackspaceLoginException(RackspaceLoggingConstants.EndpointsNotFoundError);
        }

        var result = new RackspaceLoginResponse
        {
            Token = token,
            CdnEndpoints = cdnEndpoints,
            Endpoints = endpoints
        };
        return result;
    }
    #endregion
}
