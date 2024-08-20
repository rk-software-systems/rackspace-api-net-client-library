namespace RKSoftware.Rackspace.ApiClient;

public class RackspaceObjectModel
{
    public required string ContainerName { get; set; }

    public required string Name { get; set; }

    public Stream? File { get; set; }

    public string? ContentType { get; set; }

    public string Region { get; set; } = RackspaceConstants.DefaultRegion;
}
