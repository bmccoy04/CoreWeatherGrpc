using System;
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

            var client = new CurrentConditions.CurrentConditionsClient(channel);
            var reply = await client.GetCurrentConditionsAsync(new CurrentConditionsRequest(){});

            foreach(var r in reply.CurrentConditions)
            {
                Console.WriteLine(r.Name + " is " + r.Description);
            }
            Console.ReadKey();

            GetMiddleName(new Person("First", "Last"){MiddleName = "Foo"});

        }

        static int GetMiddleName(Person person)
        {
            return person.MiddleName.Length; // With no extra code would throw a warning because MIddle name is not set in constructor

            if (person.MiddleName is null) return 0; // If you put this ahead of person.MiddleName it will get rid of warnings

            return person.MiddleName?.Length ?? 0; // Would get rid of warnings


            var middle = person.MiddleName;
            return (middle!).Length;  // the "Know better operator", turns off null check compiler warnings.
        }

        static int GetMiddleNameNullPerson(Person? person)
        {
            if (person?.MiddleName is null) return 0; // w/o pattern matching

            if(person?.MiddleName is { }) return person.MiddleName.Length; // new pattern to check "is object something"

            if(person?.MiddleName is { } middle) return middle.Length; // can give the "Is object something" a name!

            if(person?.MiddleName is { Length: var lengthOne }) return lengthOne; // uses pattern matching to dig in and get what you're looking for

            //Finally
            return person?.MiddleName is { Length: var lengthTwo} ? lengthTwo : 0; 
        }

        public class Person
        {
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            // can assign null to premitive types if nullable features are turned on
            // public string? MiddleName { get; set; } 
            public string LastName { get; set; }

            public Person(string first, string last)
            {
                FirstName = first;
                // Middle Name will now have a warning in new VS feataures
                // have to manual make primitives null or assign values
                LastName = last;
            }
        }
    }
}
