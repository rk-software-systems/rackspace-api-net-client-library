namespace RKSoftware.Rackspace.ApiClient;

public class BaseRackspaceObjectModel
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
    /// The region of the object. Default is 'IAD'.
    /// </summary>
    public string Region { get; set; } = RackspaceConstants.DefaultRegion;
}
