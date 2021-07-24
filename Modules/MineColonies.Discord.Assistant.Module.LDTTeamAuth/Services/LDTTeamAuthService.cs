using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using MineColonies.Discord.Assistant.Module.LDTTeamAuth.Config;
using MineColonies.Discord.Assistant.Module.LDTTeamAuth.Models;

namespace MineColonies.Discord.Assistant.Module.LDTTeamAuth.Services
{
    public class LDTTeamAuthService
    {
        private readonly DiscordSocketClient _discord;
        private readonly LDTTeamAuthConfig _ldtTeamAuthConfig;
        private readonly HttpClient _httpClient;

        public LDTTeamAuthService(DiscordSocketClient discord, LDTTeamAuthConfig ldtTeamAuthConfig,
            HttpClient httpClient)
        {
            _discord = discord;
            _ldtTeamAuthConfig = ldtTeamAuthConfig;
            _httpClient = httpClient;
        }

        public async Task RequestLDTTeamAuthCheck()
        {
            string url = _ldtTeamAuthConfig.LDTTeamAuthUrl;

            if (!url.EndsWith("/"))
                url += "/";
            url += "api/webhook/all";

            await _httpClient.GetAsync(url);
        }

        public async Task CheckLDTTeamAuth(Event checkEvent)
        {
            foreach (string server in _ldtTeamAuthConfig.RoleMappings.Keys)
            {
                if (!ulong.TryParse(server, out ulong serverId)) continue;
                
                SocketGuild guild = _discord.GetGuild(serverId);

                IReadOnlyCollection<SocketRole> guildRoles = guild.Roles;

                Dictionary<string, List<ulong>> rewardRoles = _ldtTeamAuthConfig.RoleMappings[server];

                Dictionary<SocketGuildUser, List<ulong>> memberRoles =
                    guild.Users.ToDictionary(
                        x => x,
                        x => x.Roles.Select(y => y.Id).ToList()
                    );

                foreach ((SocketGuildUser user, List<ulong> roles) in memberRoles)
                {
                    List<string> rewards = checkEvent.UserRewardMappings
                        .Where(x => x.Value.Contains(user.Id))
                        .Select(x => x.Key)
                        .ToList();

                    // roles to award
                    List<ulong> rewardedRoles = rewardRoles
                        .Where(x => rewards.Contains(x.Key))
                        .SelectMany(x => x.Value)
                        .Distinct()
                        .ToList();

                    // roles not rewarded less rewardedRoles
                    List<ulong> notRewardedRoles = rewardRoles
                        .Where(x => !rewards.Contains(x.Key))
                        .SelectMany(x => x.Value)
                        .Where(x => !rewardedRoles.Contains(x))
                        .Distinct()
                        .ToList();

                    List<IRole> rolesToAdd =
                        (from rewardRole in rewardedRoles
                            let role = guildRoles.FirstOrDefault(x => x.Id == rewardRole)
                            where !roles.Contains(rewardRole) && role != null
                            select role).Cast<IRole>().ToList();

                    List<IRole> rolesToRemove =
                        (from notRewardRole in notRewardedRoles
                            let role = guildRoles.FirstOrDefault(x => x.Id == notRewardRole)
                            where roles.Contains(notRewardRole) && role != null
                            select role).Cast<IRole>().ToList();

                    if (rolesToAdd.Count >= 1)
                        await user.AddRolesAsync(rolesToAdd);

                    if (!_ldtTeamAuthConfig.RemoveUsersFromRoles ||
                        _ldtTeamAuthConfig.UserExceptions.Contains(user.Id)) continue;
                    
                    if (rolesToRemove.Count >= 1)
                        await user.RemoveRolesAsync(rolesToRemove);
                }
            }
        }
    }
}