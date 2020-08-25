using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace QwiqCache
{
    class Program
    {
        public struct MyStruct
        {
            public float x;
            public float y;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Process started: " + Process.GetCurrentProcess().Id);
            var handler = new CacheHandler();

            var structStr = @"
                public struct MyStruct
                {
                    public int x;
                }
            ";

            handler.AddStruct(structStr);
            handler.AddItem("my-item", "MyStruct", "{\"x\":1337}");
            var ptr = handler.GetItemAddress("my-item");


            Console.ReadLine();
        }
    }
}
