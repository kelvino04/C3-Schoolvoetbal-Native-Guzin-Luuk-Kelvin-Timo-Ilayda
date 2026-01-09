using System;
using System.IO;
using System.Text.Json;
using praC3.Models;

namespace praC3.Services
{
    public static class DataService
    {
        private static string filePath = "appdata.json";

        public static AppData Load()
        {
            if (!File.Exists(filePath))
                return new AppData();

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<AppData>(json);
        }

        public static void Save(AppData data)
        {
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }
    }
}