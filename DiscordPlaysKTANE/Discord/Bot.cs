using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DiscordPlaysKTANE.Discord.Entities;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using System.Text.RegularExpressions;
using DiscordPlaysKTANE.Discord.Commands;
using DSharpPlus.Entities;
using System.Linq;
using System.Collections.Generic;

namespace DiscordPlaysKTANE.Discord {
    public class Bot : IDisposable {
        public static Bot Instance;

        private DiscordClient _client;
        private InteractivityModule _interactivity;
        private CommandsNextModule _cnext;
        private Config _config;
        private StartTimes _starttimes;
        private CancellationTokenSource _cts;

        public DiscordChannel Channel;

        public Bot() {
            if (!File.Exists("config.json")) {
                new Config().SaveToFile("config.json");
                #region !! Report to user that config has not been set yet !! (aesthetics)
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Black;
                WriteCenter("▒▒▒▒▒▒▒▒▒▄▄▄▄▒▒▒▒▒▒▒", 2);
                WriteCenter("▒▒▒▒▒▒▄▀▀▓▓▓▀█▒▒▒▒▒▒");
                WriteCenter("▒▒▒▒▄▀▓▓▄██████▄▒▒▒▒");
                WriteCenter("▒▒▒▄█▄█▀░░▄░▄░█▀▒▒▒▒");
                WriteCenter("▒▒▄▀░██▄░░▀░▀░▀▄▒▒▒▒");
                WriteCenter("▒▒▀▄░░▀░▄█▄▄░░▄█▄▒▒▒");
                WriteCenter("▒▒▒▒▀█▄▄░░▀▀▀█▀▒▒▒▒▒");
                WriteCenter("▒▒▒▄▀▓▓▓▀██▀▀█▄▀▀▄▒▒");
                WriteCenter("▒▒█▓▓▄▀▀▀▄█▄▓▓▀█░█▒▒");
                WriteCenter("▒▒▀▄█░░░░░█▀▀▄▄▀█▒▒▒");
                WriteCenter("▒▒▒▄▀▀▄▄▄██▄▄█▀▓▓█▒▒");
                WriteCenter("▒▒█▀▓█████████▓▓▓█▒▒");
                WriteCenter("▒▒█▓▓██▀▀▀▒▒▒▀▄▄█▀▒▒");
                WriteCenter("▒▒▒▀▀▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒");
                Console.BackgroundColor = ConsoleColor.Yellow;
                WriteCenter("WARNING", 3);
                Console.ResetColor();
                WriteCenter("Thank you Mario!", 1);
                WriteCenter("But our config.json is in another castle!");
                WriteCenter("(Please fill in the config.json that was generated.)", 2);
                WriteCenter("Press any key to exit..", 1);
                Console.SetCursorPosition(0, 0);
                Console.ReadKey();
                #endregion
                Environment.Exit(0);
            }
            this._config = Config.LoadFromFile("config.json");
            _client = new DiscordClient(new DiscordConfiguration() {
                AutoReconnect = true,
                EnableCompression = true,
                LogLevel = LogLevel.Debug,
                Token = _config.Token,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true
            });

            _interactivity = _client.UseInteractivity(new InteractivityConfiguration() {
                PaginationBehaviour = TimeoutBehaviour.Delete,
                PaginationTimeout = TimeSpan.FromSeconds(30),
                Timeout = TimeSpan.FromSeconds(30)
            });

            _starttimes = new StartTimes() {
                BotStart = DateTime.Now,
                SocketStart = DateTime.MinValue
            };

            _cts = new CancellationTokenSource();

            DependencyCollection dep = null;
            using (var d = new DependencyCollectionBuilder()) {
                d.AddInstance(new Dependencies() {
                    Interactivity = this._interactivity,
                    StartTimes = this._starttimes,
                    Cts = this._cts
                });
                dep = d.Build();
            }

            _cnext = _client.UseCommandsNext(new CommandsNextConfiguration() {
                CaseSensitive = false,
                EnableDefaultHelp = true,
                EnableDms = false,
                EnableMentionPrefix = true,
                StringPrefix = _config.Prefix,
                IgnoreExtraArguments = true,
                Dependencies = dep
            });

            //_cnext.RegisterCommands<Commands.Owner>();
            //_cnext.RegisterCommands<Commands.Interactivity>();
            _cnext.RegisterCommands<BotCommands>();
            _cnext.RegisterCommands<GameCommands>();
            _cnext.RegisterCommands<BombCommands>();

            _client.MessageCreated += MessageCreated;

            _client.Ready += OnReadyAsync;
        }

        public async Task RunAsync() {
            await _client.ConnectAsync();
            await WaitForCancellationAsync();
        }

        private async Task WaitForCancellationAsync() {
            while (!_cts.IsCancellationRequested)
                await Task.Delay(500);
        }

        private async Task OnReadyAsync(ReadyEventArgs e) {
            await Task.Yield();
            _starttimes.SocketStart = DateTime.Now;
        }

        public void Dispose() {
            this._client.Dispose();
            this._interactivity = null;
            this._cnext = null;
            this._config = null;
        }

        internal void WriteCenter(string value, int skipline = 0) {
            for (int i = 0; i < skipline; i++)
                Console.WriteLine();

            Console.SetCursorPosition((Console.WindowWidth - value.Length) / 2, Console.CursorTop);
            Console.WriteLine(value);
        }

        private static string _moduleCommandRegex;
        //private static IEnumerable<string> _commands;
        private async Task MessageCreated(MessageCreateEventArgs msg) {
            if ((Channel ?? (Channel = msg.Channel)) != msg.Channel) {
                return;
            }
            //Debug.Log(String.Join(" ", _cnext.RegisteredCommands.Select(x => x.Key)));
            if (msg.Author.Id == _client.CurrentUser.Id || msg.Message.MessageType != MessageType.Default) return;
            if (Regex.IsMatch(msg.Message.Content, _moduleCommandRegex ?? (_moduleCommandRegex = @"^" + _config.Prefix + @"[1-9][0-9]* .*"))) {
                await ModuleCommandHandler.HandleMessageAsync(msg);
            } /*else if (msg.Message.Content.StartsWith(_config.Prefix, StringComparison.InvariantCultureIgnoreCase)) {
                await msg.Message.Reply(Responses.UnrecognizedCommand.FormatThis(msg.Message.Content.Split(' ')[0]));
            }*/
        }

        public async Task<DiscordMessage> Broadcast(string content = null, bool tts = false, DiscordEmbed embed = null) {
            return await _client.SendMessageAsync(Channel, content, tts, embed);
        }
    }
}