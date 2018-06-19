using System;
using System.Threading.Tasks;
using DiscordPlaysKTANE.Discord.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace DiscordPlaysKTANE.Discord.Commands {
    internal class BotCommands {
        Dependencies dep;
        public BotCommands(Dependencies d) {
            this.dep = d;
        }

        [Command("ping")]
        [Description("Get the current ping of the bomb.")]
        public async Task PingAsync(CommandContext ctx) {
            await ctx.Reply($"pong! RTT: {ctx.Client.Ping}ms");
        }

        [Command("stayhere")]
        [Description("Force me to only work in a certain channel.")]
        public async Task ChannelSetAsync(CommandContext ctx) {
            Bot.Instance.Channel = ctx.Channel;
            await ctx.Reply("okay, I'll stay here.");
        }
    }
}
