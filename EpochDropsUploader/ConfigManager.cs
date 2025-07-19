using System;
using System.IO;
using System.Text.Json;

namespace EpochDropsUploader
{
    public class Config
    {
        public string WowRootPath { get; set; } = "";

        private static readonly string ConfigFilePath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EpochDropsUploader", "config.json");

        public static Config Load()
        {
            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    var json = File.ReadAllText(ConfigFilePath);
                    return JsonSerializer.Deserialize<Config>(json) ?? new Config();
                }
            }
            catch
            {
                // Could log error here
            }

            return new Config(); // fallback
        }

        public static void Save(Config config)
        {
            try
            {
                var directory = Path.GetDirectoryName(ConfigFilePath);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(ConfigFilePath, json);
            }
            catch
            {
                // Could log error here
            }
        }
    }
}

