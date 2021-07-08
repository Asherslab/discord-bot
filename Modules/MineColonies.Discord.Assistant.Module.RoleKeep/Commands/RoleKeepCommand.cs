using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace MineColonies.Discord.Assistant.Module.RoleKeep.Commands
{
    public class RoleKeepCommand : ModuleBase<SocketCommandContext>
    {
        private readonly Config _config;

        public RoleKeepCommand(Config config)
        {
            _config = config;
        }
        
        [Command("role-keep")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public Task RoleKeepAsync(IRole role)
        {
            new Thread(async () =>
            {
                Thread.CurrentThread.IsBackground = true;
                Thread.CurrentThread.Name = $"role-keep : {Context.Message.Id}";
                
                if (!_config.RolesToKeep.Contains(role.Id))
                    _config.RolesToKeep.Add(role.Id);
                
                await Context.Message.AddReactionAsync(new Emoji("🔄"));

                _config.KeptRolesToUsers[role.Id] = new List<ulong>();
                foreach (SocketGuildUser user in Context.Guild.Users)
                {
                    if (user.Roles.Any(r => r.Id == role.Id))
                        _config.KeptRolesToUsers[role.Id].Add(user.Id);
                }
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