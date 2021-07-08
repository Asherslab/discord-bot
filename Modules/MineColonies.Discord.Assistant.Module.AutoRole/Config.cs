using System.Collections.Generic;
using System.IO;
using System.Threading;
using static System.Text.Json.JsonSerializer;

namespace MineColonies.Discord.Assistant.Module.AutoRole
{
    public class Config
    {
        public List<ulong> AutoRoles { get; set; } = new ();
     
        // Static Stuff \\
     
        private const string ConfigFile = "autorole-config.json";
        private static readonly Mutex Mutex = new();
        
        public void Save()
        {
            Mutex.WaitOne();

            try
            {
                CheckConfigFile();
                
                using FileStream fileStream = File.OpenWrite(ConfigFile);
                using StreamWriter streamWriter = new(fileStream);
                streamWriter.Write(Serialize(this));
            }
            finally
            {
                Mutex.ReleaseMutex();
            }
        }

        public static Config Load()
        {
            Mutex.WaitOne();

            try
            {
                CheckConfigFile();

                using FileStream fileStream = File.OpenRead(ConfigFile);
                using StreamReader reader = new(fileStream);
                return Deserialize<Config>(reader.ReadToEnd());
            }
            finally
            {
                Mutex.ReleaseMutex();
            }
        }

        private static void CheckConfigFile()
        {
            if (File.Exists(ConfigFile)) return;
            
            using FileStream fileStream = File.Create(ConfigFile);
            using StreamWriter streamWriter = new(fileStream);
            streamWriter.Write("{}");
        }
    }
}