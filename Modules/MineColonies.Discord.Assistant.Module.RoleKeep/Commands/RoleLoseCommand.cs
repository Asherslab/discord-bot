using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace MineColonies.Discord.Assistant.Module.RoleKeep.Commands
{
    public class RoleLoseCommand : ModuleBase<SocketCommandContext>
    {
        private readonly Config _config;

        public RoleLoseCommand(Config config)
        {
            _config = config;
        }
        
        [Command("role-lose")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public Task RoleLoseAsync(IRole role)
        {
            new Thread(async () =>
            {
                Thread.CurrentThread.IsBackground = true;
                Thread.CurrentThread.Name = $"role-lose : {Context.Message.Id}";
                
                if (_config.RolesToKeep.Contains(role.Id))
                    _config.RolesToKeep.Remove(role.Id);
                _config.Save();

                await Context.Message.AddReactionAsync(new Emoji("U+2705"));
                await Task.Delay(TimeSpan.FromSeconds(30));
                try
                {
                    await Context.Message.DeleteAsync();
                }
                catch (Exception) { /* ignored, probably deleted by someone else */ }
            }).Start();
            return Task.CompletedTask;
        }
    }
}