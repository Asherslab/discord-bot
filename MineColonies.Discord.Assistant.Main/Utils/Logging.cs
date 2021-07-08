using System;
using System.Threading.Tasks;
using Discord;

namespace MineColonies.Discord.Assistant.Main.Utils
{
    public static class Logging
    {
        public static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}