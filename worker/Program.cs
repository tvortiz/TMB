using System;
using System.Text;
using System.Text.Json;
using RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net.Http;

class Program
{
    private static string RabbitHostName = "rabbitmq";



    static void Main()
    {
        Console.WriteLine("Starting backend worker service. Waiting 20s for RabbitMQ to be ready...");
        Thread.Sleep(15000);
        Console.WriteLine("Connecting to RabbitMQ...");

        ConnectToRabbit().Wait();

        while (true)
        {
            Thread.Sleep(1000);
        }
    }

    static async Task ConnectToRabbit()
    {
        Connect_To_Channel("NewOrder", NewOrderReceived);
        Connect_To_Channel("OrderProcessed", OrderProcessedReceived);
    }

    static async Task Connect_To_Channel(string queueName, RabbitMQ.Client.Events.AsyncEventHandler<RabbitMQ.Client.Events.BasicDeliverEventArgs> method)
    {
        var factory = new ConnectionFactory { HostName = RabbitHostName };

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

        AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += method;

        channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);

        while (true)
        {
            Thread.Sleep(1000);
        }
    }

    private static async Task NewOrderReceived(object model, BasicDeliverEventArgs ea)
    {
        string message = Encoding.UTF8.GetString(ea.Body.ToArray());

        await ChannelHandler(model, ea, message, async () =>
        {
            //esperando 5 segundos para simular processamento
            Thread.Sleep(5000);
            await CallAPI("http://backend:8080/API/Orders/ChangeStatusToProcessando/" + message);

            // colocando na fila de pedidos processados
            await SendMessageToQueue(message, "OrderProcessed");
        });
    }

    private static async Task OrderProcessedReceived(object model, BasicDeliverEventArgs ea)
    {
        string message = Encoding.UTF8.GetString(ea.Body.ToArray());

        await ChannelHandler(model, ea, message, async () =>
        {
            //esperando 5 segundos para simular processamento
            Thread.Sleep(5000);
            await CallAPI("http://backend:8080/API/Orders/ChangeStatusToFinalizado/" + message);
        });
    }

    private static async Task ChannelHandler(object model, BasicDeliverEventArgs ea, string message, Action action)
    {
        Console.WriteLine($"[x] Received: {message}");

        var consumer = (AsyncEventingBasicConsumer)model;
        var channel = consumer.Channel;

        try
        {
            action.Invoke();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[!] Error processing message: '{ex.Message}'");
            channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
        }

        Console.WriteLine($"[x] Processing completed for: '{message}'");
        channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
    }

    private static async Task CallAPI(string URL)
    {
        using HttpClient client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync(URL);
        response.EnsureSuccessStatusCode();
    }

    private static async Task SendMessageToQueue(string message, string queueName)
    {
        var factory = new ConnectionFactory { HostName = RabbitHostName };

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();
    
        var body = Encoding.UTF8.GetBytes(message);
        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: queueName, body: body);
    }
}

