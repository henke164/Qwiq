using QwiqClient.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace QwiqClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new HttpClient();
            var processIdResponse = client.GetAsync("http://localhost/pid/").Result;
            var processId = int.Parse(processIdResponse.Content.ReadAsStringAsync().Result);
            var process = Process.GetProcessById(processId);
            var memoryReader = new MemoryReader(process);

            var addressResponse = client.GetAsync("http://localhost/get/my-item").Result;
            var addressStr = addressResponse.Content.ReadAsStringAsync().Result;
            var address = int.Parse(addressStr);
            var obj = memoryReader.ReadMemory<MyStruct>(address);
        }

        public struct MyStruct
        {
            public int x;
        }
    }
}
