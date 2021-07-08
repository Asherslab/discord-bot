using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace MineColonies.Discord.Assistant.Module.AutoRole.Commands
{
    public class AutoRoleCommand : ModuleBase<SocketCommandContext>
    {
        private readonly Config _config;

        public AutoRoleCommand(Config config)
        {
            _config = config;
        }

        [Command("auto-role")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public Task AutoRoleAsync(IRole role, bool addRole = false)
        {
            new Thread(async () =>
            {
                Thread.CurrentThread.IsBackground = true;
                Thread.CurrentThread.Name = $"auto-role : {Context.Message.Id}";
                
                if (!_config.AutoRoles.Contains(role.Id))
                    _config.AutoRoles.Add(role.Id);

                if (addRole)
                {
                    await Context.Message.AddReactionAsync(new Emoji("🔄"));

                    foreach (SocketGuildUser user in Context.Guild.Users)
                    {
                        await user.AddRoleAsync(role);
                    }
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