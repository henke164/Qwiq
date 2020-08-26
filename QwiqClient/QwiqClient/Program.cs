using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace QwiqClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(TestGet);
            Console.ReadLine();
        }

        static async Task TestGet()
        {
            using (var client = new Client())
            {
                await client.Initialize();

                var obj = new MyClass()
                {
                    X = 1,
                    Y = 5,
                    Name = "Henkepenke"
                };

                await client.AddItemAsync("my-item2", obj);

                var result = await client.GetAsync<MyClass>("my-item2");

                Console.WriteLine(JsonConvert.SerializeObject(result));
            }
        }

        [Serializable]
        public class MyClass
        {
            public string Name;

            public int X;

            public int Y;
        }
    }
}
