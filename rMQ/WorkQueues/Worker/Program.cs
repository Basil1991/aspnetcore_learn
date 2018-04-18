using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace Worker {
    class Program {
        static void Main(string[] args) {
            var factory = new ConnectionFactory { HostName = "192.168.9.173", UserName = "admin", Password = "admin" };
            using (var conn = factory.CreateConnection()) {
                using (var channel = conn.CreateModel()) {
                    channel.QueueDeclare(queue: "task_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) => {
                        var body = ea.Body;
                        var msg = Encoding.UTF8.GetString(body);
                        int dots = msg.Split(".").Length - 1;
                        Thread.Sleep(dots * 1000);
                        Console.WriteLine("[x] Done");
                        //imp, It's a common mistake
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    };
                    channel.BasicConsume(queue: "task_queue", autoAck: false, consumer: consumer);
                    Console.Read();
                }
            }
        }
    }
}