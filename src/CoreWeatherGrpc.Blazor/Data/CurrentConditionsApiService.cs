
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CoreWeatherGrpc.Worker;
using Google.Protobuf.Collections;
using Grpc.Core;
using Grpc.Net.Client;

namespace CoreWeatherGrpc.Blazor.Data
{
    public class CurrentConditionsApiService
    {
        public CurrentConditionsApiService()
        {

        }

        public async Task<IEnumerable<CurrentConditionsDto>> Get()
        {
            var client = new HttpClient();   

            var response = await client.GetAsync("https://core-weather-api.azurewebsites.net/api/v1/current-conditions");

            var retVal = await response.Content.ReadAsStringAsync();
            
            // New Serializer!!!!
            return JsonSerializer.Deserialize<IEnumerable<CurrentConditionsDto>>(retVal);
        }


        public async Task<RepeatedField<CurrentCondition>> GetGrpc()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            // The port number(5001) must match the port of the gRPC server.
            var channel = GrpcChannel.ForAddress("http://localhost:5004");

            var client = new CurrentConditions.CurrentConditionsClient(channel);
            var reply = await client.GetCurrentConditionsAsync(new CurrentConditionsRequest(){});

            return reply.CurrentConditions;
        }

        public IAsyncEnumerable<CurrentConditionsReply> GetStreamingWeather(CancellationToken token)
        {
            var channel = GrpcChannel.ForAddress("http://localhost:5004");

            var client = new CurrentConditions.CurrentConditionsClient(channel);

            return client.GetCurrentConditionStream(new CurrentConditionsRequest(), cancellationToken: token)
                .ResponseStream.ReadAllAsync();
        }

    }

    public class CurrentConditionsDto
    {
        public string name { get; set; }
        public string time { get; set; }    
        public string relativeHumidity { get; set; }    
        public string visibility { get; set; }  
        public string description { get; set; }
        public string type { get; set; }
        public string temperature { get; set; }
    }
}
