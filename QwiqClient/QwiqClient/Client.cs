using Newtonsoft.Json;
using QwiqClient.Services;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.InteropServices;
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
            where T : struct
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/get/{key}");

            var addressStr = await response.Content.ReadAsStringAsync();

            if (int.TryParse(addressStr, out int address))
            {
                return _memoryIO.ReadMemory<T>(address);
            }

            return default;
        }

        public async Task<bool> AddItemAsync<T>(string key, T item)
            where T : struct
        {
            try
            {
                var address = await AllocateMemoryForItem(item);

                _memoryIO.WriteMemory<T>(address, item);

                var body = CreateJsonContent(new
                {
                    Address = address,
                    Key = key,
                    StructName = typeof(T).Name
                });

                var response = await _httpClient.PostAsync($"{_baseUrl}/bind", body);

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public async Task AddStruct<T>()
            where T : struct
        {
            var body = CreateJsonContent(new
            {
                StructCode = StructToString.CreateString(typeof(T))
            });

            await _httpClient.PostAsync($"{_baseUrl}/add-struct", body);
        }

        private async Task<int> AllocateMemoryForItem(object item)
        {
            var allocBody = CreateJsonContent(new
            {
                Length = Marshal.SizeOf(item)
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
    }
}
