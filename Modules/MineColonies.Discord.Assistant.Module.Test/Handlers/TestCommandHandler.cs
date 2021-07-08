using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using MineColonies.Discord.Assistant.Interfaces.Interfaces;

namespace MineColonies.Discord.Assistant.Module.Test.Handlers
{
    public class TestCommandHandler : ICommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _provider;
        private readonly CommandService _commands = new();

        public TestCommandHandler(DiscordSocketClient client, IServiceProvider provider)
        {
            _client = client;
            _provider = provider;
        }

        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetAssembly(GetType()), _provider);
        }

        public async Task HandleCommandAsync(SocketMessage msg)
        {
            // Don't process the command if it was a system message
            if (msg is not SocketUserMessage message) return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasStringPrefix("test!", ref argPos) ||
                  message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            // Create a WebSocket-based command context based on the message
            SocketCommandContext context = new(_client, message);

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            await _commands.ExecuteAsync(
                context,
                argPos,
                _provider);
        }
    }
}