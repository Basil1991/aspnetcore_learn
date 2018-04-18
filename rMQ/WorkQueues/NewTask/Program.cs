using RabbitMQ.Client;
using System;
using System.Text;

namespace NewTask {
    class Program {
        static void Main(string[] args) {
            var factory = new ConnectionFactory { HostName = "192.168.9.173", UserName = "admin", Password = "admin" };
            using (var conn = factory.CreateConnection()) {
                using (var channel = conn.CreateModel()) {
                    var msg = GetMsg(args);
                    var body = Encoding.UTF8.GetBytes(msg);
                    var props = channel.CreateBasicProperties();
                    props.Persistent = false;
                    channel.BasicPublish(exchange: "", routingKey: "task_queue", basicProperties: props, body: body);
                }
            }
        }

        static string GetMsg(string[] args) {
            return ((args.Length > 0) ? string.Join(" ", args) : "Hello World");
        }
    }
}
