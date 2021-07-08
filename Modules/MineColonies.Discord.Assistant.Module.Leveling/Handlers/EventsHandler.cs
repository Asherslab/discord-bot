using System.Threading.Tasks;
using Discord.WebSocket;
using MineColonies.Discord.Assistant.Interfaces.Interfaces.Events;
using MineColonies.Discord.Assistant.Module.Leveling.Database;

namespace MineColonies.Discord.Assistant.Module.Leveling.Handlers
{
    public class EventsHandler : IMessageReceivedHandler
    {
        private readonly Config _config;
        private readonly DatabaseContext _database;

        public EventsHandler(Config config, DatabaseContext database)
        {
            _config = config;
            _database = database;
        }

        public async Task MessageReceived(SocketMessage message)
        {
            throw new System.NotImplementedException();
        }
    }
}