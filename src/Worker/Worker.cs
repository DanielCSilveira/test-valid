using System.Text;
using System.Text.Json;
using Infra;
using Infra.Model;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Worker;

public class WorkerService : BackgroundService
{
    private readonly ILogger<WorkerService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ConnectionFactory _factory;
    private IConnection? _connection;
    private IChannel? _channel;

    private readonly string queueName;


    public WorkerService(ILogger<WorkerService> logger,
        IServiceScopeFactory scopeFactory,
        IConfiguration _config)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;

        var rabbitConfig = _config.GetSection("RabbitMQ");

        _factory = new ConnectionFactory
        {
            HostName = rabbitConfig.GetValue<string>("Hostname") ?? throw new ArgumentNullException("RabbitMq:Hostname"),
            Port = rabbitConfig.GetValue<int?>("Port") ?? throw new ArgumentNullException("RabbitMq:Port"),
            UserName = rabbitConfig.GetValue<string>("UserName") ?? throw new ArgumentNullException("RabbitMq:Hostname"),
            Password = rabbitConfig.GetValue<string>("Password") ?? throw new ArgumentNullException("RabbitMq:Password")
        };

        queueName = rabbitConfig.GetValue<string>("QueueName") ?? throw new ArgumentNullException("RabbitMq:QueueName");
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {


        _connection = await _factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await _channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Worker conectado ao RabbitMQ.");

        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_channel == null) return;

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += Consumer_ReceivedAsync;

        await _channel.BasicConsumeAsync(
            queue: queueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);
    }


    /// <summary>
    /// Consume and update Order, prove concept a async work process a job after a new order entry, in this case only change status to PENDING
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventDelivery"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private async Task Consumer_ReceivedAsync(object sender, BasicDeliverEventArgs eventDelivery)
    {
        try
        {
            var body = eventDelivery.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);
            var message = JsonSerializer.Deserialize<Order>(json);

            if (message != null)
            {
                _logger.LogInformation("Mensagem recebida: {@msg}", message);

                // resolve o DbContext ou service que atualiza pedido
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var order = await db.Orders.FindAsync(new object[] { message.Id });
                if (order != null)
                {
                    order.Status = "PENDING";
                    await db.SaveChangesAsync();
                    _logger.LogInformation("Pedido {id} atualizado para Pending.", order.Id);
                }
            }

            // confirma o processamento
            await _channel.BasicAckAsync(eventDelivery.DeliveryTag, false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar mensagem.");
        }

    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null) await _channel.CloseAsync(cancellationToken);
        if (_connection != null) await _connection.CloseAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }
}


