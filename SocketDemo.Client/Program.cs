using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using SocketDemo.Lib;

namespace SocketDemo.Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var server = new TcpClient();
            server.Connect(IPAddress.Loopback, 8005);
            using var stream = server.GetStream();
            
            while (true)
            {
                var request = new Message
                {
                    Type = "get_all",
                    Body = ""
                };

                TCP.SendMessage(stream, request);

                var response = TCP.GetMessage(stream);

                PrintResponse(response);

                Console.ReadKey();
            }

            server.Close();
        }

        static void PrintResponse(Message response)
        {
            var staffers = JsonSerializer.Deserialize<IEnumerable<Staffer>>(response.Body);

            foreach (var staffer in staffers)
            {
                Console.WriteLine($"{staffer.Id}: {staffer.LastName}");
            }
        }
    }
}