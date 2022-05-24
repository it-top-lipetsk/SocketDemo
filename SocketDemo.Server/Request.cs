namespace SocketDemo.Server;

public class Request
{
    public string Type { get; set; } // get_all, get_by_id, ...
    public string Body { get; set; } // null, <id>
}

