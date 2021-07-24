using System.Threading.Tasks;

namespace MineColonies.Discord.Assistant.Interfaces.Interfaces.Events
{
    public interface IReadyHandler : IEventHandler
    {
        public Task Ready();
    }
}