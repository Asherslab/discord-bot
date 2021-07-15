using Microsoft.Extensions.DependencyInjection;

namespace MineColonies.Discord.Assistant.Interfaces.Interfaces
{
    public interface IModuleStartup
    {
        public string ModuleName { get; }

        public void ConfigureServices(IServiceCollection services)
        {
        }
    }
}