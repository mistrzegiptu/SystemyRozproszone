using MessageDTO;
using System.Collections.Concurrent;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Text;

internal class ChatServer
{
    private readonly Socket _tcpListener;
    private readonly Socket _udpListener;

    private ConcurrentDictionary<int, ConnectedClient> _connectedClients = [];
    private int _nextClientId;

    public ChatServer(IPEndPoint endPoint)
    {
        _tcpListener = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _tcpListener.Bind(endPoint);

        _udpListener = new Socket(endPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
        _udpListener.Bind(endPoint);
    }

    public async Task StartAsync()
    {
        _tcpListener.Listen(100);
        Console.WriteLine("Server started");

        _ = Task.Run(ListenForUdpAsync);

        while (true)
        {
            Socket clientSocket = await _tcpListener.AcceptAsync();

            _ = Task.Run(() => HandleTcpClientAsync(clientSocket));
        }
    }

    private async Task HandleTcpClientAsync(Socket clientSocket)
    {
        try
        {
            byte[] nicknameBuffer = new byte[256];
            int bytesReaded = await clientSocket.ReceiveAsync(nicknameBuffer, SocketFlags.None);

            if (bytesReaded == 0)
                return;

            string nickname = Encoding.UTF8.GetString(nicknameBuffer[..bytesReaded]);

            bool isTaken = _connectedClients.Any(c => c.Value.Nickname.Equals(nickname));
            if(isTaken)
            {
                await clientSocket.SendAsync(BitConverter.GetBytes(-1), SocketFlags.None);
                clientSocket.Close();
                return;
            }

            int newClientId = _nextClientId;
            _nextClientId++;

            await clientSocket.SendAsync(BitConverter.GetBytes(newClientId), SocketFlags.None);

            var newClient = new ConnectedClient { Id = newClientId, Nickname = nickname, Socket = clientSocket };
            _connectedClients.TryAdd(newClientId, newClient);

            Console.WriteLine($"{nickname} ({newClientId}) joined the chat!");

            byte[] receiveBuffer = new byte[1024];
            while(true)
            {
                bytesReaded = await clientSocket.ReceiveAsync(receiveBuffer, SocketFlags.None);

                if (bytesReaded == 0)
                    break;

                byte[] actualMessage = receiveBuffer[..bytesReaded];

                await BroadcastTcpAsync(actualMessage, newClientId);
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            clientSocket.Close();
        }
    }

    private async Task BroadcastTcpAsync(byte[] data, int senderId)
    {
        var clientsToBroadcast = _connectedClients.Where(c => c.Value.Id != senderId).Select(c => c.Value);

        foreach(var client in clientsToBroadcast)
        {
            try
            {
                await client.Socket.SendAsync(data, SocketFlags.None);
            }
            catch { }
        }
    }

    private async Task ListenForUdpAsync()
    {
        byte[] buffer = new byte[1024];

        EndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);

        while(true)
        {
            try
            {
                var result = await _udpListener.ReceiveFromAsync(buffer, SocketFlags.None, clientEndPoint);
                byte[] receivedMessage = buffer[..result.ReceivedBytes];

                if(result.ReceivedBytes == 4)
                {
                    int clientId = BitConverter.ToInt32(receivedMessage);

                    if(_connectedClients.TryGetValue(clientId, out var client))
                    {
                        client.UdpEndPoint = result.RemoteEndPoint;
                    }
                }
                else
                {
                    await BroadcastUdpAsync(receivedMessage, result.RemoteEndPoint);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    private async Task BroadcastUdpAsync(byte[] data, EndPoint sender, List<int>? targetIds = null)
    {
        var udpMessage = UdpMessage.Parse(data);

        var clientsToBroadcast = _connectedClients.Where(c => c.Value.UdpEndPoint != null && !c.Value.UdpEndPoint.Equals(sender)).Select(c => c.Value);

        if (udpMessage?.TargetIds != null && udpMessage.TargetIds.Any())
        {
            clientsToBroadcast = clientsToBroadcast.Where(c => udpMessage.TargetIds.Contains(c.Id));
        }

        foreach(var client in clientsToBroadcast)
        {
            try
            {
                await _udpListener.SendToAsync(data, SocketFlags.None, client.UdpEndPoint);
            }
            catch { }
        }
    }
}