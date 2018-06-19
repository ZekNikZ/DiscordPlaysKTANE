using System;
using DSharpPlus.EventArgs;
using DiscordPlaysKTANE.Game;
using System.Threading.Tasks;
using System.Linq;
using DSharpPlus.Entities;
using System.IO;
using DiscordPlaysKTANE.Game.Modules;
using Svg;
using System.Drawing;
using Gif.Components;
using System.Threading;

namespace DiscordPlaysKTANE.Discord.Commands {
    public static class ModuleCommandHandler {
        public static async Task HandleMessageAsync(MessageCreateEventArgs msg) {
            if (!msg.Message.RightChannel()) return;

            if (!GameManager.Instance.BombInProgress) {
                await msg.Message.Reply(ResponsesTemplates.NotInBomb);
                return;
            }
            var parts = msg.Message.Content.Split(new string[] { " " }, 2, StringSplitOptions.None);
            int id = int.Parse(parts[0].Substring(1));
            if (id <= 0 || id > GameManager.Instance.CurrentBomb.ModuleCount) {
                await msg.Message.Reply(ResponsesTemplates.BadModuleID.FormatThis(id));
                return;
            }

            if (parts[1].StartsWith("view", StringComparison.InvariantCultureIgnoreCase)) {
                await ViewModuleAsync(msg.Message, id);
            } else if (parts[1].StartsWith("help", StringComparison.InvariantCultureIgnoreCase)) {
                await GetHelpMessageAsync(msg.Message, id);
            } else {
                if (GameManager.Instance.CurrentBomb.IsModuleSolved(id)) {
                    await msg.Message.Reply(ResponsesTemplates.BadModuleID.FormatThis(id));
                    return;
                }
                BaseModule module = GameManager.Instance.CurrentBomb.Modules.First(m => m.ModuleID == id);
                string response = module.ProcessDiscordMessage(parts[1], msg.Message);
                await HandleResponse(msg.Message, response, module);
            }
        }

        public static async Task HandleResponse(DiscordMessage msg, string response, BaseModule module) {
            switch (response) {
                case ResponseStrings.InvalidCommand:
                    await msg.Reply(ResponsesTemplates.InvalidModuleCommand.FormatThis(module.ModuleID));
                    break;
                case ResponseStrings.InvalidArguments:
                    await msg.Reply(ResponsesTemplates.InvalidModuleArguments.FormatThis(module.ModuleID));
                    break;
                case ResponseStrings.MissingArguments:
                    await msg.Reply(ResponsesTemplates.ModuleMissingArguments);
                    break;
                case ResponseStrings.UnsubmittablePenalty:
                    // TODO: Leaderboard
                    break;
                case ResponseStrings.Solve:
                    await msg.RespondAsync(ResponsesTemplates.ModuleSolve.FormatThis(module.ModuleID, module.ModuleName, msg.Author.Mention));
                    break;
                case ResponseStrings.Strike:
                    await msg.RespondAsync(ResponsesTemplates.ModuleStrike.FormatThis(module.ModuleID, module.ModuleName, msg.Author.Mention));
                    break;
                case ResponseStrings.Waiting:
                    break;
                default:
                    if (response.StartsWith(ResponseStrings.SendToChat, StringComparison.InvariantCultureIgnoreCase)) {
                        await msg.Reply(response.Substring(ResponseStrings.SendToChat.Length));
                    }
                    break;
            }
        }

        public static async Task ViewModuleAsync(DiscordMessage msg, int id) {
            //Thread thread = new Thread(delegate () {
            BaseModule module = GameManager.Instance.CurrentBomb.Modules.First(m => m.ModuleID == id);
            var panels = module.GetImage();
            MemoryStream outStream = new MemoryStream();
            string extension;
            if (panels.Count() == 1) {
                extension = "png";
                var panel = panels.First();
                if (panel is SvgDocument) {
                    var bitmap = ((SvgDocument)panel).Draw();
                    bitmap.Save(outStream, System.Drawing.Imaging.ImageFormat.Png);
                } else if (panel is Bitmap) {
                    ((Bitmap)panel).Save(outStream, System.Drawing.Imaging.ImageFormat.Png);
                } else {
                    throw new FormatException();
                }
            } else {
                extension = "gif";
                AnimatedGifEncoder gifEncoder = new AnimatedGifEncoder();
                gifEncoder.Start(outStream);
                gifEncoder.SetDelay(500);
                gifEncoder.SetRepeat(0);
                foreach (var panel in panels) {
                    if (panel is SvgDocument) {
                        var p = panel as SvgDocument;
                        gifEncoder.AddFrame(p.Draw());
                    } else if (panel is Tuple<SvgDocument, int>) {
                        var p = panel as Tuple<SvgDocument, int>;
                        for (int i = 0; i < p.Item2; i++) {
                            gifEncoder.AddFrame(p.Item1.Draw());
                        }
                    } else if (panel is Bitmap) {
                        gifEncoder.AddFrame((Bitmap)panel);
                    } else {
                        throw new FormatException();
                    }
                }
                gifEncoder.Finish();
            }
            outStream.Position = 0;
            var embedBuilder = new DiscordEmbedBuilder().WithTitle($"{module.ModuleName} (#{module.ModuleID})").WithImageUrl(@"attachment://image." + extension);
            await msg.RespondWithFileAsync(outStream, "image." + extension, content: msg.Author.Mention, embed: embedBuilder.Build());
            outStream.Dispose();
            //});
            //thread.Start();
            return;
        }

        public static async Task GetHelpMessageAsync(DiscordMessage msg, int id) {
            BaseModule module = GameManager.Instance.CurrentBomb.Modules.First(m => m.ModuleID == id);
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder();
            embedBuilder.AddField("Manual", $"https://ktane.timwi.de/HTML/{module.ModuleName}.html".Replace(" ", "%20"));
            embedBuilder.AddField("Help", module.HelpMessage.FormatThis(module.ModuleID));
            await msg.Reply(embed: embedBuilder.WithTitle($"Help for: {module.ModuleName}").WithColor(DiscordColor.Red).Build());
        }
    }
}