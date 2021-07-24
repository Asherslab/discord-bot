using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace MineColonies.Discord.Assistant.Main
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(x =>
                {
                    // We will use the environment variable DISQORD_TOKEN for the bot token.
                    x.AddEnvironmentVariables("ASSISTANT_");
                })
                .ConfigureDiscordHost((ctx, config) =>
                {
                    config.SocketConfig = new DiscordSocketConfig
                    {
                        LogLevel = LogSeverity.Verbose,
                        AlwaysDownloadUsers = true,
                        MessageCacheSize = 200
                    };

                    config.Token = ctx.Configuration["token"];
                })
                .UseCommandService((_, config) =>
                {
                    config.DefaultRunMode = RunMode.Async;
                    config.CaseSensitiveCommands = false;
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("http://localhost:9999");
                    webBuilder.UseStartup<Startup>();
                });

        /*public static void Main()
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
        }*/
    }
}