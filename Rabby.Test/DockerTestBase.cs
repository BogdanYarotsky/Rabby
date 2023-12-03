namespace Rabby.Test;

[TestClass]
public abstract class DockerTestBase
{
    private const string _engineEndpoint = "npipe://./pipe/docker_engine";
    private const string _containerName = "rabbitmq-tests-container";
    private const string _imageName = "rabbitmq";
    private const string _imageTag = "3.12.10-management-alpine";
    private static readonly Uri _engineUri = new(_engineEndpoint);
    private static readonly DockerClientConfiguration _clientConfig = new(_engineUri);
    private static readonly ImagesCreateParameters _image = new()
    {
        FromImage = _imageName,
        Tag = _imageTag
    };

    private static readonly CreateContainerParameters _container = new()
    {
        Image = $"{_imageName}:{_imageTag}",
        Name = _containerName,
        ExposedPorts = new Dictionary<string, EmptyStruct>
        {
            ["5672"] = new(),
            ["15672"] = new()
        },
        HostConfig = new HostConfig
        {
            PortBindings = new Dictionary<string, IList<PortBinding>>
            {
                ["5672"] = new PortBinding[] { new() { HostPort = "5672" } },
                ["15672"] = new PortBinding[] { new() { HostPort = "15672" } }
            }
        }
    };

    [AssemblyInitialize]
    public static async Task InitializeDockerContainer(TestContext _)
    {

        var client = _clientConfig.CreateClient();
        var containers = await client.Containers.ListContainersAsync(new ContainersListParameters { All = true });
        var container = containers.FirstOrDefault(c => c.Names.Contains("/" + _containerName));
        if (container is not null)
        {
            if (container.State != "running")
                await client.Containers.StartContainerAsync(container.ID, new ContainerStartParameters());
        }
        else
        {
            await client.Images.CreateImageAsync(_image, null, new Progress<JSONMessage>());
            var response = await client.Containers.CreateContainerAsync(_container);
            await client.Containers.StartContainerAsync(response.ID, new ContainerStartParameters());
        }
    }
}