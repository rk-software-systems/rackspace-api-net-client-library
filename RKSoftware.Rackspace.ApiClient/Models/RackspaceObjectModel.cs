namespace RKSoftware.Rackspace.ApiClient;

public class RackspaceObjectModel
{
    /// <summary>
    /// The name of container 
    /// </summary>
    public required string ContainerName { get; set; }

    /// <summary>
    /// The name of object
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// File data
    /// </summary>
    public Stream? File { get; set; }

    /// <summary>
    /// The content type of the object.
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// The region of the object. Default is 'IAD'.
    /// </summary>
    public string Region { get; set; } = RackspaceConstants.DefaultRegion;

    /// <summary>
    /// The number of seconds to wait before deleting the object.
    /// </summary>
    public long? DeleteAfter { get; set; }
}
