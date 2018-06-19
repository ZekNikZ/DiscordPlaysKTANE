using System;
using System.Threading.Tasks;
using Discord.Commands;
using DiscordPlaysKTANE.Game;

namespace DiscordPlaysKTANE.Discord.CommandSets {
    [Name("BombCommands")]
    public class BombCommandSet : ModuleBase<SocketCommandContext> {
        [Command("bomb edgework")]
        public Task BombHandlerAsync() => ReplyAsync("`" + GameManager.Instance.GetEdgeworkString() + "`");
    }
}
