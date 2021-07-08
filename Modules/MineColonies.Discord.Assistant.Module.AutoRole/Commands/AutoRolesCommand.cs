using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace MineColonies.Discord.Assistant.Module.AutoRole.Commands
{
    public class AutoRolesCommand : ModuleBase<SocketCommandContext>
    {
        private readonly Config _config;

        public AutoRolesCommand(Config config)
        {
            _config = config;
        }
        
        [Command("auto-roles")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public Task AutoRolesAsync()
        {
            new Thread(async () =>
            {
                Thread.CurrentThread.IsBackground = true;
                Thread.CurrentThread.Name = $"auto-roles : {Context.Message.Id}";

                IMessage reply;
                if (_config.AutoRoles.Any())
                {
                    reply = await ReplyAsync(_config.AutoRoles.Select(id => $"<@&{id}>")
                        .Aggregate((prev, next) => prev + ", " + next));
                }
                else
                {
                    reply = await ReplyAsync("No AutoRoles Set");
                }

                await Context.Message.AddReactionAsync(new Emoji("✅"));
                await Task.Delay(TimeSpan.FromSeconds(30));

                try
                {
                    await Context.Message.DeleteAsync();
                }
                catch (Exception) { /* ignored, probably deleted by someone else */ }
                
                try
                {
                    await reply.DeleteAsync();
                }
                catch (Exception) { /* ignored, probably deleted by someone else */ }
            }).Start();
            return Task.CompletedTask;
        }
    }
}