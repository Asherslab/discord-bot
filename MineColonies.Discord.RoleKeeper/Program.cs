using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MineColonies.Discord.RoleKeeper.Handlers;

namespace MineColonies.Discord.RoleKeeper
{
    internal class Program
    {
        public static void Main()
            => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;

        private async Task MainAsync()
        {
            Config.LoadConfig();
            
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                AlwaysDownloadUsers = true
            });
            _client.Log += Log;

            string token = Environment.GetEnvironmentVariable("DISCORD_KEY");

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            _client.UserJoined += EventsHandler.UserJoined;
            _client.GuildMemberUpdated += EventsHandler.UserUpdated;
            _client.Ready += async () =>
            {
                Console.WriteLine("Bot is connected!");
                await new CommandHandler(_client, new CommandService()).InstallCommandsAsync();
            };

            await Task.Delay(-1);
        }

        private static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}