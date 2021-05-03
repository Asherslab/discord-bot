using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace MineColonies.Discord.RoleKeeper.Modules
{
    public class RoleKeeperModule : ModuleBase<SocketCommandContext>
    {
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
                
                if (!Config.Instance.RolesToKeep.Contains(role.Id))
                    Config.Instance.RolesToKeep.Add(role.Id);
                
                await Context.Message.AddReactionAsync(new Emoji("🔄"));

                Config.Instance.KeptRolesToUsers[role.Id] = new List<ulong>();
                foreach (SocketGuildUser user in Context.Guild.Users)
                {
                    if (user.Roles.Any(r => r.Id == role.Id))
                        Config.Instance.KeptRolesToUsers[role.Id].Add(user.Id);
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
                
                if (Config.Instance.RolesToKeep.Contains(role.Id))
                    Config.Instance.RolesToKeep.Remove(role.Id);
                Config.SaveConfig();

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

                IMessage reply = await ReplyAsync(Config.Instance.RolesToKeep.Select(id => $"<@&{id}>").Aggregate((prev, next) => prev + ", " + next));
                
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