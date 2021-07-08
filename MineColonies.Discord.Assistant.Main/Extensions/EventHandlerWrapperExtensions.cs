using Discord.WebSocket;
using MineColonies.Discord.Assistant.Main.Handlers;

namespace MineColonies.Discord.Assistant.Main.Extensions
{
    public static class EventHandlerWrapperExtensions
    {
        public static void RegisterEvents(this EventHandlerWrapper handler, DiscordSocketClient client)
        {
            client.UserJoined += handler.UserJoined;
            client.GuildMemberUpdated += handler.GuildMemberUpdated;
            client.MessageReceived += handler.MessageReceived;
        }
    }
}