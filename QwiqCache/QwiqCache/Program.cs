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
        static readonly CacheHandler CacheHandler = new CacheHandler();
        static HttpCommunicator Communicator;

        static void Main(string[] args)
        {
            var settings = SettingsLoader.LoadSettings();
            Communicator = new HttpCommunicator(settings.Port)
            {
                OnGetItemRequest = OnGetItemRequest,
                OnProcessIdRequested = OnProcessIdRequested,
                OnAddItemRequest = OnAddItemRequest
            };

            Communicator.Start();

            Console.WriteLine($"Process started: {Process.GetCurrentProcess().Id} on port {settings.Port}");

            var inputHandler = new InputHandler(CacheHandler);
            inputHandler.Start();
        }

        private static void OnAddItemRequest(HttpListenerContext context, AddItemBody body)
        {
            Console.WriteLine("Allocating memory: " + body.Length + " bytes");
            var pointer = CacheHandler.Allocate(body.Length);
            CacheHandler.AddItem(body.Key, pointer, body.Length);
            Communicator.Send(context, pointer.ToString());
        }

        private static void OnGetItemRequest(HttpListenerContext context, string key)
        {
            var ptr = CacheHandler.GetItem(key);
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
