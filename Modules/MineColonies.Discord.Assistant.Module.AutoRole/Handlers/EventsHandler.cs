using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using MineColonies.Discord.Assistant.Interfaces.Interfaces.Events;

namespace MineColonies.Discord.Assistant.Module.AutoRole.Handlers
{
    public class EventsHandler : IUserUpdatedHandler, IUserJoinedHandler
    {
        private readonly Config _config;

        public EventsHandler(Config config)
        {
            _config = config;
        }

        public Task UserJoined(IGuildUser user)
        {
            if (user.IsPending != false) return Task.CompletedTask;
            
            new Thread(async () => await FixUserRoles(user)).Start();

            return Task.CompletedTask;
        }

        public Task GuildMemberUpdated(IGuildUser oldUser, IGuildUser newUser)
        {
            // handle pending user activating
            if (oldUser.IsPending != true || newUser.IsPending != false) return Task.CompletedTask;
            
            new Thread(async () => await FixUserRoles(newUser)).Start();
            
            return Task.CompletedTask;
        }

        private async Task FixUserRoles(IGuildUser user)
        {
            foreach (IRole role in user.Guild.Roles.Where(role =>_config.AutoRoles.Contains(role.Id)))
            {
                if (_config.AutoRoles.Contains(role.Id))
                {
                    await user.AddRoleAsync(role);
                }
            }
        }
    }
}