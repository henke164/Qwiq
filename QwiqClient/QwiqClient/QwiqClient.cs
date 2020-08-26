using Newtonsoft.Json;
using Qwiq.Models;
using Qwiq.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace Qwiq
{
    public class QwiqClient : IDisposable
    {
        public bool Connected { get; set; }
        private string _baseUrl;
        private MemoryHandler _memoryIO;
        private HttpClient _httpClient;

        public QwiqClient(int port)
        {
            _httpClient = new HttpClient();
            _baseUrl = $"http://localhost:{port}";
        }

        public void Dispose()
        {
            Connected = false;
            _httpClient.Dispose();
        }

        public async Task<bool> Connect()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/pid/");
                var pidStr = await response.Content.ReadAsStringAsync();

                if (int.TryParse(pidStr, out int processId))
                {
                    var process = Process.GetProcessById(processId);
                    _memoryIO = new MemoryHandler(process);
                    Connected = true;
                    return true;
                }
            }
            catch
            {
                Console.WriteLine("Error occured");
            }

            return false;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            if (!Connected)
            {
                return default;
            }

            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/get/{key}");

                var stringContent = await response.Content.ReadAsStringAsync();

                var content = JsonConvert.DeserializeObject<CacheObject>(stringContent);

                var result = _memoryIO.ReadMemory(content.Address, content.Length);

                return (T)ByteArrayToObject(result);
            }
            catch
            {
                return default;
            }
        }

        public async Task<bool> AddAsync<T>(string key, T item)
        {
            if (!Connected)
            {
                return false;
            }

            try
            {
                var bytes = ObjectToByteArray(item);

                var body = CreateJsonContent(new AddItemRequestModel
                {
                    Key = key,
                    Length = bytes.Length
                });

                var allocateResp = await _httpClient.PostAsync($"{_baseUrl}/add", body);
                var addressStr = await allocateResp.Content.ReadAsStringAsync();
                var address = int.Parse(addressStr);

                _memoryIO.WriteMemory(address, bytes);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
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
