using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace SocketDemo.Lib;

public static class TCP
{
    public static Message GetMessage(NetworkStream stream)
    {
        var builder = new StringBuilder();
        var data = new byte[64];
        var bytes = 0;
        do
        {
            bytes = stream.Read(data, 0, data.Length);
            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
        } while (stream.DataAvailable);
        var request = JsonSerializer.Deserialize<Message>(builder.ToString());
        return request;
    }

    public static void SendMessage(NetworkStream stream, Message message)
    {
        var _message = JsonSerializer.Serialize(message);
        var data = Encoding.Unicode.GetBytes(_message);
        stream.Write(data, 0, data.Length);
    }
}