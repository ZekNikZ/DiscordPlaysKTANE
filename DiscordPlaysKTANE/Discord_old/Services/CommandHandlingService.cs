using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordPlaysKTANE.Discord.Services {
    public class CommandHandlingService {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;

        public CommandHandlingService(IServiceProvider services) {
            _commands = services.GetRequiredService<CommandService>();
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _services = services;

            _discord.MessageReceived += MessageReceivedAsync;
        }

        public async Task InitializeAsync() {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        public async Task MessageReceivedAsync(SocketMessage rawMessage) {
            Debug.Log(rawMessage);

            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            var argPos = 0;
            if (!message.HasStringPrefix(DiscordClient.Instance.Prefix, ref argPos)) return;

            var context = new SocketCommandContext(_discord, message);
            var result = await _commands.ExecuteAsync(context, argPos, _services);

            if (result.Error.HasValue &&
                result.Error.Value != CommandError.UnknownCommand) // it's bad practice to send 'unknown command' errors
                await context.Channel.SendMessageAsync(result.ToString());
        }
    }
}
