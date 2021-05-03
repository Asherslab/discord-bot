using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace MineColonies.Discord.Assistant.Main
{
    internal class Program
    {
        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;

        private async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _client.Log += Log;

            string token = Environment.GetEnvironmentVariable("DISCORD_KEY");

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            _client.Ready += async () =>
            {
                Console.WriteLine("Bot is connected!");
            };

            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}