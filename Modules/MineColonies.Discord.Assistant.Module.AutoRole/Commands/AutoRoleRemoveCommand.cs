using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace MineColonies.Discord.Assistant.Module.AutoRole.Commands
{
    public class AutoRoleRemoveCommand : ModuleBase<SocketCommandContext>
    {
        private readonly Config _config;

        public AutoRoleRemoveCommand(Config config)
        {
            _config = config;
        }

        [Command("auto-role-remove")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public Task AutoRoleRemoveAsync(IRole role)
        {
            new Thread(async () =>
            {
                Thread.CurrentThread.IsBackground = true;
                Thread.CurrentThread.Name = $"auto-role-remove : {Context.Message.Id}";
                
                if (_config.AutoRoles.Contains(role.Id))
                    _config.AutoRoles.Remove(role.Id);

                _config.Save();

                await Context.Message.AddReactionAsync(new Emoji("✅"));
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