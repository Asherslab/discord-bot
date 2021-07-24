using System.Collections.Generic;

namespace MineColonies.Discord.Assistant.Module.LDTTeamAuth.Models
{
    // <rewardId, discordUsers>
    public record Event(Dictionary<string, List<ulong>> UserRewardMappings);
}