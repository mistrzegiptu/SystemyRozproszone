using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;

if (!IPAddress.TryParse(ConfigurationManager.AppSettings["ServerIP"], out var ipAddress))
    throw new ConfigurationErrorsException();

if (!int.TryParse(ConfigurationManager.AppSettings["ServerPort"], out var port))
    throw new ConfigurationErrorsException();

IPEndPoint ipEndPoint = new(ipAddress, port);

using Socket listener = new(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

listener.Bind(ipEndPoint);
listener.Listen();

var handler = listener.Accept();
while(true)
{
    var buffer = new byte[1024];
    var received = handler.Receive(buffer, SocketFlags.None);
    var response = Encoding.UTF8.GetString(buffer, 0, received);

    Console.WriteLine(response);
    Console.WriteLine(handler?.RemoteEndPoint?.ToString());
}