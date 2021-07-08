using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using MineColonies.Discord.Assistant.Interfaces.Interfaces.Events;

namespace MineColonies.Discord.Assistant.Module.RoleKeep.Handlers
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
            if (oldUser.IsPending == true && newUser.IsPending == false)
            {
                new Thread(async () => await FixUserRoles(newUser)).Start();
                return Task.CompletedTask;
            }
            
            new Thread(async () => await FixUserRoles(newUser)).Start();
            
            // if roles changed, update Kept users if needed
            if (!oldUser.RoleIds.SequenceEqual(newUser.RoleIds))
            {
                new Thread(() =>
                {
                    try
                    {
                        foreach (ulong roleId in _config.RolesToKeep)
                        {
                            // for safety
                            _config.KeptRolesToUsers[roleId] ??= new List<ulong>();

                            if (_config.KeptRolesToUsers[roleId].Contains(roleId) &&
                                !newUser.RoleIds.Contains(roleId))
                            {
                                _config.KeptRolesToUsers[roleId].Remove(oldUser.Id);
                            }

                            if (!_config.KeptRolesToUsers[roleId].Contains(roleId) &&
                                newUser.RoleIds.Contains(roleId))
                            {
                                _config.KeptRolesToUsers[roleId].Add(oldUser.Id);
                            }
                        }
                    }
                    finally
                    {
                        _config.Save();
                    }
                }).Start();
            }
            
            return Task.CompletedTask;
        }

        private async Task FixUserRoles(IGuildUser user)
        {
            foreach (IRole role in user.Guild.Roles.Where(role => _config.RolesToKeep.Contains(role.Id)))
            {
                if (_config.KeptRolesToUsers.ContainsKey(role.Id) &&
                    _config.KeptRolesToUsers[role.Id].Contains(user.Id))
                {
                    await user.AddRoleAsync(role);
                }
            }
        }
    }
}