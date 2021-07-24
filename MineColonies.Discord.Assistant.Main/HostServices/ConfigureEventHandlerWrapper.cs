using System.Threading;
using System.Threading.Tasks;
using Discord.Addons.Hosting;
using Discord.Addons.Hosting.Util;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using MineColonies.Discord.Assistant.Main.Handlers;

namespace MineColonies.Discord.Assistant.Main.HostServices
{
    public class ConfigureEventHandlerWrapper : DiscordClientService
    {
        private readonly EventHandlerWrapper _handler;

        public ConfigureEventHandlerWrapper(DiscordSocketClient client, ILogger<DiscordClientService> logger,
            EventHandlerWrapper handler) : base(client, logger)
        {
            _handler = handler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Client.Ready += _handler.Ready;
            await Client.WaitForReadyAsync(stoppingToken);
            
            Client.UserJoined += _handler.UserJoined;
            Client.GuildMemberUpdated += _handler.GuildMemberUpdated;
            Client.MessageReceived += _handler.MessageReceived;
        }
    }
}