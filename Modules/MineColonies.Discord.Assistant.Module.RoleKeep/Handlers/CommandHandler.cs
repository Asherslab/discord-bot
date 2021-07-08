using System;
using System.Reflection;
using Discord.WebSocket;
using MineColonies.Discord.Assistant.BaseClasses.Handlers;

namespace MineColonies.Discord.Assistant.Module.RoleKeep.Handlers
{
    public class CommandHandler : BaseCommandHandler
    {
        public CommandHandler(DiscordSocketClient client, IServiceProvider provider) : base(client, provider, "testing-rk!")
        {
        }

        protected override Assembly GetAssembly()
        {
            return Assembly.GetAssembly(GetType());
        }
    }
}