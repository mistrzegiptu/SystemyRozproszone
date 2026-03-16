using System.Configuration;
using System.Net;

if (!IPAddress.TryParse(ConfigurationManager.AppSettings["ServerIP"], out var ipAddress))
    throw new ConfigurationErrorsException();

if (!int.TryParse(ConfigurationManager.AppSettings["ServerPort"], out var port))
    throw new ConfigurationErrorsException();

IPEndPoint ipEndPoint = new(ipAddress, port);
ChatServer chatServer = new(ipEndPoint);

await chatServer.StartAsync();