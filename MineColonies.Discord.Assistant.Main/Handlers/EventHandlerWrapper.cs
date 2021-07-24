using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using MineColonies.Discord.Assistant.Interfaces.Interfaces.Events;

namespace MineColonies.Discord.Assistant.Main.Handlers
{
    public class EventHandlerWrapper
    {
        private readonly IServiceProvider _provider;

        public EventHandlerWrapper(IServiceProvider provider)
        {
            _provider = provider;
        }

        public Task UserJoined(IGuildUser user)
        {
            using IServiceScope scope = _provider.CreateScope();

            foreach (IEventHandler eventHandler in scope.ServiceProvider.GetServices<IEventHandler>())
            {
                if (eventHandler is IUserJoinedHandler handler)
                    new Thread(async () => await handler.UserJoined(user)).Start();
            }
            
            return Task.CompletedTask;
        }

        public Task GuildMemberUpdated(IGuildUser oldUser, IGuildUser newUser)
        {
            using IServiceScope scope = _provider.CreateScope();

            foreach (IEventHandler eventHandler in scope.ServiceProvider.GetServices<IEventHandler>())
            {
                if (eventHandler is IUserUpdatedHandler handler)
                    new Thread(async () => await handler.GuildMemberUpdated(oldUser, newUser)).Start();
            }
            
            return Task.CompletedTask;
        }

        public Task MessageReceived(SocketMessage message)
        {
            using IServiceScope scope = _provider.CreateScope();

            foreach (IEventHandler eventHandler in scope.ServiceProvider.GetServices<IEventHandler>())
            {
                if (eventHandler is IMessageReceivedHandler handler)
                    new Thread(async () => await handler.MessageReceived(message)).Start();
            }
            
            return Task.CompletedTask;
        }

        public Task Ready()
        {
            using IServiceScope scope = _provider.CreateScope();
            
            foreach (IEventHandler eventHandler in scope.ServiceProvider.GetServices<IEventHandler>())
            {
                if (eventHandler is IReadyHandler handler)
                    new Thread(async () => await handler.Ready()).Start();
            }

            return Task.CompletedTask;
        }
    }
}