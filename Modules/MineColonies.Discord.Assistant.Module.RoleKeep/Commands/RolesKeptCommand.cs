using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace MineColonies.Discord.Assistant.Module.RoleKeep.Commands
{
    public class RolesKeptCommand : ModuleBase<SocketCommandContext>
    {
        private readonly Config _config;

        public RolesKeptCommand(Config config)
        {
            _config = config;
        }

        [Command("roles-kept")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public Task RolesKeptAsync()
        {
            new Thread(async () =>
            {
                Thread.CurrentThread.IsBackground = true;
                Thread.CurrentThread.Name = $"roles-kept : {Context.Message.Id}";
                
                IMessage reply;
                if (_config.RolesToKeep.Count > 0)
                {
                    reply = await ReplyAsync(_config.RolesToKeep.Select(id => $"<@&{id}>").Aggregate((prev, next) => prev + ", " + next));
                }
                else
                {
                    reply = await ReplyAsync("No RoleKeeps Set");
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