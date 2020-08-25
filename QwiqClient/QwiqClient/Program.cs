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
                var obj = await client.GetAsync<MyStruct>("my-item");
                Console.WriteLine(obj.x);
            }
        }

        public struct MyStruct
        {
            public int x;
        }
    }
}
