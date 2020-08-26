using Newtonsoft.Json;
using QwiqClient.Models;
using QwiqClient.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace QwiqClient
{
    public class Client : IDisposable
    {
        private string _baseUrl = "http://localhost";
        private MemoryIO _memoryIO;
        private HttpClient _httpClient;

        public Client()
        {
            _httpClient = new HttpClient();
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        public async Task Initialize()
        {
            var response = await _httpClient.GetAsync("http://localhost/pid/");
            var pidStr = await response.Content.ReadAsStringAsync();

            if (int.TryParse(pidStr, out int processId))
            {
                var process = Process.GetProcessById(processId);
                _memoryIO = new MemoryIO(process);
                return;
            }

            throw new Exception("Could not connect to QwiqCache");
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/get/{key}");

            var stringContent = await response.Content.ReadAsStringAsync();

            var content = JsonConvert.DeserializeObject<CacheObject>(stringContent);
            
            var result = _memoryIO.ReadMemory(content.Address, content.Length);

            return (T)ByteArrayToObject(result);
        }

        public async Task<bool> AddItemAsync<T>(string key, T item)
        {
            try
            {
                var bytes = ObjectToByteArray(item);
                var address = await AllocateMemory(bytes.Length);

                _memoryIO.WriteMemory(address, bytes);

                var body = CreateJsonContent(new
                {
                    Address = address,
                    Key = key,
                    Length = bytes.Length
                });

                var response = await _httpClient.PostAsync($"{_baseUrl}/bind", body);

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        private async Task<int> AllocateMemory(object length)
        {
            var allocBody = CreateJsonContent(new
            {
                Length = length
            });

            var allocateResp = await _httpClient.PostAsync($"{_baseUrl}/allocate/", allocBody);
            var addressStr = await allocateResp.Content.ReadAsStringAsync();
            var address = int.Parse(addressStr);
            return address;
        }

        private StringContent CreateJsonContent(object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            return new StringContent(json);
        }

        private byte[] ObjectToByteArray(object obj)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        private object ByteArrayToObject(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                return binForm.Deserialize(memStream);
            }
        }

    }
}
