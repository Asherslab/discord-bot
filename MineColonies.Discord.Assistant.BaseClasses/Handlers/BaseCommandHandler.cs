using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using MineColonies.Discord.Assistant.Interfaces.Interfaces;
// ReSharper disable MemberCanBePrivate.Global

namespace MineColonies.Discord.Assistant.BaseClasses.Handlers
{
    public abstract class BaseCommandHandler : ICommandHandler
    {
        protected readonly DiscordSocketClient Client;
        protected readonly IServiceProvider Provider;
        protected readonly string Prefix;
        
        protected readonly CommandService Commands = new();

        protected BaseCommandHandler(DiscordSocketClient client, IServiceProvider provider, string prefix)
        {
            Client = client;
            Provider = provider;
            Prefix = prefix;
        }

        protected abstract Assembly GetAssembly();

        public virtual async Task InitializeAsync()
        {
            await Commands.AddModulesAsync(GetAssembly(), Provider);
        }

        public virtual async Task HandleCommandAsync(SocketMessage msg)
        {
            // Don't process the command if it was a system message
            if (msg is not SocketUserMessage message) return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!message.HasStringPrefix(Prefix, ref argPos)/*( || message.HasMentionPrefix(_client.CurrentUser, ref argPos))*/ ||
                message.Author.IsBot)
                return;

            // Create a WebSocket-based command context based on the message
            SocketCommandContext context = new(Client, message);

            // Execute the command with the command context we just
            // created, along withExecuteAsync the service provider for precondition checks.
            await Commands.ExecuteAsync(
                context,
                argPos,
                Provider);
        }
    }
}