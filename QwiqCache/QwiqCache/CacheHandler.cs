using QwiqCache.Models;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace QwiqCache
{
    public class CacheHandler
    {
        public Dictionary<string, CacheObject> CacheDictionary = new Dictionary<string, CacheObject>();

        public int Allocate(int length)
        {
            var bytes = new byte[length];
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            return (int)handle.AddrOfPinnedObject();
        }

        public CacheObject GetItem(string key)
        {
            if (!CacheDictionary.ContainsKey(key))
            {
                return null;
            }

            return CacheDictionary[key];
        }

        public bool AddItem(string key, int objAddress, int byteLength)
        {
            var cacheObj = new CacheObject
            {
                Address = objAddress,
                Length = byteLength
            };

            if (CacheDictionary.ContainsKey(key))
            {
                CacheDictionary[key] = cacheObj;
            }
            else
            {
                CacheDictionary.Add(key, cacheObj);
            }

            return true;
        }
    }
}
