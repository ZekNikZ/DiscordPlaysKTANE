using System.Threading;
using DiscordPlaysKTANE.Discord;
using DiscordPlaysKTANE.Game.Modules.Vanilla;
using DiscordPlaysKTANE.Game.Bombs;
using Svg;
using System.IO;
using System;
using System.Drawing;
using System.Xml;
using System.Text;
using System.Runtime.CompilerServices;
using DiscordPlaysKTANE.Game.Modules;
using DiscordPlaysKTANE.Game;

namespace DiscordPlaysKTANE {
    class Program {
        static void Main(string[] args) {
            RandomUtil.InitRandom();
            ModulePools.Init();

            ThreadStart gameRef = new ThreadStart(GameManager.StartThread);
            Thread gameThread = new Thread(gameRef);
            gameThread.Start();

            using (Bot.Instance = new Bot()) {
                Bot.Instance.RunAsync().Wait();
            }
        }
    }
}
