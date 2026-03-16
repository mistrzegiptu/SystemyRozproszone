using MessageDTO;
using System.Net;
using System.Net.Sockets;
using System.Text;

internal class Client : IDisposable
{
    private const int HandshakeSize = 4;
    private const int BufferSize = 1024;

    private IPEndPoint _endPoint;
    private Socket _tcpSocket;
    private Socket _udpSocket;
    private string _nickname = string.Empty;

    public int ClientId { get; private set; }

    private bool _isDisposed = false;

    public Client(IPEndPoint endPoint)
    {
        _endPoint = endPoint;
        _tcpSocket = new(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _udpSocket = new(endPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
    }

    public async Task<bool> InitAsync(string nickname)
    {
        _nickname = nickname;

        try
        {
            await _tcpSocket.ConnectAsync(_endPoint);

            byte[] nicknameBytes = Encoding.UTF8.GetBytes(_nickname);
            await _tcpSocket.SendAsync(nicknameBytes, SocketFlags.None);

            byte[] ackBuffer = new byte[HandshakeSize];
            int bytesRead = await _tcpSocket.ReceiveAsync(ackBuffer, SocketFlags.None);

            if(bytesRead != HandshakeSize)
            {
                Console.WriteLine("Invalid handshake size");
                return false;
            }
            
            var responseId = BitConverter.ToInt32(ackBuffer);

            if (responseId < 0)
            {
                Console.WriteLine("Nickname already in use");
                return false;
            }

            ClientId = responseId;

            _udpSocket.Connect(_endPoint);

            byte[] udpHello = BitConverter.GetBytes(ClientId);
            await _udpSocket.SendAsync(udpHello, SocketFlags.None);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message); 
            return false;
        }
    }

    public async Task SendAsyncTcp(string message)
    {
        try
        {
            TcpMessage messageDTO = new() { ClientNickname = _nickname ?? string.Empty, ClientId = ClientId, Message = message };

            await _tcpSocket.SendAsync(messageDTO.GetBytes(), SocketFlags.None);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public async Task<string?> ReadTcpAsync()
    {
        try
        {
            byte[] receiveBuffer = new byte[BufferSize];
            var bytesReceived = await _tcpSocket.ReceiveAsync(receiveBuffer, SocketFlags.None);

            if (bytesReceived == 0) 
                return null;

            var messageDTO = TcpMessage.Parse(receiveBuffer[..bytesReceived]);

            return messageDTO?.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    public async Task SendAsyncUdp(string message, List<int>? targetIds = null)
    {
        try
        {
            UdpMessage messageDTO = new() { ClientNickname = _nickname, ClientId = ClientId, Message = message, TargetIds = targetIds };

            await _udpSocket.SendAsync(messageDTO.GetBytes(), SocketFlags.None);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public async Task<string?> ReadUdpAsync()
    {
        try
        {
            byte[] receiveBuffer = new byte[BufferSize];
            var bytesReceived = await _udpSocket.ReceiveAsync(receiveBuffer, SocketFlags.None);

            var messageDTO = UdpMessage.Parse(receiveBuffer[..bytesReceived]);

            return messageDTO?.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    public void Dispose()
    {
        if (_isDisposed)
            return;

        if(_tcpSocket.Connected)
        {
            try
            {
                _tcpSocket.Shutdown(SocketShutdown.Both);
            }
            catch { }
        }

        _tcpSocket?.Dispose();
        _udpSocket?.Dispose();

        _isDisposed = true;
    }
}
