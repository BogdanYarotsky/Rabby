namespace Rabby.Test
{
    using System.Net.Http.Headers;
    using System.Text;

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
        public async Task ExchangeIsDeclared()
        {
            Engine.Execute("declare exchange name=my-new-exchange type=fanout".Split(' '));
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic", Convert.ToBase64String("guest:guest"u8));
            var response = await client.GetAsync("http://localhost:15672/api/exchanges/%2F/my-new-exchange");
            Assert.IsTrue(response.IsSuccessStatusCode);
        }
    }
}
