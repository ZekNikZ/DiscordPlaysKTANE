using System;
using System.Threading.Tasks;
using DiscordPlaysKTANE.Discord.Entities;
using DiscordPlaysKTANE.Game;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Linq;
using DSharpPlus.Entities;

namespace DiscordPlaysKTANE.Discord.Commands {
    [Group("bomb")]
    [Description("Commands that related to the current bomb.")]
    internal class BombCommands {
        Dependencies dep;
        public BombCommands(Dependencies d) {
            this.dep = d;
        }

        [Command("edgework")]
        [Description("Get the edgework on the bomb.")]
        public async Task EdgeworkAsync(CommandContext ctx) {
            if (!ctx.RightChannel()) return;
            if (GameManager.Instance.BombInProgress) {
                await ctx.Reply("`" + GameManager.Instance.GetEdgeworkString() + "`");
            } else {
                await ctx.Reply(ResponsesTemplates.NotInBomb);
            }
        }

        [Command("time")]
        [Description("Get the time remaining on the bomb.")]
        public async Task TimeAsync(CommandContext ctx) {
            if (!ctx.RightChannel()) return;
            if (GameManager.Instance.BombInProgress) {
                await ctx.Reply($"Time Remaining: {GameManager.Instance.CurrentBomb.BombInfo.GetFormattedTime()}");
            } else {
                await ctx.Reply(ResponsesTemplates.NotInBomb);
            }
        }

        [Command("modules")]
        [Description("Get the unclaimed and unsolved modules on the bomb.")]
        public async Task ModulesAsync(CommandContext ctx) {
            if (!ctx.RightChannel()) return;
            if (GameManager.Instance.BombInProgress) {
                var results = GameManager.Instance.CurrentBomb.Modules
                                         .Where(x => !x.Solved)
                                         .GroupBy(m => m.ModuleName, m => m.ModuleID, (key, g) => new { Module = key, IDs = g.ToList() })
                                         .Select(x => "{0}: {1}".FormatThis(x.Module, x.IDs.Count > 5 ? String.Join(", ", x.IDs.Take(5)) + "..." : String.Join(",", x.IDs)));
                await ctx.Reply("**Unclaimed Modules:**\n```" + String.Join("\n", results) + "```"); // TODO: Fix
            } else {
                await ctx.Reply(ResponsesTemplates.NotInBomb);
            }
        }

        [Command("status")]
        [Description("Get some basic information about the bomb.")]
        public async Task StatusAsync(CommandContext ctx) {
            if (!ctx.RightChannel()) return;
            if (GameManager.Instance.BombInProgress) {
                DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder();
                embedBuilder.AddField("Time Remaining", GameManager.Instance.CurrentBomb.BombInfo.GetFormattedTime() + " of " + GameManager.Instance.CurrentBomb.BombInfo.GetFormattedStartTime());
                var modules = GameManager.Instance.CurrentBomb.Modules;
                embedBuilder.AddField("Modules", $"{modules.Length} modules ({modules.Count(m => m.Solved)} solved, {modules.Count(m => !m.Solved)} unsolved)");
                await ctx.Reply(embed: embedBuilder.WithTitle($"Bomb Information").WithColor(DiscordColor.Gold).Build());
            } else {
                await ctx.Reply(ResponsesTemplates.NotInBomb);
            }
        }
    }
}