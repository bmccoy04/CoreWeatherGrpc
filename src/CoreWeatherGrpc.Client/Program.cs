﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using CoreWeatherGrpc.Worker;
using Grpc.Net.Client;

namespace CoreWeatherGrpc.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            // The port number(5001) must match the port of the gRPC server.
            var channel = GrpcChannel.ForAddress("http://localhost:5004");
            var client = new Greeter.GreeterClient(channel);
            var reply = await client.SayHelloAsync(
                              new HelloRequest { Name = "GreeterClient" });
            Console.WriteLine("Greeting: " + reply.Message);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
