using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace CoreWeatherGrpc.Worker
{
    public class CurrentConditionsService : CurrentConditions.CurrentConditionsBase
    {
        private readonly ILogger<GreeterService> _logger;
        public CurrentConditionsService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override async Task<CurrentConditionReply> GetCurrentCondition(CurrentConditionRequest request, ServerCallContext context)
        {
            _logger.LogWarning("Get current condition called");
            var client = new HttpClient();

            var response = await client.GetAsync("https://core-weather-api.azurewebsites.net/api/v1/current-conditions/" + request.PlanetId);

            var retVal = await response.Content.ReadAsStringAsync();

            // New Serializer!!!!
            var jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.PropertyNameCaseInsensitive = true;

            var currentConditionReply = JsonSerializer.Deserialize<CurrentConditionReply>(retVal, jsonSerializerOptions);

            var currentConditionsReply = new CurrentConditionsReply();

            return currentConditionReply;
        }

        public override async Task<CurrentConditionsReply> GetCurrentConditions(CurrentConditionsRequest request, ServerCallContext context)
        {
            _logger.LogWarning("Get current conditions called");

            var client = new HttpClient();

            var response = await client.GetAsync("https://core-weather-api.azurewebsites.net/api/v1/current-conditions");

            var retVal = await response.Content.ReadAsStringAsync();

            // New Serializer!!!!
            var jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.PropertyNameCaseInsensitive = true;

            var conditions = JsonSerializer.Deserialize<IEnumerable<CurrentCondition>>(retVal, jsonSerializerOptions);

            var currentConditions = new Google.Protobuf.Collections.RepeatedField<CurrentCondition>();
            currentConditions.AddRange(conditions);

            var currentConditionsReply = new CurrentConditionsReply();
            currentConditionsReply.CurrentConditions.AddRange(currentConditions);

            return currentConditionsReply;
        }

        public override async Task GetCurrentConditionStream(CurrentConditionsRequest request, IServerStreamWriter<CurrentConditionsReply> responseStream, ServerCallContext context)
        {

            while (!context.CancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("Background task is running");

                var client = new HttpClient();

                var response = await client.GetAsync("https://core-weather-api.azurewebsites.net/api/v1/current-conditions");

                var retVal = await response.Content.ReadAsStringAsync();

                // New Serializer!!!!
                var jsonSerializerOptions = new JsonSerializerOptions();
                jsonSerializerOptions.PropertyNameCaseInsensitive = true;

                var conditions = JsonSerializer.Deserialize<IEnumerable<CurrentCondition>>(retVal, jsonSerializerOptions);

                var currentConditions = new Google.Protobuf.Collections.RepeatedField<CurrentCondition>();
                currentConditions.AddRange(conditions);

                var currentConditionsReply = new CurrentConditionsReply();
                currentConditionsReply.CurrentConditions.AddRange(currentConditions);

                await responseStream.WriteAsync(currentConditionsReply);
                await Task.Delay(TimeSpan.FromSeconds(2));
            }
        }
    }
}