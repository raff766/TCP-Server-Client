using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class ChatClient {
    public const string ServerAddress = "localhost";
    public const int Port = 2020;
    public const string Name = "Test";

    private TcpClient server;

    static void Main(string[] args)
    {
        ChatClient chatClient = new ChatClient();
        chatClient.Connect();

        while(true) {
            string input = Console.ReadLine();
            chatClient.SendMessage(input);
        }
    }

    public void Connect() {
        server = new TcpClient(ServerAddress, Port);
        Console.WriteLine("Connected to server at {0}.", server.Client.RemoteEndPoint);

        Task.Run(() => BeginRecievingMessages());
    }

    private void BeginRecievingMessages() {
        while(true) {
            byte[] inputBuffer = new byte[server.ReceiveBufferSize];
            int recievedBytes = server.GetStream().Read(inputBuffer, 0, inputBuffer.Length);
            if (recievedBytes > 0) {
                Console.WriteLine(Encoding.UTF8.GetString(inputBuffer, 0, recievedBytes));
            }
        }
    }

    public void SendMessage(string message) {
        byte[] outputBuffer = Encoding.UTF8.GetBytes(message);
        server.GetStream().Write(outputBuffer, 0, outputBuffer.Length);
    }
}
