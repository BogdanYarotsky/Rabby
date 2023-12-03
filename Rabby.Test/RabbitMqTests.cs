namespace Rabby.Test
{
    [TestClass]
    public class RabbitMqTests
    {
        private IConnection _connection = null!;
        private IModel _channel = null!;

        [TestInitialize]
        public async Task StartRabbitMq()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            bool isRabbitMqReady = false;
            int maxAttempts = 30;
            int attempt = 0;
            while (attempt < maxAttempts && !isRabbitMqReady)
            {
                try
                {
                    attempt++;
                    _connection = factory.CreateConnection();
                    _channel = _connection.CreateModel();
                    isRabbitMqReady = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Attempt {attempt} failed: {ex.Message}");
                    if (attempt < maxAttempts)
                    {
                        await Task.Delay(250);
                    }
                }
            }

            if (!isRabbitMqReady)
            {
                throw new Exception("Unable to connect to RabbitMQ after multiple attempts.");
            }
        }

        [TestCleanup]
        public void CleanUpRabbitMq()
        {
            _channel.Close();
            _connection.Close();
        }

        [TestMethod]
        public void ExchangeIsCreated()
        {
            const string exchangeName = "test_exchange";
            const string exchangeType = "direct"; // Use appropriate exchange type
            // Declare the exchange
            _channel?.ExchangeDeclare(exchange: exchangeName, type: exchangeType);
            // Assert (This needs to be improved for a real test)
            Assert.IsTrue(_channel?.IsOpen, "Channel is not open");
        }

        [TestMethod]
        public void AnotherIsCreated()
        {
            const string exchangeName = "test_exchange";
            const string exchangeType = "direct"; // Use appropriate exchange type
            // Declare the exchange
            _channel?.ExchangeDeclare(exchange: exchangeName, type: exchangeType);
            // Assert (This needs to be improved for a real test)
            Assert.IsTrue(_channel?.IsOpen, "Channel is not open");
        }
    }
}
