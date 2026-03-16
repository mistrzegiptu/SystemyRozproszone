using MessageDTO;

internal class ClientConsoleReader
{
    private readonly Client _client;

    public ClientConsoleReader(Client client)
    {
        _client = client;
    }

    public async Task StartReadingAsync()
    {
        Console.WriteLine("Connected. (T {message} to chat, U [sus/lenny/flip/nyan] to send image, M [sus/lenny/flip/nyan] {ids eg. 1 2 3} to send multicast, E to leave)");

        while(true)
        {
            var userInput = Console.ReadLine();

            if (string.IsNullOrEmpty(userInput)) 
                continue;

            var splittedInput = userInput.Split(' ');
            switch(splittedInput[0])
            {
                case "T":
                    await _client.SendAsyncTcp(splittedInput[1]);

                    break;
                case "U":
                    var asciiArt = AsciiUtils.Create(splittedInput[1]);

                    await _client.SendAsyncUdp(asciiArt);
                    break;
                case "M":
                    var asciiToMulticast = AsciiUtils.Create(splittedInput[^1]);
                    var targetIds = splittedInput.Skip(1).SkipLast(1).Select(int.Parse).ToList();

                    await _client.SendAsyncUdp(asciiToMulticast, targetIds);
                    break;
                case "E":
                    return;
            }
        }
    }
}
