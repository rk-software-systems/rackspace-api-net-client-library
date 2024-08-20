namespace RKSoftware.Rackspace.ApiClient;

public interface IContainerRackspaceService
{
    Task<List<string>> GetContainers(RackspaceLoginResponse login, string region = RackspaceConstants.DefaultRegion);

    Task<RackspaceContainerInfoModel> GetContainerCdnInfo(RackspaceLoginResponse login, string containerName, string region = RackspaceConstants.DefaultRegion);

    Task<RackspaceContainerInfoModel?> GetContainerCdnInfoOrNullIfNotFound(RackspaceLoginResponse login, string containerName, string region = RackspaceConstants.DefaultRegion);

    Task<bool> CreateContainer(RackspaceLoginResponse login, string containerName, string region = RackspaceConstants.DefaultRegion);

    Task<bool> EnableContainerCdn(RackspaceLoginResponse login, string containerName, string region = RackspaceConstants.DefaultRegion);

    Task<bool> DeleteContainer(RackspaceLoginResponse login, string containerName, string region = RackspaceConstants.DefaultRegion);
}
