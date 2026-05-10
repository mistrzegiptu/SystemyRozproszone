using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using SpaceSystem.Shared;
using SpaceSystem.Shared.Models;


Console.Write("Input agency name: ");

string agencyName = Console.ReadLine() ?? "agency" + DateTime.Now.ToString();
int taskId = 1;

var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(exchange: RabbitConfig.ExchangeName, type: ExchangeType.Topic, durable: true, autoDelete: false);

var replyQueueName = (await channel.QueueDeclareAsync()).QueueName;

await channel.QueueBindAsync(queue: replyQueueName, exchange: RabbitConfig.ExchangeName, routingKey: $"ack.agency.{agencyName}");
await channel.QueueBindAsync(queue: replyQueueName, exchange: RabbitConfig.ExchangeName, routingKey: RabbitConfig.RoutingKeyAdminAgencies);
await channel.QueueBindAsync(queue: replyQueueName, exchange: RabbitConfig.ExchangeName, routingKey: RabbitConfig.RoutingKeyAdminAll);

var consumer = new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += Consumer_ReceivedAsync;
Task Consumer_ReceivedAsync(object sender, BasicDeliverEventArgs @event)
{
    var body = @event.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    var routingKey = @event.RoutingKey;

    Console.ForegroundColor = ConsoleColor.Magenta;
    Console.WriteLine($"\n[RECEIVED] (Key: {routingKey}) Message: {message}");
    Console.ResetColor();
    Console.Write("Choose option (1-People, 2-Cargos, 3-Satellites, q-quit): ");

    return Task.CompletedTask;
}

await channel.BasicConsumeAsync(queue: replyQueueName, autoAck: true, consumer: consumer);

while(true)
{
    Console.WriteLine("\nChoose service to delegate:");
    Console.WriteLine("1 - Transport people");
    Console.WriteLine("2 - Transport cargo");
    Console.WriteLine("3 - Put satellite into orbit");
    Console.WriteLine("q - Exit");

    Console.Write("Choose option: ");
    var input = Console.ReadLine();

    string serviceType = "";
    string routingKey = "";

    switch (input)
    {
        case "1":
            serviceType = "people";
            routingKey = "task.people";
            break;
        case "2":
            serviceType = "cargo";
            routingKey = "task.cargo";
            break;
        case "3":
            serviceType = "satellite";
            routingKey = "task.satellite";
            break;
        case "q":
            return;
        default:
            Console.WriteLine("Wrong option, try again");
            continue;
    }

    var order = new SpaceOrder
    {
        AgencyName = agencyName,
        TaskId = taskId++,
        ServiceType = serviceType
    };

    string jsonMessage = JsonSerializer.Serialize(order);
    var body = Encoding.UTF8.GetBytes(jsonMessage);

    await channel.BasicPublishAsync(exchange: RabbitConfig.ExchangeName, routingKey: routingKey, body: body);

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"[SENT] Order {order.TaskId} for service: '{serviceType}'.");
    Console.ResetColor();
}