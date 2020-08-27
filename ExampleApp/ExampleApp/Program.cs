using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var person = new Person
            {
                Id = 1337,
                Name = "Henke",
                Pets = new List<Animal>
                {
                    new Animal
                    {
                        Name = "Lillan",
                        Type = PetType.Cat,
                        Age = 9
                    },
                    new Animal
                    {
                        Name = "Dogge",
                        Type = PetType.Dog,
                        Age = 1
                    }
                }
            };

            AddAndGetItemExample(person);

            Console.ReadLine();
        }

        static async Task AddAndGetItemExample(Person person)
        {
            var client = new Qwiq.QwiqClient(port: 1988);

            var connected = await client.Connect();

            if (!connected)
            {
                Console.WriteLine("Could not connect to Qwiq cache");
                return;
            }

            var persons = new List<Person>();

            // Create a list with 100000 persons
            for (var x = 0; x < 100000; x++)
            {
                persons.Add(person);
            }

            // Add list to cache
            await client.AddAsync("person-list", persons);

            // Get list from cache
            var result = await client.GetAsync<List<Person>>("person-list");

            // Convert it to json for readability
            Console.WriteLine(JsonConvert.SerializeObject(result));
        }

        // Models

        enum PetType { Dog, Cat }

        [Serializable]
        class Person
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public IList<Animal> Pets { get; set; }
        }

        [Serializable]
        class Animal
        {
            public string Name { get; set; }
            public PetType Type { get; set; }
            public int Age { get; set; }
        }
    }
}
