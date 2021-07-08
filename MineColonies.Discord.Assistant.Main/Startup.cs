using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using MineColonies.Discord.Assistant.Interfaces.Interfaces;
using MineColonies.Discord.Assistant.Main.Extensions;
using MineColonies.Discord.Assistant.Main.Handlers;
using MineColonies.Discord.Assistant.Main.Utils;
using MineColonies.Discord.Assistant.Module.AutoRole;
using MineColonies.Discord.Assistant.Module.RoleKeep;

namespace MineColonies.Discord.Assistant.Main
{
    public static class Startup
    {
        private static List<IModuleStartup>? _modules = new()
        {
            new AutoRoleModuleStartup(),
            new RoleKeepModuleStartup()
        };
        
        private static readonly List<IModuleStartup> EnabledModules = new();

        public static void ConfigureModules(List<string> enabledModules)
        {
            foreach (IModuleStartup module in _modules!.Where(module => enabledModules.Contains(module.ModuleName)))
            {
                EnabledModules.Add(module);
            }

            _modules = null;
        }

        public static async Task ConfigureClient(ServiceProvider provider, DiscordSocketClient client)
        {
            client.Log += Logging.Log;

            string? token = Environment.GetEnvironmentVariable("DISCORD_KEY");

            await client.LoginAsync(TokenType.Bot, token);

            foreach (ICommandHandler commandHandler in provider.GetServices<ICommandHandler>())
            {
                await commandHandler.InitializeAsync();
                client.MessageReceived += commandHandler.HandleCommandAsync;
            }

            provider.GetRequiredService<EventHandlerWrapper>().RegisterEvents(client);

            foreach (IModuleStartup moduleStartup in EnabledModules)
            {
                await moduleStartup.ConfigureClient(client);
            }
        }

        public static async Task ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<DiscordSocketClient>();
            services.AddSingleton<EventHandlerWrapper>();

            foreach (IModuleStartup moduleStartup in EnabledModules)
            {
                await moduleStartup.ConfigureServices(services);
            }
        }
    }
}