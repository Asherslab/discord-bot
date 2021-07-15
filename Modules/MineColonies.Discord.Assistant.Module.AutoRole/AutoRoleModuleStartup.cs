using Microsoft.Extensions.DependencyInjection;
using MineColonies.Discord.Assistant.Interfaces.Extensions;
using MineColonies.Discord.Assistant.Interfaces.Interfaces;
using MineColonies.Discord.Assistant.Module.AutoRole.Commands;
using MineColonies.Discord.Assistant.Module.AutoRole.Handlers;

namespace MineColonies.Discord.Assistant.Module.AutoRole
{
    public class AutoRoleModuleStartup : IModuleStartup
    {
        public string ModuleName => "auto-role";

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCommandHandler<CommandHandler>();
            services.AddEventHandler<EventsHandler>();
            
            services.AddSingleton<AutoRoleCommand>();
            services.AddSingleton<AutoRoleRemoveCommand>();
            services.AddSingleton<AutoRolesCommand>();
            
            services.AddSingleton(Config.Load());
        }
    }
}