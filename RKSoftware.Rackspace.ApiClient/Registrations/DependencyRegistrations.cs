using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RKSoftware.Rackspace.ApiClient;

public static class DependencyRegistrations
{
    public static void RegisterRackspaceServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        serviceCollection.AddHttpClient(nameof(Rackspace));
        serviceCollection.Configure<RackspaceSettings>(configuration.GetSection(nameof(RackspaceSettings)));
        serviceCollection.AddScoped<IAuthorizationRackspaceService, AuthorizationRackspaceService>();
        serviceCollection.AddScoped<IContainerRackspaceService, ContainerRackspaceService>();
        serviceCollection.AddScoped<IObjectRackspaceService, ObjectRackspaceService>();
    }
}
