using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DiscordPlaysKTANE.Discord;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace DiscordPlaysKTANE {
    public static class StringExtensions {
        public static string FormatThis(this string format, params object[] args) {
            return String.Format(format, args);
        }

        public static int FindUniqueIn(this string str, string[] list) {
            var matches = list.Where(x => x.Contains(str));
            return matches.Count() == 1 ? Array.IndexOf(list, matches.First()) : -1;
        }

        public static int FindUniqueIn(this string str, List<string> list) {
            var matches = list.Where(x => x.Contains(str));
            return matches.Count() == 1 ? list.IndexOf(matches.First()) : -1;
        }
    }

    public static class CommandExtensions {
        public static async Task<DiscordMessage> Reply(this CommandContext ctx, string content = null, bool tts = false, DiscordEmbed embed = null) {
            return await ctx.RespondAsync(ctx.Member.Mention + " " + content, tts, embed);
        }

        public static bool RightChannel(this CommandContext ctx) {
            return (Bot.Instance.Channel ?? (Bot.Instance.Channel = ctx.Channel)) == ctx.Channel;
        }

        public static async Task<DiscordMessage> Reply(this DiscordMessage msg, string content = null, bool tts = false, DiscordEmbed embed = null) {
            return await msg.RespondAsync(msg.Author.Mention + " " + content, tts, embed);
        }

        public static bool RightChannel(this DiscordMessage msg) {
            return (Bot.Instance.Channel ?? (Bot.Instance.Channel = msg.Channel)) == msg.Channel;
        }
    }
}
