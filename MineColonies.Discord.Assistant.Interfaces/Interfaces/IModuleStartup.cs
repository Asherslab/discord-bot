using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace MineColonies.Discord.Assistant.Interfaces.Interfaces
{
    public interface IModuleStartup
    {
        public string ModuleName { get; }

        public Task ConfigureClient(DiscordSocketClient client)
        {  
            return Task.CompletedTask;
        }

        public Task ConfigureServices(IServiceCollection services)
        {  
            return Task.CompletedTask;
        }
    }
}