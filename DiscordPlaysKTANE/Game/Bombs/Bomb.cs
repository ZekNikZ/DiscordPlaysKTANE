using System;
using DiscordPlaysKTANE.Game.Modules;
using System.Linq;
namespace DiscordPlaysKTANE.Game.Bombs {
    public class Bomb {
        public BombInfo BombInfo;

        public BaseModule[] Modules;

        public Bomb(int modules) {
            Modules = new BaseModule[modules];
        }

        public int ModuleCount => (int)(_moduleCount ?? (_moduleCount = Modules.Length));
        private int? _moduleCount;

        public bool IsModuleSolved(int id) {
            return Modules.First(m => m.ModuleID == id).Solved;
        }
    }
}
