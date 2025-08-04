using System;
using System.Text;
using System.Text.Json;
using RabbitMQ;
using RabbitMQ.Client;


public class RabbitMQMessenger : IMessenger, IDisposable
{
    private static string RabbitHostName = "rabbitmq";
    private static string channelName = "NewOrder";

    private IConnection _connection;
    private IChannel _channel;

    

    public RabbitMQMessenger()
    {
        Startconnection().Wait();
    }

    private async Task Startconnection()
    {
        var factory = new ConnectionFactory { HostName = RabbitHostName };
        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        await _channel.QueueDeclareAsync(queue: channelName, durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }

    public async void SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);

        await _channel.BasicPublishAsync(exchange: string.Empty, routingKey: channelName, body: body);
        Console.WriteLine($" [x] Sent '{message}' to channel {channelName}");
    }
}