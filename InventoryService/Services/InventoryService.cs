using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InventoryService.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace InventoryService.Services
{
    public class InventoryService : BackgroundService
    {
        private readonly ILogger<InventoryService> _logger;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string? _queueName;

        public InventoryService(IConnection connection, IConfiguration configuration, ILogger<InventoryService> logger)
        {
            _logger = logger;
            _connection = connection;
            _channel = _connection.CreateModel();
            _queueName = configuration["RabbitMQ:QueueName"];

            _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var product = JsonConvert.DeserializeObject<Product>(message);

                _logger.LogInformation($"Received product: {product?.Name}");
                // Logica per aggiornare l'inventario
            };

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}