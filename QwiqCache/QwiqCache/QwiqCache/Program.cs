using QwiqCache.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace QwiqCache
{
    class Program
    {
        static CacheHandler CHandler = new CacheHandler();
        static HttpCommunicator Communicator = new HttpCommunicator();

        static void Main(string[] args)
        {
            Communicator.OnGetItemRequest = OnGetItemRequest;
            Communicator.OnProcessIdRequested = OnProcessIdRequested;

            Communicator.Start();

            Console.WriteLine("Process started: " + Process.GetCurrentProcess().Id);


            var structStr = @"
                public struct MyStruct
                {
                    public int x;
                }
            ";

            CHandler.AddStruct(structStr);
            CHandler.AddItem("my-item", "MyStruct", "{\"x\":1337}");

            Console.ReadLine();
        }

        static void OnGetItemRequest(HttpListenerContext context, string key)
        {
            var ptr = CHandler.GetItemAddress(key);
            Communicator.Send(context, ptr.ToString());
        }

        static void OnProcessIdRequested(HttpListenerContext context)
        {
            var pid = Process.GetCurrentProcess().Id;
            Communicator.Send(context, pid.ToString());
        }
    }
}
