using AuroraMod.Common.Systems;
using AuroraMod.Common.UI;
using System;
using System.Collections.Generic;

using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;


namespace AuroraMod.Common.ModPlayers
{
    public class AbilityPlayer : ModPlayer
    {
        public static int AbilityCooldown { get; private set; }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (AuroraKeybindSystem.AbilityKeyBind.JustPressed && AbilityCooldown <= 0)
            {
                Item item = ModContent.GetInstance<AbilitySlot>().FunctionalItem;
                if (item is not null)
                {
                    IAbilityItem abilityItem = item.ModItem as IAbilityItem;
                    abilityItem.OnUse(Player, Player.GetSource_FromThis());
                    AbilityCooldown = abilityItem.Cooldown;
                }
            }
        }

        public override void PostUpdate()
        {
            if (AbilityCooldown > 0)
                AbilityCooldown--;
        }
    }
}
