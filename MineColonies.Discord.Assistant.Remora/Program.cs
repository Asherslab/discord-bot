using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Remora.Discord.Hosting.Extensions;

namespace MineColonies.Discord.Assistant.Remora
{
    public class Program
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
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .AddDiscordService(services =>
                {
                    IConfiguration configuration = services.GetRequiredService<IConfiguration>();
                    return configuration["Token"];
                });
    }
}