using Newtonsoft.Json;
using QwiqCache.Models;
using QwiqCache.Services;
using System;
using System.Diagnostics;
using System.Net;

namespace QwiqCache
{
    class Program
    {
        static CacheHandler CHandler = new CacheHandler();
        static HttpCommunicator Communicator;

        static void Main(string[] args)
        {
            var port = 1988;
            Communicator = new HttpCommunicator(port);
            Communicator.OnGetItemRequest = OnGetItemRequest;
            Communicator.OnProcessIdRequested = OnProcessIdRequested;
            Communicator.OnBindItemRequest = OnBindItemRequest;
            Communicator.OnAllocateRequest = OnAllocateRequest;
            Communicator.Start();

            Console.WriteLine($"Process started: {Process.GetCurrentProcess().Id} on port {port}");
            Console.WriteLine("Commands:");
            Console.WriteLine("count\tDisplay key count.");
            Console.WriteLine("keys\tDisplay all keys.");
            Console.WriteLine("get {key}\tDisplay pointer address and datalength.");

            while (true)
            {
                try
                {
                    Console.Write(">");
                    var cmd = Console.ReadLine();
                    var parameters = cmd.Split(' ');

                    switch (parameters[0])
                    {
                        case "count":
                            Console.WriteLine(CHandler.CacheDictionary.Count);
                            break;

                        case "keys":
                            foreach (var key in CHandler.CacheDictionary.Keys)
                            {
                                Console.WriteLine(key);
                            }
                            break;

                        case "get":
                            var cacheKey = parameters[1];
                            var cacheDetails = CHandler.CacheDictionary[cacheKey];
                            Console.WriteLine($"Pointer address: {cacheDetails.Address}, Datalength: {cacheDetails.Length}");
                            break;
                    }
                }
                catch
                {
                    Console.WriteLine("Invalid input...");
                }
            }
        }

        private static void OnAllocateRequest(HttpListenerContext context, AllocateMemoryBody body)
        {
            Console.WriteLine("Allocating memory: " + body.Length + " bytes");
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
            if (ptr == null)
            {
                Communicator.Send(context, null);
                return;
            }
            Communicator.Send(context, JsonConvert.SerializeObject(ptr));
        }

        private static void OnProcessIdRequested(HttpListenerContext context)
        {
            var pid = Process.GetCurrentProcess().Id;
            Communicator.Send(context, pid.ToString());
        }
    }
}
