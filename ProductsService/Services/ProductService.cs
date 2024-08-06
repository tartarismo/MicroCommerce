using System.Text;
using ProductsService.Models;
using RabbitMQ.Client;
using Newtonsoft.Json;

namespace ProductsService.Services
{
    public class ProductService : IProductService
    {
        private readonly IConnection _connection;
        private readonly string? _queueName;

        public ProductService(IConnection connection, IConfiguration configuration)
        {
            _connection = connection;
            _queueName = configuration["RabbitMQ:QueueName"];
        }

        public void AddProduct(Product product)
        {
            using var channel = _connection.CreateModel();
            channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(product));
            channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: null, body: body);
        }
    }
}