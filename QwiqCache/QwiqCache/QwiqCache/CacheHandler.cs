using Newtonsoft.Json;
using QwiqCache.Services;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace QwiqCache
{
    public class CacheHandler
    {
        private StructGenerator _structGenerator;

        private Dictionary<string, Type> _structDictionary = new Dictionary<string, Type>();

        private Dictionary<string, CacheObject> _cacheDictionary = new Dictionary<string, CacheObject>();

        public CacheHandler()
        {
            _structGenerator = new StructGenerator();
        }

        public int Allocate(int length)
        {
            var bytes = new byte[length];
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            return (int)handle.AddrOfPinnedObject();
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

        public bool AddItem(string key, string structName, int objAddress)
        {
            if (!_structDictionary.ContainsKey(structName))
            {
                return false;
            }

            var type = _structDictionary[structName];

            var result = Marshal.PtrToStructure(new IntPtr(objAddress), type);

            var cacheObj = new CacheObject
            {
                Object = result,
                Address = new IntPtr(objAddress)
            };

            if (_cacheDictionary.ContainsKey(key))
            {
                _cacheDictionary[key] = cacheObj;
            }
            else
            {
                _cacheDictionary.Add(key, cacheObj);
            }

            return true;
        }

        public bool AddStruct(string structStr)
        {
            var result = _structGenerator.BuildStruct(structStr);
            if (result == null)
            {
                return false;
            }

            if (_structDictionary.ContainsKey(result.Name))
            {
                _structDictionary[result.Name] = result.Type;
                return true;
            }

            _structDictionary.Add(result.Name, result.Type);
            return true;
        }

        internal class CacheObject
        {
            public object Object { get; set; }
            public IntPtr Address { get; set; }
        }
    }
}
