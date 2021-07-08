using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using MineColonies.Discord.Assistant.Interfaces.Interfaces;
using MineColonies.Discord.Assistant.Module.Test.Commands;
using MineColonies.Discord.Assistant.Module.Test.Handlers;

namespace MineColonies.Discord.Assistant.Module.Test
{
    public class TestModuleStartup : IModuleStartup
    {
        public string ModuleName => "test";

        public Task ConfigureClient(DiscordSocketClient client)
        {
            return Task.CompletedTask;
        }
        
        public Task ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ICommandHandler, TestCommandHandler>();
            services.AddSingleton<TestCommand>();
            return Task.CompletedTask;
        }
    }
}