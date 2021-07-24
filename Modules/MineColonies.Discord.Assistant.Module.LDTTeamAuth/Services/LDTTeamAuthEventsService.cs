using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using MineColonies.Discord.Assistant.Module.LDTTeamAuth.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using IConnection = RabbitMQ.Client.IConnection;

namespace MineColonies.Discord.Assistant.Module.LDTTeamAuth.Services
{
    public class LDTTeamAuthEventsService : BackgroundService
    {
        private readonly LDTTeamAuthService _ldtTeamAuthService;
        
        private readonly Channel<Event?> _events = Channel.CreateBounded<Event?>(new BoundedChannelOptions(500)
        {
            FullMode = BoundedChannelFullMode.Wait
        });

        public LDTTeamAuthEventsService(LDTTeamAuthService ldtTeamAuthService)
        {
            _ldtTeamAuthService = ldtTeamAuthService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            ConnectionFactory factory = new() {HostName = "localhost"};
            using IConnection connection = factory.CreateConnection();
            using IModel model = connection.CreateModel();

            model.QueueDeclare("events",
                false,
                false,
                false,
                null);

            EventingBasicConsumer consumer = new(model);
            consumer.Received += async (_, ea) =>
            {
                byte[] body = ea.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);
                await _events.Writer.WriteAsync(JsonSerializer.Deserialize<Event>(message), stoppingToken);
            };
            model.BasicConsume("events", true, consumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                Event? receivedEvent = await _events.Reader.ReadAsync(stoppingToken);
                
                if (receivedEvent == null) continue;

                await _ldtTeamAuthService.CheckLDTTeamAuth(receivedEvent);
            }
        }
    }
}