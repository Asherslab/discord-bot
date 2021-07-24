using System.Collections.Generic;

namespace MineColonies.Discord.Assistant.Module.LDTTeamAuth.Config
{
    public class LDTTeamAuthConfig
    {
        public ulong LoggingChannelId { get; set; }
        
        public string LDTTeamAuthUrl { get; set; }
        // <DiscordServerId, <RewardId, RoleId>>
        public Dictionary<string, Dictionary<string, List<ulong>>> RoleMappings { get; set; } = new();
        
        public bool RemoveUsersFromRoles { get; set; }

        public List<ulong> UserExceptions { get; set; } = new();
    }
}