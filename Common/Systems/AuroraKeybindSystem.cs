using System;
using System.Collections.Generic;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AuroraMod.Common.Systems
{
    public class AuroraKeybindSystem : ModSystem
    {
        public static ModKeybind AbilityKeyBind { get; private set; }

        public override void Load()
        {
            AbilityKeyBind = KeybindLoader.RegisterKeybind(Mod, "Off-Hand Ability", "F");
        }
    }
}
