using System;
using DiscordPlaysKTANE.Game.Modules;
namespace DiscordPlaysKTANE.Game.Bombs {
    public static class BombGenerator {

        public static Bomb Generate() {
            return Generate(Constants.DEFAULT_MODULES, Constants.DEFAULT_SETTINGS);
        }

        public static Bomb Generate(int modules) {
            return Generate(modules, Constants.DEFAULT_SETTINGS);
        }

        public static Bomb Generate(GeneratorSettings settings) {
            return Generate(Constants.DEFAULT_MODULES, settings);
        }

        public static Bomb Generate(int modules, GeneratorSettings settings) {
            Bomb result = new Bomb(modules);

            result.BombInfo = new BombInfo();
            result.BombInfo.Init();

            for (int i = 0; i < modules; i++) {
                if (RandomUtil.Double() <= settings.VanillaPercentage) {
                    result.Modules[i] = ModulePools.Vanilla.Random();
                } else {
                    result.Modules[i] = ModulePools.Mods.Random();
                }
                result.Modules[i].BombInfo = result.BombInfo;
                result.Modules[i].ModuleID = i + 1;
            }

            for (int i = 0; i < modules; i++) {
                result.Modules[i].Init();
            }
            return result;
        }

        public class GeneratorSettings {
            public static readonly GeneratorSettings VANILLA = new GeneratorSettings(1, 0);

            public readonly double VanillaPercentage;
            public readonly double ModPercentage;

            public GeneratorSettings(double vanilla, double mods) {
                VanillaPercentage = vanilla / (vanilla + mods);
                ModPercentage = mods / (vanilla + mods);
            }
        }
    }
}
