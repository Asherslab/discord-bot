using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;

namespace MineColonies.Discord.RoleKeeper.Handlers
{
    public class EventsHandler
    {
        public static Task UserJoined(IGuildUser user)
        {
            if (user.IsPending == false)
            {
                new Thread(async () => await FixUserRoles(user)).Start();
            }

            return Task.CompletedTask;
        }

        public static Task UserUpdated(IGuildUser oldUser, IGuildUser newUser)
        {
            // handle pending user activating
            if (oldUser.IsPending == true && newUser.IsPending == false)
            {
                new Thread(async () => await FixUserRoles(newUser)).Start();
                return Task.CompletedTask;
            }

            // if roles changed, update Kept users if needed
            if (!oldUser.RoleIds.SequenceEqual(newUser.RoleIds))
            {
                new Thread(() =>
                {
                    try
                    {
                        foreach (ulong roleId in Config.Instance.RolesToKeep)
                        {
                            // for safety
                            Config.Instance.KeptRolesToUsers[roleId] ??= new List<ulong>();

                            if (Config.Instance.KeptRolesToUsers[roleId].Contains(roleId) &&
                                !newUser.RoleIds.Contains(roleId))
                            {
                                Config.Instance.KeptRolesToUsers[roleId].Remove(oldUser.Id);
                            }

                            if (!Config.Instance.KeptRolesToUsers[roleId].Contains(roleId) &&
                                newUser.RoleIds.Contains(roleId))
                            {
                                Config.Instance.KeptRolesToUsers[roleId].Add(oldUser.Id);
                            }
                        }
                    }
                    finally
                    {
                        Config.SaveConfig();
                    }
                }).Start();
            }

            return Task.CompletedTask;
        }

        private static async Task FixUserRoles(IGuildUser user)
        {
            foreach (IRole role in user.Guild.Roles.Where(role =>
                Config.Instance.RolesToKeep.Contains(role.Id) || Config.Instance.AutoRoles.Contains(role.Id)))
            {
                if ((Config.Instance.KeptRolesToUsers.ContainsKey(role.Id) &&
                     Config.Instance.KeptRolesToUsers[role.Id].Contains(user.Id)) ||
                    Config.Instance.AutoRoles.Contains(role.Id))
                {
                    await user.AddRoleAsync(role);
                }
            }
        }
    }
}