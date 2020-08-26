using Newtonsoft.Json;
using System;
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

                await client.AddStruct<MyStruct>();

                var obj = new MyStruct()
                {
                    x = 13,
                    y = 54,
                    z = 5
                };

                await client.AddItemAsync("my-item2", obj);

                var result = await client.GetAsync<MyStruct>("my-item2");

                Console.WriteLine(JsonConvert.SerializeObject(result));
            }
        }

        public struct MyStruct
        {
            public int x;
            public int y;
            public int z;
        }
    }
}
