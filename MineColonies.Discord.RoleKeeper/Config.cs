using System.Collections.Generic;
using System.IO;
using System.Threading;
using static System.Text.Json.JsonSerializer;

namespace MineColonies.Discord.RoleKeeper
{
    public class Config
    {
        public List<ulong> RolesToKeep { get; set; } = new ();
        
        public List<ulong> AutoRoles { get; set; } = new ();
        
        public Dictionary<ulong, List<ulong>> KeptRolesToUsers { get; set; } = new ();
     
        // Static Stuff \\
        
        public static Config Instance = new();
        private static readonly Mutex Mutex = new();
        
        public static void SaveConfig()
        {
            Mutex.WaitOne();

            try
            {
                CheckConfigFile();
                
                using FileStream fileStream = File.OpenWrite("config.json");
                using StreamWriter streamWriter = new(fileStream);
                streamWriter.Write(Serialize(Instance));
            }
            finally
            {
                Mutex.ReleaseMutex();
            }
        }

        public static void LoadConfig()
        {
            Mutex.WaitOne();

            try
            {
                CheckConfigFile();
                
                using FileStream fileStream = File.OpenRead("config.json");
                using StreamReader reader = new(fileStream);
                Instance = Deserialize<Config>(reader.ReadToEnd());
            }
            finally
            {
                Mutex.ReleaseMutex();
            }
        }

        private static void CheckConfigFile()
        {
            if (File.Exists("config.json")) return;
            
            using FileStream fileStream = File.Create("config.json");
            using StreamWriter streamWriter = new(fileStream);
            streamWriter.Write("{}");
        }
    }
}