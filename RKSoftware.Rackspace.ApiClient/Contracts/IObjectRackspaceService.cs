namespace RKSoftware.Rackspace.ApiClient;

public interface IObjectRackspaceService
{
    Task<Stream?> GetObject(RackspaceLoginResponse login, RackspaceObjectModel obj);

    Task<bool> DeleteObject(RackspaceLoginResponse login, RackspaceObjectModel obj);

    Task<bool> UploadObject(RackspaceLoginResponse login, RackspaceObjectModel obj);
}
