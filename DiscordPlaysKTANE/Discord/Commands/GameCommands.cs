using System;
using System.Threading.Tasks;
using DiscordPlaysKTANE.Discord.Entities;
using DiscordPlaysKTANE.Game;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace DiscordPlaysKTANE.Discord.Commands {
    internal class GameCommands {
        Dependencies dep;
        public GameCommands(Dependencies d) {
            this.dep = d;
        }

        [Command("newbomb")]
        [Description("Start a new bomb with a certain amount of modules.")]
        public async Task NewBombAsync(CommandContext ctx, int modules = Constants.DEFAULT_MODULES) {
            if (!ctx.RightChannel()) return;
            if (modules > 0) {
                if (GameManager.Instance.NewBomb(modules)) {
                    await ctx.Reply("starting a new bomb with {0} modules...".FormatThis(modules));
                } else {
                    await ctx.Reply("already in a bomb!");
                }
            } else {
                await ctx.Reply("try again with a positive integer!");
            }
        }

        [Command("detonate")]
        [Description("Detonate the currently active bomb.")]
        public async Task DetonateAsync(CommandContext ctx) {
            if (!ctx.RightChannel()) return;
            if (GameManager.Instance.Detonate()) {
                //await ctx.Reply("detonating the bomb...");
            } else {
                await ctx.Reply("there is no bomb running!");
            }
        }
    }
}