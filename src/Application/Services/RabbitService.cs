using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace Application.Services
{



    public class RabbitService : IRabbitService
    {
        private readonly ConnectionFactory _factory;
        private readonly string _defaultQueue;

        private IConnection? _connection;
        private IChannel? _channel;
        public RabbitService(IConfiguration config)
        {
            var section = config.GetSection("RabbitMQ");

            _factory = new ConnectionFactory
            {
                HostName = section["HostName"] ?? throw new ArgumentNullException("RabbitMq:Hostname"),
                Port = int.TryParse(section["Port"], out var port) ? port : throw new ArgumentNullException("RabbitMq:Hostname"),
                UserName = section["UserName"] ?? throw new ArgumentNullException("RabbitMq:UserName"),
                Password = section["Password"] ?? throw new ArgumentNullException("RabbitMq:Passsword")
            };

            _defaultQueue = section["QueueName"] ?? "default";
        }

        public async Task InitAsync()
        {
            _connection = await _factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

      
        }

        public async Task PublishAsync(string queue, object data)
        {
            if (_channel == null)
            {
                await InitAsync();

                if (_channel == null)
                    throw new Exception("erro ao inicializar o rabbit service");

            }
            var json = JsonSerializer.Serialize(data);
            var body = Encoding.UTF8.GetBytes(json);

            var props = new BasicProperties
            {
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent 
            };
            // garante que a fila existe
            await _channel.QueueDeclareAsync(
                queue: queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            await _channel.BasicPublishAsync<BasicProperties>(
                exchange: "",
                routingKey: queue,
                mandatory: false,
                basicProperties: props,
                body: body
            );
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null)
                await _channel.CloseAsync();

            if (_connection != null)
                await _connection.CloseAsync();
        }
    }


}