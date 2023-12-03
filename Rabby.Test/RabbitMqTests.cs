namespace Rabby.Test
{
    [TestClass]
    public class RabbitMqTests
    {
        private DockerClient client;
        private string containerId;
        private IConnection connection;
        private IModel channel;

        [TestInitialize]
        public async Task StartRabbitMq()
        {
            client = new DockerClientConfiguration(new Uri("npipe://./pipe/docker_engine")).CreateClient();

            // Pull the RabbitMQ image
            await client.Images.CreateImageAsync(new ImagesCreateParameters
            {
                FromImage = "rabbitmq",
                Tag = "3.12.10-alpine"
            }, null, new Progress<JSONMessage>());

            // Start the RabbitMQ container
            var response = await client.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Image = "rabbitmq:3.12.10-alpine",
                Name = "rabbitmq-tests-container",
                ExposedPorts = new Dictionary<string, EmptyStruct>
                {
                    { "5672", new EmptyStruct() }
                },
                HostConfig = new HostConfig
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        ["5672"] = new[] {new PortBinding { HostPort = "5672"}}
                    }
                }
            });

            containerId = response.ID;

            await client.Containers.StartContainerAsync(containerId, new ContainerStartParameters());

            var factory = new ConnectionFactory { HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
        }

        [TestCleanup]
        public async Task CleanUpRabbitMq()
        {
            // Cleanup the RabbitMQ connection
            channel?.Close();
            connection?.Close();

            // Stop and remove the RabbitMQ container
            await client.Containers.StopContainerAsync(containerId, new ContainerStopParameters());
            await client.Containers.RemoveContainerAsync(containerId, new ContainerRemoveParameters());
        }

        [TestMethod]
        public void ExchangeIsCreated()
        {
            string exchangeName = "test_exchange";
            string exchangeType = "direct"; // Use appropriate exchange type

            // Declare the exchange
            channel.ExchangeDeclare(exchange: exchangeName, type: exchangeType);

            // Assert (This needs to be improved for a real test)
            Assert.IsTrue(channel.IsOpen, "Channel is not open");
        }
    }
}
