namespace RKSoftware.Rackspace.ApiClient;

public class RackspaceContainerInfoModel
{
    public required string Name { get; set; }

    public bool IsCdnEnabled { get; set; }

    public string? CdnUri { get; set; }

    public string? CdnSslUri { get; set; }
}
