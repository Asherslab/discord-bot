using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MineColonies.Discord.Assistant.Remora.Commands;
using MineColonies.Discord.Assistant.Remora.Responders;
using Remora.Commands.Extensions;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Services;
using Remora.Discord.Core;
using Remora.Discord.Gateway.Extensions;
using Remora.Results;

namespace MineColonies.Discord.Assistant.Remora
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDiscordCommands(true);
            services.AddCommandGroup<TestCommands>();
            services.AddResponder<Responder>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            ILogger<Startup> log = app.ApplicationServices.GetRequiredService<ILogger<Startup>>();
            SlashService slashService = app.ApplicationServices.GetRequiredService<SlashService>();
            CancellationTokenSource cancellationSource = new();

            Result checkSlashSupport = slashService.SupportsSlashCommands();
            if (!checkSlashSupport.IsSuccess)
            {
                log.LogWarning
                (
                    "The registered commands of the bot don't support slash commands: {Reason}",
                    checkSlashSupport.Unwrap().Message
                );
            }
            else
            {
                Result updateSlash = slashService.UpdateSlashCommandsAsync(new Snowflake(139070364159311872), cancellationSource.Token).Result;
                if (!updateSlash.IsSuccess)
                {
                    log.LogWarning("Failed to update slash commands: {Reason}", updateSlash.Unwrap().Message);
                }
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello World!"); });
            });
        }
    }
}