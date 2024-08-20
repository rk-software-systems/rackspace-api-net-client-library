namespace RKSoftware.Rackspace.ApiClient;

public interface IAuthorizationRackspaceService
{
    Task<RackspaceLoginResponse> Login();
}
