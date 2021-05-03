using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace MineColonies.Discord.RoleKeeper.Modules
{
    public class AutoRoleModule : ModuleBase<SocketCommandContext>
    {
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
                
                if (!Config.Instance.AutoRoles.Contains(role.Id))
                    Config.Instance.AutoRoles.Add(role.Id);

                if (addRole)
                {
                    await Context.Message.AddReactionAsync(new Emoji("🔄"));

                    foreach (SocketGuildUser user in Context.Guild.Users)
                    {
                        await user.AddRoleAsync(role);
                    }
                }

                Config.SaveConfig();

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
                
                if (Config.Instance.AutoRoles.Contains(role.Id))
                    Config.Instance.AutoRoles.Remove(role.Id);

                Config.SaveConfig();

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
                
                IMessage reply = await ReplyAsync(Config.Instance.AutoRoles.Select(id => $"<@&{id}>").Aggregate((prev, next) => prev + ", " + next));
                
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