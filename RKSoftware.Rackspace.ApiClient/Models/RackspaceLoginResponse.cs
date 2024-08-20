namespace RKSoftware.Rackspace.ApiClient;

public class RackspaceLoginResponse
{
    public required string Token { get; set; }

    public required Dictionary<string, string> CdnEndpoints { get; set; }

    public required Dictionary<string, string> Endpoints { get; set; }
}
