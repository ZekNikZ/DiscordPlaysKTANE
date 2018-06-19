using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace DiscordPlaysKTANE.Discord.CommandSets {
    [Name("BotCommands")]
    public class BotCommandSet : ModuleBase<SocketCommandContext> {
        [Command("ping")]
        public Task PingAsync()
            => ReplyAsync("pong!");

        [Command("echo")]
        public Task EchoAsync([Remainder] string text)
            // Insert a ZWSP before the text to prevent triggering other bots!
            => ReplyAsync('\u200B' + text);

        [Command("list")]
        public Task ListAsync(params string[] objects)
            => ReplyAsync("You listed: " + string.Join("; ", objects));

        [Command("test")]
        public Task TextAsync() {
            Debug.Log("here!!!");
            return null;
        }

        protected override async Task<IUserMessage> ReplyAsync(string message, bool isTTS = false, Embed embed = default(Embed), RequestOptions options = default(RequestOptions)) {
            Debug.Log("Reply started");
            var x = await base.ReplyAsync(message, isTTS, embed, options);
            Debug.Log("Reply finished");
            return x;
        }
    }
}
