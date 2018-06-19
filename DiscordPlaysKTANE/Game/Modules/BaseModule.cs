using DiscordPlaysKTANE.Game.Bombs;
using System;
using Svg;
using System.Collections.Generic;
using DSharpPlus.Entities;

namespace DiscordPlaysKTANE.Game.Modules {
    public abstract class BaseModule {
        public int ModuleID;

        public abstract string ModuleName { get; }

        public abstract string HelpMessage { get; }

        public BombInfo BombInfo;

        public bool Solved;

        public virtual void Init() { }

        public virtual void OnActivate() { }

        public abstract string ProcessDiscordMessage(string command, DiscordMessage msg);

        // object must be either SvgDocument or Tuple<SvgDocument, int>
        public abstract IEnumerable<object> GetImage();

        public virtual SvgDocument Template => _template ?? (_template = ImageUtil.LoadModuleTemplate(ModuleName));

        private SvgDocument _template;

        public void HandlePass() {
            Debug.Log("Module Passed!");
            Solved = true;
        }

        public void HandleStrike() {
            Debug.Log("Module Striked!");
            BombInfo.HandleStrike();
        }

        public void Log(object message) {
            Debug.Log($"[{ModuleName} #{ModuleID}] {message}");
        }

        public void LogFormat(string message, params object[] args) {
            Debug.LogFormat($"[{ModuleName} #{ModuleID}] {message}", args);
        }
    }
}