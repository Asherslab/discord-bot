using System.Threading;
using System.Threading.Tasks;
using Discord.Commands;

namespace MineColonies.Discord.Assistant.Module.Test.Commands
{
    public class TestCommand : ModuleBase<SocketCommandContext>
    {
        [Command("test")]
        public Task TestAsync()
        {
            new Thread(async () =>
            {
                await ReplyAsync("Test!");
            }).Start();
            
            return Task.CompletedTask;
        }
    }
}