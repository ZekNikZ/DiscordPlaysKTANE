using System;
using System.Threading;
using DSharpPlus.Interactivity;

namespace DiscordPlaysKTANE.Discord.Entities {
    internal class Dependencies {
        internal InteractivityModule Interactivity { get; set; }
        internal StartTimes StartTimes { get; set; }
        internal CancellationTokenSource Cts { get; set; }
    }
}
