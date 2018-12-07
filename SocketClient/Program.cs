using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SocketClient
{
    class Program
    {
        static void Main(string[] args)
        {
            int messageCounter = 1;
            int secondsBeforeNextMessage = 5;
            TcpClient tcpClient = new TcpClient();

            string hostname = Dns.GetHostName();
            Console.WriteLine($"Socket client started on '{hostname}'");

            string serverAddress = Environment.GetEnvironmentVariable("ServerAddress");
            string serverPort = Environment.GetEnvironmentVariable("ServerPort");
            int port = int.Parse(serverPort);

            try
            {
                Console.WriteLine($"Connecting to {serverAddress}:{serverPort}");
                tcpClient.Connect(serverAddress, port);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"tcpClient.Connect exception: {ex.Message}");
                return;
            }

            NetworkStream networkStream = tcpClient.GetStream();
            StreamReader clientStreamReader = new StreamReader(networkStream);
            StreamWriter clientStreamWriter = new StreamWriter(networkStream);

            while (true)
            {
                try
                {
                    string message = $"Hello from {hostname} at {DateTime.Now}";

                    Console.WriteLine($"Sending message #{messageCounter++}: '{message}'");
                    clientStreamWriter.WriteLine(message);
                    clientStreamWriter.Flush();

                    Console.WriteLine("Waiting for reply...");

                    do
                    {
                        message = clientStreamReader.ReadLine();
                    } while (string.IsNullOrEmpty(message));

                    Console.WriteLine($"Message received at {DateTime.Now}: '{message}'");

                    secondsBeforeNextMessage = 2 * secondsBeforeNextMessage;
                    DateTime timeNow = DateTime.Now;

                    Console.WriteLine($"Sending next message in {secondsBeforeNextMessage} seconds at {timeNow.AddSeconds(secondsBeforeNextMessage)}");
                    Thread.Sleep(secondsBeforeNextMessage * 1000);
                    Console.WriteLine("");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n>>>>>>>> Exception: {ex.Message}\n");
                    return;
                }
            }
        }
    }
}
