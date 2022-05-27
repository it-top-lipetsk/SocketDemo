using System.Net;
using System.Net.Sockets;
using System.Text;
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

            var request = new Request
            {
                Type = "get_all",
                Body = ""
            };
            var message = JsonSerializer.Serialize(request);
            var data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length);
            
            var builder = new StringBuilder();
            data = new byte[64];
            var bytes = 0;
            do
            {
                bytes = stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            } while (stream.DataAvailable);
            var response = JsonSerializer.Deserialize<Response>(builder.ToString());

            PrintResponse(response);
            
            server.Close();
        }

        static void PrintResponse(Response response)
        {
            var staffers = JsonSerializer.Deserialize<IEnumerable<Staffer>>(response.Body);

            foreach (var staffer in staffers)
            {
                Console.WriteLine($"{staffer.Id}: {staffer.LastName}");
            }
        }
    }
}