using RabbitMQ.Client;
using System;
using System.Text;

namespace Send
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "192.168.9.173", UserName = "admin", Password = "admin" };
            using (var conn = factory.CreateConnection()) {
                using (var channel = conn.CreateModel()) {
                    channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

                    string msg = "Hello World";
                    var body = Encoding.UTF8.GetBytes(msg);

                    channel.BasicPublish(exchange: "", routingKey: "hello", basicProperties: null, body: body);
                    Console.WriteLine(@"[x] sent {0}", msg);
                    Console.Read();
                }
            }
        }
    }
}
