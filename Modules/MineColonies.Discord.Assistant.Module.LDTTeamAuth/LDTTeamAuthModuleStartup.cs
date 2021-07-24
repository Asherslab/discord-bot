using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MineColonies.Discord.Assistant.Interfaces.Extensions;
using MineColonies.Discord.Assistant.Interfaces.Interfaces;
using MineColonies.Discord.Assistant.Module.LDTTeamAuth.Config;
using MineColonies.Discord.Assistant.Module.LDTTeamAuth.Handlers;
using MineColonies.Discord.Assistant.Module.LDTTeamAuth.Services;

namespace MineColonies.Discord.Assistant.Module.LDTTeamAuth
{
    public class LDTTeamAuthModuleStartup : IModuleStartup
    {
        public string ModuleName => "LDTTeamAuth";

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<LDTTeamAuthLoggingService>();
            services.AddHostedService<LDTTeamAuthEventsService>();

            services.AddTransient<LDTTeamAuthService>();

            services.AddTransient(sp =>
            {
                IConfiguration config = sp.GetRequiredService<IConfiguration>();
                LDTTeamAuthConfig? ldtTeamAuthConfig = config.GetSection("LDTTeamAuth").Get<LDTTeamAuthConfig>();

                if (ldtTeamAuthConfig == null)
                    throw new Exception("LDTTeamAuth not set in configuration!");

                return ldtTeamAuthConfig;
            });

            services.AddEventHandler<ReadyHandler>();
        }
    }
}