using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
#pragma warning disable 4014

class ChatServer {
    private const int Port = 2020;
    private readonly List<TcpClient> tcpClients = new List<TcpClient>();
    private TcpListener tcpListener;

    public ChatServer() {
        tcpListener = new TcpListener(IPAddress.Any, Port);
    }

    static void Main(string[] args) {
        new ChatServer().Run();
    }

    public void Run() {
        tcpListener.Start();
        while (true) {
            TcpClient newClient = tcpListener.AcceptTcpClient();
            tcpClients.Add(newClient);
            Console.WriteLine("Created connection with {0}", newClient.Client.RemoteEndPoint);
            Task.Run(() => BeginRecievingMessages(newClient));
            SendMessage("Welcome!", newClient);
        }
    }

    public void SendMessage(string message, params TcpClient[] tcpClients) {
        byte[] encodedString = Encoding.UTF8.GetBytes(message);
        foreach (TcpClient client in tcpClients) {
            client.GetStream().Write(encodedString, 0, encodedString.Length);
        }
    }

    public void BeginRecievingMessages(TcpClient tcpClient) {
        while(true) {
            byte[] inputBuffer = new byte[tcpClient.ReceiveBufferSize];
            int bytesRead = tcpClient.GetStream().Read(inputBuffer, 0, inputBuffer.Length);

            if (bytesRead > 0) {
                string recievedMsg = Encoding.UTF8.GetString(inputBuffer, 0, bytesRead);
                SendMessage(recievedMsg, tcpClients.Except(new List<TcpClient>(){tcpClient}).ToArray());
            }
        }
    }
}