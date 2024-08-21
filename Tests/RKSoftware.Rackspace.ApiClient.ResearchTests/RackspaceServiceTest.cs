using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace RKSoftware.Rackspace.ApiClient.ResearchTests;

public class RackspaceServiceTest
{
    #region constants

    private const int _plumberProfileId = 1125426;

    #endregion

    #region fields   

    private readonly IAuthorizationRackspaceService _authorizationRackspaceService;
    private readonly IContainerRackspaceService _containerRackspaceService;
    private readonly IObjectRackspaceService _objectRackspaceService;
    private readonly IServiceScope _scope;
    #endregion

    #region ctors
    public RackspaceServiceTest()
    {
        var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json")
                .Build();

        var services = new ServiceCollection();
        services.RegisterRackspaceServices(configuration);
        var serviceProvider = services.BuildServiceProvider();
        _scope = serviceProvider.CreateScope();
        _authorizationRackspaceService = _scope.ServiceProvider.GetRequiredService<IAuthorizationRackspaceService>();
        _containerRackspaceService = _scope.ServiceProvider.GetRequiredService<IContainerRackspaceService>();
        _objectRackspaceService = _scope.ServiceProvider.GetRequiredService<IObjectRackspaceService>();
    }
    #endregion

    #region methods

    [Fact]
    public async Task LoginTest()
    {
        var loginResponse = await _authorizationRackspaceService.Login();
        Assert.NotNull(loginResponse);
    }

    [Fact]
    public async Task GetContainersTest()
    {
        var loginResponse = await _authorizationRackspaceService.Login();
        Assert.NotNull(loginResponse);

        var containers = await _containerRackspaceService.GetContainers(loginResponse);
        Assert.NotNull(containers);

        var containerName = GetContainerNameFromProfileID(_plumberProfileId);

        Assert.Contains(containerName, containers);
    }

    [Fact]
    public async Task GetContainerInfoTest()
    {
        var loginResponse = await _authorizationRackspaceService.Login();
        Assert.NotNull(loginResponse);

        var containerName = GetContainerNameFromProfileID(_plumberProfileId);

        var info = await _containerRackspaceService.GetContainerCdnInfoOrNullIfNotFound(loginResponse, containerName);
        Assert.NotNull(info);
    }

    [Fact]
    public async Task GetCustomTestContainerInfoTest()
    {
        var loginResponse = await _authorizationRackspaceService.Login();
        Assert.NotNull(loginResponse);

        var containerName = RackspaceConstants.DefaultTestContainerName;

        var info = await _containerRackspaceService.GetContainerCdnInfoOrNullIfNotFound(loginResponse, containerName);
        Assert.NotNull(info);
    }

    [Fact]
    public async Task CreateContainerTest()
    {
        var loginResponse = await _authorizationRackspaceService.Login();
        Assert.NotNull(loginResponse);

        var containerName = RackspaceConstants.DefaultTestContainerName;

        var ok = await _containerRackspaceService.CreateContainer(loginResponse, containerName);
        Assert.True(ok);
    }

    [Fact]
    public async Task EnableContainerCdnTest()
    {
        var loginResponse = await _authorizationRackspaceService.Login();
        Assert.NotNull(loginResponse);

        var containerName = RackspaceConstants.DefaultTestContainerName;

        var ok = await _containerRackspaceService.EnableContainerCdn(loginResponse, containerName);
        Assert.True(ok);
    }

    [Fact]
    public async Task DeleteContainerTest()
    {
        var loginResponse = await _authorizationRackspaceService.Login();
        Assert.NotNull(loginResponse);

        var containerName = RackspaceConstants.DefaultTestContainerName;

        var ok = await _containerRackspaceService.DeleteContainer(loginResponse, containerName);
        Assert.True(ok);
    }

    [Fact]
    public async Task GetObjectTest()
    {
        var loginResponse = await _authorizationRackspaceService.Login();
        Assert.NotNull(loginResponse);

        var containerName = "26";
        var objectName = "00__big_buck_bunny_720p_1mb.mp4";

        var obj = new RackspaceObjectModel
        {
            ContainerName = containerName,
            Name = objectName,
        };

        await _objectRackspaceService.GetObject(loginResponse, obj);
    }

    [Fact]
    public async Task DeleteObjectTest()
    {
        var loginResponse = await _authorizationRackspaceService.Login();
        Assert.NotNull(loginResponse);

        var containerName = "26";
        var objectName = "00__big_buck_bunny_720p_1mb.mp4";

        var obj = new RackspaceObjectModel
        {
            ContainerName = containerName,
            Name = objectName,
        };

        await _objectRackspaceService.DeleteObject(loginResponse, obj);
    }

    [Fact]
    public async Task UploadObjectTest()
    {
        var loginResponse = await _authorizationRackspaceService.Login();
        Assert.NotNull(loginResponse);

        var containerName = "26";
        var objectName = "00__big_buck_bunny_720p_1mb.mp4";

        var stream = File.OpenRead("big_buck_bunny_720p_1mb.mp4");

        var obj = new RackspaceObjectModel
        {
            ContainerName = containerName,
            Name = objectName,
            File = stream,
            ContentType = "video/mp4"
        };
        await _objectRackspaceService.UploadObject(loginResponse, obj);
    }

    #endregion

    #region helpers

    private static string GetContainerNameFromProfileID(int profileId)
    {
        return (profileId % 100).ToString(CultureInfo.InvariantCulture);
    }

    #endregion
}
