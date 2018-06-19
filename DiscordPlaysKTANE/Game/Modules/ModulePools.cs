using System;
using System.Collections.Generic;
using DiscordPlaysKTANE.Game.Modules.Vanilla;

namespace DiscordPlaysKTANE.Game.Modules {
    public class VanillaModulePool : ModulePool {
        public override void InitializeModulePool() {
            ModuleTypes.Add(typeof(TheButtonModule));
            ModuleTypes.Add(typeof(SimonSaysModule));
        }
    }

    public class ModModulePool : ModulePool {
        public override void InitializeModulePool() {

        }
    }

    public abstract class ModulePool {
        public List<Type> ModuleTypes = new List<Type>();

        public abstract void InitializeModulePool();

        public BaseModule Random() {
            BaseModule module = (BaseModule)Activator.CreateInstance(ModuleTypes[RandomUtil.Int(ModuleTypes.Count)]);
            return module;
        }
    }

    public static class ModulePools {
        public static readonly VanillaModulePool Vanilla = new VanillaModulePool();
        public static readonly ModModulePool Mods = new ModModulePool();

        public static void Init() {
            Vanilla.InitializeModulePool();
            Mods.InitializeModulePool();
        }
    }
}
