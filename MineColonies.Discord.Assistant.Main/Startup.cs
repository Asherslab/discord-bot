using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MineColonies.Discord.Assistant.Interfaces.Interfaces;
using MineColonies.Discord.Assistant.Main.Configuration;
using MineColonies.Discord.Assistant.Main.Handlers;
using MineColonies.Discord.Assistant.Main.HostServices;
using MineColonies.Discord.Assistant.Module.AutoRole;
using MineColonies.Discord.Assistant.Module.LDTTeamAuth;
using MineColonies.Discord.Assistant.Module.LDTTeamAuth.Services;
using MineColonies.Discord.Assistant.Module.RoleKeep;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace MineColonies.Discord.Assistant.Main
{
    public class Startup
    {
        //TODO: switch to auto discovery
        private static readonly List<IModuleStartup>? Modules = new()
        {
            new AutoRoleModuleStartup(),
            new RoleKeepModuleStartup(),
            new LDTTeamAuthModuleStartup()
        };

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private readonly IConfiguration _configuration;

        private IEnumerable<IModuleStartup> GetModules()
        {
            ModulesConfig modulesConfig = _configuration.GetSection("Modules").Get<ModulesConfig>();

            Console.WriteLine($"Enabled Modules: {JsonSerializer.Serialize(modulesConfig.Enabled)}");
            
            return Modules!.Where(module => modulesConfig.Enabled.Contains(module.ModuleName)).ToList();
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<EventHandlerWrapper>();

            services.AddHttpClient();
            services.AddHostedService<ConfigureCommandHandlers>();
            services.AddHostedService<ConfigureEventHandlerWrapper>();

            foreach (IModuleStartup moduleStartup in GetModules())
            {
                moduleStartup.ConfigureServices(services);
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }
    }
}