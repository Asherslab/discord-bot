using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using MineColonies.Discord.Assistant.Module.LDTTeamAuth.Config;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using IConnection = RabbitMQ.Client.IConnection;
using JsonSerializer = System.Text.Json.JsonSerializer;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MineColonies.Discord.Assistant.Module.LDTTeamAuth.Services
{
    public class LDTTeamAuthLoggingService : BackgroundService
    {
        private readonly DiscordSocketClient _discord;
        private readonly LDTTeamAuthConfig _ldtTeamAuthConfig;

        private readonly Channel<AuthEmbed?> _embeds = Channel.CreateBounded<AuthEmbed?>(new BoundedChannelOptions(500)
        {
            FullMode = BoundedChannelFullMode.Wait
        });

        public LDTTeamAuthLoggingService(DiscordSocketClient discord, LDTTeamAuthConfig ldtTeamAuthConfig)
        {
            _discord = discord;
            _ldtTeamAuthConfig = ldtTeamAuthConfig;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            ConnectionFactory factory = new() {HostName = "localhost"};
            using IConnection connection = factory.CreateConnection();
            using IModel model = connection.CreateModel();

            model.QueueDeclare("embeds",
                false,
                false,
                false,
                null);

            EventingBasicConsumer consumer = new(model);
            consumer.Received += async (_, ea) =>
            {
                byte[] body = ea.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);
                await _embeds.Writer.WriteAsync(JsonSerializer.Deserialize<AuthEmbed>(message), stoppingToken);
            };

            model.BasicConsume("embeds", true, consumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                AuthEmbed? embed = await _embeds.Reader.ReadAsync(stoppingToken);

                if (embed == null) continue;

                IMessageChannel? channel = _discord.GetChannel(_ldtTeamAuthConfig.LoggingChannelId) as IMessageChannel;

                if (channel == null) continue;

                EmbedBuilder embedBuilder = new();

                if (embed.Fields != null)
                {
                    foreach (AuthEmbed.AuthField field in embed.Fields.Where(field => field.Value.Length >= 1024))
                    {
                        field.Value = $"{field.Value[..1021]}...";
                    }
                    
                    foreach (AuthEmbed.AuthField field in embed.Fields)
                    {
                        embedBuilder.AddField(new EmbedFieldBuilder()
                            .WithName(field.Name)
                            .WithValue(field.Value)
                            .WithIsInline(field.Inline));
                    }
                }

                embedBuilder.WithTitle(embed.Title);
                embedBuilder.WithDescription(embed.Description);
                embedBuilder.WithTimestamp(DateTimeOffset.Parse(embed.Timestamp));
                embedBuilder.WithColor(new Color(embed.Color));

                if (embed.EmbedFooter != null)
                {
                    embedBuilder.Footer = new EmbedFooterBuilder()
                        .WithText(embed.EmbedFooter.Text)
                        .WithIconUrl(embed.EmbedFooter.IconUrl);
                }
                
                await channel.SendMessageAsync(embed: embedBuilder.Build(), options: new RequestOptions
                {
                    CancelToken = stoppingToken,
                    RetryMode = RetryMode.RetryRatelimit
                });
            }
        }

        public class AuthEmbed
        {
            [JsonPropertyName("title")]
            public string Title { get; set; } = null!;

            [JsonPropertyName("description")]
            public string Description { get; set; } = null!;

            [JsonPropertyName("color")]
            public uint Color { get; set; }

            [JsonPropertyName("timestamp")]
            // ReSharper disable once UnusedMember.Global
            public string Timestamp => DateTime.Now.ToString("o", CultureInfo.InvariantCulture);

            [JsonPropertyName("footer")]
            public AuthFooter? EmbedFooter { get; set; }

            public class AuthFooter
            {
                [JsonPropertyName("text")]
                public string Text { get; set; } = null!;

                [JsonPropertyName("icon_url")]
                public string IconUrl { get; set; } = null!;
            }

            [JsonPropertyName("fields")]
            public List<AuthField>? Fields { get; set; } = new();

            public class AuthField
            {
                [JsonPropertyName("name")]
                public string Name { get; set; } = null!;

                [JsonPropertyName("value")]
                public string Value { get; set; } = null!;

                [JsonPropertyName("inline")]
                public bool Inline { get; set; }
            }
        }
    }
}