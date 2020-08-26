using Newtonsoft.Json;
using QwiqCache.Models;
using QwiqCache.Services;
using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;

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
            Communicator.OnBindItemRequest = OnBindItemRequest;
            Communicator.OnAllocateRequest = OnAllocateRequest;
            Communicator.Start();

            Console.WriteLine("Process started: " + Process.GetCurrentProcess().Id);

            Console.ReadLine();
        }

        private static void OnAllocateRequest(HttpListenerContext context, AllocateMemoryBody body)
        {
            var pointer = CHandler.Allocate(body.Length);
            Communicator.Send(context, pointer.ToString());
        }

        private static void OnBindItemRequest(HttpListenerContext context, BindItemBody body)
        {
            Console.WriteLine("Adding new item: " + body.Key);
            CHandler.AddItem(body.Key, body.Address, body.Length);
            Communicator.Send(context, "True");
        }

        private static void OnGetItemRequest(HttpListenerContext context, string key)
        {
            var ptr = CHandler.GetItem(key);
            Communicator.Send(context, JsonConvert.SerializeObject(ptr));
        }

        private static void OnProcessIdRequested(HttpListenerContext context)
        {
            var pid = Process.GetCurrentProcess().Id;
            Communicator.Send(context, pid.ToString());
        }
    }
}
