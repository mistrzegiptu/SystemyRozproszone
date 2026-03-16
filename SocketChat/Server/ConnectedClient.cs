using System.Net;
using System.Net.Sockets;

internal class ConnectedClient
{
    public int Id { get; set; }
    public required string Nickname { get; set; }
    public required Socket Socket { get; set; }

    public EndPoint? UdpEndPoint { get; set; }
}