using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using DiscordPlaysKTANE.Discord.Services;

namespace DiscordPlaysKTANE.Discord {
    public partial class DiscordClient {
        public static DiscordClient Instance => _instance ?? (_instance = new DiscordClient());
        private static DiscordClient _instance;

        public string Prefix => _prefix ?? (_prefix = "!");
        private string _prefix;

        public DiscordClient(string prefix = "!") {
            _prefix = prefix;
        }

        public void Connect() {
            this.MainAsync().GetAwaiter().GetResult();
        }
    }

    public partial class DiscordClient {
        public async Task MainAsync() {
            var services = ConfigureServices();

            var client = services.GetRequiredService<DiscordSocketClient>();

            client.Log += LogAsync;
            services.GetRequiredService<CommandService>().Log += LogAsync;

            await client.LoginAsync(TokenType.Bot, File.ReadAllText(Constants.TokenPath));
            await client.StartAsync();

            await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

            await Task.Delay(-1);
        }

        private Task LogAsync(LogMessage log) {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private IServiceProvider ConfigureServices() {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                .BuildServiceProvider();
        }
    }
}
