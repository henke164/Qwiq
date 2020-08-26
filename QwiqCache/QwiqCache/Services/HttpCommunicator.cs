using Newtonsoft.Json;
using QwiqCache.Models;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QwiqCache.Services
{
    public class HttpCommunicator
    {
        private string[] _prefixes;

        public Action<HttpListenerContext> OnProcessIdRequested { get; set; }

        public Action<HttpListenerContext, string> OnGetItemRequest { get; set; }

        public Action<HttpListenerContext, AddItemBody> OnAddItemRequest { get; set; }

        public HttpCommunicator(int port)
        {
            _prefixes = new string[]
            {
                $"http://localhost:{port}/pid/",
                $"http://localhost:{port}/add/",
                $"http://localhost:{port}/get/",
            };
        }

        public void Start()
        {
            Task.Run(HandleRequests);
        }

        private void HandleRequests()
        {
            var listener = new HttpListener();

            foreach (var s in _prefixes)
            {
                listener.Prefixes.Add(s);
            }

            listener.Start();

            while (true)
            {
                var context = listener.GetContext();
                var request = context.Request;
                var urlParts = request.RawUrl.Split('/');

                if (request.HttpMethod == "GET")
                {
                    switch (urlParts[1])
                    {
                        case "pid":
                            OnProcessIdRequested(context);
                            break;
                        case "get":
                            var key = urlParts[2];
                            OnGetItemRequest(context, key);
                            break;

                    }
                }

                if (request.HttpMethod == "POST")
                {
                    switch (urlParts[1])
                    {
                        case "add":
                            OnAddItemRequest(context, GetBodyObject<AddItemBody>(request.InputStream));
                            break;
                    }
                }
            }
        }

        public void Send(HttpListenerContext context, string responseString)
        {
            var response = context.Response;
            var buffer = Encoding.UTF8.GetBytes(responseString);

            response.ContentLength64 = buffer.Length;

            using (var output = response.OutputStream)
            {
                output.Write(buffer, 0, buffer.Length);
            }
        }

        private T GetBodyObject<T>(Stream stream)
        {
            var allocateRdr = new StreamReader(stream);
            var json = allocateRdr.ReadToEnd();
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
