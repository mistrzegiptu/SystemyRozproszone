using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;

Console.Write("Input your nickname: ");
var nickname = Console.ReadLine();

if (!IPAddress.TryParse(ConfigurationManager.AppSettings["ServerIP"], out var ipAddress))
    throw new ConfigurationErrorsException();

if(!int.TryParse(ConfigurationManager.AppSettings["ServerPort"], out var port))
    throw new ConfigurationErrorsException();

IPEndPoint ipEndPoint = new(ipAddress, port);

using Socket client = new(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

client.Connect(ipEndPoint);

try
{
    while (true)
    {
        Console.Write("Enter message (EXIT to leave): ");
        var message = Console.ReadLine();

        if (string.IsNullOrEmpty(message) || message == "EXIT")
            break;

        var messageBytes = Encoding.UTF8.GetBytes(message);

        client.Send(messageBytes, SocketFlags.None);
    }
}
catch(Exception ex)
{
    Console.WriteLine(ex.ToString());
}
finally
{
    client.Shutdown(SocketShutdown.Both);
}