using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordPlaysKTANE.Game;

namespace DiscordPlaysKTANE.Discord.CommandSets {
    [Name("GameCommands")]
    public class GameCommandSet : ModuleBase<SocketCommandContext> {
        [Command("newbomb")]
        public async Task NewBombAsync(int modules) {
            if (modules > 0) {
                if (GameManager.Instance.NewBomb(modules)) {
                    await ReplyAsync("starting a new bomb with {0} modules...".FormatThis(modules));
                } else {
                    await ReplyAsync("already in a bomb!");
                }
            } else {
                await ReplyAsync("try again with a positive integer!");
            }
        }

        [Command("detonate")]
        public async Task DetonateAsync() {
            if (GameManager.Instance.Detonate()) {
                await ReplyAsync("detonating the bomb...");
            } else {
                await ReplyAsync("there is no bomb running!");
            }
        }
    }
}
