using System.Threading.Tasks;
using Discord.WebSocket;

namespace MineColonies.Discord.Assistant.Interfaces.Interfaces.Events
{
    public interface IMessageReceivedHandler : IEventHandler
    {
        public Task MessageReceived(SocketMessage message);
    }
}