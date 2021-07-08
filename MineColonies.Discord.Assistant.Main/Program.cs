using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace MineColonies.Discord.Assistant.Main
{
    internal class Program
    {
        public static void Main()
            => new Program().MainAsync().GetAwaiter().GetResult();

        private readonly IServiceCollection _serviceCollection = new ServiceCollection();

        private async Task MainAsync()
        {
            Startup.ConfigureModules(new List<string> {"test", "auto-role"});
            await Startup.ConfigureServices(_serviceCollection);
            await using ServiceProvider serviceProvider = _serviceCollection.BuildServiceProvider();

            DiscordSocketClient client = serviceProvider.GetRequiredService<DiscordSocketClient>();

            await Startup.ConfigureClient(serviceProvider, client);
            await client.StartAsync();

            await Task.Delay(Timeout.Infinite);
        }
    }
}