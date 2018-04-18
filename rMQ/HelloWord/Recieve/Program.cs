using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Recieve {
    class Program {
        static void Main(string[] args) {
            var factory = new ConnectionFactory { HostName = "192.168.9.173", UserName = "admin", Password = "admin" };
            using (var conn = factory.CreateConnection()) {
                using (var channel = conn.CreateModel()) {
                    channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) => {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine("[x] Rec {0}", message);
                    };

                    channel.BasicConsume(queue: "hello", autoAck: true, consumer: consumer);
                    Console.Read();
                }
            }
        }
    }
}
