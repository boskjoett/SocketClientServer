using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace SocketServer
{
    class Program
    {
        static int messageCounter = 1;
        static List<TcpClient> connectedClients = new List<TcpClient>();

        static void Main(string[] args)
        {
            int port = 5000;
            string hostname = Dns.GetHostName();
            Console.WriteLine($"Socket server started on '{hostname}'");

            string serverPort = Environment.GetEnvironmentVariable("ServerPort");
            if (!string.IsNullOrEmpty(serverPort))
            {
                port = int.Parse(serverPort);
            }

            try
            {
                IPEndPoint local = new IPEndPoint(IPAddress.Any, port);
                TcpListener listener = new TcpListener(local);

                listener.Start();
                Console.WriteLine($"Listening on port {port}");

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    Console.WriteLine("Client connected");

                    connectedClients.Add(client);
                    HandleClient(client);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"tcpClient.Connect exception:{ex.Message}");
                return;
            }
        }

        private static void HandleClient(TcpClient tcpClient)
        {
            string message;
            NetworkStream networkStream = tcpClient.GetStream();
            StreamReader clientStreamReader = new StreamReader(networkStream);
            StreamWriter clientStreamWriter = new StreamWriter(networkStream);

            while (true)
            {
                try
                {
                    do
                    {
                        message = clientStreamReader.ReadLine();
                    } while (string.IsNullOrEmpty(message));

                    Console.WriteLine($"Echoing message #{messageCounter++} received from client {connectedClients.IndexOf(tcpClient) + 1} at {DateTime.Now}: '{message}'");
                    message = $"Echo: {message}";
                    clientStreamWriter.WriteLine(message);
                    clientStreamWriter.Flush();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n>>>>>>>> Exception: {ex.Message}\n");
                    break;
                }
            }

            clientStreamReader.Dispose();
            clientStreamWriter.Dispose();
            networkStream.Dispose();
        }
    }
}
