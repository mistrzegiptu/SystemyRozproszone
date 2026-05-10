using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SpaceSystem.Shared;
using System.Text;

Console.WriteLine("ADMIN PANEL");

var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(exchange: RabbitConfig.ExchangeName, type: ExchangeType.Topic, durable: true, autoDelete: false);

var adminQueueName = (await channel.QueueDeclareAsync()).QueueName;

await channel.QueueBindAsync(queue: adminQueueName, exchange: RabbitConfig.ExchangeName, routingKey: "#");

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += (sender, @event) =>
{
    var body = @event.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    var routingKey = @event.RoutingKey;

    if (!routingKey.StartsWith("admin."))
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine($"\n[LISTENING] (Key: {routingKey}) Message: {message}");
        Console.ResetColor();
    }

    return Task.CompletedTask;
};

await channel.BasicConsumeAsync(queue: adminQueueName, autoAck: true, consumer: consumer);

while (true)
{
    Console.WriteLine("\nBROADCAST MENU");
    Console.WriteLine("Choose sending mode");
    Console.WriteLine("1. All agencies");
    Console.WriteLine("2. All carriers");
    Console.WriteLine("3. All agencies and carriers");
    Console.WriteLine("q. Exit");
    Console.Write("Your choice: ");

    var choice = Console.ReadLine();

    string routingKey = "";
    switch (choice)
    {
        case "1":
            routingKey = RabbitConfig.RoutingKeyAdminAgencies;
            break;
        case "2":
            routingKey = RabbitConfig.RoutingKeyAdminCarriers;
            break;
        case "3":
            routingKey = RabbitConfig.RoutingKeyAdminAll;
            break;
        case "q":
            return;
        default:
            Console.WriteLine("Wrong choice, try again.");
            continue;
    }

    Console.Write("Enter message: ");
    string text = Console.ReadLine() ?? string.Empty;

    var body = Encoding.UTF8.GetBytes(text);

    await channel.BasicPublishAsync(exchange: RabbitConfig.ExchangeName, routingKey: routingKey, body: body);

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"[SENT] Message broadcasted with key: {routingKey}.");
    Console.ResetColor();
}