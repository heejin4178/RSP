using System;
using System.IO;

namespace Server.Data
{
    [Serializable]
    public class ServerConfig
    {
        public string dataPath;
    }

    public class ConfigManager
    {
        public static ServerConfig Config { get; private set; }

        public static void LoadConfig()
        {
            // /Users/heejinkim/RSP/Server/Server/bin/Debug/netcoreapp3.1 경로에 있음
            string text = File.ReadAllText("config.json");
            Config = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerConfig>(text);
        }
    }
    
}