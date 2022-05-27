using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Dapper;
using Microsoft.Data.Sqlite;
using SocketDemo.Lib;

namespace SocketDemo.Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var server = new TcpListener(IPAddress.Loopback, 8005);
            server.Start();
            
            Console.WriteLine("Сервер запущен");

            while (true)
            {
                var client = server.AcceptTcpClient();
                Task.Run(() =>
                {
                    Console.WriteLine("Новый клиент подключился");
                    
                    var stream = client.GetStream();
                    var builder = new StringBuilder();
                    var data = new byte[64];
                    var bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    } while (stream.DataAvailable);
                    var request = JsonSerializer.Deserialize<Request>(builder.ToString());
                    Response response = null;
                    switch (request.Type)
                    {
                        case "get_all":
                            response = GetAllStaffers();
                            break;
                        case "get_by_id":
                            break;
                    }

                    var message = JsonSerializer.Serialize(response);
                    data = Encoding.Unicode.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                });
            }
            
            server.Stop();
        }

        static Response GetAllStaffers()
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

            const string STR = "Data Source=staffer.db;";
            using var db = new SqliteConnection(STR);
            var sql = @"SELECT tab_persons.id AS 'id', 
       first_name, last_name, date_of_birth,
       department, position
        FROM tab_staffers
            JOIN tab_persons 
                ON tab_staffers.person_id = tab_persons.id
            JOIN tab_department
                ON tab_staffers.department_id = tab_department.id
            JOIN tab_positions
                ON tab_staffers.position_id = tab_positions.id;";
            var staffers = db.Query<Staffer>(sql);
            return new Response
            {
                Type = "staffers",
                Body = JsonSerializer.Serialize(staffers)
            };
        }
    }
}