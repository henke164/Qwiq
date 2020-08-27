using Newtonsoft.Json;
using QwiqCache.Models;
using System.IO;

namespace QwiqCache.Services
{
    public static class SettingsLoader
    {
        public static Settings LoadSettings()
        {
            using (var sr = new StreamReader("./settings.json"))
            {
                var str = sr.ReadToEnd();
                var settings = JsonConvert.DeserializeObject<Settings>(str);
                return settings;
            }
        }
    }
}
