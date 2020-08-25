using System;
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

        public Action<HttpListenerContext, string, string, string> OnAddItemRequest { get; set; }

        public Action<HttpListenerContext, string> OnAddStructRequest { get; set; }

        public HttpCommunicator()
        {
            _prefixes = new string[]
            {
                "http://localhost/pid/",
                "http://localhost/add/",
                "http://localhost/get/",
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

            Console.WriteLine("Listening..");

            while (true)
            {
                var context = listener.GetContext();
                var request = context.Request;

                if (request.HttpMethod == "GET")
                {
                    switch (request.RawUrl.Split('/')[1])
                    {
                        case "pid":
                            OnProcessIdRequested(context);
                            break;
                        case "get":
                            var key = request.RawUrl.Split('/')[2];
                            OnGetItemRequest(context, key);
                            break;

                    }
                }

                if (request.HttpMethod == "POST")
                {
                    switch (request.RawUrl)
                    {
                        case "/add-struct":
                            break;
                        case "/add-item":
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
    }
}
