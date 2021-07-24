using System.Threading.Tasks;
using MineColonies.Discord.Assistant.Interfaces.Interfaces.Events;
using MineColonies.Discord.Assistant.Module.LDTTeamAuth.Services;

namespace MineColonies.Discord.Assistant.Module.LDTTeamAuth.Handlers
{
    public class ReadyHandler : IReadyHandler
    {
        private readonly LDTTeamAuthService _authService;

        public ReadyHandler(LDTTeamAuthService authService)
        {
            _authService = authService;
        }

        public async Task Ready()
        {
            await _authService.RequestLDTTeamAuthCheck();
        }
    }
}