using System;
using System.Reflection;
using Discord.WebSocket;
using MineColonies.Discord.Assistant.BaseClasses.Handlers;

namespace MineColonies.Discord.Assistant.Module.AutoRole.Handlers
{
    public class CommandHandler : BaseCommandHandler
    {
        public CommandHandler(DiscordSocketClient client, IServiceProvider provider) : base(client, provider, "ar!")
        {
        }

        protected override Assembly GetAssembly()
        {
            return Assembly.GetAssembly(GetType());
        }
    }
}