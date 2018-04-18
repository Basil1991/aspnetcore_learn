using Grpc.Core;
using GRPCDemo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace gRPCServer {
    class Program {
        const int Port = 9007;
        public static void Main(string[] args) {
            Server server = new Server {
                Services = { gRPC.BindService(new gRPCImpl()) },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("gRPC server listening on port " + Port);
            Console.WriteLine("任意键退出...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
    public class gRPCImpl : gRPC.gRPCBase {
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context) {
            return Task.FromResult(new HelloReply { Message = "Hello " + request.Name });
        }
        public override async Task ServerStreamSayHello(HelloRequest request, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context) {
            await responseStream.WriteAsync(new HelloReply { Message = "Hello " + request.Name });
            await responseStream.WriteAsync(new HelloReply { Message = "Hello1 " + request.Name });
            await responseStream.WriteAsync(new HelloReply { Message = "Hello2 " + request.Name });
        }
        public override Task<HelloReply> ClientStreamSayHello(IAsyncStreamReader<HelloRequest> requestStream, ServerCallContext context) {
            string users = "";
            while (requestStream.MoveNext().Result) {
                users += requestStream.Current.Name + ";";
            }
            return Task.FromResult(new HelloReply { Message = "Hello " + users });
        }
        public override async Task biStreamSayHello(IAsyncStreamReader<HelloRequest> requestStream, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context) {
            while (requestStream.MoveNext().Result) {
                await responseStream.WriteAsync(new HelloReply { Message = "Hello1 " + requestStream.Current.Name });
                await responseStream.WriteAsync(new HelloReply { Message = "Hello2 " + requestStream.Current.Name });
            }
        }
    }
}
