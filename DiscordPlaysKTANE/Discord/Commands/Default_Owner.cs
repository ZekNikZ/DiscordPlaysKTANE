using System;
using System.Threading.Tasks;
using DiscordPlaysKTANE.Discord.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace DiscordPlaysKTANE.Discord.Commands {
    [Group("owner"), Aliases("o"), RequireOwner]
    internal class Owner {
        Dependencies dep;
        public Owner(Dependencies d) {
            this.dep = d;
        }

        [Command("shutdown")]
        public async Task ShutdownAsync(CommandContext ctx) {
            await ctx.RespondAsync("Shutting down!");
            dep.Cts.Cancel();
        }
    }
}
