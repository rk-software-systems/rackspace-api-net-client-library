namespace RKSoftware.Rackspace.ApiClient;

public class RackspaceObjectModel : BaseRackspaceObjectModel
{
    /// <summary>
    /// File data
    /// </summary>
    public Stream? File { get; set; }

    /// <summary>
    /// The content type of the object.
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// The number of seconds to wait before deleting the object.
    /// </summary>
    public long? DeleteAfter { get; set; }
}
