using Microsoft.Extensions.DependencyInjection;
using MineColonies.Discord.Assistant.Interfaces.Extensions;
using MineColonies.Discord.Assistant.Interfaces.Interfaces;
using MineColonies.Discord.Assistant.Module.RoleKeep.Commands;
using MineColonies.Discord.Assistant.Module.RoleKeep.Handlers;

namespace MineColonies.Discord.Assistant.Module.RoleKeep
{
    public class RoleKeepModuleStartup : IModuleStartup
    {
        public string ModuleName => "role-keep";

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCommandHandler<CommandHandler>();
            services.AddEventHandler<EventsHandler>();
            
            services.AddSingleton<RoleKeepCommand>();
            services.AddSingleton<RoleLoseCommand>();
            services.AddSingleton<RolesKeptCommand>();
            
            services.AddSingleton(Config.Load());
        }
    }
}