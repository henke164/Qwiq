using QwiqClient.Services;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace QwiqClient
{
    public class Client : IDisposable
    {
        private string _baseUrl = "http://localhost";
        private MemoryReader _memoryReader;
        private HttpClient _httpClient;

        public Client()
        {
            _httpClient = new HttpClient();
        }

        public async Task Initialize()
        {
            var response = await _httpClient.GetAsync("http://localhost/pid/");
            var pidStr = await response.Content.ReadAsStringAsync();

            if (int.TryParse(pidStr, out int processId))
            {
                var process = Process.GetProcessById(processId);
                _memoryReader = new MemoryReader(process);
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
                return _memoryReader.ReadMemory<T>(address);
            }

            return default;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
