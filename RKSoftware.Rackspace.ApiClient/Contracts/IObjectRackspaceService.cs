namespace RKSoftware.Rackspace.ApiClient;

public interface IObjectRackspaceService
{
    Task<Stream?> GetObject(RackspaceLoginResponse login, BaseRackspaceObjectModel obj);

    Task<bool> DeleteObject(RackspaceLoginResponse login, BaseRackspaceObjectModel obj);

    Task<bool> UploadObject(RackspaceLoginResponse login, RackspaceObjectModel obj);

    Task<bool> PurgeCdnObject(RackspaceLoginResponse login, BaseRackspaceObjectModel obj);
}
