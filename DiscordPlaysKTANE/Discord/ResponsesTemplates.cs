namespace DiscordPlaysKTANE.Discord {
    public static class ResponsesTemplates {
        public const string NotInBomb = "Error: a bomb is not active. Use `!newbomb` to start a bomb.";
        public const string BadModuleID = "Error: module {0} is not available.";
        public const string InvalidModuleCommand = "Error: invalid command. Use `!{0} help` for help.";
        public const string InvalidModuleArguments = "Error: invalid argument(s). Use `!{0} help` for help.";
        public const string ModuleMissingArguments = "Error: missing arguments. Use `!{0} help` for help.";
        public const string UnrecognizedCommand = "Error: unrecognized command: `{0}`";
        public const string ModuleSolve = ":white_check_mark: Module {0} ({1}) solved by {2}!";
        public const string ModuleStrike = ":x: Strike by {2} on module {0} ({1})!";
        public const string BombDefused = ":star: Bomb defused! Counter-terrorists win!";
        public const string BombExploded = ":bomb: Bomb exploded! We'll get them next time!";
    }
}
