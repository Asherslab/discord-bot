using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MineColonies.Discord.Assistant.Interfaces.Interfaces;
using MineColonies.Discord.Assistant.Module.Leveling.Database;

namespace MineColonies.Discord.Assistant.Module.Leveling
{
    public class LevelingModuleStartup : IModuleStartup
    {
        public string ModuleName => "leveling-module";

        public Task ConfigureServices(IServiceCollection services)
        {
            /*services.AddCommandHandler<CommandHandler>();
            services.AddEventHandler<EventsHandler>();
            
            services.AddSingleton<RoleKeepCommand>();
            services.AddSingleton<RoleLoseCommand>();
            services.AddSingleton<RolesKeptCommand>();*/

            services.AddDbContext<DatabaseContext>(opts =>
            {
                opts.UseSqlite("leveling.db",
                    b => b.MigrationsAssembly("MineColonies.Discord.Assistant.Module.Leveling"));
            });

            services.AddSingleton(Config.Load());

            return Task.CompletedTask;
        }
    }
}