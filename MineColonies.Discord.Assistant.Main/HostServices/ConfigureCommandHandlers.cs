using System;
using System.Threading;
using System.Threading.Tasks;
using Discord.Addons.Hosting;
using Discord.Addons.Hosting.Util;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MineColonies.Discord.Assistant.Interfaces.Interfaces;

namespace MineColonies.Discord.Assistant.Main.HostServices
{
    public class ConfigureCommandHandlers : DiscordClientService
    {
        private readonly IServiceProvider _provider;

        public ConfigureCommandHandlers(DiscordSocketClient client, ILogger<DiscordClientService> logger,
            IServiceProvider provider) : base(client, logger)
        {
            _provider = provider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Client.WaitForReadyAsync(stoppingToken);
            
            foreach (ICommandHandler commandHandler in _provider.GetServices<ICommandHandler>())
            {
                await commandHandler.InitializeAsync();
                Client.MessageReceived += commandHandler.HandleCommandAsync;
            }
        }
    }
}