using System.Threading.Tasks;
using Discord.WebSocket;

namespace MineColonies.Discord.Assistant.Interfaces.Interfaces
{
    public interface ICommandHandler
    {
        public Task InitializeAsync();
        public Task HandleCommandAsync(SocketMessage msg);
    }
}