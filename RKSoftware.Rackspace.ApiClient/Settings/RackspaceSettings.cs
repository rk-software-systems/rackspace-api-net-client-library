namespace RKSoftware.Rackspace.ApiClient;

public class RackspaceSettings
{
    public required string Username { get; set; }

    public required string Password { get; set; }

    public required string ApiKey { get; set; }

    public required Uri TokenUrl { get; set; }
}
