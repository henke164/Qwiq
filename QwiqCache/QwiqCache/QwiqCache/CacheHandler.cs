using QwiqCache.Models;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace QwiqCache
{
    public class CacheHandler
    {

        private Dictionary<string, CacheObject> _cacheDictionary = new Dictionary<string, CacheObject>();

        public CacheHandler()
        {
        }

        public int Allocate(int length)
        {
            var bytes = new byte[length];
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            return (int)handle.AddrOfPinnedObject();
        }

        public CacheObject GetItem(string key)
        {
            if (!_cacheDictionary.ContainsKey(key))
            {
                return null;
            }

            var obj = _cacheDictionary[key];

            return obj;
        }

        public bool AddItem(string key, int objAddress, int byteLength)
        {
            var cacheObj = new CacheObject
            {
                Address = objAddress,
                Length = byteLength
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
    }
}
