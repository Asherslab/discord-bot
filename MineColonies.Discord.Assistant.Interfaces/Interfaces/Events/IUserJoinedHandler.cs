using System.Threading.Tasks;
using Discord;

namespace MineColonies.Discord.Assistant.Interfaces.Interfaces.Events
{
    public interface IUserJoinedHandler : IEventHandler
    {
        public Task UserJoined(IGuildUser user);
    }
}