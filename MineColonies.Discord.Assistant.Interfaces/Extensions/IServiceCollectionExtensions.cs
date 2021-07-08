using Microsoft.Extensions.DependencyInjection;
using MineColonies.Discord.Assistant.Interfaces.Interfaces;
using MineColonies.Discord.Assistant.Interfaces.Interfaces.Events;

namespace MineColonies.Discord.Assistant.Interfaces.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommandHandler<T>(this IServiceCollection collection) where T : class, ICommandHandler
        {
            return collection.AddSingleton<ICommandHandler, T>();
        }
        
        public static IServiceCollection AddEventHandler<T>(this IServiceCollection collection) where T : class, IEventHandler
        {
            return collection.AddSingleton<IEventHandler, T>();
        }
    }
}