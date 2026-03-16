internal class ClientConsolePrinter
{
    private readonly Client _client;

    public ClientConsolePrinter(Client client)
    {
        _client = client;
    }

    public async Task ListenTcpAsync()
    {
        while(true)
        {
            var message = await _client.ReadTcpAsync();

            if(message == null)
            {
                Console.WriteLine("TCP connection lost");
                break;
            }

            Console.WriteLine(message);
        }
    }

    public async Task ListenUdpAsync()
    {
        while(true)
        {
            var message = await _client.ReadUdpAsync();

            if (message == null)
            {
                Console.WriteLine("UDP stopped");
                break;
            }

            Console.WriteLine(message);
        }
    }
}