using DiscordPlaysKTANE.Game.Bombs;

namespace DiscordPlaysKTANE {
    public static class Constants {
        public const string ImagePath = @"/Users/matthewmccaskill/Projects/DiscordPlaysKTANE/DiscordPlaysKTANE/Game/Modules/ImageTemplates/{0}.svg";
        public const string TokenPath = @"/Users/matthewmccaskill/Projects/DiscordPlaysKTANE/DiscordPlaysKTANE/token.txt";

        public const int DEFAULT_MODULES = 5;
        public static readonly BombGenerator.GeneratorSettings DEFAULT_SETTINGS = BombGenerator.GeneratorSettings.VANILLA;
    }
}
