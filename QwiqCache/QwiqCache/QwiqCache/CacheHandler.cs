using Newtonsoft.Json;
using QwiqCache.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QwiqCache
{
    public class CacheHandler
    {
        private StructGenerator _structGenerator;

        private Dictionary<string, Type> _structDictionary = new Dictionary<string, Type>();

        private Dictionary<string, CacheObject> _cacheDictionary = new Dictionary<string, CacheObject>();

        public class CacheObject
        {
            public object Object { get; set; }
            public IntPtr Address { get; set; }
        }

        public CacheHandler()
        {
            _structGenerator = new StructGenerator();
        }

        public IntPtr GetItemAddress(string key)
        {
            if (!_cacheDictionary.ContainsKey(key))
            {
                return IntPtr.Zero;
            }

            var obj = _cacheDictionary[key];

            return obj.Address;
        }

        public bool AddItem(string key, string structName, string json)
        {
            if (!_structDictionary.ContainsKey(structName))
            {
                return false;
            }

            var type = _structDictionary[structName];

            var obj = JsonConvert.DeserializeObject(json, type);

            var handle = GCHandle.Alloc(obj, GCHandleType.Pinned);

            _cacheDictionary.Add(key, new CacheObject
            { 
                Object = obj,
                Address = handle.AddrOfPinnedObject()
            });

            return true;
        }

        public bool AddStruct(string structStr)
        {
            var result = _structGenerator.BuildStruct(structStr);
            _structDictionary.Add(result.Name, result.Type);
            return true;
        }
    }
}
