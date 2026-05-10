
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SpaceSystem.Shared;
using SpaceSystem.Shared.Models;
using System.Text;
using System.Text.Json;

Console.Write("Input carrier name: ");
string carrierName = Console.ReadLine() ?? "carrier" + DateTime.Now.ToString();

Console.WriteLine("Choose your service:");
Console.WriteLine("1. People transport + cargo transport");
Console.WriteLine("2. People transport + sending satellite into orbit");
Console.WriteLine("3. Cargo transport + sending satellite into orbit");

Console.Write("Your choice: ");

string choice = Console.ReadLine() ?? "1";

var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(exchange: RabbitConfig.ExchangeName, type: ExchangeType.Topic, durable: true, autoDelete: false);

await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

var consumerTask = new AsyncEventingBasicConsumer(channel);

consumerTask.ReceivedAsync += async (sender, @event) =>
{
    var body = @event.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);

    var order = JsonSerializer.Deserialize<SpaceOrder>(message);

    if(order != null)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n[ORDER RECIEVED] Executing order #{order.TaskId} ({order.ServiceType}) for {order.AgencyName}...");

        Console.WriteLine($"[ORDER DONE] Sending back ack.");
        Console.ResetColor();

        string ackMessage = $"Order #{order.TaskId} ({order.ServiceType}) done by {carrierName}";
        var ackBody = Encoding.UTF8.GetBytes(ackMessage);

        await channel.BasicPublishAsync(exchange: RabbitConfig.ExchangeName, routingKey: $"ack.agency.{order.AgencyName}", body: ackBody);

        await channel.BasicAckAsync(deliveryTag: @event.DeliveryTag, multiple: false);
    }
};

List<string> selectedServices = new List<string>();
switch (choice)
{
    case "1":
        selectedServices.Add("people");
        selectedServices.Add("cargo");
        break;
    case "2":
        selectedServices.Add("people");
        selectedServices.Add("satellite");
        break;
    case "3":
        selectedServices.Add("cargo");
        selectedServices.Add("satellite");
        break;
    default:
        Console.WriteLine("Wrong choice. Exiting.");
        return;
}

foreach (var service in selectedServices)
{
    string queueName = $"queue_{service}";
    string routingKey = $"task.{service}";

    await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false);
    await channel.QueueBindAsync(queue: queueName, exchange: RabbitConfig.ExchangeName, routingKey: routingKey);

    await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumerTask);
}

var adminQueueName = (await channel.QueueDeclareAsync()).QueueName;
await channel.QueueBindAsync(queue: adminQueueName, exchange: RabbitConfig.ExchangeName, routingKey: RabbitConfig.RoutingKeyAdminCarriers);
await channel.QueueBindAsync(queue: adminQueueName, exchange: RabbitConfig.ExchangeName, routingKey: RabbitConfig.RoutingKeyAdminAll);

var adminConsumer = new AsyncEventingBasicConsumer(channel);
adminConsumer.ReceivedAsync += (sender, @event) =>
{
    var body = @event.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);

    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\n[ADMIN BROADCAST] {message}");
    Console.ResetColor();

    return Task.CompletedTask;
};

await channel.BasicConsumeAsync(queue: adminQueueName, autoAck: true, consumer: adminConsumer);

Console.WriteLine($"\nCarrier {carrierName} is now running and listening for: {string.Join(" and ", selectedServices)}...");
Console.WriteLine("Press [Enter] to exit.");
Console.ReadLine();