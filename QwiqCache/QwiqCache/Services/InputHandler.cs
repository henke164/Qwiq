using System;

namespace QwiqCache.Services
{
    public class InputHandler
    {
        private readonly CacheHandler _cHandler;

        public InputHandler(CacheHandler cHandler)
        {
            _cHandler = cHandler;
        }

        public void Start()
        {
            Console.WriteLine("Commands\t\tDetails");
            Console.WriteLine("-------------------");
            Console.WriteLine("count\t\t\tDisplay key count.");
            Console.WriteLine("keys\t\t\tDisplay all keys.");
            Console.WriteLine("get {key}\t\tDisplay pointer address and datalength.");

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
                            Console.WriteLine(_cHandler.CacheDictionary.Count);
                            break;

                        case "keys":
                            foreach (var key in _cHandler.CacheDictionary.Keys)
                            {
                                Console.WriteLine(key);
                            }
                            break;

                        case "get":
                            var cacheKey = parameters[1];
                            var cacheDetails = _cHandler.CacheDictionary[cacheKey];
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
    }
}
