
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

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
