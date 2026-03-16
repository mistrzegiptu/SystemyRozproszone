using System.Configuration;
using System.Net;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

Console.Write("Input your nickname: ");
var nickname = Console.ReadLine() ?? string.Empty;

if (!IPAddress.TryParse(ConfigurationManager.AppSettings["ServerIP"], out var ipAddress))
    throw new ConfigurationErrorsException();

if(!int.TryParse(ConfigurationManager.AppSettings["ServerPort"], out var port))
    throw new ConfigurationErrorsException();

IPEndPoint ipEndPoint = new(ipAddress, port);

using var client = new Client(ipEndPoint);

if(await client.InitAsync(nickname))
{
    var printer = new ClientConsolePrinter(client);
    var reader = new ClientConsoleReader(client);

    var printTcpTask = Task.Run(() => printer.ListenTcpAsync());
    var printUdpTask = Task.Run(() => printer.ListenUdpAsync());

    await reader.StartReadingAsync();
}
else
{
    Console.WriteLine("Failed to connect");
}