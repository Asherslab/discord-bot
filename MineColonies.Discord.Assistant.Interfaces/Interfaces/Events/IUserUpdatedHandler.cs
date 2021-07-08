using System.Threading.Tasks;
using Discord;

namespace MineColonies.Discord.Assistant.Interfaces.Interfaces.Events
{
    public interface IUserUpdatedHandler : IEventHandler
    {
        public Task GuildMemberUpdated(IGuildUser oldUser, IGuildUser newUser);
    }
}