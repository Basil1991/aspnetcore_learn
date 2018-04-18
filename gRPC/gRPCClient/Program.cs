using Grpc.Core;
using GRPCDemo;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace gRPCClient {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("Begin");
            //for (int i = 0; i < 100; i++) {
            //    Do();
            //    ServerStreamDo();
            //    ClientStreamDo();
            //    BiStreamDo();
            //}
            Test();
            Console.WriteLine("任意键退出...");
            Console.ReadKey();
        }
        static void Do() {
            Channel channel = new Channel("127.0.0.1:9007", ChannelCredentials.Insecure);
            var client = new gRPC.gRPCClient(channel);

            var reply = client.SayHello(new HelloRequest { Name = "Basil" });
            Console.WriteLine("Recieved Do:" + reply.Message);

            channel.ShutdownAsync().Wait();
        }
        static async void ServerStreamDo() {
            Channel channel = new Channel("127.0.0.1:9007", ChannelCredentials.Insecure);
            var client = new gRPC.gRPCClient(channel);

            using (var call = client.ServerStreamSayHello(new HelloRequest { Name = "Jed" })) {
                while (await call.ResponseStream.MoveNext()) {
                    var current = call.ResponseStream.Current;
                    Console.WriteLine("Received ServerStream " + current.Message);
                }
            }

            channel.ShutdownAsync().Wait();
        }
        static async void ClientStreamDo() {
            Channel channel = new Channel("127.0.0.1:9007", ChannelCredentials.Insecure);
            var client = new gRPC.gRPCClient(channel);

            HelloRequest[] requests = { new HelloRequest { Name = "Basil" }, new HelloRequest { Name = "Jed" } };
            using (var call = client.ClientStreamSayHello()) {
                foreach (var request in requests) {
                    await call.RequestStream.WriteAsync(request);
                }
                await call.RequestStream.CompleteAsync();
                HelloReply reply = await call.ResponseAsync;
                Console.WriteLine("Received ClientStream " + reply.Message);
            }

            channel.ShutdownAsync().Wait();
        }
        static async void BiStreamDo() {
            Channel channel = new Channel("127.0.0.1:9007", ChannelCredentials.Insecure);
            var client = new gRPC.gRPCClient(channel);

            HelloRequest[] requests = { new HelloRequest { Name = "Basil" }, new HelloRequest { Name = "Jed" } };
            using (var call = client.biStreamSayHello()) {
                var responseReaderTask = Task.Run(async () => {
                    while (await call.ResponseStream.MoveNext()) {
                        var current = call.ResponseStream.Current;
                        Console.WriteLine("Received ServerStream " + current.Message);
                    }
                });

                foreach (var request in requests) {
                    await call.RequestStream.WriteAsync(request);
                }
                await call.RequestStream.CompleteAsync();
                await responseReaderTask;
            }

            channel.ShutdownAsync().Wait();
        }
        static async void Test() {
            var taskR = Task.Run(() => {
                Thread.Sleep(1000);
                Console.WriteLine("after sleep 1000");
                return "123";
            });
            await taskR;
            Console.Write(taskR.Result);
        }
    }
}
