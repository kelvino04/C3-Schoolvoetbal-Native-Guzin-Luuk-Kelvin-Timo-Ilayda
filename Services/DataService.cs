using System;
using System.IO;
using System.Text.Json;
using praC3;

namespace praC3.Services
{
    public static class DataService
    {
        private static string folder = "Data";
        private static string path = "Data/data.json";

        public static AppData Load()
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            if (!File.Exists(path))
                File.WriteAllText(path, "{\"Users\":[],\"Matches\":[],\"Bets\":[]}");

            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<AppData>(json);
        }

        public static void Save(AppData data)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(path, json);
        }
    }
}
